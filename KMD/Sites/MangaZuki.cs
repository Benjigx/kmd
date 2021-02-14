using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    // Uses madara theme
    class MangaZuki : MangaSite
    {
        protected override string Identifier => "//meta[@content='Mangazuki']";
        protected override string MangaNameXpath => "//div[@class='post-title']/h1";
        protected override string MangaCoverXpath => "//div[@class='summary_image']//img";
        protected override string MangaCoverImgAttribute => "data-src";
        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//li[@class='wp-manga-chapter']/a");

        protected override string ChapterImagesXpath => "//img[@class='wp-manga-chapter-img']";
    }
}
