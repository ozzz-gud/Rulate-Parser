using AngleSharp.Html.Dom;
using System;
using System.Linq;

namespace Src.Services
{
    public struct WebPage
    {
        public IHtmlDocument Html { get; set; }
        public Uri Url { get; set; }

        public WebPage(IHtmlDocument html, Uri url)
        {
            Html = html;
            Url = url;
        }
        public int Id
        {
            get => int.Parse(Url.Segments.Last().Replace("/", ""));
        }
    }
}
