using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class GoldenMangas : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'Golden mangas')]";
        protected override string MangaCoverXpath => "//div[@class='col-sm-4 text-right']/img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//ul[@id='capitulos']/li/a",
                x => x.GetAttributeValue("href", null).Split('/').Last());

        protected override string ChapterImagesXpath => "//img[contains(@class, 'img-manga')]";
    }
}