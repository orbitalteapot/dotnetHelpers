using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OrbitalHelpers;

namespace UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void StringExtensionsTests()
        {
            const string myString = "This is a test string";
            var foundChar = myString.ContainsChar('T');
            var lengthString = myString.ContainsNumberOfChar('t');
            Assert.IsTrue(foundChar);
            Assert.AreEqual(3, lengthString);
        }
    }

}