# Project Title: ML22/23-1 Scalar Encoder with Buckets

#### Group Name: Team_Scalar_Encoder

**Group Members**
1. Aqib Javed
2. Haris Abbas Qureshi
3. Saad Jamil

### Link to Project: [Team_Scalar_Encoder_Link](https://github.com/AqibJaved123/neocortexapi_Team_ScalarEncoder/tree/Team_ScalarEncoder)

**Summary of Project:**
===================================================

Initially, our task was to integrate buckets into Scalar Encoder in Neocortexapi as done by the Numenta but during the 
the project, the new version of Neocortexapi was released, and Prof. Dobric implemented the method GetBucketIndex.
Then our task was to validate this new implementation and this was done by us implementing various Unit Tests.

 # Implementation of Unit Test
We have created Unit Tests methods for the Scalar Encoder with Buckets.
Unit test methods are
![2](https://user-images.githubusercontent.com/116574834/228849381-4a6fceae-d94d-4ff8-96a3-aafd37800d3f.png)




**Unit Tests location** :  [ScalarEncoderExperimentalTests.cs](https://github.com/AqibJaved123/neocortexapi_Team_ScalarEncoder/blob/44e0d5f65b0f2fa5e5ac0a1b3577903a0f610abf/source/UnitTestsProject/EncoderTests/ScalarEncoderExperimentalTests.cs)
# Results
The purpose of this test method is to validate the functionality of the Scalar Encoder with buckets with the periodic setting. The updated encoder (Scalar Encoder with buckets) encodes these values with buckets using the formulas of periodic input. The unit test takes numeric values, their encoded form, and the corresponding bucket number.
In this case, each DataRow attribute specifies a different input value and the expected encoded form of that value. The expected encoded forms are specified as arrays of integers, and the expected bucket numbers are specified as integers. The actual encoding and bucket calculation are performed using the formulas provided in the summary of the test method.
The purpose of the test method is to ensure that the Scalar Encoder with buckets with periodic settings is working correctly for a range of input values. The test cases are designed to cover a range of values and edge cases to ensure that the encoder is functioning correctly in all cases.
![complete Screenshort](https://user-images.githubusercontent.com/116574834/228849785-48680466-e40c-4b8d-8547-5abfc1c1ef82.png)

![3](https://user-images.githubusercontent.com/116574834/228849897-02a911c5-2f06-4de3-8a76-96d9ac10070e.png)

# Conclusion 
The execution of the scalar encoder with containers was effective, as it gave an adaptable and proficient strategy for encoding scalar qualities into meager circulated portrayals. The use of buckets made it possible to have more precise control over the encoding process, which made it easier to encode values with a lot of variation. Additionally, the encoder was able to encode cyclic values thanks to the use of periodicity.

# References
[1]. A. Lavin, S. Ahmad, and J. Hawkins, “Sparse distributed representations,” Numenta.com, 2022. [Online]. Available: https://www.numenta.com/assets/pdf/biological-and-machine-intelligence/BaMI-SDR.pdf. [Accessed: 29-Mar-2023].
[2]. S. Purdy, Numenta.com. [Online]. Available: https://www.numenta.com/assets/pdf/biological-and-machine-intelligence/BaMI-Encoders.pdf. [Accessed: 29-Mar-2023].
[3].“NeoCortexAPI,” Github.io. [Online]. Available: https://ddobric.github.io/neocortexapi/. [Accessed: 29-Mar-2023].

 
 