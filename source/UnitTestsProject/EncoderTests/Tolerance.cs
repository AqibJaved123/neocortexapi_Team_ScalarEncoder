// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace UnitTestsProject.EncoderTests
{
    internal class Tolerance
    {
        private double epsilon;

        public Tolerance(double epsilon)
        {
            this.epsilon = epsilon;
        }

        public bool AreEqual(double expected, double actual)
        {
            return Math.Abs(expected - actual) <= epsilon;
        }
    }
}
