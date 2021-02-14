using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class Zymk : MangaSite
    {
        protected override string Identifier => "//meta[@content='format=html5;url=//m.zymk.cn']";
        protected override string MangaNameXpath => "//h1[@class='title']";
        protected override string MangaCoverXpath => "//div[@class='comic-cover']/img";
        protected override string MangaCoverImgAttribute => "data-src";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters(
                "//ul[@id='chapterList']/li/a",
                x => x.InnerText,
                x => $"{BaseUri}/{x.GetAttributeValue("href", null)}");

        protected override IEnumerable<string> GetChapterImages()
        {
            var script =
                Doc.DocumentNode.SelectNodes("//script")
                    .Select(x => x.InnerText)
                    .First(x => x.Contains("var cnzz_comic"));
            var groups = Regex.Match(
                    script,
                    "chapter_addr:\"(.*?)\",start_var:(.*?),end_var:(.*?),.*?chapter_id:(.*?),.*?comic_definition:.*?\"(.*?)\"")
                .Groups;

            var extraCharCodes = int.Parse(groups[4].Value.Last().ToString());
            var link = string.Concat(groups[1].Value.Select(x => (char) (x - extraCharCodes)));
            var firstPage = int.Parse(groups[2].Value);
            var lastPage = int.Parse(groups[3].Value);
            var linkLastPart = groups[5].Value;
            return Enumerable.Range(firstPage, lastPage).Select(x => $"http://mhpic.xiaomingtaiji.net/comic/{link}{x}.jpg{linkLastPart}");
        }
    }
}