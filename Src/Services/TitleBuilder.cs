using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.XPath;
using Src.Models;
using Src.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Src.Services
{
    public static class TitleBuilder
    {
        public static Title Get(IHtmlDocument html, Uri url)
        {
            try
            {
                GetThisUrl(html);
            }
            catch (Exception)
            {
                return new Title()
                {
                    BLOCKED = true,
                    Url = url,
                };
            }
            return new Title()
            {
                Url = url,
                Name = ParseName(html),
                Is18 = ParseIs18(html),
                Type = ParseType(html),
                Rate = ParseRate(html),
                Likes = ParseCountLikes(html),
                Author = ParseAuthor(html),
                Publisher = ParsePublisher(html),
                PublishStatus = ParsePublishStatus(html),
                PublishYear = ParsePublishYear(html),
                Team = ParseTeam(html),
                AnotherNames = ParseAnotherNames(html),
                Links = ParseLinks(html),
                PeopleVoices = ParsePeopleVoices(html),
                BasePrice = ParseBasePrice(html),
                PriceToReadAllChapters = ParseChaptersPrice(html),
                Images = ParseImages(html),
                Description = ParseDescription(html),
                Genre = ParseGenres(html),
                Tags = ParseTags(html),
                TranslateStatus = ParseTranslateStatus(html),
                AverageChapterSizeInChars = ParseAverageChapterSizeInChars(html),
                TranslateSize = ParseTranslateSize(html),
            };
        }

        public static readonly Uri baseUri = new Uri("https://tl.rulate.ru/");
        private static Uri GetThisUrl(IHtmlDocument html)
        {
            var elem = html.QuerySelector(Selectors.TitleUrl);
            if (elem != null)
            {
                var partUrl = elem.GetAttribute("href");
                return new Uri(baseUri, partUrl);
            }
            throw new Exception("Доступ Запрещен");
        }
        private static string ParseName(IHtmlDocument html)
        {
            return html.QuerySelector(Selectors.TitleName).TextContent;
        }
        private static bool ParseIs18(IHtmlDocument html)
        {
            return html.Body.SelectSingleNode(Selectors.TitleIsFor18X) != null;
        }
        private static string ParseType(IHtmlDocument html)
        {
            return html.QuerySelector(Selectors.Type).TextContent;
        }
        private static Rate ParseRate(IHtmlDocument html)
        {
            var strRateOfTitle = html.Body.SelectSingleNode(Selectors.RateOfTitleX)?.TextContent;
            var strRateOfTranslate = html.Body.SelectSingleNode(Selectors.RateOfTranslateX)?.TextContent;
            var strRateOfVoice = html.Body.SelectSingleNode(Selectors.RateOfVoicesX)?.TextContent;
            return new Rate(strRateOfTitle, strRateOfTranslate, strRateOfVoice);
        }
        private static int ParseCountLikes(IHtmlDocument html)
        {
            var elem = html.QuerySelector(Selectors.CountLikes);
            if (elem != null)
            {
                var str = elem.TextContent;
                return int.Parse(str);
            }
            return 0;
        }
        private static NameAndLink ParseAuthor(IHtmlDocument html)
        {
            return ParseNameAndLink(html, Selectors.AuthorX);
        }
        private static NameAndLink ParsePublisher(IHtmlDocument html)
        {
            return ParseNameAndLink(html, Selectors.PublisherX);
        }
        private static NameAndLink ParseTeam(IHtmlDocument html)
        {
            return ParseNameAndLink(html, Selectors.TeamX);
        }
        private static NameAndLink ParseNameAndLink(IHtmlDocument html, string xpathSelector)
        {
            var elem = html.Body.SelectSingleNode(xpathSelector);
            if (elem != null)
            {
                var url = (elem as IElement).GetAttribute("href");
                return new NameAndLink()
                {
                    Name = elem.TextContent,
                    Link = new Uri(baseUri, url),
                };
            }
            return null;
        }

        private static string ParsePublishStatus(IHtmlDocument html)
        {
            return html.Body.SelectSingleNode(Selectors.PublishStatusX)?.TextContent;
        }
        private static int ParsePublishYear(IHtmlDocument html)
        {
            var elem = html.Body.SelectSingleNode(Selectors.PublishYearX);
            if (elem != null)
            {
                if (int.TryParse(elem.TextContent, out int result))
                {
                    return result;
                }
            }
            return 0;
        }
        private static List<string> ParseAnotherNames(IHtmlDocument html)
        {
            var elems = html.Body.SelectNodes(Selectors.AnotherTitleNamesX);
            if (elems.Count != 0)
            {
                var anotherNames = new List<string>();
                foreach (var elem in elems)
                {
                    anotherNames.Add(elem.TextContent);
                }
                return anotherNames;
            }
            return new List<string>();
        }
        private static List<Uri> ParseLinks(IHtmlDocument html)
        {
            var elems = html.Body.SelectNodes(Selectors.LinksX);
            if(elems.Count != 0)
            {
                var links = new List<Uri>();
                foreach (var elem in elems)
                {
                    var url = (elem as IElement).GetAttribute("href").Replace(Selectors.ProxyString, string.Empty);
                    links.Add(new Uri(url));
                }
                return links;
            }
            return new List<Uri>();
        }
        private static int[] ParsePeopleVoices(IHtmlDocument html)
        {
            var Key = html.Body.SelectNodes(Selectors.KeyPeopleVoicesX);
            var voices = html.Body.SelectNodes(Selectors.CountPeoplesVoicesX);
            if(voices.Count == Key.Count && voices.Count != 0)
            {
                var peoplesVoices = new int[5] { 0, 0, 0, 0, 0 };
                for (int i = 0; i < Key.Count; i++)
                {
                    var key = int.Parse(Key[i].TextContent);
                    var value = int.Parse(voices[i].TextContent);
                    peoplesVoices[key-1] = value;
                }
                return peoplesVoices;
            }
            return new int[] { 0 };
        }
        private static double ParseBasePrice(IHtmlDocument html)
        {
            var elem = html.Body.SelectSingleNode(Selectors.BaseChapterPriceX);
            if (elem != null)
            {
                var strPrice = elem.TextContent;
                return strPrice.Length != 0 ? double.Parse(strPrice, CultureInfo.InvariantCulture) : 0;
            }
            return 0;

        }
        private static double ParseChaptersPrice(IHtmlDocument html)
        {
            List<double> prices = new List<double>();
            var elems = html.QuerySelectorAll(Selectors.ChaptersPrice);
            foreach (var elem in elems)
            {
                var strPrice = elem.GetAttribute("data-price");
                if (strPrice != null && strPrice.Length != 0)
                {
                    var intPrice = double.Parse(strPrice, CultureInfo.InvariantCulture);
                    prices.Add(intPrice);
                }
            }
            return prices.Sum();
        }
        private static List<Uri> ParseImages(IHtmlDocument html)
        {
            var elems = html.QuerySelectorAll(Selectors.Images);
            var links = from elem in elems
                        select new Uri(baseUri, elem.GetAttribute("src"));
            return links.Distinct().ToList();
        }
        private static List<string> ParseDescription(IHtmlDocument html)
        {
            var anchor = html.Body.SelectSingleNode(Selectors.DescriptionAfterThisX);
            return anchor.GetFollowingSiblingSequenceText("p");
        }

        private static List<string> ParseGenres(IHtmlDocument html)
        {
            return ParseGenresAndTags(html, Selectors.GenresX);
        }
        private static List<string> ParseTags(IHtmlDocument html)
        {
            return ParseGenresAndTags(html, Selectors.TagsX);
        }
        private static List<string> ParseGenresAndTags(IHtmlDocument html, string xpathSelector, string separator = ", ")
        {
            var strGenres = html.Body.SelectSingleNode(xpathSelector)?.TextContent;
            if(strGenres != null && strGenres.Length != 0)
                return strGenres.Split(separator).ToList();
            return new List<string>();
        }

        private static string ParseTranslateStatus(IHtmlDocument html)
        {
            var elem = html.Body.SelectSingleNode(Selectors.TranslateStatusX).TextContent;
            return elem?.ToLower();
        }
        private static int ParseAverageChapterSizeInChars(IHtmlDocument html)
        {
            var elem = html.Body.SelectSingleNode(Selectors.AverageChapterSizeX).TextContent;
            elem = elem.Replace(" символов", "").Replace(" ","");
            return int.Parse(elem);
        }
        private static TranslateSize ParseTranslateSize(IHtmlDocument html)
        {
            var elem = html.Body.SelectSingleNode(Selectors.TranslateSizeX).TextContent;
            var str = Regex.Replace(elem, @"\s{2,}", "").Trim();
            return new TranslateSize(str);
        }
    }
}
