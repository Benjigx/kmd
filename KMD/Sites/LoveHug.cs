using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    public class LoveHug : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'LoveHug')]";
        protected override string MangaNameXpath => "//ul[@class='manga-info']/h3";
        protected override string MangaCoverXpath => "//img[@class='thumbnail']";
        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//ul[@class='list-chapters at-series']/a",
                x => x.SelectSingleNode("./li/div[@class='chapter-name text-truncate']").InnerText);

        protected override string ChapterImagesXpath => "//img[@class='chapter-img']";
    }
}
