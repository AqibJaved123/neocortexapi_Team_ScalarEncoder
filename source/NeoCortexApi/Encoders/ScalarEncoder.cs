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
        /// This method encodes the input into an array of active bits using a scalar encoder.
        /// It takes into account both periodic and non-periodic encoders.
        /// The active bits are set based on the bucket index calculated for the input value.
        /// </summary>
        /// <param name="inputData">The input value to be encoded.</param>
        /// <param name="output">The array of active bits to be modified and returned.</param>
        /// <returns>The array of active bits.</returns>
        public bool[] EncodedArray(double inputData, bool[] output)
        {
            // Ensure the input is a valid double value
            double input = Convert.ToDouble(inputData, CultureInfo.InvariantCulture);

            // Handle case when the input is NaN (Not a Number)
            switch (input)
            {
                case double.NaN:
                    return output;
            }

            // Get the bucket value for the input
            int? bucketVal = GetFirstOnBit(input);

            switch (bucketVal)
            {
                case null:
                    // No bucket value found, return the original output array
                    break;

                default:
                    // Bucket index for the input value
                    int bucketIdx = bucketVal.Value;

                    // Define the bin range based on the bucket index
                    int minbin = bucketIdx;
                    int maxbin = minbin + 2 * HalfWidth;

                    // Adjust bins for periodic encoders
                    switch (Periodic)
                    {
                        case true:
                            // Adjust for periodic encoders when maxbin exceeds the array length
                            switch (maxbin >= N)
                            {
                                case true:
                                    int bottombins = maxbin - N + 1;
                                    for (int i = 0; i < bottombins; i++)
                                    {
                                        // Set active bits for bottom bins
                                        output[i] = true;
                                    }
                                    maxbin = N - 1;
                                    break;
                            }

                            // Adjust for periodic encoders when minbin is less than 0
                            switch (minbin < 0)
                            {
                                case true:
                                    int topbins = -minbin;
                                    for (int i = 0; i < topbins; i++)
                                    {
                                        // Set active bits for top bins
                                        output[N - i - 1] = true;
                                    }
                                    minbin = 0;
                                    break;
                            }
                            break;
                    }

                    // Set active bits for the calculated bin range
                    for (int i = minbin; i <= maxbin; i++)
                    {
                        output[i] = true;
                    }
                    break;
            }

            // Output 1-D array of the same length as the parameter N    
            return output;
        }


        /// <summary>
        /// Gets the bucket index of the given value using a scalar encoder.
        /// </summary>
        /// <param name="inputValue">The data to be encoded. Must be of type decimal.</param>
        /// <returns>The bucket index or null if the input value is outside the valid range.</returns>
        public int? ScalarEncoderDetermineBucketIndex(decimal inputValue)
        {
            // Check if the input value is outside the valid range
            if ((double)inputValue < MinVal || (double)inputValue > MaxVal)
            {
                return null;
            }

            // Calculate the normalized fraction based on the input value
            decimal normalizedFraction = CalculateNormalizedFraction(inputValue);

            // Determine the bucket index using the normalized fraction
            int index = CalculateBucketIndex(normalizedFraction);

            // Handle periodic conditions for the bucket index
            if (index == BucketCount)
            {
                // Wrap around to the first bucket if the index equals the total number of buckets
                index = 0;
            }

            // Adjust the index for periodic conditions and a specific epsilon threshold
            if (Periodic && index == 0 && Math.Abs(inputValue - (decimal)MaxVal) <= (decimal)Epsilon)
            {
                // Set the index to the last bucket if it's the first bucket and meets the epsilon condition
                index = (int)(BucketCount - 1);
            }

            // Check if the input value is within the specified bucket radius
            if (BucketRadius >= 0 && !IsWithinBucketRadius(inputValue, index))
            {
                return null;
            }

            return index;
        }

        // Calculate the normalized fraction based on the input value
        private decimal CalculateNormalizedFraction(decimal inputValue)
        {
            decimal fraction = (decimal)(((double)inputValue - MinVal) / (MaxVal - MinVal));

            // Adjust the fraction for periodic conditions
            if (Periodic)
            {
                fraction = fraction - Math.Floor(fraction);
            }

            return fraction;
        }

        // Calculate the bucket index based on the normalized fraction
        private new int CalculateBucketIndex(decimal normalizedFraction)
        {
            return (int)Math.Floor(normalizedFraction * (decimal)BucketCount);
        }

        // Check if the input value is within the specified bucket radius of the given bucket index
        private bool IsWithinBucketRadius(decimal inputValue, int index)
        {
            decimal bucketWidth = ((decimal)MaxVal - (decimal)MinVal) / (decimal)BucketCount;
            decimal bucketCenter = (bucketWidth * index) + (bucketWidth / 2) + (decimal)MinVal;

            // Check if the absolute difference between the input value and the bucket center is within the bucket radius
            return Math.Abs((decimal)inputValue - bucketCenter) <= (decimal)BucketRadius * bucketWidth;
        }


        /// <summary>
        /// This code calculates bucket information for a scalar value based on the provided encoder parameters. 
        /// It first clips the input value to the specified range, calculates the bucket index and center, and then 
        /// calculates the bucket bounds. It also handles periodic encoding by wrapping the bucket index and choosing 
        /// the closest edge as the bucket center. The function returns an integer array containing the bucket index, 
        /// the rounded bucket center, and the rounded bucket start and end points.
        /// </summary>
        /// <param name="input">The scalar value to be encoded.</param>
        /// <returns>An integer array containing bucket information.</returns>
        public int[] ScalarEncoderAnalyzeinfo(double input)
        {
            // Clip input to the specified range
            input = ClipToRange(input, MinVal, MaxVal);

            // Calculate bucket information
            double bucketWidth = CalculateBucketWidth();
            int bucketIndex = CalculateBucketIndex(input, bucketWidth);
            double bucketCenter = CalculateBucketCenter(bucketIndex, bucketWidth);
            double bucketStart = CalculateBucketStart(bucketIndex, bucketWidth);
            double bucketEnd = CalculateBucketEnd(bucketIndex, bucketWidth);

            // Handle periodic encoding
            if (Periodic)
            {
                AdjustForPeriodicEncoding(ref bucketIndex, ref bucketCenter, ref bucketStart, ref bucketEnd, input);
            }

            // Return the bucket information
            return new int[] { bucketIndex, (int)Math.Round(bucketCenter), (int)Math.Round(bucketStart), (int)Math.Round(bucketEnd) };
        }

        // Clip the input value to the specified range
        private double ClipToRange(double value, double minValue, double maxValue)
        {
            return Math.Max(minValue, Math.Min(value, maxValue));
        }

        // Calculate the width of each bucket
        private double CalculateBucketWidth()
        {
            return (MaxVal - MinVal) / N;
        }

        // Calculate the bucket index based on the input value and bucket width
        private int CalculateBucketIndex(double input, double bucketWidth)
        {
            return (int)((input - MinVal) / bucketWidth);
        }

        // Calculate the center of the specified bucket
        private double CalculateBucketCenter(int bucketIndex, double bucketWidth)
        {
            return MinVal + (bucketIndex + 0.5) * bucketWidth;
        }

        // Calculate the start point of the specified bucket
        private double CalculateBucketStart(int bucketIndex, double bucketWidth)
        {
            return MinVal + bucketIndex * bucketWidth;
        }

        // Calculate the end point of the specified bucket
        private double CalculateBucketEnd(int bucketIndex, double bucketWidth)
        {
            return MinVal + (bucketIndex + 1) * bucketWidth;
        }

        // Adjust bucket information for periodic encoding
        private void AdjustForPeriodicEncoding(ref int bucketIndex, ref double bucketCenter, ref double bucketStart, ref double bucketEnd, double input)
        {
            // Wrap bucket index to handle periodic conditions
            bucketIndex = (bucketIndex % N + N) % N;

            // Calculate distance to the start and end edges
            double distToStart = input - bucketStart;
            double distToEnd = bucketEnd - input;

            // Wrap distances for periodicity
            distToStart = WrapDistanceForPeriodicEncoding(distToStart);
            distToEnd = WrapDistanceForPeriodicEncoding(distToEnd);

            // Choose the closest edge as the bucket center
            bucketCenter = (distToStart < distToEnd) ? bucketStart : bucketEnd;
        }

        // Wrap distance for periodic encoding
        private double WrapDistanceForPeriodicEncoding(double distance)
        {
            if (distance < 0)
            {
                // Adjust distance to handle periodic conditions
                distance += MaxVal - MinVal;
            }
            return distance;
        }


        /// <summary>
        /// Generates a string description of a list of ranges.
        /// </summary>
        /// <param name="ranges">A list of Tuple values representing the start and end values of each range.</param>
        /// <returns>A string representation of the ranges, where each range is separated by a comma and space.</returns>


        public string GenerateNumericRangeDescription(List<Tuple<double, double>> numericRanges)
        {
            // Validate input
            if (numericRanges == null || numericRanges.Count == 0)
            {
                throw new ArgumentException("Numeric ranges cannot be null or empty.");
            }

            // Initialize the description
            StringBuilder descriptionBuilder = new StringBuilder();

            // Iterate through each range
            foreach (var range in numericRanges)
            {
                double start = range.Item1;
                double end = range.Item2;

                // Append the range to the description
                if (start == end)
                {
                    descriptionBuilder.Append($"{start:F2}");
                }
                else
                {
                    descriptionBuilder.Append($"{start:F2}-{end:F2}");
                }

                // Add a comma for multiple ranges
                descriptionBuilder.Append(", ");
            }

            // Remove the trailing comma and space
            descriptionBuilder.Length -= 2;

            // Return the generated description
            return descriptionBuilder.ToString();
        }


        private string DecodedToStr(Tuple<Dictionary<string, Tuple<List<int>, string>>, List<string>> tuple)
        {
            throw new NotImplementedException();
        }

        private void PPrint(double[] output)
        {
            throw new NotImplementedException();
        }

        private Tuple<Dictionary<string, Tuple<List<int>, string>>, List<string>> Decode(double[] output)
        {
            throw new NotImplementedException();
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
        /// Calculates BucketMatch scores between expected and actual values using the ScalarEncoder's parameters.
        /// </summary>
        public static class BucketMatchScore
        {
            // Parameters for configuration
            private static double MaximumValue;
            private static double MinimumValue;
            private static bool IsPeriodic;
            private static bool ClipInputs;
            private static int ElementCount;

            /// <summary>
            /// Configure parameters for the BucketMatch class.
            /// </summary>
            /// <param name="max">The maximum value for the range.</param>
            /// <param name="min">The minimum value for the range.</param>
            /// <param name="isPeriodic">Flag indicating whether the range is periodic.</param>
            /// <param name="clipInputs">Flag indicating whether to clip inputs to the specified range.</param>
            /// <param name="elementCount">The number of elements in the range.</param>
            public static void ConfigureParameters(double max, double min, bool isPeriodic, bool clipInputs, int elementCount)
            {
                MaximumValue = max;
                MinimumValue = min;
                IsPeriodic = isPeriodic;
                ClipInputs = clipInputs;
                ElementCount = elementCount;
            }

            /// <summary>
            /// Get the bucket match scores between expected and actual values.
            /// </summary>
            /// <param name="expectedValues">Array of expected values.</param>
            /// <param name="actualValues">Array of actual values.</param>
            /// <param name="useFractionalDifference">Flag to determine whether fractional or absolute closeness score should be returned.</param>
            /// <returns>An array of bucket match scores.</returns>
            public static double[] GetBucketMatchScore(double[] expectedValues, double[] actualValues, bool useFractionalDifference = true)
            {
                double expectedValue = expectedValues[0];
                double actualValue = actualValues[0];

                // Calculate the difference between expected and actual values
                double difference = CalculateDifference(expectedValue, actualValue);

                // Calculate the match score based on the difference
                double matchScore = useFractionalDifference
                    ? CalculateFractionalMatchScore(difference)
                    : CalculateAbsoluteMatchScore(difference);

                // Return the match score in an array
                return new double[] { matchScore };
            }

            // Calculate the difference between expected and actual values
            private static double CalculateDifference(double expected, double actual)
            {
                if (IsPeriodic)
                {
                    // Adjust values for periodic conditions
                    expected = expected % MaximumValue;
                    actual = actual % MaximumValue;

                    // Calculate the minimum distance considering periodicity
                    return Math.Min(Math.Abs(expected - actual), MaximumValue - Math.Abs(expected - actual));
                }
                else
                {
                    // Calculate the absolute difference
                    return Math.Abs(expected - actual);
                }
            }

            // Calculate the fractional match score based on the difference
            private static double CalculateFractionalMatchScore(double difference)
            {
                // Calculate the value range
                double valueRange = CalculateValueRange();

                // Calculate the error percentage and limit it to 1.0
                double errorPercentage = difference / valueRange;
                errorPercentage = Math.Min(1.0, errorPercentage);

                // Calculate and return the fractional match score
                return 1.0 - errorPercentage;
            }

            // Calculate the absolute match score based on the difference
            private static double CalculateAbsoluteMatchScore(double difference)
            {
                // Return the absolute difference as the absolute match score
                return difference;
            }

            // Calculate the value range considering clipping inputs
            private static double CalculateValueRange()
            {
                double range = (MaximumValue - MinimumValue) + (ClipInputs ? 0 : (2 * (MaximumValue - MinimumValue) / (ElementCount - 1)));
                return range;
            }
        }

        /// <summary>
        /// Computes the boundaries of a specified value within a range of values.
        /// Throws exceptions for invalid input or encoder constraints.
        /// </summary>
        /// <param name="value">The value to be placed into a bucket.</param>
        /// <returns>An array with the lower and upper bounds of the value's bucket.</returns>
        /// <exception cref="ArgumentException">Thrown for non-numeric or out-of-range input.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the bucket width is deemed invalid.</exception>
        public double[] DetermineBucketBounds(double value)
        {
            // Handle special cases
            switch (double.IsNaN(value) || double.IsInfinity(value))
            {
                case true:
                    throw new ArgumentException("Invalid number provided as input.");
                default:
                    break;
            }

            switch (value < this.MinVal || value >= this.MaxVal)
            {
                case true:
                    throw new ArgumentException("Input value is beyond the encoder's specified range.");
                default:
                    break;
            }

            int numberOfBuckets = 100;

            // Compute the width of each bucket
            double bucketWidth = (this.MaxVal - this.MinVal) / (double)this.NumBuckets;

            switch (double.IsInfinity(bucketWidth) || double.IsNaN(bucketWidth) || bucketWidth <= 0.0)
            {
                case true:
                    throw new InvalidOperationException("Invalid bucket width detected.");
                default:
                    break;
            }

            Console.WriteLine("Bucket Width: " + bucketWidth);

            // Determine the index of the bucket containing the input value
            int bucketIndex = (int)((value - this.MinVal) / bucketWidth);
            Console.WriteLine("Bucket Index: " + bucketIndex);

            // Compute the lower and upper bounds of the bucket
            double lowerBound = bucketIndex * bucketWidth + this.MinVal;
            Console.WriteLine("Lower Bound: " + lowerBound);

            double upperBound = (bucketIndex + 1) * bucketWidth + this.MinVal;
            Console.WriteLine("Upper Bound: " + upperBound);

            // Return the bucket boundaries
            return new double[] { lowerBound, upperBound };
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