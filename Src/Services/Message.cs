using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Src.Models;

namespace Src.Services
{
    public static class Message
    {
        private static readonly string n = Environment.NewLine;
        private static readonly string nn = Environment.NewLine + Environment.NewLine;

        public static string Get(Title title)
        {
            if (title.BLOCKED)
            {
                return "BLOCKED";
            }
            return GenerateMessage(title);
        }
        public static string Tag(string tag)
        {
            if(!string.IsNullOrEmpty(tag))
            {
                tag = Regex.Replace(tag, @"\s+", "_");
                tag = Regex.Replace(tag, @"\W", "");
                return $"#{tag}@catrun";
            }
            return "";
        }
        public static string GetTagsInString(IEnumerable<string> tagsOrGenreList)
        {
            var tags = tagsOrGenreList?.Select(t => Tag(t));
            if(tags!=null)
                return string.Join(" ", tags);
            return string.Empty;
        }
        public static string GetRateInString(Rate rate)
        {
            if(rate!=null)
            {
                var title = rate.ForTitle != null ? $"Качество произведения: {rate.ForTitle?.ToString()}{n}" : null;
                var translate = rate.ForTranslate != null ? $"Качество перевода: {rate.ForTranslate?.ToString()}{n}" : null;
                var voice = rate.ForVoice != null ? $"Качество озвучки: {rate.ForVoice?.ToString()}{n}" : null;

                return string.Concat(title, translate, voice).Trim();
            }
            return string.Empty;
        }
        public static string GetAdultInString(bool is18)
        {
            return is18 ? $"🔞Да {Tag("ДА18")}" : $"Нет {Tag("НЕТ18")}";
        }
        public static string GetPriceInString(double price)
        {
            return price == 0 ? Tag("free") : $"{price} руб.";
        }
        public static string GetPublishStatus(string status)
        {
            if(!string.IsNullOrEmpty(status))
            {
                var subString = status[0].ToString().ToUpper() + status.Substring(1);
                subString = subString.Replace("ё", "е");
                return Tag($"Выпуск{subString}");
            }
            return null;
        }
        public static string GetTranslateStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return string.Empty;

            if (status.Contains("в работе"))
                return Tag("ПереводВРаботе");
            else if (status.Contains("заброшен"))
                return Tag("ПереводЗаброшен");
            else if (status == "завершён")
                return Tag("ПереводЗавершен");
            else if (status.Contains("завершён"))
                return Tag("ПереводВродеЗавершен");
            else if (status.Contains("перерыв"))
                return Tag("ПереводНаПерерыве");
            else if (status.Contains("ожидание новых глав"))
                return Tag("ПереводОжидаетНовыхГлав");

            return string.Empty;

        }
        public static string GenerateMessage(Title title)
        {
            return
                $"❗{title.Name}{nn}" +
                $"📔Жанры: {GetTagsInString(title.Genre)}{n}" +
                $"📔Теги: {GetTagsInString(title.Tags)}{n}" +
                $"📔18+: {GetAdultInString(title.Is18.Value)}{nn}" +
                $"{title.ShortDescription}{nn}" +
                $"{GetRateInString(title.Rate)}{n}" +
                $"Лайков: {title.Likes}{n}" +
                $"Цена полного прочтения: {GetPriceInString(title.PriceToReadAllChapters.Value)}{n}" +
                $"Выпуск: {GetPublishStatus(title.PublishStatus)}{n}" +
                $"Состояние перевода: {GetTranslateStatus(title.TranslateStatus)}{n}" +
                $"Средний размер глав: {title.AverageChapterSizeInChars} символов{n}" +
                $"Размер перевода: {title.TranslateSize}{nn}" +
                title.Images?.FirstOrDefault() + n +
                title.Url.ToString();
        }
    }
}
