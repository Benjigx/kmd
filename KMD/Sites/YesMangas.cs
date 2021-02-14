using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class YesMangas : MangaSite
    {
        protected override string Identifier => "//a[@class='navbar-link'][@title='YES Mangás']";
        protected override string MangaNameXpath => "//h1[@class='title']";
        protected override string MangaCoverXpath => "//div[@id='descricao']//img";
        protected override string MangaCoverImgAttribute => "data-path";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//div[@id='capitulos']//a");

        protected override string ChapterImagesXpath => "//div[@class='read-slideshow']//img";
    }
}
