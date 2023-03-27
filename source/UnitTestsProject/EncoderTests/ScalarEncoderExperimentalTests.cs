// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortex;
using NeoCortexApi;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using NeoCortexApi.Utility;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Tsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;

namespace UnitTestsProject.EncoderTests
{

    ///<summary>
    /// Defines the <see cref="EncoderTests" />
    ///      Scalar Encoder is an encoding technique that encodes input into array of bits and then produces output as 0's and 1's(continuous block).
    ///        The output is produced based on encoding parameters. There are few types of encoding parameters such as :
    ///        N - Number of bits in the encoded output. It should be greater than or equal to W always.
    ///        W - It is the 'Width' and it always determines continuous block of 1's. Width W should always be odd number.
    ///            MinVal - It is the minimum value of input signal.
    ///            MaxVal - It is the maximum value of Input signal.
    ///            
    ///           Radius - Two inputs separated by more than the radius have non-overlapping representations. Two inputs separated by less than the radius will overlap in at least some of their bits.
    ///            Resolution - Two inputs separated by greater than, or equal to the resolution are guaranteed to have different representations.
    ///            Period - If set 'true' then input value wraps around such that 'MinVal' = 'MaxVal'
    ///                     If set 'false' then the input values are all unique and 'MinVal'!= 'MaxVal'
    ///            ClipInput - Two inputs separated by greater than, or equal to the resolution are guaranteed to have different representations.
    ///            Formulae to calculate:
    ///            Resolution when Periodic = 'true' => (Range)/(N) => (MaxVal-MinVal)/(N).
    ///            Resolution when Periodic = 'false' => (Range)/(N-W) => (MaxVal-MinVal)/(N-W).
    ///</summary>
    ///
















    [TestClass]
    public class ScalarEncoderScalarEncoderExperimentalTestsTests
    {

        // Unit Test A Starts here
        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem : This is basic unit test for Scalar Enocder with buckets.In this test we just taking
        // first tweenty numeric values from 0 to 20.The updated encoder(Scalar Encoder with buckets)
        // encodes these values with bucket.
        // Here we are providing the unit test with numeric values, its encoded form and corrosponding
        // bucket number as well.If our minVal=0, maxVal=20,Width=5 and Range=20 ,So total bucket can be calculated
        // by using this formula b=n-w+1 and which are 16 (b=20-5+1).For Example if we want to encode value 10,
        // then its encoded form is { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, } and its mapping
        // bucket is 8 which can be calculated by using formulas given below
        //Resolution = (MaxVal - MinVal) / (N - W)
        //RangeInternal = MaxVal - MinVal
        //HalfWidth = (W - 1) / 2
        //Padding = HalfWidth
        //Range = RangeInternal + Resolution
        //centerbin = int (((input - MinVal) + Resolution / 2) / Resolution) + Padding

        //bucket_index = centerbin - HalfWidth
        // </summary>
      

        [DataRow(0, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(1, 1, new int[] { 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(2, 2, new int[] { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(3, 2, new int[] { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(4, 3, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(5, 4, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(6, 5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(7, 5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(8, 6, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(9, 7, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(10, 8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(11, 8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(12, 9, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, })]
        [DataRow(13, 10, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, })]
        [DataRow(14, 11, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]
        [DataRow(15, 11, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]
        [DataRow(16, 12, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, })]
        [DataRow(17, 13, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, })]
        [DataRow(18, 14, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, })]
        [DataRow(19, 14, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, })]
        [DataRow(20, 15, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]



        public void ScalarEncoderWithBucketBasicUnitTest(double input, double bucket, int[] expectedResult)
        {


            string outFolder = nameof(ScalarEncoderWithBucketBasicUnitTest);
            Directory.CreateDirectory(outFolder);
            DateTime now = DateTime.Now;
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "N", 20},
                { "MinVal", (double)0}, // Min value = (0).
                { "MaxVal", (double)20}, // Max value = (20).
                { "Periodic", false }, 
                { "Name", "Basic"},
                { "ClipInput", false},
            });

            var result = encoder.Encode(input);

            int? bucketIndex = encoder.GetBucketIndex(input);

            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
            var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

            // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Gray, Color.Green, text: $"value:{input} /bucket:{bucketIndex}");


            Debug.WriteLine(input);
            Debug.WriteLine(bucket);
            Debug.WriteLine(bucketIndex);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


            Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.


        }

        // Unit Test A Ends here


        // Unit Test B Starts here
        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem : This is basic unit test for Scalar Enocder with buckets. In this test, we are testing 
        // the ability of the encoder to handle negative values. We are encoding 5 negative values ranging from -20 to -1.
        // Here, our minVal=-20, maxVal=-1, Width=5, and Range=19. Total buckets can be calculated by using 
        // the formula b=n-w+1, which is 11 (b=15-5+1). For example, if we want to encode value -12, then 
        // its encoded form is { 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, } and its mapping bucket is 4,
        // which can be calculated by using the formula given below
        //Resolution = (MaxVal - MinVal) / (N - W)
        //RangeInternal = MaxVal - MinVal
        //HalfWidth = (W - 1) / 2
        //Padding = HalfWidth
        //Range = RangeInternal + Resolution
        //centerbin = int (((input - MinVal) + Resolution / 2) / Resolution) + Padding

        //bucket_index = centerbin - HalfWidth
        // </summary>
        [DataRow(-20, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-19, 1, new int[] { 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-18, 1, new int[] { 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-17, 2, new int[] { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-16, 2, new int[] { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-15, 3, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-14, 3, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-13, 4, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-12, 4, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-12, 4, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-11, 5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, })]
        [DataRow(-10, 5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, })]
        [DataRow(-9, 6, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]
        [DataRow(-8, 6, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]
        [DataRow(-7, 7, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, })]
        [DataRow(-6, 7, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, })]
        [DataRow(-5, 8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, })]
        [DataRow(-4, 8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, })]
        [DataRow(-3, 9, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, })]
        [DataRow(-2, 9, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, })]
        [DataRow(-1, 10, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]


        public void ScalarEncoderWithBucketUintTestNegativeValues(double input, double bucket, int[] expectedResult)
        {
            string outFolder = nameof(ScalarEncoderWithBucketUintTestNegativeValues);
            Directory.CreateDirectory(outFolder);
            DateTime now = DateTime.Now;
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "N", 15},
                { "MinVal", (double)-20}, // Min value =  -20
                { "MaxVal", (double)-1}, // Max value     -1
                { "Periodic", false },
                { "Name", "Basic"},
                { "ClipInput", false},
            });

            var result = encoder.Encode(input);

            int? bucketIndex = encoder.GetBucketIndex(input);

            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
            var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

            // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Gray, Color.Green, text: $"value:{input} /bucket:{bucketIndex}");


            Debug.WriteLine(input);
            Debug.WriteLine(bucket);
            Debug.WriteLine(bucketIndex);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


            Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.


        }
        // Unit Test B Ends here


        // Unit Test C Starts here
        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem: This is a unit test for Scalar Encoder with buckets that encodes decimal values. 
        // In this test, we provide a range of decimal values and their expected encoded form, 
        // as well as the corresponding bucket number based on the given encoding parameters.In this
        // case the N=25, W=7 , MinVal =0.0 and MaxVal =1.0 .By using the formula given in above unit test
        // we calculated that total buckets are 18 .If we take any random input like 0.3 then their corrosponding
        // bucket is 5 which is also calculaated by the formula given in above unit tests and its encoded form
        // is { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, }. As bucket for 0.3
        // is 5th that is the reason that ctive bit streams are started from 5th bit.
        // </summary>
        [DataRow(0.0, 0, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(0.1, 2, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(0.2, 4, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(0.3, 5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(0.4, 7, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(0.5, 9, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(0.6, 11, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(0.7, 13, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, })]
        [DataRow(0.8, 14, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]
        [DataRow(0.9, 16, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, })]
        [DataRow(1.0, 18, new int[]  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, })]

        public void ScalarEncoderWithBucketUnitTestDecimalValues(double input, double bucket, int[] expectedResult)
        {
            string outFolder = nameof(ScalarEncoderWithBucketUnitTestDecimalValues);
            Directory.CreateDirectory(outFolder);
            DateTime now = DateTime.Now;
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 7},
                { "N", 25},
                { "MinVal", (double)0}, // Min value =  0
                { "MaxVal", (double)1}, // Max value =  1
                { "Periodic", false },
                { "Name", "Basic"},
                { "ClipInput", false},
            });

            var result = encoder.Encode(input);

            int? bucketIndex = encoder.GetBucketIndex(input);

            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
            var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

            // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Gray, Color.Green, text: $"value:{input} /bucket:{bucketIndex}");


            Debug.WriteLine(input);
            Debug.WriteLine(bucket);
            Debug.WriteLine(bucketIndex);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


            Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.


        }
        // Unit Test C Ends here

        // Unit Test D Starts here
        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem : This is basic unit test for Scalar Enocder with buckets.In this test we provide just 
        // Radius instead of providing direct "N" to the encoder.The "N" can be calculated by the formulas 
        // given below to this
        // N= floor(w * (Range / Radius) + 2 * Padding)
        //
        // Here we took the Numeric values 0 to 15 for encoding.The updated encoder(Scalar Encoder with buckets)
        // encodes these values with bucket.
        // In the unit test we are providing numeric values, its encoded form and corrosponding
        // bucket number as well.Now in this case  minVal=0, maxVal=15,Width=5 and Radius = 3.75,So "N" can be calculted total bucket can be calculated
        // by using the formual given above and which is 25 .
        // </summary>
        [DataRow(0, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(1, 1, new int[] { 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(2, 3, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(3, 4, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(4, 5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(5, 7, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(6, 8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(7, 9, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(8, 11, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(9, 12, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(10, 13, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(11, 15, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, })]
        [DataRow(12, 16, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]
        [DataRow(13, 17, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, })]
        [DataRow(14, 19, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, })]
        [DataRow(15, 20, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]


        public void ScalarEncoderWithBucketRadiusUnitTest(double input, double bucket, int[] expectedResult)
        {


            string outFolder = nameof(ScalarEncoderWithBucketRadiusUnitTest);
            Directory.CreateDirectory(outFolder);
            DateTime now = DateTime.Now;
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "Radius", 3.75}, // Now Radius instead of "N"
                { "MinVal", (double)0}, // Min value = (0).
                { "MaxVal", (double)15}, // Max value = (15).
                { "Periodic", false },
                { "Name", "Basic"},
                { "ClipInput", false},
            });

            var result = encoder.Encode(input);

            int? bucketIndex = encoder.GetBucketIndex(input);

            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
            var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

            // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Gray, Color.Green, text: $"value:{input} /bucket:{bucketIndex}");
            Debug.WriteLine(encoder.N);
            Debug.WriteLine(input);
            Debug.WriteLine(bucket);
            Debug.WriteLine(bucketIndex);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


            Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.


        }

        // Unit Test D Ends here


        // Unit Test E Starts here
        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        //This unit test verifies the Scalar Encoder with buckets' behavior when clipInput is set to true instead
        //of false.When clipInput is true, input values outside of the specified range will be encoded to the maximum
        //or minimum input value.For example, values lower than minVal will encode to the first bucket (or lower value),
        //and values greater than maxVal will encode to the last bucket (or max value).
        //This test case uses various numeric inputs that are outside the range of minVal and maxVal, and verifies the
        //correct encoding of these inputs. This test has has a minimum value of 0, maximum value of 20, and width of 5.
        // </summary>

        [DataRow(0, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-100, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(200, 15, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]
        [DataRow(35, 15, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]
        [DataRow(-100, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(-2500, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(3500, 15, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]
        [DataRow(20.5, 15, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]
        [DataRow(-0.5, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]


        public void ScalarEncoderWithBucketClipInputUnitTest(double input, int bucket, int[] expectedResult)
        {


            string outFolder = nameof(ScalarEncoderWithBucketClipInputUnitTest);
            Directory.CreateDirectory(outFolder);
            DateTime now = DateTime.Now;
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "N", 20}, 
                { "MinVal", (double)0}, // Min value = (0).
                { "MaxVal", (double)20}, // Max value = (20).
                { "Periodic", false },
                { "Name", "Basic"},
                { "ClipInput", true},  //For Clipping values outside the range
            });

            var result = encoder.Encode(input);

            int? bucketIndex = encoder.GetBucketIndex(input);

            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
            var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

            // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Gray, Color.Green, text: $"value:{input} /bucket:{bucketIndex}");
            Debug.WriteLine(input);
            Debug.WriteLine(bucket);
            Debug.WriteLine(bucketIndex);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


            Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.


        }

        // Unit Test E Ends here

        // Unit Test F Starts here
        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // This unit test validates the functionality of the Scalar Encoder with buckets with periodic setting.
        // The test includes the first twenty numeric values from 0 to 20. The updated encoder (Scalar Encoder with buckets)
        // encodes these values with buckets using the formulas of periodic input. The unit test takes  numeric values,
        // their encoded form, and the corresponding bucket number. In this case, the minimum value is 0, maximum value
        // is 20, width is 5, and range is 20. Total bucket can be calculated by using the formula b = n - w + 1, which
        // is 16 (b = 20 - 5 + 1). For example, if we want to encode value 10, its encoded form is { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, } and its mapping bucket is 8, which can
        // be calculated by using the following formulas:
        // Padding = 0
        // Resolution = Range / N
        // NInternal = N - 2 * Padding;
        // x = floor((input - MinVal) * NInternal / Range + Padding)
        // HalfWidth = (W - 1) / 2
        // bucket_index  = centerbin - HalfWidth
        // </summary>
        [DataRow(0, -2, new int[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, })]
        [DataRow(1, -1, new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, })]
        [DataRow(2, 0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(3, 1, new int[] { 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(4, 2, new int[] { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(5, 3, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(6, 4, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(7, 5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(8, 6, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(9, 7, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(10, 8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(11, 9, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, })]
        [DataRow(12, 10, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, })]
        [DataRow(13, 11, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]
        [DataRow(14, 12, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, })]
        [DataRow(15, 13, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, })]
        [DataRow(16, 14, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, })]
        [DataRow(17, 15, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]
        [DataRow(18, 16, new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, })]
        [DataRow(19, 17, new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, })]
        //[DataRow(20, 18, new int[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, })]

        public void ScalarEncoderWithBucketPeriodicUnitTest(double input, double bucket, int[] expectedResult)
        {


            string outFolder = nameof(ScalarEncoderWithBucketPeriodicUnitTest);
            Directory.CreateDirectory(outFolder);
            DateTime now = DateTime.Now;
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "N", 20},
                { "MinVal", (double)0}, // Min value = (0).
                { "MaxVal", (double)20}, // Max value = (20).
                { "Periodic", true },
                { "Name", "Basic"},
                { "ClipInput", false},
            });

            var result = encoder.Encode(input);

            int? bucketIndex = encoder.GetBucketIndex(input);

            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
            var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

            // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Gray, Color.Green, text: $"value:{input} /bucket:{bucketIndex}");


            Debug.WriteLine(input);
            Debug.WriteLine(bucket);
            Debug.WriteLine(bucketIndex);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


            Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.


        }

        // Unit Test F Ends here








        // Unit test Number# 1
        // <summary>
        // Problem : Encoding the different Month of Year
        // This MinVal is 0 (January) and the MaxVal 12 (December).
        // The range is calculated with the formula MaxVal – MinVal = 11.
        // The number of bits that are set to encode a single value the ‘width’ of output signal ‘W’ used for representation is 3.
        // Total number of bits in the output ‘N’ used for representation is 14.
        // We are choosing the value of N=14 and W = 3 to get the desired output which shifts between January to December like shown below:
        // So, choose the encoding parameters such that resolution addresses the problem.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>
        // Total buckets  = 14-3+1 = 12


        // b=N-W+1
        // where TotalBuckets=5, minValue=0, and Range=11, 
        // ith bucket=  ((int)(((input - MinVal) + Resolution / 2) / Resolution)) + Padding
        //x = centerbin - HalfWidth   

 


        // ith bucket= floor(TotalBuckets*(Value-minValue)/Range)
        // where TotalBuckets=12, minValue=0, and Range=11, 
        //we can plug in each value from 0 to 11 for Value and solve for ithbucket using the floor function to round down to the nearest integer.

        /* where TotalBuckets=12, minValue=0, and Range=11, we can plug in each value from 0 to 11 for Value and solve for ithbucket using the floor function to round down to the nearest integer.

For Value = 0:
ithbucket = floor(12*(0-0)/11) = floor(0) = 0

For Value = 1:
ithbucket = floor(12*(1-0)/11) = floor(1.09) = 1

For Value = 2:
ithbucket = floor(12*(2-0)/11) = floor(2.18) = 2

For Value = 3:
ithbucket = floor(12*(3-0)/11) = floor(3.27) = 3

For Value = 4:
ithbucket = floor(12*(4-0)/11) = floor(4.36) = 4

For Value = 5:
ithbucket = floor(12*(5-0)/11) = floor(5.45) = 5

For Value = 6:
ithbucket = floor(12*(6-0)/11) = floor(6.55) = 6

For Value = 7:
ithbucket = floor(12*(7-0)/11) = floor(7.64) = 7

For Value = 8:
ithbucket = floor(12*(8-0)/11) = floor(8.73) = 8

For Value = 9:
ithbucket = floor(12*(9-0)/11) = floor(9.82) = 9

For Value = 10:
ithbucket = floor(12*(10-0)/11) = floor(10.91) = 10

For Value = 11:
ithbucket = floor(12*(11-0)/11) = floor(12) = 11

Therefore, the ith bucket for values 0 to 11 using the given formula would be:

0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11




  */



        [TestMethod]
        [TestCategory("Months of the Year")]


        [DataRow(0, 0, new int[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // To represent Jan.
        [DataRow(1, 1, new int[] { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // To represent Feb.
        [DataRow(2, 2, new int[] { 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // To represent Mar.
        [DataRow(3, 3, new int[] { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })] // To represent Apr.
        [DataRow(4, 4, new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })] // To represent May.
        [DataRow(5, 5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, })] // To represent June.
        [DataRow(6, 6, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, })] // To represent July.
        [DataRow(7, 7, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, })] // To represent Aug.
        [DataRow(8, 8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, })] // To represent Sep.
        [DataRow(9, 9, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, })] // To represent Oct.
        [DataRow(10, 10, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, })] // To represent Nov.
        [DataRow(11, 11, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, })] // To represent Dec.

        public void ScalarEncoderWithBucketMonthOfYear(double input, double bucket, int[] expectedResult)
        {
            string outFolder = nameof(ScalarEncodingExperiment);

            Directory.CreateDirectory(outFolder);

            DateTime now = DateTime.Now;

            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                 { "W", 3},
                { "N", 14},
                { "MinVal", (double)0}, // Min value = (0).
                { "MaxVal", (double)11}, // Max value = (11).
                { "Periodic", false},
                { "Name", " Month of the Year"},
                { "ClipInput", true},
            });


            {

                var result = encoder.Encode(input);

                int? bucketIndex = encoder.GetBucketIndex(input);

                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

                //  NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Black, Color.Red, text: $"value:{input} /bucket:{bucketIndex}");

                Debug.WriteLine(input);
                Debug.WriteLine(bucket);
                Debug.WriteLine(bucketIndex);
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


                Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.

                // Assert.IsTrue is used to check whether the given input result matches with the expected result.
            }
        }
        [TestMethod]
        [TestCategory("Prod")]

        // Unit Test Number # 2
        // <summary>
        // Problem: To encode the availability of Bus in a Bus station (Whole Day Schedule).
        // Let's assume that train will be available every 30 minutes.
        // Firstly 24 hours clock is converted into minutes which will be equal to 1440 minutes.
        // Start of the day will be the minimum value which is 0 and end of the day will be the maximum value which is 1440.
        // At the beginning of next day clock resets and it will again start from 0, hence it is a periodic process and the same routine continues every day.
        // The values of parameters 'N' and 'W' should be selected in such a way that we get a resolution of 30.0 .
        // Resolution for this scenario = 1440/48 = 30.0 .
        // After encoding we can see the shift after every 60 minutes.
        // Time interval between adjacent trains can be changed by altering the values of 'N' and 'W' for the known min and max value.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>
        // b=N-W+1
        // Total buckets  = 24-11+1 = 14
        // where TotalBuckets=14, minValue=0, and Range=1440, 
        // ith bucket=  ((int)(((input - MinVal) + Resolution / 2) / Resolution)) + Padding
        //x = centerbin - HalfWidth   

        [DataRow(40, -5.0, new int[] { 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]
        [DataRow(100, -4.0, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, })]
        [DataRow(155, -3.0, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, })]
        [DataRow(300, 0.0, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(422, 2.0, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(500, 3.0, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(670, 6.0, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(700, 6.0, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(889, 9.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]
        [DataRow(900, 10.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, })]
        [DataRow(969, 11.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, })]
        [DataRow(1000, 11.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, })]
        [DataRow(1139, 13.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]
        [DataRow(1200, 15.0, new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]
        [DataRow(1350, 17.0, new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, })]
        [DataRow(1400, 18.0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, })]
        public void ScalarEncoderWithBucketBusStationschedule(double input, double bucket, int[] expectedResult)
        {
            string outFolder = nameof(ScalarEncodingExperiment);

            Directory.CreateDirectory(outFolder);

            DateTime now = DateTime.Now;

            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
    {
        { "W", 11},
        { "N", 24},
        { "MinVal", (double)0.0},
        { "MaxVal", (double)1440.0},
        { "Periodic", true},
        { "Name", "Bus station Schedule"},
        { "ClipInput", true},
    });


            {

                var result = encoder.Encode(input);

                int? bucketIndex = encoder.GetBucketIndex(input);

                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

                NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Black, Color.Red, text: $"value:{input} /bucket:{bucketIndex}");


                Debug.WriteLine(input);
                Debug.WriteLine(bucket);
                Debug.WriteLine(bucketIndex);
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


                Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.

                // Assert.IsTrue is used to check whether the given input result matches with the expected result.
            }
        }

        [TestMethod]
        [TestCategory("Prod")]

        // UNIT TEST NUMBER # 3

        // <summary>
        // Problem: Encoding the people participant in a Game show with unique entrance number
        // Considering Participant have Entrance numbers from 0-100.
        // We have to differenciate each Entrance number, so we have to choose N and W such that Resolution is 1.0 .
        // Resolution = (Range/(N-W)); 
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>
        // </summary>
        // b=N-W+1
        // Total buckets  = 21-11+1 = 11
        // where TotalBuckets=11, minValue=0, and Range=100, 
        // ith bucket=  ((int)(((input - MinVal) + Resolution / 2) / Resolution)) + Padding
        //x = centerbin - HalfWidth   


        [DataRow(2.0, 0.0, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // Encoding Participant having Entrance number 2.
        [DataRow(7.0, 1.0, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // Encoding Participant having Entrance number 7.
        [DataRow(34.0, 3.0, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })] // Encoding Participant having Entrance number 10
        [DataRow(57.0, 6.0, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, })] // Encoding Participant having Entrance number 1.
        [DataRow(78.0, 8.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, })]  // Encoding Participant having Entrance number 8.
        [DataRow(85.0, 9.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, })]  // Encoding Participant having Entrance number 3.
        [DataRow(96.0, 10.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]  // Encoding Participant having Entrance number 9.

        public void ScalarEncoderWithBucketTicketNumber(double input, double bucket, int[] expectedResult)
        {
            string outFolder = nameof(ScalarEncodingExperiment);

            Directory.CreateDirectory(outFolder);

            DateTime now = DateTime.Now;

            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
         { "W",11},
        { "N", 21 },
        { "MinVal", (double)0},      // Min value of Entrance number.
        { "MaxVal", (double)100},     // Max value of Entrance number.
        { "Periodic", false},
        { "Name", "Participant Entrance Number Range"},
        { "ClipInput", true},
    });


            {

                var result = encoder.Encode(input);

                int? bucketIndex = encoder.GetBucketIndex(input);

                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

                // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Black, Color.Red, text: $"value:{input} /bucket:{bucketIndex}");


                Debug.WriteLine(input);
                Debug.WriteLine(bucket);
                Debug.WriteLine(bucketIndex);
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


                Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.

                // Assert.IsTrue is used to check whether the given input result matches with the expected result.
            }
        }
        // Unit Test # 4
        //// <summary>
        // Problem: Encoding the different category of people in the Company according to their ages.
        // Let us say we have Young,  adults, Middle Age and senior People in the Company.
        // We have to differenciate Ages based on this category.
        //Considering Min age would be 0 to 18 year and max age 60 years.
        // The range is calculated with the formula MaxVal – MinVal = 59.
        // The number of bits that are set to encode a single value the ‘width’ of output signal ‘W’ used for representation is 3.
        // Total number of bits in the output ‘N’ used for representation is 14.
        // We are choosing the value of N=14 and W = 3 to get the desired output
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>
        // b=N-W+1
        // Total buckets  = 7-3+1 = 5
        // where TotalBuckets=5, minValue=0, and Range=59, 
        // ith bucket=  ((int)(((input - MinVal) + Resolution / 2) / Resolution)) + Padding
        //x = centerbin - HalfWidth   


        [TestMethod]
        [TestCategory("Age category of empolyees")]


        [DataRow(18.0, 1.0, new int[] { 0, 1, 1, 1, 0, 0, 0, })] // Encoding the age  0-18 years.
        [DataRow(30.0, 2.0, new int[] { 0, 0, 1, 1, 1, 0, 0, })] // Encoding the age 19-30 years.
        [DataRow(35.0, 2.0, new int[] { 0, 0, 1, 1, 1, 0, 0, })] // Encoding the age 30-35 years.
        [DataRow(40.0, 3.0, new int[] { 0, 0, 0, 1, 1, 1, 0, })] // Encoding the age 35-40 years.
        [DataRow(45.0, 3.0, new int[] { 0, 0, 0, 1, 1, 1, 0, })] // Encoding the age 45-50 years.
        [DataRow(58.0, 4.0, new int[] { 0, 0, 0, 0, 1, 1, 1, })] // Encoding the age 50-60 years.


        public void ScalarEncoderWithBucketAgeCategories(double input, double bucket, int[] expectedResult)
        {
            string outFolder = nameof(ScalarEncodingExperiment);

            Directory.CreateDirectory(outFolder);

            DateTime now = DateTime.Now;

            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
        {
             { "W", 3},
             { "N", 7},
             { "MinVal", (double)0}, // Min value = (0).
             { "MaxVal", (double)59}, // Max value = (59).
             { "Periodic", false},
             { "Name", "  Age category of empolyees"},
             { "ClipInput", true},
    });


            {

                var result = encoder.Encode(input);

                int? bucketIndex = encoder.GetBucketIndex(input);

                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

                // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Black, Color.Red, text: $"value:{input} /bucket:{bucketIndex}");



                Debug.WriteLine(input);
                Debug.WriteLine(bucket);
                Debug.WriteLine(bucketIndex);
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


                Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.

                // Assert.IsTrue is used to check whether the given input result matches with the expected result.
            }
        }




        // Unit test number # 5
        // <summary>
        // Problem: Encoding the Temperature range in Category and their effects.
        // Considering  have Temperature Range from -10.0-100 celsius.
        // 
        // Calculate the bit width: The bit width is the range of values represented by each bit.
        // It can be calculated by dividing the input range by the number of bits. In this case, the bit width would be 10,
        // as 100 (the maximum temperature value) divided by 10 bits gives us a bit width of 10.
        //
        //Encode the values: The temperature values from -10.0 to 100 can be encoded using the scalar encoder by dividing the input range into equal intervals of bit width 10.
        // Resolution = (Range/(N-W)); 
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // b=N-W+1
        // Total buckets  = 20-11+1 = 10
        // where TotalBuckets=14, minValue=-10, and Range=100, 
        // ith bucket=  ((int)(((input - MinVal) + Resolution / 2) / Resolution)) + Padding
        //x = centerbin - HalfWidth 


        [TestMethod]
        [TestCategory("Prod")]


        [DataRow(-9.0, -5.0, new int[] { 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, })]   //Temperature range (0-10)
        [DataRow(-2.0, -4.0, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, })]   //Temperature range (10-20)
        [DataRow(8.0, -2.0, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, })]   //Temperature range (20-30)
        [DataRow(31.0, 2.0, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]   //Temperature range (30-40)
        [DataRow(57.0, 7.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, })]   //Temperature range (40-50)
        [DataRow(73.0, 10.0, new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]   //Temperature range (50-60)
        [DataRow(81.0, 11.0, new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]   //Temperature range (60-70)
        [DataRow(93.0, 13.0, new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, })]   //Temperature range (70-80)
        [DataRow(98.0, 14.0, new int[] { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, })]   //Temperature range (90-100)


        public void ScalarEncoderWithBucketTemperatureRanges(double input, double bucket, int[] expectedResult)
        {
            string outFolder = nameof(ScalarEncodingExperiment);

            Directory.CreateDirectory(outFolder);

            DateTime now = DateTime.Now;

            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
    {
        { "W", 11},
        { "N", 20},
        { "MinVal", (double)-10.0},
        { "MaxVal", (double)100.0},
        { "Periodic", true},
        { "Name", "Bus station Schedule"},
        { "ClipInput", true},
    });


            {

                var result = encoder.Encode(input);

                int? bucketIndex = encoder.GetBucketIndex(input);

                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

                NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Black, Color.Red, text: $"value:{input} /bucket:{bucketIndex}");


                Debug.WriteLine(input);
                Debug.WriteLine(bucket);
                Debug.WriteLine(bucketIndex);
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));


                Assert.IsTrue(expectedResult.SequenceEqual(result) && bucket == bucketIndex); // Assert.IsTrue is used to check whether the given input result and bucket matches with the expected result and expected bucket.

                // Assert.IsTrue is used to check whether the given input result matches with the expected result.
            }
        }

        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem : Encoding the different days of week
        // This MinVal is 0 (Sunday) and the MaxVal 6 (Saturday).
        // The range is calculated with the formula MaxVal – MinVal = 7.
        // The number of bits that are set to encode a single value the ‘width’ of output signal ‘W’ used for representation is 3.
        // Total number of bits in the output ‘N’ used for representation is 8.
        // We are choosing the value of N=9 and W = 3 to get the desired output which shifts between Monday to Sunday like shown below:
        // So, choose the encoding parameters such that resolution addresses the problem.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>

        [DataRow(0, new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 1, })] // To represent Monday.
        [DataRow(1, new int[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, })] // To represent Tuesday.
        [DataRow(2, new int[] { 0, 1, 1, 1, 0, 0, 0, 0, 0, })] // To represent Wednesday.
        [DataRow(3, new int[] { 0, 0, 1, 1, 1, 0, 0, 0, 0, })] // To represent Thursday.
        [DataRow(4, new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 0, })] // To represent Friday.
        [DataRow(5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 0, })] // To represent Saturday.
        [DataRow(6, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, })] // To represent Sunday.

        public void ScalarEncoderUnitTestWeek(double input, int[] expectedResult)
        {
            CortexNetworkContext ctx = new CortexNetworkContext();
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
 { "W", 3},
 { "N", 9},
 { "MinVal", (double)0}, // Min value = (0).
 { "MaxVal", (double)7}, // Max value = (7).
 { "Periodic", true}, // Since Monday would repeat again.
 { "Name", "Days Of Week"},
 { "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            //printBitMap(encoder, nameof(ScalarEncoderUnitTestWeek)); // Calling the Bitmap method to show output in Bitmap Format.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result)); // Assert.IsTrue is used to check whether the given input result matches with the expected result.
        }


        [TestMethod]
        [TestCategory("Experiment")]


        [DataRow(0, new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 1, })] // To represent Monday.
        [DataRow(1, new int[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, })] // To represent Tuesday.
        [DataRow(2, new int[] { 0, 1, 1, 1, 0, 0, 0, 0, 0, })] // To represent Wednesday.
        [DataRow(3, new int[] { 0, 0, 1, 1, 1, 0, 0, 0, 0, })] // To represent Thursday.
        [DataRow(4, new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 0, })] // To represent Friday.
        [DataRow(5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 0, })] // To represent Saturday.
        [DataRow(6, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, })] // To represent Sunday.
        public void ScalarEncodingExperiment(double input, int[] expectedResult)
        {
            string outFolder = nameof(ScalarEncodingExperiment);

            Directory.CreateDirectory(outFolder);

            DateTime now = DateTime.Now;

            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
 { "W", 3},
{ "N", 9},
{ "MinVal", (double)0}, // Min value = (0).
{ "MaxVal", (double)7}, // Max value = (7).
{ "Periodic", true}, // Since Monday would repeat again.
{ "Name", "Days Of Week"},
{ "ClipInput", true},
});

            //for (decimal i = 0.0M; i < (long)encoder.MaxVal; i += 0.1M)
            {
                var result = encoder.Encode(input);

                int? bucketIndex = encoder.GetBucketIndex(input);

                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

                NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Gray, Color.Green, text: $"v:{input} /b:{bucketIndex}");
                Debug.WriteLine(input);
                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));

                Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));
                Assert.IsTrue(expectedResult.SequenceEqual(result)); // Assert.IsTrue is used to check whether the given input result matches with the expected result.
            }
        }


        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem : Encoding the different Noise frequency intensity that human ear is exposed to.
        // The Data what we have is that, human ear is exposed to different range of noise intensity levels.
        // The range 0-39.9db is No risk ; range 40.0-120.0db is Danger ; range 120.1db and above is damage.
        // We have to encode this problem by selecting suitable encoding parameters.
        // We have 3 ranges i.e (0-39.9db) has one encoding value, 40.0-119.5db has another encoding value, 120.0db and above has different encoding value.
        // By choosing Resoultion as 80.0, we can address this problem. 
        // Choosing N and W in a way that resolution = 80.0, then we can encode different range by different encoding values.
        // Resolution = Range/N-W ; (160-0)/2 = 80.0 .
        // Now we have three different ranges described by three different encoding values.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>

        [DataRow(10, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, })]    // Encoding the range 0-39.9 noise intensity level.
        [DataRow(20.5, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, })]
        [DataRow(35, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, })]    // Giving inputs of frequencies belonging to first range.
        [DataRow(40, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 0, })]
        [DataRow(55, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 0, })]    // Encoding the range 40.0-120.0 noise intensity level.
        [DataRow(85, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 0, })]
        [DataRow(110.5, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 0, })]    // Giving inputs of frequencies belonging to second range.
        [DataRow(119.5, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 0, })]
        [DataRow(120.0, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, })]
        [DataRow(120.5, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, })]    // Encoding the range 120.1db and above noise intensity level.
        [DataRow(135.5, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, })]
        [DataRow(158.0, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, })]    // Giving inputs of frequencies belonging to third range.

        public void ScalarEncoderUnitTestHearingFrequency(double input, int[] expectedResult)

        {
            CortexNetworkContext ctx = new CortexNetworkContext();
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W",7},
{ "N", 9 },
{ "MinVal", (double)0},    // The lowest noise intensity is 0db.
{ "MaxVal", (double)160},  // The highest noise intensity is 160db.
{ "Periodic", false},
{ "Name", "Frequency Range"},
{ "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            //printBitMap(encoder, nameof(ScalarEncoderUnitTestHearingFrequency)); // Calling the Bitmap method to show output in Bitmap Format.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result));  // Assert.IsTrue is used to check whether the given imput result matches with the expected result.
        }


        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem: Encoding the different category of people traveling in U-bhan according to their age.
        // Let us say we have childrens, teenagers, adults, and senior citizens traveling in U-bahn. We have to differenciate travelers based on this category.
        // Let us choose the bracket of age as difference of 9.9 years.
        // Considering Min age would be 1 year and max age 100 years.
        // To design this problem we have to choose N and W according to the Min and Max value so that Resolution is ~9.9.
        // Resolution here is Range/(N-W) ; (100-1)/10 = 9.9.
        // So we are encoding different category age of people in different way.
        // We can also change the N and W with respect to Min and Max value to take a different resolution and encode the ages.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>

        [DataRow(6.0, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // Encoding the age 6-15.9 years.
        [DataRow(15.0, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // Encoding the age 6-15.9 years.
        [DataRow(16.0, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })] // Encoding the age 16-25.9 years.
        [DataRow(26.0, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })] // Encoding the age 16-25.9 years.
        [DataRow(27.0, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })] // Encoding the age 26-35.9 years.
        [DataRow(38.0, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, })] // Encoding the age 36-45.9 years.
        [DataRow(99.5, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })] // Encoding the age 85-105.9 years.

        public void ScalarEncoderUnitTestPassenger(double input, int[] expectedResult)

        {
            CortexNetworkContext ctx = new CortexNetworkContext();
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W",21},
{ "N", 31 },
//{ "Radius", 10.0},
//{ "Resolution", 10.0 },
{ "MinVal", (double)1},    // Min age is 1 year.
{ "MaxVal", (double)100},  // Max age is 100 years.
{ "Periodic", false},
{ "Name", "Frequency Range"},
{ "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            //printBitMap(encoder, nameof(ScalarEncoderUnitTestPassenger)); // Calling the Bitmap method to show output in Bitmap Format.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result));  // Assert.IsTrue is used to check whether the given imput result matches with the expected result.
        }


        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem: Encoding goals scored by different players holding different jersey number.
        // Considering players have jersey numbers 0-10.
        // We have to differenciate each jersey number, so we have to choose N and W such that Resolution is 1.0 .
        // Resolution = (Range/(N-W)); (10-0)/10;
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>

        [DataRow(1.0, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // Encoding goal scored by player having jersey number 1.
        [DataRow(2.0, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })]  // Encoding goal scored by player having jersey number 2.
        [DataRow(3.0, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]  // Encoding goal scored by player having jersey number 3.
        [DataRow(4.0, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, })]  // Encoding goal scored by player having jersey number 4.
        [DataRow(5.0, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, })]  // Encoding goal scored by player having jersey number 5. 
        [DataRow(6.0, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, })]  // Encoding goal scored by player having jersey number 6. 
        [DataRow(7.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, })]  // Encoding goal scored by player having jersey number 7. 
        [DataRow(8.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, })]  // Encoding goal scored by player having jersey number 8.
        [DataRow(9.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, })]  // Encoding goal scored by player having jersey number 9.
        [DataRow(10.0, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]  // Encoding goal scored by player having jersey number 10.

        public void ScalarEncoderUnitTestGoals(double input, int[] expectedResult)

        {
            CortexNetworkContext ctx = new CortexNetworkContext();
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W",11},
{ "N", 21 },
//{ "Resolution", 1.0 },
{ "MinVal", (double)0},      // Min value of jersey number.
{ "MaxVal", (double)10},     // Max value of jersey number.
{ "Periodic", false},
{ "Name", "Frequency Range"},
{ "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            //printBitMap(encoder, nameof(ScalarEncoderUnitTestGoals)); // Calling the Bitmap method to show output in Bitmap Format.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result)); // Assert.IsTrue is used to check whether the given imput result matches with the expected result.
        }


        [TestMethod]
        // <summary>
        // Problem: According to general MIDI Level 1 devices, the channel 10 is dedicated exclusively for drum sounds which varies for drum keys 35-81. This means each of these drum keys produce different sounds when an instrument is played.
        // For example if a drummer is playing a choir using these keys then he should be in the position to identify next drum key that has to be played so that the music that he is choiring is uninterrupted.	        
        // We are encoding each of these drum sounds uniquely from key 35 - 81. Hence we are considering MaxVal and MinVal, 81 and 35 respectively.
        // The range here would be MaxVal - MinVal = 46
        // The number of active bits is W= 7 for each representation.
        // The total number of bits N is represented as 53.
        // We are choosing the value of N=53 and W = 9 to get the desired output
        // As this example is for non-periodic, then resolution is calculated to be 1
        // he exact starting position of active bits can be calculated by the formula: [(Input - MinVal) +(Resolution /2)]/Resolution + Padding
        // For example, In Drumbeats Value 49 is taken to calculate the same where input is 49, MinVal is 35, Resolution is 3 and padding is 3. After calculating using the above formula and then subtracting with the halfwidth we get the bit position which is 15.
        // In the below values we have encoded few drum keys between 35 - 81 that produce different sounds.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>

        [TestCategory("Prod")]
        [DataRow(35, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This encoding value is for drum key 35 that produce sound "Acoustic Bass Drum"
        [DataRow(42, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This encoding value is for drum key 42 that produce sound "Closed Hi Hat"
        [DataRow(49, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This encoding value is for drum key 49 that produce sound "Crash Cymbal 1"
        [DataRow(52, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This encoding value is for drum key 52 that produce sound "Chinese Cymbal"
        [DataRow(58, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This encoding value is for drum key 58 that produce sound "Vibraslap"
        [DataRow(68, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This encoding value is for drum key 68 that produce sound "Low Agogo"
        [DataRow(69, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This encoding value is for drum key 69 that produce sound "Cabasa"
        [DataRow(70, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This encoding value is for drum key 70 that produce sound "Maracas"
        [DataRow(79, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, })] // This encoding value is for drum key 79 that produce sound "Open Cuica"
        [DataRow(81, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, })] // This encoding value is for drum key 81 that produce sound "Open Triangle"

        public void ScalarEncoderUnitTestDrumBeats(double input, int[] expectedResult)
        {
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 7},
{ "N", 53},
{ "MinVal", (double)35}, //The drum key starts from keynote number 35.
{ "MaxVal", (double)81}, //The drum key ends at keynote number 81.
{ "Periodic", false},
{ "Name", "Different Drum Sounds"},
{ "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result));  // Assert.IsTrue is used to check whether the given imput result matches with the expected result.
        }


        [TestMethod]
        // <summary>
        // Problem: The main idea is to encode basic colors based on its primary colors i.e RGB (Red, Green, Blue). 
        // Each basic color has its own RGB decimal code from 0-255. So based on this we are encoding basic colors.
        // For example, if a basic color has RGB decimal code of (255,100,0) then it implies that color Red is maximum, Green is medium and Blue is not present at all which results in a shade of Orange.
        // In reality there are totally 13 basic colors and 3 primary colors. However Red has decimal value of (255,0,0), blue has decimal value of (0,255,0) and Green has decimal value of (0,0,255).	      
        // To design basic colors (including primary color) we are considering Maxvalue = 16 because obviously there are 16 colors in total and minvalue = 1. 
        // The total number of bits N is considered as N = 22
        // We are choosing the value of N=22 and W = 7 to get the desired output
        // As RGB representation is non-periodic, the resolution is calculated based on formula Range/N-W. Hence resolution is 1.
        // To know from which bit the active bits start, below mentioned formula can be used:[(Input-MinVal) +(Resolution /2)]/Resolution + Padding
        // For example, In RGB Value 4 is taken to calculate the same where input is 4, MinVal is 1 Resolution is 1 and padding is 3. After calculating using the above formula and then subtracting with the halfwidth we get the bit position which is 4.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>

        [TestCategory("Prod")]
        [DataRow(1, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes Black color with decimal code (R,G,B)   = (0,0,0)
        [DataRow(2, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes White color with decimal code (R,G,B)   = (255,255,255)
        [DataRow(3, new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes Red color with decimal code (R,G,B)     = (255,0,0)
        [DataRow(4, new int[] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes Green color with decimal code (R,G,B)   = (0,255,0)
        [DataRow(5, new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes Blue color with decimal code (R,G,B)    = (0,0,255)
        [DataRow(6, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes Yellow color with decimal code (R,G,B)  = (255,255,0)
        [DataRow(7, new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes Cyan color with decimal code (R,G,B)    = (0,255,255)
        [DataRow(8, new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes Magenta color with decimal code (R,G,B) = (255,0,255)
        [DataRow(9, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })] // This enocodes Silver color with decimal code (R,G,B)  = (192,192,192)
        [DataRow(10, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, })] // This enocodes Gray color with decimal code (R,G,B)    = (128,128,128)
        [DataRow(11, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, })] // This enocodes Maroon color with decimal code (R,G,B)  = (128,0,0)
        [DataRow(12, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, })] // This enocodes Olive color with decimal code (R,G,B)   = (128,128,0)
        [DataRow(13, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, })] // This enocodes Lime color with decimal code (R,G,B)    = (0,128,0)
        [DataRow(14, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, })] // This enocodes Purple color with decimal code (R,G,B)  = (128,0,128)
        [DataRow(15, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, })] // This enocodes Teal color with decimal code (R,G,B)    = (0,128,128)
        [DataRow(16, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, })] // This enocodes Navy color with decimal code (R,G,B)    = (0,0,128)

        public void ScalarEncoderUnitTestBasicColors(double input, int[] expectedResult)

        {
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 7},
{ "N", 22},
//{ "Radius", 1.5},
//{ "Resolution", 1.0},
{ "MinVal", (double)1}, // Minimum value of Basic color starts from 1.
{ "MaxVal", (double)16},// Maximum value of Basic color is 16.
{ "Periodic", false},
{ "Name", "Primary and Basic color encoding"},
{ "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result)); // Assert.IsTrue is used to check whether the given input result matches with the expected result.

        }


        [TestMethod]
        // <summary>
        // Problem: This test is to encode ASCII values.
        // Computer can only understand numbers, so ASCII is used to represent different letter, symbol, punctuation, number and others. 
        // There are 128 ASCII values where ‘Min’ is equal to 0 and ‘Max’ is equal to 127.
        // The value of range is computed which is Max-Min. Hence range for ASCII code is 127
        // The number of active bits 'W' used for each representation is 21.
        // As ASCII code is non-periodic, resolution is calculated which is 1.
        // In the output to know from which bit the ones start, below formula will be used to calculate which is: [(Input-MinVal) +(Resolution /2)]/Resolution + Padding
        // For example, ASCII code 1 is taken to calculate the same where input is 1, MinVal is 0 Resolution is 1 and padding is -0.5. After calculating using the above formula the bit position is 1
        // Same process will be applied for all the ASCII codes.
        // Hence, ASCII code 1 will be represented with 148 bits where 21 are active bits starting at 1st bit position 
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>
        [TestCategory("Prod")]
        [DataRow(1, new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // 1   represents  ASCII value of "SOH"	
        [DataRow(5, new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // 5   represents  ASCII value of "ENQ"
        [DataRow(10, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // 10  represents  ASCII value of "LF"
        [DataRow(23, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // 23  represents  ASCII value of "ETB"
        [DataRow(32, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // 32  represents  ASCII value of "SPACE"
        [DataRow(48, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // 48  represents  ASCII value of "0"
        [DataRow(65, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // 65  represents  ASCII value of "A"
        [DataRow(97, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // 97  represents  ASCII value of "a"
        [DataRow(127, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]  // 127 represents  ASCII value of "DEL"

        public void ScalarEncoderUnitTestASCII(double input, int[] expectedResult)
        {
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 21},
{ "N", 148},
{ "MinVal", (double)0.0},
{ "MaxVal", (double)127.0},
{ "Periodic", false},
{ "Name", "ASCII representation"},
{ "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result));   // Assert here it is used to check whether the given input and output matches.

        }


        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem: To encode the availability of train in a station (Entire Day Schedule).
        // Let's assume that train will be available every 20 minutes.
        // Firstly 24 hours clock is converted into minutes which will be equal to 1440 minutes.
        // Start of the day will be the minimum value which is 0 and end of the day will be the maximum value which is 1440.
        // At the beginning of next day clock resets and it will again start from 0, hence it is a periodic process and the same routine continues every day.
        // The values of parameters 'N' and 'W' should be selected in such a way that we get a resolution of 20.0 .
        // Resolution for this scenario = 1440/72 = 20.0 .
        // After encoding we can see the shift after every 20 minutes.
        // Time interval between adjacent trains can be changed by altering the values of 'N' and 'W' for the known min and max value.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>

        [DataRow(10, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]  // Train arrives at 00.10
        [DataRow(40, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, })]  // Train arrives at 00.40
        [DataRow(70, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, })]  // Train arrives at 01.10
        [DataRow(95, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, })]  // Train arrives at 01.35
        [DataRow(600, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // Train arrives at 10.00
        [DataRow(750, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // Train arrives at 12.30
        [DataRow(840, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // Train arrives at 14.00
        [DataRow(900, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]  // Train arrives at 15.00
        [DataRow(1260, new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]  // Train arrives at 21.00
        [DataRow(1430, new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, })]  // Train arrives at 23.50

        public void ScalarEncoderUnitTestTrainAvailability(double input, int[] expectedResult)

        {
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 21},
{ "N", 72},
{ "MinVal", (double)0.0},
{ "MaxVal", (double)1440.0},
{ "Periodic", true},
{ "Name", "Train Availability"},
{ "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result));   // Assert.IsTrue is used to check whether the given input result matches with the expected result.
        }



        [TestMethod]
        [TestCategory("Prod")]
        // <summary>
        // Problem: We have got the data for Power consumption(In Kilowatts) for one particular month. The data has been further divided according to date and time(24 hours clock).
        // In total we have 744 readings for the entire month.The encoding parameters has to be chosen in a such way that all 744 readings can be represented uniquely. Hence we have chosen N = 744.
        // The data has Minimum and Maximum power consumption of 4.7KW and 81.9KW respectively. Based on this we have chosen encoding parameter of MinVal ='4.7' and MaxVal = '81.9'
        // The MinVal for this is 4.7 and the MaxVal for this 81.9
        // Computing the Range = MaxVal – MinVal which is equal to 77.2. 
        // The number of bits that are set to encode a single value the ‘width’ of output signal ‘W’ used for representation is 7.
        // Total number of bits in the output ‘N’ used for representation is 744. 
        // As power consumption is non- periodic, resolution is calculated which is 0.104.
        // In the output to know from which bit the ones start, below formula will be used to calculate which is: [(Input-MinVal) +(Resolution /2)]/Resolution + Padding
        // Same process will be applied for all the values of power consumption.
        // Once the input has been encoded, we are calling the Bitmap method to show output in 2D Bitmap Format.
        // </summary>

        [DataRow(4.7, new int[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(16.8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(27.2, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(44.8, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(58.2, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(66.3, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(75.1, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(78.6, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, })]
        [DataRow(81.2, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, })]

        public void ScalarEncoderUnitTestPowerConsumption(double input, int[] expectedResult)
        {

            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 7},
{ "N", 744},
//{ "Radius", 1.5},
//{ "Resolution", 0.1},
{ "MinVal", (double)4.7},  // Minimum Power consumption is 4.7KW
{ "MaxVal", (double)81.9}, // Maximum Power consumption is 81.9KW
{ "Periodic", false},
{ "Name", "Power Consumption in December"},
{ "ClipInput", true},
});

            var result = encoder.Encode(input); // Encoding the input according to the encoding parameters.

            Debug.WriteLine(input);
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedResult));

            Assert.IsTrue(expectedResult.SequenceEqual(result)); // Assert.IsTrue is used to check whether the given imput result matches with the expected result.

        }


        /// <summary>
        /// Prints out all encoder values with their similarities.
        /// </summary>
        [TestMethod]
        public void ScalarEncodingTest()
        {
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 3},
{ "N", 10},
{ "MinVal", (double)0},
{ "MaxVal", (double)8},
{ "Periodic", false},
{ "Name", "Power Consumption in December"},
{ "ClipInput", true},
});

            string results = encoder.TraceSimilarities();

            Debug.WriteLine(results);
            Debug.WriteLine("");
            Debug.WriteLine(MathHelpers.SdrMem(7, 100));
            Debug.WriteLine(MathHelpers.SdrMem(10, 100));
            Debug.WriteLine(MathHelpers.SdrMem(15, 100));
            Debug.WriteLine(MathHelpers.SdrMem(20, 100));

            Debug.WriteLine("");
            Debug.WriteLine("All encoded values. No value should be identical.");
            Debug.WriteLine("");

            for (int i = 0; i < 9; i++)
            {
                Debug.WriteLine(Helpers.StringifyVector(encoder.Encode((double)i)));
            }

            PrintBitMap(encoder, nameof(ScalarEncodingTest));

        }

        /// <summary>
        /// Prints out all encoder values with their similarities.
        /// </summary>
        [TestMethod]
        public void ScalarEncodingHighDensityTest()
        {
            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 3},
{ "N", 10},
{ "MinVal", (double)0},
{ "MaxVal", (double)20},
{ "Periodic", false},
{ "Name", "Power Consumption in December"},
{ "ClipInput", true},
});

            string results = encoder.TraceSimilarities();

            Debug.WriteLine(results);
            Debug.WriteLine("");
            Debug.WriteLine(MathHelpers.SdrMem(7, 100));
            Debug.WriteLine(MathHelpers.SdrMem(10, 100));
            Debug.WriteLine(MathHelpers.SdrMem(15, 100));
            Debug.WriteLine(MathHelpers.SdrMem(20, 100));

            Debug.WriteLine("");
            Debug.WriteLine("All encoded values. Some values will have the same encoded bits.");
            Debug.WriteLine("");

            for (int i = 0; i < 20; i++)
            {
                Debug.WriteLine(Helpers.StringifyVector(encoder.Encode((double)i)));
            }

            PrintBitMap(encoder, nameof(ScalarEncodingHighDensityTest));

        }

        /// <summary>
        /// Prints out all encoder values with their similarities by encoding of the Day and Time in the combined SDR.
        /// </summary>
        [TestMethod]
        public void WeekTimeEncodingTest()
        {
            var folderName = Directory.CreateDirectory(nameof(WeekTimeEncodingTest)).Name;

            ScalarEncoder timeEncoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 5},
{ "N", 30},
{ "MinVal", (double)0},
{ "MaxVal", (double)24},
{ "Periodic", false},
{ "Name", "Time of the day."},
{ "ClipInput", true},
});

            Console.WriteLine("\nTime");
            Console.WriteLine(timeEncoder.TraceSimilarities());

            ScalarEncoder dayEncoder = new ScalarEncoder(new Dictionary<string, object>()
{
{ "W", 3},
{ "N", 10},
{ "MinVal", (double)1},
{ "MaxVal", (double)8},
{ "Periodic", false},
{ "Name", "Day of the week."},
{ "ClipInput", true},
});

            Console.WriteLine("\nDay of the Week");
            Console.WriteLine(dayEncoder.TraceSimilarities());

            string results = timeEncoder.TraceSimilarities();

            Dictionary<string, int[]> sdrDict = new Dictionary<string, int[]>();

            for (int day = 1; day < 8; day++)
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    var sdrHour = timeEncoder.Encode(hour);
                    var sdrDay = dayEncoder.Encode(day);
                    List<int> sdrDayTime = new List<int>();

                    sdrDayTime.AddRange(sdrDay);
                    sdrDayTime.AddRange(sdrHour);

                    string key = $"{day.ToString("00")}-{hour.ToString("00")}";

                    sdrDict.Add(key, sdrDayTime.ToArray());

                    var str = Helpers.StringifyVector(sdrDayTime.ToArray());

                    //int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(sdrDayTime.ToArray(), (int)Math.Sqrt(sdrDayTime.ToArray().Length), (int)Math.Sqrt(sdrDayTime.ToArray().Length));
                    //var twoDimArray = ArrayUtils.Transpose(twoDimenArray);

                    //NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, Path.Combine(folderName, $"{key}.jpg"), Color.Black, Color.Gray, text: $"{day}-{hour}".ToString());
                }
            }

            Console.WriteLine("\nDay-Time");

            Console.WriteLine(Helpers.TraceSimilarities(sdrDict));

            PrintBitMap(timeEncoder, nameof(ScalarEncodingTest));

        }


        /// <summary>
        /// Prints out the images of encoded values in the whole range.
        /// This Method is used by all the UnitTests to create a separate folder for each UnitTest cases and correspondingly generates Bitmap files in it.
        /// The Bitmap files contain 2D bitmap images(Pixel Images in .png format) that has all the encoded values from our UnitTest cases.
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="folderName"></param>
        public void PrintBitMap(ScalarEncoder encoder, string folderName)
        {
            string filename;
            Directory.CreateDirectory(folderName);
            Dictionary<string, int[]> sdrMap = new Dictionary<string, int[]>();

            List<string> inputValues = new List<string>();

            for (double i = (long)encoder.MinVal; i < (long)encoder.MaxVal; i++)
            {
                string key;

                inputValues.Add(key = getKey(i));

                var encodedInput = encoder.Encode(i);

                sdrMap.Add(key, ArrayUtils.IndexWhere(encodedInput, (el) => el == 1));

                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(encodedInput, (int)Math.Sqrt(encodedInput.Length), (int)Math.Sqrt(encodedInput.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);
                filename = i + ".png";

                NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, Path.Combine(folderName, filename), Color.Black, Color.Gray, text: i.ToString());
            }

            var similarities = MathHelpers.CalculateSimilarityMatrix(sdrMap);

            var results = Helpers.RenderSimilarityMatrix(inputValues, similarities);

            Debug.Write(results);
            Debug.WriteLine("");
        }


        private string getKey(double i)
        {
            return $"{i.ToString("000")}";
        }
    }
}