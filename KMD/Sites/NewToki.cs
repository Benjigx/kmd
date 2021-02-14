using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KMD.Sites
{
    public class NewToki : MangaSite
    {
        protected override string Identifier => "//link[@href='/img/newtoki/apple-touch-icon.png']";
        protected override string MangaNameXpath => "//div[@class='view-content']/span";
        protected override string MangaCoverXpath => "//div[@class='view-img']/img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters(
                "//ul[@class='list-body']/li",
                x => x.SelectSingleNode(".//div[@class='wr-num']").InnerText,
                x => x.SelectSingleNode(".//div[@class='wr-subject']/a"));

        protected override IEnumerable<string> GetChapterImages()
        {
            var scriptTag = Doc.DocumentNode
                .SelectNodes("//script")
                .Select(x => x.InnerText)
                .First(x => x.Contains("html_data"));
            var str = string.Concat(Regex.Matches(scriptTag, @"([0-9A-F]{2})\.")
                .Cast<Match>()
                .Select(x => (char) Convert.ToInt32(x.Groups[1].Value, 16)));
            return Regex.Matches(str, @"data-.*?""(.*?)""")
                .Cast<Match>()
                .Select(x => x.Groups[1].Value);

        }
    }
}