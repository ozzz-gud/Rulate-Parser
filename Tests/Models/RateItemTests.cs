using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Src.Models.Tests
{
    [TestClass()]
    public class RateItemTests
    {
        [TestMethod()]
        public void RateItemConstructor_EmptyString_InitedByZero()
        {
            // Arrange
            var expectedNumber = 0;
            var expectedTextValue = "0 / 0";
            // Act
            RateItem item = new RateItem("");
            // Assert
            Assert.AreEqual(expectedNumber, item.AverageRate);
            Assert.AreEqual(expectedNumber, item.PeopleVoted);
            Assert.AreEqual(expectedTextValue, item.ToString());
        }
        [TestMethod()]
        public void RateItemConstructor_StringByTemplate_InitedRight()
        {
            // Arrange
            var expectedRate = 4.26;
            var expectedCount = 50;
            var expectedTextValue = $"{expectedRate} / {expectedCount}";
            // Act
            RateItem item = new RateItem("     средняя 4.26, голосов 50");
            // Assert
            Assert.AreEqual(expectedRate, item.AverageRate);
            Assert.AreEqual(expectedCount, item.PeopleVoted);
            Assert.AreEqual(expectedTextValue, item.ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RateItemConstructor_Null_ThrowException()
        {
            // Arrange
            // Act
            _ = new RateItem(null);
            // Assert
        }
    }
}