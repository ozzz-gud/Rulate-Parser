using AngleSharp.Html.Dom;
using AngleSharp.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Src.Services.Tests
{
    [TestClass()]
    public class IElementExtensionTests
    {
        private static IHtmlDocument html;
        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            var url = new Uri("https://tl.rulate.ru/book/87");
            html = WebPageDownloader.Download(url).Result.Html;
        }

        [TestMethod()]
        public void GetFollowingSiblingSequenceTest()
        {
            var sequence = html.Body.SelectSingleNode("//div[@id='Info']/div[@class='btn-toolbar']");
            var ps = sequence.GetFollowingSiblingSequence("p");

            Assert.AreEqual(26, ps.Count);
        }
        [TestMethod()]
        public void GetFollowingSiblingSequenceTest2()
        {
            var sequence = html.Body.SelectSingleNode("//div[@class='tools']/h5[contains(text(),'подписке')]/../dl/dt[contains(text(),'Абонемент')]");
            var ps = sequence.GetFollowingSiblingSequence("dd");

            Assert.AreEqual(2, ps.Count);
        }
    }
}