using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;

namespace Src.Models
{
    public class RateItem
    {

        private static readonly Regex _regexGroupRateAndCount = new Regex(@"\d?[\.,]?\d+", RegexOptions.Compiled);
        private const string _zeroString = "0";
        private readonly MatchCollection _matches;

        /// <exception cref="System.ArgumentNullException"
        public RateItem(string textValue)
        {
            _matches = _regexGroupRateAndCount.Matches(textValue);
            var value1 = _matches?.ElementAtOrDefault(0)?.Value ?? _zeroString;
            var value2 = _matches?.ElementAtOrDefault(1)?.Value ?? _zeroString;
            AverageRate = double.Parse(value1, CultureInfo.InvariantCulture);
            PeopleVoted = int.Parse(value2);
        }
        public RateItem() { }

        public double AverageRate { get; set; }
        public int PeopleVoted { get; set; }

        public override string ToString()
        {
            return $"{AverageRate} / {PeopleVoted}"; ;
        }
    }
    public class Rate
    {
        public RateItem ForTitle { get; set; }
        public RateItem ForTranslate { get; set; }
        public RateItem ForVoice { get; set; }

        public Rate(string title, string translate, string voice)
        {
            ForTitle = Set(title);
            ForVoice = Set(voice);
            ForTranslate = Set(translate);
        }
        private static RateItem Set(string rate)
        {
            if (!string.IsNullOrEmpty(rate))
                return new RateItem(rate);
            return null;
        }
    }
}
