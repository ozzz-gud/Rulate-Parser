using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Src.Services.Tests
{
    [TestClass()]
    public class WebPageDownloaderTests
    {
        [TestMethod()]
        public void WebPageDownload_TitlePageAdult_Downloaded()
        {
            var url = new Uri("https://tl.rulate.ru/book/25500");
            var page = WebPageDownloader.Download(url).Result;

            var expectedTitle = "Mom's wet swimsuit / Мокрый купальник Мамы :: Tl.Rulate.ru - новеллы и ранобэ читать онлайн";
            Assert.AreEqual(expectedTitle, page.Html.Title);
        }
        [TestMethod()]
        public void WebPageDownload_TitlePageNotAdult_Downloaded()
        {
            var url = new Uri("https://tl.rulate.ru/book/96724");
            var page = WebPageDownloader.Download(url).Result;

            var expectedTitle = "Law of the Devil / Закон Дьявола :: Tl.Rulate.ru - новеллы и ранобэ читать онлайн";
            Assert.AreEqual(expectedTitle, page.Html.Title);
        }
        [TestMethod()]
        public void DownloadPage_CatPage_Downloaded()
        {
            var url = new Uri("https://tl.rulate.ru/search/index/cat/2/Book_page/1");
            var page = WebPageDownloader.Download(url).Result;

            var expectedTitle = "Поиск :: Tl.Rulate.ru - новеллы и ранобэ читать онлайн";

            Assert.AreEqual(expectedTitle, page.Html.Title);
        }
    }
}