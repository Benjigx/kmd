using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KMD.Sites
{
    public class MangaDex : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'MangaDex')]";
        protected override string MangaNameXpath => "//span[@class='mx-1']";
        protected override string MangaCoverXpath => "//a[@title='See covers']/img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//a[contains(@href, '/chapter') and @class='text-truncate']",
                x => x
                    .InnerText
                    .Replace("Ch.", ""));

        protected override async Task<IEnumerable<string>> GetChapterImagesAsync(CancellationToken ct)
        {
            using var client = DefaultClient(Url);

            var response = await client.GetAsync(
                $"{BaseUri}/api/?id={Url.Split('/').Last()}&server=null&saver=1&type=chapter", ct);
            var obj = JsonConvert.DeserializeObject<ChapterInfo>(await response.Content.ReadAsStringAsync());
            return obj!.PageArray.Select(x => $"{obj.Server}{obj.Hash}/{x}");
        }

        [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public class ChapterInfo
        {
            public string Server { get; set; }
            public string Hash { get; set; }
            public List<string> PageArray { get; set; }
        }
    }
}