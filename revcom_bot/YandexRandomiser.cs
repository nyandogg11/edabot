using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace edabot
{
    public class YandexRandomiser
    {
        public static async Task<String> GetRandomSearchResult(string url)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(url));
                var responseToString = response.Content.ReadAsStringAsync();
                JObject parsedResponse = JObject.Parse(responseToString.Result);
                var searchResults = parsedResponse["features"].SelectMany(t => t["properties"]["CompanyMetaData"]["name"].Parent).Values().ToList();
                var randomResult = searchResults[new Random().Next(0, searchResults.Count)].ToString();
                if (randomResult == "KillFish")
                {
                    randomResult = "Ты пидор";
                }
                return randomResult;
            }
            catch (Exception)
            {
                return "Что-то пошло не так";
            }
        }
    }
}
