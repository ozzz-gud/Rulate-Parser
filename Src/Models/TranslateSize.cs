using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace Src.Models
{
    public class TranslateSize
    {
        private static readonly Regex _regexChaptersAndPages = new Regex(@"(\d+ ?\d+)", RegexOptions.Compiled);
        private const string _zeroString = "0";
        private readonly MatchCollection _matches;

        /// <exception cref="System.ArgumentNullException"
        public TranslateSize(string textValue)
        {
            _matches = _regexChaptersAndPages.Matches(textValue);
            var value1 = _matches?.ElementAtOrDefault(0)?.Value.Replace(" ", "") ?? _zeroString;
            var value2 = _matches?.ElementAtOrDefault(1)?.Value.Replace(" ", "") ?? _zeroString;
            CountChapters = int.Parse(value1);
            CountPages = int.Parse(value2);
        }
        public TranslateSize() { }
        public int CountChapters { get; set; }
        public int CountPages { get; set; }
        public override string ToString()
        {
            return $"{CountChapters} глав / {CountPages} страниц";
        }
    }
}
