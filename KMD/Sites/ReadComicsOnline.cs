using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class ReadComicsOnline : MangaSite
    {
        protected override string Identifier => "//a[@href='https://readcomicsonline.ru']";
        protected override string MangaNameXpath => "//h2";
        protected override string MangaCoverXpath => "//div[@class='boxed']/img";
        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//ul[@class='chapters']//a");

        protected override string ChapterImagesXpath => "//div[@id='all']/img";
        protected override string ChapterImagesImgAttribute => "data-src";
    }
}
