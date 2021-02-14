using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KMD.Sites
{
    public class LeitorNet : MangaSite
    {
        protected override string Identifier => "//a[@class='brand-expanded'][@title='Leitor.net']";
        protected override string MangaNameXpath => "//div[@id='series-data']//h1";
        protected override string MangaCoverXpath => "//img[@class='cover']";
        protected override async Task<IEnumerable<Chapter>> GetMangaChaptersAsync(CancellationToken ct)
        {
            var list = new List<Chapter>();
            using var client = DefaultClient();
            for (var i = 1; i < 40; i++)
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"{BaseUri}/series/chapters_list.json?page={i}&id_serie={Url.Split('/').Last()}");
                request.Headers.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                var response = await client.SendAsync(request, ct);
                var jsonString = await response.Content.ReadAsStringAsync();
                var chapters = JsonConvert.DeserializeObject<dynamic>(jsonString)["chapters"];
                if (!(chapters is JArray))
                    break;
                for (var j = 0; j < chapters.Count; j++)
                {
                    var chapterTitle = (string)chapters[j]["number"];
                    IEnumerable<JProperty> obj = chapters[j]["releases"].Properties();
                    list.Add(new Chapter(chapterTitle, obj.First().Value["link"]!.Value<string>()));
                }
            }

            return list;
        }

        protected override async Task<IEnumerable<string>> GetChapterImagesAsync(CancellationToken ct)
        {
            var match = Regex.Match(
                Doc.DocumentNode.SelectSingleNode("//script[contains(@src, 'token')]").GetAttributeValue("src", null),
                @"token=(\w*)&id_release=(\d*)");
            var key = match.Groups[1].Value;
            var idRelease = match.Groups[2].Value;

            using var client = DefaultClient();
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{BaseUri}/leitor/pages/{idRelease}.json?key={key}");
            request.Headers.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
            var response = await client.SendAsync(request, ct);
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JObject>(jsonString)["images"]!.Value<JArray>().ToObject<List<string>>();
        }
    }
}
