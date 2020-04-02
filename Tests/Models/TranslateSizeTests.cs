using Microsoft.VisualStudio.TestTools.UnitTesting;
using Src.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Src.Models.Tests
{
    [TestClass()]
    public class TranslateSizeTests
    {
        [TestMethod()]
        public void TranslateSizeConstructor_EmptyString_InitedByZero()
        {
            // Arrange
            var expectedNumber = 0;
            var expectedTextValue = $"{expectedNumber} глав / {expectedNumber} страниц";
            // Act
            TranslateSize item = new TranslateSize("");
            // Assert
            Assert.AreEqual(expectedNumber, item.CountChapters);
            Assert.AreEqual(expectedNumber, item.CountPages);
            Assert.AreEqual(expectedTextValue, item.ToString());
        }
        [TestMethod()]
        public void RateItemConstructor_StringByTemplate_InitedRight()
        {
            // Arrange
            var expectedChapters = 165;
            var expectedPages = 1945;
            var expectedTextValue = $"{expectedChapters} глав / {expectedPages} страниц";
            // Act
            TranslateSize item = new TranslateSize("165 глав / 1 945 страниц");
            // Assert
            Assert.AreEqual(expectedChapters, item.CountChapters);
            Assert.AreEqual(expectedPages, item.CountPages);
            Assert.AreEqual(expectedTextValue, item.ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RateItemConstructor_Null_ThrowException()
        {
            // Arrange
            // Act
            _ = new TranslateSize(null);
            // Assert
        }
    }
}