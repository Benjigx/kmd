using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class Nhentai : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'nhentai')]";
        protected override string MangaNameXpath => "//span[@class='pretty']";
        protected override string MangaCoverXpath => "//div[@id='cover']//img";
        protected override string MangaCoverImgAttribute => "data-src";
        protected override IEnumerable<Chapter> GetMangaChapters() => new[] {new Chapter("Único", Url)};
        protected override string ChapterImagesXpath => "//a[@class='gallerythumb']//img[@class='lazyload']";
        protected override string ChapterImagesImgAttribute => "data-src";

        protected override IEnumerable<string> GetChapterImages()
        {
            return base.GetChapterImages()
                .Select(x => Regex.Match(x, "galleries\\/(.*?)t(.jpg|.png|.gif)"))
                .Select(x => $"https://i.nhentai.net/galleries/{x.Groups[1].Value}{x.Groups[2].Value}");
        }
    }
}
