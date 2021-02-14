using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class HiperCool : MangaSite
    {
        protected override string Identifier => "//meta[@name='HipercooL']";
        protected override string MangaNameXpath => "//span[@class='title']";
        protected override string MangaCoverXpath => "//div[@class='cover']//img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//div[@class='chapter']//a[@class='title']");

        protected override string ChapterImagesXpath => "//div[@class='pages']//img[@class='page']";
    }
}
