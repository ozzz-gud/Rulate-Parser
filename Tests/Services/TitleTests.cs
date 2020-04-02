using Microsoft.VisualStudio.TestTools.UnitTesting;
using Src.Models;
using System;

namespace Src.Services.Tests
{
    [TestClass()]
    public class TitleTests
    {
        [TestMethod()]
        public void TitleTest()
        {
            var url = new Uri("https://tl.rulate.ru/book/6180");
            var expectedAuthor = "Mo Xiang Tong Xiu";
            var expectedCountChapters = 82;
            var expectedCountPages = 1579;

            var page = WebPageDownloader.Download(url).Result;
            Title title = TitleBuilder.Get(page.Html, page.Url);


            Assert.AreEqual(expectedCountChapters, title.TranslateSize.CountChapters);
            Assert.AreEqual(expectedCountPages, title.TranslateSize.CountPages);
            Assert.AreEqual(expectedAuthor, title.Author.Name);
        }

        [TestMethod()]
        public void TitleTest1()
        {
            var url = new Uri("https://tl.rulate.ru/book/17435");
            var expectedImagesCount = 11;
            var expectedId = 17435;
            var page = WebPageDownloader.Download(url).Result;
            Title title = TitleBuilder.Get(page.Html, page.Url);

            Assert.AreEqual(expectedImagesCount, title.Images.Count);
            //Assert.AreEqual(expectedPrice, title.PriceToReadAllChapters);
            Assert.AreEqual(expectedId, title.ID);
        }

        [TestMethod()]
        public void TitleTest2()
        {
            var url = new Uri("https://tl.rulate.ru/book/173");
            
            var page = WebPageDownloader.Download(url).Result;
            Title title = TitleBuilder.Get(page.Html, page.Url);

            Assert.AreEqual("завершён", title.TranslateStatus);
            var status = Message.GetTranslateStatus(title.TranslateStatus);
            Assert.AreEqual("#ПереводЗавершен@catrun", status);
        }
    }
}