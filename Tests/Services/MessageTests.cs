using Src.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Src.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Services.Tests
{
    [TestClass()]
    public class MessageTests
    {
        [TestMethod()]
        public void GetTagsInString_ListOfStrings_Tags()
        {
            var listString = new List<string>() { "hello", "wo rd", "the{}", "in (thd)" };
            var expectedTags = String.Join(
                " ",
                "#hello@catrun",
                "#wo_rd@catrun",
                "#the@catrun",
                "#in_thd@catrun");

            var tags = Message.GetTagsInString(listString);

            Assert.AreEqual(expectedTags, tags);
        }

        [TestMethod()]
        public void GetTagsInString_EmptyCollection_EmptyString()
        {
            var tags = Message.GetTagsInString(new List<string>());

            Assert.AreEqual("", tags);
        }
        [TestMethod()]
        public void GetTagsInString_Null_EmptyString()
        {
            var tags = Message.GetTagsInString(null);

            Assert.AreEqual("", tags);
        }

        [TestMethod()]
        public void GetRateInString_RateObject_RighString()
        {
            var expectedString = "Качество произведения: 4,23 / 491";
            var rate = new Rate("средняя 4.23, голосов 491", null, null);
            var message = Message.GetRateInString(rate);

            Assert.AreEqual(expectedString, message);
        }

        [TestMethod()]
        public void GetRateInString_RateObject2_RighString()
        {
            var expectedString =
                "Качество произведения: 4,23 / 491" +
                Environment.NewLine +
                "Качество озвучки: 4,25 / 300";
            Console.WriteLine(expectedString);

            var rate = new Rate("средняя 4.23, голосов 491", null, "средняя 4.25, голосов 300");
            var message = Message.GetRateInString(rate);

            Console.WriteLine(message);

            Assert.AreEqual(expectedString, message);
        }

        [TestMethod()]
        public void GetAdultInStringTest()
        {
            var expectedAdult = "🔞Да #ДА18@catrun";
            var expectedNotAdult = "Нет #НЕТ18@catrun";

            var adult = Message.GetAdultInString(true);
            var notAdult = Message.GetAdultInString(false);

            Assert.AreEqual(expectedAdult, adult);
            Assert.AreEqual(expectedNotAdult, notAdult);
        }

        [TestMethod()]
        public void GenerateMessageTest()
        {
            var listUrl = new List<Uri>()
            {
                new Uri("https://tl.rulate.ru/book/6180"),
                new Uri("https://tl.rulate.ru/book/87"),
                new Uri("https://tl.rulate.ru/book/8632"),
                new Uri("https://tl.rulate.ru/book/17435"),
                new Uri("https://tl.rulate.ru/book/9536"),
                new Uri("https://tl.rulate.ru/book/18685"),
                new Uri("https://tl.rulate.ru/book/543")
            };
            var tasks = listUrl.Select(url => WebPageDownloader.Download(url));
            Console.WriteLine("Download Tasks Created");

            var pages = Task.WhenAll(tasks).Result;
            Console.WriteLine("Pages Downloaded");

            var titles = pages.Select(p => TitleBuilder.Get(p.Html, p.Url));

            var messages = titles.Select(t => Message.Get(t));

            foreach (var str in messages)
            {
                Console.WriteLine(str);
            }
        }
    }
}