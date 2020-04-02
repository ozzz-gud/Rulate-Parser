using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.XPath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Src.Services
{
    public static class WebPageDownloader
    {
        static readonly HttpClient client = new HttpClient();
        static readonly HtmlParser parser = new HtmlParser();

        public static async Task<WebPage> Download(Uri url)
        {
            WebPage page = new WebPage
            {
                Url = url,
                Html = await DownloadAndParse(url)
            };
            await Unblock(page);
            Debug.WriteLine($"PageDownloaded {page.Id}");

            return page;
        }
        private static async Task Unblock(WebPage page)
        {
            var blocked = page.Html.Body.SelectSingleNode(Selectors.IsPageBlokedByAgeX) != null;
            if (blocked)
            {
                await IAmKnow(page.Id);
                page.Html = await DownloadAndParse(page.Url);
            }
        }
        private static async Task<IHtmlDocument> DownloadAndParse(Uri url)
        {
            var htmlCode = await client.GetStringAsync(url);
            return await parser.ParseDocumentAsync(htmlCode);
        }
        private static async Task IAmKnow(int bookUrl)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("path", $"/book/{bookUrl}"),
                new KeyValuePair<string, string>("ok", "Да")
            });
            _ = await client.PostAsync($"https://tl.rulate.ru/mature?path=/book/{bookUrl}", content);
        }
    }
}
