﻿// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NeoCortexApi.Entities;
using NeoCortexApi.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace NeoCortexApi.Encoders
{


    /// <summary>
    /// Defines the <see cref="ScalarEncoderExperimental" />
    /// </summary>
    public class ScalarEncoder : EncoderBase
    {
        /// <summary>
        /// Gets a value indicating whether IsDelta
        /// </summary>
        public override bool IsDelta => throw new NotImplementedException();

        /// <summary>
        /// Gets the Width
        /// </summary>
        public override int Width => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarEncoderExperimental"/> class.
        /// </summary>
        public ScalarEncoder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarEncoderExperimental"/> class.
        /// </summary>
        /// <param name="encoderSettings">The encoderSettings<see cref="Dictionary{string, object}"/></param>
        public ScalarEncoder(Dictionary<string, object> encoderSettings)
        {
            this.Initialize(encoderSettings);
        }

        /// <summary>
        /// The AfterInitialize
        /// </summary>
        public override void AfterInitialize()
        {
            if (W % 2 == 0)
            {
                throw new ArgumentException("W must be an odd number (to eliminate centering difficulty)");
            }

            HalfWidth = (W - 1) / 2;

            // For non-periodic inputs, padding is the number of bits "outside" the range,
            // on each side. I.e. the representation of minval is centered on some bit, and
            // there are "padding" bits to the left of that centered bit; similarly with
            // bits to the right of the center bit of maxval
            Padding = Periodic ? 0 : HalfWidth;

            if (double.NaN != MinVal && double.NaN != MaxVal)
            {
                if (MinVal >= MaxVal)
                {
                    throw new ArgumentException("maxVal must be > minVal");
                }

                RangeInternal = MaxVal - MinVal;
            }

            // There are three different ways of thinking about the representation. Handle
            // each case here.
            InitEncoder(W, MinVal, MaxVal, N, Radius, Resolution);

            //nInternal represents the output _area excluding the possible padding on each side
            NInternal = N - 2 * Padding;

            if (Name == null)
            {
                if ((MinVal % ((int)MinVal)) > 0 ||
                    (MaxVal % ((int)MaxVal)) > 0)
                {
                    Name = "[" + MinVal + ":" + MaxVal + "]";
                }
                else
                {
                    Name = "[" + (int)MinVal + ":" + (int)MaxVal + "]";
                }
            }

            //Checks for likely mistakes in encoder settings
            if (IsRealCortexModel)
            {
                if (W < 21 || W <= 2)
                {
                    throw new ArgumentException(
                        "Number of bits in the SDR (%d) must be greater than 2, and recommended >= 21 (use forced=True to override)");
                }
            }
        }


        protected void InitEncoder(int w, double minVal, double maxVal, int n, double radius, double resolution)
        {
            if (n != 0)
            {
                if (double.NaN != minVal && double.NaN != maxVal)
                {
                    if (!Periodic)
                    {
                        Resolution = RangeInternal / (N - W);
                    }
                    else
                    {
                        Resolution = RangeInternal / N;
                    }

                    Radius = W * Resolution;

                    if (Periodic)
                    {
                        Range = RangeInternal;
                    }
                    else
                    {
                        Range = RangeInternal + Resolution;
                    }
                }
            }
            else
            {
                if (radius != 0)
                {
                    Resolution = Radius / w;
                }
                else if (resolution != 0)
                {
                    Radius = Resolution * w;
                }
                else
                {
                    throw new ArgumentException(
                        "One of n, radius, resolution must be specified for a ScalarEncoder");
                }

                if (Periodic)
                {
                    Range = RangeInternal;
                }
                else
                {
                    Range = RangeInternal + Resolution;
                }

                double nFloat = w * (Range / Radius) + 2 * Padding;
                N = (int)(nFloat);
            }
        }


        /// <summary>   
        /// Decoding an array of outputs based on specified parameters.          
        /// Decodes an array of outputs based on the provided parameters and returns an array of decoded inputs.
        /// </summary>
        /// <param name="encodedOutput">The array of encoded outputs to be decoded.</param>
        /// <param name="minValue">The minimum value for the decoded inputs.</param>
        /// <param name="maxValue">The maximum value for the decoded inputs.</param>
        /// <param name="arrayLength">The length of the input array.</param>
        /// <param name="width">A parameter affecting the decoding process.</param>
        /// <param name="isPeriodic">Indicates whether the decoded input array should be checked for periodicity.</param>
        /// <returns>An array of decoded inputs.</returns>

        public static int[] Decode(int[] encodedOutput, int minValue, int maxValue, int arrayLength, double width, bool isPeriodic)
        {
            // Extract runs of 1s from the encoded output
            List<int[]> runsList = ExtractRuns(encodedOutput);

            // Adjust periodic space if necessary
            AdjustPeriodicSpace(isPeriodic, runsList, encodedOutput.Length);

            // Map runs to input values
            List<int> decodedInput = MapRunsToInput(runsList, arrayLength, minValue, maxValue, width, isPeriodic);

            // Sort and adjust periodicity of the decoded input
            SortAndAdjustPeriodic(decodedInput, minValue, maxValue, isPeriodic);

            return decodedInput.ToArray();
        }

        /// <summary>
        /// Extracts runs of 1s from the output array and returns a list of run information.
        /// </summary>
        private static List<int[]> ExtractRuns(int[] output)
        {
            List<int[]> runs = new List<int[]>();
            int start = -1, prev = 0, count = 0;

            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] == 0)
                {
                    if (start != -1)
                    {
                        runs.Add(new int[] { start, prev, count });
                        start = -1;
                        count = 0;
                    }
                }
                else
                {
                    if (start == -1)
                        start = i;

                    prev = i;
                    count++;
                }
            }

            if (start != -1)
                runs.Add(new int[] { start, prev, count });

            return runs;
        }

        /// <summary>
        /// Adjusts the periodic space by merging the first and last runs if they cover the entire array.
        /// </summary>
        private static void AdjustPeriodicSpace(bool isPeriodic, List<int[]> runsList, int outputLength)
        {
            if (!isPeriodic || runsList.Count <= 1) return;

            int[] firstRun = runsList[0];
            int[] lastRun = runsList[runsList.Count - 1];

            if (firstRun[0] == 0 && lastRun[1] == outputLength - 1)
            {
                firstRun[1] = lastRun[1];
                firstRun[2] += lastRun[2];
                runsList.RemoveAt(runsList.Count - 1);
            }
        }

        /// <summary>
        /// Wraps the value within the specified range.
        /// </summary>
        private static int Wrap(int value, int minValue, int maxValue)
        {
            int range = maxValue - minValue + 1;

            if (value < minValue)
            {
                value += range * ((minValue - value) / range + 1);
            }

            return minValue + (value - minValue) % range;
        }

        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        private static double Map(double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        /// <summary>
        /// Maps the runs of 1s to input values and returns a list of decoded input values.
        /// </summary>
        private static List<int> MapRunsToInput(List<int[]> runsList, int arrayLength, int minValue, int maxValue, double width, bool isPeriodic)
        {
            List<int> decodedInput = new List<int>();

            foreach (int[] run in runsList)
            {
                int left = (int)Math.Floor(run[0] + 0.5 * (run[2] - width));
                int right = (int)Math.Floor(run[1] - 0.5 * (run[2] - width));

                if (left < 0 && isPeriodic)
                {
                    left += arrayLength;
                    right += arrayLength;
                }

                for (int i = left; i <= right; i++)
                {
                    int value = (int)Math.Round(Map(i, 0, arrayLength - 1, minValue, maxValue));

                    if (isPeriodic)
                        value = Wrap(value, minValue, maxValue);

                    if (value >= minValue && value <= maxValue)
                        decodedInput.Add(value);
                }
            }

            return decodedInput;
        }




        /// <summary>
        /// Gets the index of the first non-zero bit.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Null in a case of an error.</returns>
        /// <exception cref="ArgumentException"></exception>
        protected int? GetFirstOnBit(double input)
        {
            if (input == double.NaN)
            {
                return null;
            }
            else
            {
                if (input < MinVal)
                {
                    if (ClipInput && !Periodic)
                    {
                        Debug.WriteLine("Clipped input " + Name + "=" + input + " to minval " + MinVal);

                        input = MinVal;
                    }
                    else
                    {
                        throw new ArgumentException($"Input ({input}) less than range ({MinVal} - {MaxVal}");
                    }
                }
            }

            if (Periodic)
            {
                if (input >= MaxVal)
                {
                    throw new ArgumentException($"Input ({input}) greater than periodic range ({MinVal} - {MaxVal}");
                }
            }
            else
            {
                if (input > MaxVal)
                {
                    if (ClipInput)
                    {

                        Debug.WriteLine($"Clipped input {Name} = {input} to maxval MaxVal");
                        input = MaxVal;
                    }
                    else
                    {
                        throw new ArgumentException($"Input ({input}) greater than periodic range ({MinVal} - {MaxVal}");
                    }
                }
            }

            int centerbin;
            if (Periodic)
            {
                centerbin = (int)((input - MinVal) * NInternal / Range + Padding);
            }
            else
            {
                centerbin = ((int)(((input - MinVal) + Resolution / 2) / Resolution)) + Padding;
            }

            return centerbin - HalfWidth;
        }


        /// <summary>
        /// Gets the bucket index of the given value.
        /// </summary>
        /// <param name="inputData">The data to be encoded. Must be of type double.</param>
        /// <param name="bucketIndex">The bucket index.</param>
        /// <returns></returns>
        /// Understanding the new method to get the bucket index.
        /// Now Try to Implement Buckets Code While usinf Professor Hint
        public int? GetBucketIndex(object inputData)
        {
            double input = Convert.ToDouble(inputData, CultureInfo.InvariantCulture);
            if (input == double.NaN)
            {
                return null;
            }

            int? bucketVal = GetFirstOnBit(input);

            return bucketVal; 
        }


        /// <summary>
        /// Encodes the given scalar value as SDR as defined by HTM.
        /// </summary>
        /// <param name="inputData">The inputData<see cref="object"/></param>
        /// <returns>The <see cref="int[]"/></returns>
        public override int[] Encode(object inputData)
        {
            int[] output = null;

            double input = Convert.ToDouble(inputData, CultureInfo.InvariantCulture);
            if (input == double.NaN)
            {
                return output;
            }

            int? bucketVal = GetFirstOnBit(input);
            if (bucketVal != null)
            {
                output = new int[N];

                int bucketIdx = bucketVal.Value;
                //Arrays.fill(output, 0);
                int minbin = bucketIdx;
                int maxbin = minbin + 2 * HalfWidth;
                if (Periodic)
                {
                    if (maxbin >= N)
                    {
                        int bottombins = maxbin - N + 1;
                        int[] range = ArrayUtils.Range(0, bottombins);
                        ArrayUtils.SetIndexesTo(output, range, 1);
                        maxbin = N - 1;
                    }
                    if (minbin < 0)
                    {
                        int topbins = -minbin;
                        ArrayUtils.SetIndexesTo(output, ArrayUtils.Range(N - topbins, N), 1);
                        minbin = 0;
                    }
                }

                ArrayUtils.SetIndexesTo(output, ArrayUtils.Range(minbin, maxbin + 1), 1);
            }

            // Output 1-D array of same length resulted in parameter N    
            return output;
        }


        /// <summary>
        /// This method enables running in the network.
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="learn"></param>
        /// <returns></returns>
        public int[] Compute(object inputData, bool learn)
        {
            return Encode(inputData);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="List{T}"/></returns>
        public override List<T> GetBucketValues<T>()
        {
            throw new NotImplementedException();
        }

        //public static object Deserialize<T>(StreamReader sr, string name)
        //{
        //    var excludeMembers = new List<string> { nameof(ScalarEncoder.Properties) };
        //    return HtmSerializer2.DeserializeObject<T>(sr, name, excludeMembers);
        //}
    }
}