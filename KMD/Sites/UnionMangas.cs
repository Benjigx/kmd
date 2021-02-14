using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    public class UnionMangas : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'Union Mangás')]";
        protected override string MangaNameXpath => "//div[@class='col-md-8 tamanho-bloco-perfil']//h2";
        protected override string MangaCoverXpath => "//img[@class='img-thumbnail']";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters(
                "//div[@class='row lancamento-linha']/div[1]/a",
                x => x.GetAttributeValue("href", null).Split('/').Last());

        protected override string ChapterImagesXpath => "//img[contains(@class, 'img-manga')]";
    }
}