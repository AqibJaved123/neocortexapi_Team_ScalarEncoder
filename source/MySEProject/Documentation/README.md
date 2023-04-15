# **Project Title: ML22/23-1 Scalar Encoder with Buckets**

#### Group Name: Team_Scalar_Encoder

**Group Members**
1. Aqib Javed
2. Haris Abbas Qureshi
3. Saad Jamil

### Link to Project: [Team_Scalar_Encoder_Link](https://github.com/AqibJaved123/neocortexapi_Team_ScalarEncoder/tree/Team_ScalarEncoder)

**Summary of Project:**
===================================================
This project aimed to validate the effectiveness of the Scalar Encoder with Bucket as a new method for scalar value encoding in machine learning systems. Initially, the task was to integrate buckets into the Scalar Encoder in Neocortexapi, but a new version of Neocortexapi was released, and the method GetBucketIndex was already implemented by Prof. Dobric. Therefore, the team's task was to validate this new implementation by implementing various Unit Tests. The comprehensive testing showed that the Scalar Encoder with Bucket can accurately encode numerical values, while also being robust to noise and varying degrees of sparsity.The findings suggest that the Scalar Encoder with Bucket is a valuable encoding method that can improve the performance of machine learning algorithms and contribute to our understanding of the brain. The project provides a framework for further testing and development of this new method.

# **Introduction**


## **1. Encoder**

Encoders are used to convert raw input data into Sparse Distributed Representations (SDRs), which can be processed by Hierarchical Temporal Memory (HTM) networks. Different types of encoders are used to encode different types of data, including Scalar Encoders for continuous scalar values, Category Encoders for categorical values, Date Encoders for dates and times, and Coordinate Encoders for spatial coordinates.

## **2. Scalar Encoder**

The Scalar Encoder is a type of encoding method used in Hierarchical Temporal Memory (HTM) to represent scalar data. It is a simple yet powerful method that splits a range of values into smaller sub-ranges and maps them to a set of active bits. This results in a Sparse Distributed Representation (SDR) that provides a compact and efficient way to represent scalar data. To encode a value with Scalar Encoder, we first choose the range of values we want to represent, such as temperature or speed. Then we divide this range into smaller sub-ranges and map each sub-range to a set of active bits. The number of active bits in each representation can be adjusted based on the desired level of precision.
 For example, let's say we want to represent the temperature of a room that can range from 0 to 100 degrees Fahrenheit. We divide this range into 100 sub-ranges and map each sub-range to a set of 20 active bits. This results in an SDR of 2000 bits, with 20 active bits representing each sub-range. Suppose the temperature in the room is 72 degrees Fahrenheit. We map this value to the corresponding sub-range, which is represented by a set of 20 active bits. The remaining bits are inactive, resulting in an SDR with high sparsity and show below. 

                               ……00000000000000111111111111111111000000000….

The Scalar Encoder is a flexible encoding method that can be used to represent a wide range of scalar data. It is particularly useful for encoding data with high dimensionality, such as time-series data. The resulting SDRs are compact, efficient, and can be easily used in machine learning models to make predictions

## **3. Scalar Encoder with Bucket**

The Scalar Encoder with Bucket is an extension of the standard Scalar Encoder that used in Hierarchical Temporal Memory (HTM) systems. The Bucket Encoder adds an extra level of flexibility by allowing for encoding of values that may fall outside the defined range. To use the Bucket Encoder, the range of values to be encoded is still defined by minimum and maximum values, but then divided into a number of equally sized buckets. Each bucket represents a range of values and is assigned a unique Sparse Distributed Representation (SDR) of active and inactive bits. The resulting encoding provides a more granular representation of the input values. 

*Here are the generals steps for encoding a value with this approach but there is little difference when we consider periodic and non periodic behaviour of SDR.*

                    i)	Choose the range of values that you want to be able to represent, minVal and maxVal. 
                   ii)	Compute the Range

                             Range = maxVal - minVal. 

                  iii)	Choose total number of bits representing SDR n. 
                   iv)	Choose the number of active bits to have in each representation, w.
                    v)	Compute the total number of buckets, b:

                              b=n-w+1

                  vi)	For a given value, v, determine the bucket, i, that it falls into: 

                            i =floor(b*(v-minVal)/Range)

                 vii)	Create the encoded representation by starting with n unset bits and then set the w consecutive bits starting at index i to active.

_**Here is an example of encoding the outside temperature for a location where the temperature varies between 0℉ and 20℉ using a Scalar Encoder with Bucket with non-periodic SDR:**_

<center>

| Parameter  | Variable  | Value |
| :------------ |:---------------:| -----:|
| Minimum Value      | minVal | 0 |
| Maximum Value      | maxVal        |   20 |
| Range | R        |    20 |
| Number of Bits| n       |    20 |
| Width| w       |    5 |
</center>

The total number of buckets  is computed to be 

``` console
                           b = 20-5+1 = 16
```


Now we can compute the bucket index for the Temperature value 10℉ by using these expressions:

```console
                           Resolution = (MaxVal - MinVal) / (N - W)
                           RangeInternal = MaxVal - MinVal
                           HalfWidth = (W - 1) / 2
                           Padding = HalfWidth
                           Range = RangeInternal + Resolution
                           x = float (((input - MinVal) + Resolution / 2) / Resolution) + Padding

                           bucket_index = x - HalfWidth
```

                               bucket_index = 8

And the representation will be 20 bits with 5 consecutive active bits starting at the 8th bit and shown below:

$$
00000001111100000000
$$
## **4. Importance of Buckets**

Buckets are a crucial element of the scalar encoder with bucket approach. They allow for the efficient representation of continuous values in a binary format. By splitting the range of values into discrete buckets, each bucket can be mapped to a set of active bits in the binary representation. This mapping allows for efficient computation and storage, as only a small number of bits need to be activated to represent each value. 

The number of buckets chosen for a particular implementation determines the granularity of the encoding. A larger number of buckets will result in a more finely grained representation, while a smaller number of buckets will result in a coarser representation. The choice of the number of buckets should be made based on the application's requirements, balancing the need for precision with the computational and storage costs of a more fine-grained encoding. 

In addition to the number of buckets, the size of each bucket is also a critical parameter in the scalar encoder with bucket approach. By adjusting the size of the buckets, the range of values that can be represented can be tailored to the needs of the application. A smaller bucket size allows for a more precise representation of values within the range, while a larger bucket size can improve the encoding's robustness to noise and variability in the input. 

The use of buckets in the scalar encoder with bucket approach allows for an efficient and flexible representation of continuous values in a binary format, making it a powerful tool for a wide range of applications in machine learning and artificial intelligence.

# **Methods**
 The main methods used in this project are `GetBucketIndex()` and `ScalarEncoder()`. Below you can find the snippet of these methods.
## **ScalarEncoder()**

```c#
public ScalarEncoder(Dictionary<string, object> encoderSettings)
        {
            this.Initialize(encoderSettings);
        }
```
The constructor takes in a dictionary object called `encoderSettings` as its parameter and initializes the object by calling the "Initialize" method with the `encoderSettings` as its argument. The purpose of this constructor is to set up the object's initial state when it is first created.


The following code snippet shows an example of how to create a new instance of the ScalarEncoder

```c#
ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
{
    { "W", 3},
    { "N", 14},
    { "MinVal", (double)0},
    { "MaxVal", (double)11},
    { "Periodic", false},
    { "Name", " Month of the Year"},
    { "ClipInput", true},
});
```
Here is an explanation of the different settings:


- **W**: This parameter sets the number of active bits used to encode each value. In this case, it is set to 3.

- **N**: This parameter sets the total number of bits used to represent the encoder's output. In this case, it is set to 14.

- **MinVal**: This parameter sets the minimum value that the encoder will encode. In this case, it is set to 0.

- **MaxVal**: This parameter sets the maximum value that the encoder will encode. In this case, it is set to 11.

- **Periodic**: This parameter determines whether the encoder's output should be treated as periodic (i.e., wrapping around when it reaches the maximum value). In this case, it is set to false, meaning that the encoder's output is not periodic.

- **Name**: This parameter is just a string that identifies the encoder. In this case, it is set to "Month of the Year".

- **ClipInput**: This parameter determines whether input values outside of the encoder's range should be clipped to the minimum or maximum value. In this case, it is set to true, meaning that input values outside of the range will be clipped to the minimum or maximum value.

## **GetBucketIndex()**
`GetBucketIndex()` is the main method that integrates buckets concept into Scalar Encoder. Below is the code snippet of this method.

```c#
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
```

The `GetFirstOnBit` method in the ScalarEncoder class retrieves the index of the first non-zero bit for a given input. It performs various checks on the input value to ensure it falls within the expected range and handles any out-of-range values according to the ClipInput and Periodic properties. The `GetBucketIndex()` method, which relies on `GetFirstOnBit`, returns the bucket index of the given input. 


 # **Unit Tests**

 In order to validate the effectiveness and accuracy of the Scalar Encoder with Bucket method, we conducted a comprehensive set of unit tests covering various aspects of the encoding process. These tests aimed to evaluate the performance of the encoder under different conditions, such as varying levels of sparsity and noise, and to assess its ability to generalize to unseen data.

The unit tests included encoding of random numerical values, testing the robustness of the encoder to noise and sparsity, evaluating its ability to generalize to unseen data, and testing for edge cases and potential errors.

Overall, the unit tests demonstrated that the Scalar Encoder with Bucket is a highly accurate and effective method for encoding scalar values. Additionally, the tests showed that the Scalar Encoder with Bucket can be used to encode a wide range of numerical values, from small integers to large real numbers, with varying levels of precision and granularity.

## **How to Execute Unit Tests**

Our all Unit Tests are located in  [ScalarEncoderExperimentalTests.cs](https://github.com/AqibJaved123/neocortexapi_Team_ScalarEncoder/blob/44e0d5f65b0f2fa5e5ac0a1b3577903a0f610abf/source/UnitTestsProject/EncoderTests/ScalarEncoderExperimentalTests.cs) file and our all Unit Test name starts with `ScalarEnoderWithBucket...`**

The screenshot displays all the test cases that were conducted to validate the scalar encoder with bucket as an encoding method for representing numerical data.

![2](https://user-images.githubusercontent.com/116574834/228849381-4a6fceae-d94d-4ff8-96a3-aafd37800d3f.png)

### **Steps**
- Go to Test Class [ScalarEncoderExperimentalTests.cs](https://github.com/AqibJaved123/neocortexapi_Team_ScalarEncoder/blob/44e0d5f65b0f2fa5e5ac0a1b3577903a0f610abf/source/UnitTestsProject/EncoderTests/ScalarEncoderExperimentalTests.cs) 
- Then goto `ScalarEncoderScalarEncoderExperimentalTestsTests` 
- Here you will find all the unit test implemented by us.


## **Results**
Below is the result of `ScalarEncoderWithBucketTemperatureRanges` unit test after the sucesssful execution.

![complete Screenshort](https://user-images.githubusercontent.com/116574834/228849785-48680466-e40c-4b8d-8547-5abfc1c1ef82.png)

Here in the output window different values are shown and their details are gievn below:

- Encoded Value : The numeric value which is encoded by the encoder
- Expected Bucket : The bucket numeber calculated manually
- Actual Bucket : The bucket number assigned by the code
- Expected Value: Manually computed SDR
- Prdicted Value : SDR Computed by the Code

![3](https://user-images.githubusercontent.com/116574834/228849897-02a911c5-2f06-4de3-8a76-96d9ac10070e.png)

# Conclusion 
This project validates the scalar encoder with bucket as an encoding method for numerical data. The encoder is shown to be accurate, flexible, and scalable, making it suitable for a variety of applications. The effectiveness of the encoder was validated through several unit tests, including encoding of random numerical values and testing its robustness to noise and sparsity. Overall, the scalar encoder with bucket has the potential to be a powerful tool for encoding numerical data.

# References
[1]. A. Lavin, S. Ahmad, and J. Hawkins, “Sparse distributed representations,” Numenta.com, 2022. [Online]. Available: https://www.numenta.com/assets/pdf/biological-and-machine-intelligence/BaMI-SDR.pdf. [Accessed: 29-Mar-2023].
[2]. S. Purdy, Numenta.com. [Online]. Available: https://www.numenta.com/assets/pdf/biological-and-machine-intelligence/BaMI-Encoders.pdf. [Accessed: 29-Mar-2023].
[3].“NeoCortexAPI,” Github.io. [Online]. Available: https://ddobric.github.io/neocortexapi/. [Accessed: 29-Mar-2023].

 
 