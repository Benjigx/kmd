using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class GekkouScans : MangaSite
    {
        protected override string Identifier => "//link[@href='http://leitor.gekkouscans.com.br']";
        protected override string MangaNameXpath => "//h2[@class='widget-title']";
        protected override string MangaCoverXpath => "//div[@class='boxed']//img";
        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//h5[@class='chapter-title-rtl']/a", 
                x => x.GetAttributeValue("href", "").Split('/').Last());

        protected override string ChapterImagesXpath => "//div[@id='all']/img";
        protected override string ChapterImagesImgAttribute => "data-src";
    }
}
