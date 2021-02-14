using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class SaikaiScan : MangaSite
    {
        protected override string Identifier => "//meta[@content='Saikai Scan']";
        protected override string MangaNameXpath => "//h2";
        protected override string MangaCoverXpath => "//div[@class='cover']//img";
        protected override string MangaCoverImgAttribute => "data-src";
        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//div[contains(@class, 'chapters')]/ul/li/a");

        protected override string ChapterImagesXpath => "//div[@class='images-block']/img";
    }
}
