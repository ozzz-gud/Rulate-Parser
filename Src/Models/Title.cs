using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Src.Models
{
    public class Title
    {
        public List<string> Description { get; set; }
        [JsonIgnore]
        public string FullDescription
        {
            get
            {
                return Description != null
                    ? string.Join(" ", Description)
                    : null;
            }
        }
        [JsonIgnore]
        public string ShortDescription
        {
            get
            {
                return Description != null
                    ? string.Join(" ", Description.Take(5))
                    : null;
            }
        }

        public Uri Url { get; set; }

        public int ID
        {
            get => int.Parse(Url.Segments.Last().Replace("/", ""));
        }
        public int? Likes { get; set; }
        public int? AverageChapterSizeInChars { get; set; }
        public int? PublishYear { get; set; }
        public int? CountPurchasedChapters { get; set; }

        public bool? Is18 { get; set; }
        public bool BLOCKED { get; set; }


        public List<Uri> Images { get; set; }
        public List<Uri> Links { get; set; }
        public List<string> Genre { get; set; }
        public List<string> Tags { get; set; }
        public List<string> AnotherNames { get; set; }

        public int[] PeopleVoices { get; set; }

        public double? BasePrice { get; set; }
        public double? PriceToReadAllChapters { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public string PublishStatus { get; set; }
        public string TranslateStatus { get; set; }

        public TranslateSize TranslateSize { get; set; }
        public string Message
        {
            get
            {
                try
                {
                    return Services.Message.Get(this);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public NameAndLink Author { get; set; }
        public NameAndLink Publisher { get; set; }
        public NameAndLink Team { get; set; }
        public Rate Rate { get; set; }
    }
}
