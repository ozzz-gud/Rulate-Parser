using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Src.Services.Tests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void ParseAllCatalogTest()
        {
            int firstPageNumber = 5;
            int lastPageNumber = 6;

            Parser parser = new Parser();
            var titles = parser.Parse(firstPageNumber, lastPageNumber).Result;

            Assert.AreEqual(100, titles.Count);
            foreach (var title in titles)
            {
                Console.WriteLine(title.Name);
            }
        }
    }
}