using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Src.Services
{
    public static class Exporter
    {
        public static void Save<T>(List<T> titles, string pathToFile)
        {
            var json = JsonConvert.SerializeObject(titles, Formatting.None, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText(pathToFile, json);
        }
        public static List<T> Load<T>(string pathToFile)
        {
            try
            {
                var json = File.ReadAllText(pathToFile);
                return JsonConvert.DeserializeObject<List<T>>(json, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            catch (FileNotFoundException)
            {
                return new List<T>();
            }
        }
    }
}
