using AngleSharp.Html.Dom;
using Microsoft.EntityFrameworkCore;
using Src.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Services
{
    public class Parser
    {
        public List<Title> Titles { get; private set; }
        public List<Uri> UrisToTitles { get; set; }
        public Parser(string pathToTitlesFile = null, string pathToTitlesUris = null)
        {
            Titles = !string.IsNullOrEmpty(pathToTitlesFile)
                ? Exporter.Load<Title>(pathToTitlesFile)
                : new List<Title>();

            UrisToTitles = !string.IsNullOrEmpty(pathToTitlesUris)
                ? Exporter.Load<Uri>(pathToTitlesUris)
                : new List<Uri>();
        }

        private const string URL_TEMPLATE_FOR_CATALOG_PAGE = "https://tl.rulate.ru/search/index/cat/2/Book_page/{0}";
        private static Uri GetCatPageUrl(int catPageNumber)
        {
            return new Uri(string.Format(URL_TEMPLATE_FOR_CATALOG_PAGE, catPageNumber));
        }
        private static List<Uri> GetCatPageUrls(int firstPageNumber, int lastPageNumber)
        {
            var allCatUrls = new List<Uri>();
            for (int number = firstPageNumber; number <= lastPageNumber; number++)
            {
                var uri = GetCatPageUrl(number);
                allCatUrls.Add(uri);
            }
            return allCatUrls;
        }
        private static List<Uri> GetTitlesUrl(IEnumerable<WebPage> catPages)
        {
            var eListUrls = catPages.Select(catPage => GetTitlesUrl(catPage));

            List<Uri> allTitlesUrls = new List<Uri>();
            foreach (var list in eListUrls)
                allTitlesUrls.AddRange(list);

            return allTitlesUrls;
        }
        private static List<Uri> GetTitlesUrl(WebPage catPage)
        {
            var elems = catPage.Html.QuerySelectorAll(Selectors.AllTitlesOnPage);
            List<Uri> titleUrls = new List<Uri>();
            foreach (var elem in elems)
            {
                var partUrl = elem.GetAttribute("href");
                var fullUrl = new Uri(TitleBuilder.baseUri, partUrl);
                titleUrls.Add(fullUrl);
            }
            return titleUrls;
        }
        private void RemoveParsedTitles(List<Uri> linksToTitles)
        {
            var parsedUrls = Titles.Select(t => t.Url);
            var unicAndNotParsed = linksToTitles
                .Where(link => !parsedUrls.Contains(link))
                .Distinct()
                .ToList();
        }
        public static int GetLastCatPageNumber(WebPage catPage)
        {
            var href = catPage.Html.QuerySelector("#yw0 > li.last > a").GetAttribute("href");
            var pageNumber = href.Split("/").Last();
            return int.Parse(pageNumber);
        }


        public async Task<List<Title>> Parse(int firstPageNumber = 1, int lastPageNumber = 1)
        {
            if(!UrisToTitles.Any())
            {
                List<Uri> catUrls = GetCatPageUrls(firstPageNumber, lastPageNumber);

                Console.WriteLine($"{DateTime.Now} ||Начата загрузка CatPages");
                UrisToTitles = await GetTitlesUrlsFromCatPages(catUrls);
                Console.WriteLine($"{DateTime.Now} ||Загрузка CatPages Завершена");
                RemoveParsedTitles(UrisToTitles);
                Console.WriteLine($"{DateTime.Now} ||Обнаружено {UrisToTitles.Count} новых тайтлов");

                Exporter.Save(UrisToTitles, "temp.json");
            }

            await DownloadTitles(UrisToTitles);

            return Titles;
        }
        private const int TAKE_COUNT = 50;
        private const int DELAY = 2000; // 2 seconds
        private const int DELAY_IF_EXCEPTION = 10000; // 10 seconds
        private async Task<List<Uri>> GetTitlesUrlsFromCatPages(List<Uri> catUrls)
        {
            List<Uri> urlsForTitles = new List<Uri>();
            var catPagesUrls = catUrls.Take(TAKE_COUNT);
            int skipTitles = TAKE_COUNT;

            var downloadTasks = catPagesUrls.Select(link => WebPageDownloader.Download(link)).ToList();
            while(downloadTasks.Any())
            {
                Task<WebPage> _task = null;
                try
                {
                    _task = await Task.WhenAny(downloadTasks);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} ||Error Message:: {ex.Message}");
                    await Task.Delay(DELAY_IF_EXCEPTION);
                    _task.Start();
                }
                finally
                {
                    urlsForTitles.AddRange(GetTitlesUrl(_task.Result));
                    Console.WriteLine($"{DateTime.Now} ||CatPages {_task.Result.Id} Downloaded");
                    downloadTasks.Remove(_task);

                    await Task.Delay(DELAY);
                    var newTask = catUrls.Skip(skipTitles).Take(1).Select(link => WebPageDownloader.Download(link));
                    skipTitles += 1;
                    downloadTasks.AddRange(newTask);
                }
            }

            return urlsForTitles;
        }
        private async Task DownloadTitles(List<Uri> AllTitlesUrls)
        {
            var urlsForTitles = AllTitlesUrls.Take(TAKE_COUNT);
            int skipTitles = TAKE_COUNT;

            var downloadTasks = urlsForTitles.Select(link => WebPageDownloader.Download(link)).ToList();
            while (downloadTasks.Any())
            {
                Task<WebPage> _task = null;
                try
                {
                    _task = await Task.WhenAny(downloadTasks);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} ||Error Message:: {ex.Message}");
                    await Task.Delay(DELAY_IF_EXCEPTION);
                    _task.Start();
                }
                finally
                {
                    Titles.Add(TitleBuilder.Get(_task.Result.Html, _task.Result.Url));
                    Console.WriteLine($"{DateTime.Now} ||Добавлен новый тайтл. ВСЕГО:{Titles.Count}");
                    downloadTasks.Remove(_task);

                    await Task.Delay(DELAY);
                    var newTask = AllTitlesUrls.Skip(skipTitles).Take(1).Select(link => WebPageDownloader.Download(link));
                    skipTitles += 1;
                    downloadTasks.AddRange(newTask);
                }
            }
        }
    }
}
