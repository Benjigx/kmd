using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    public class SoManga : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'SoMangá')]";
        protected override string MangaCoverXpath => "//div[@class='row manga']//img[@class='img-responsive']";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//ul[@class='capitulos']/li/a",
                x => x.GetAttributeValue("href", null).Split('/').Last());

        protected override string ChapterImagesXpath => "//img[contains(@class, 'img-manga')]";
    }
}