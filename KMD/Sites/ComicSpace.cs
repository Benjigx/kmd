using System.Collections.Generic;
using System.Linq;

namespace KMD.Sites
{
    public class ComicSpace : MangaSite
    {
        protected override string Identifier => "//script[@data-ad-client='ca-pub-4587983768629718']";

        protected override string MangaNameXpath => "//h2[@class='widget-title']";

        protected override string MangaCoverXpath => "//img[@class='img-responsive-serie-right']";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//h5[@class='chapter-title-rtl']/a", 
                x => x.GetAttributeValue("href", "").Split('/').Last());

        protected override string ChapterImagesXpath => "//div[@class='viewer-cnt']//div[@id='all']/img";

        protected override string ChapterImagesImgAttribute => "data-src";
    }
}
