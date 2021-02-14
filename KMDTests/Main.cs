using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMD;
using NUnit.Framework;
using System.Text.Json;

namespace KMDTests
{
    public class Main
    {
        [TestCaseSource(nameof(SiteList)), Parallelizable]
        public async Task Start(Site site)
        {
            var manga = await MangaSite.GetMangaInfo(site.MangaUrl);
            Assert.AreEqual(site.MangaName, manga.Name);
            Assert.AreEqual(site.MangaCover, manga.Cover);
        }

        public static IEnumerable<TestCaseData> SiteList()
        {
            var file = File.ReadAllText("sites.json", Encoding.UTF8);
            var jsonByte = Encoding.UTF8.GetBytes(file);
            var sites = JsonSerializer.Deserialize<Site[]>(jsonByte,
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            return sites!.Select(x =>
            {
                var testCaseData = new TestCaseData(x);
                testCaseData.SetName(x.Name);
                return testCaseData;
            });
        }
    }

    public class Site
    {
        public string Name { get; set; }
        public string MangaUrl { get; set; }
        public string MangaName { get; set; }
        public string MangaCover { get; set; }
    }
}