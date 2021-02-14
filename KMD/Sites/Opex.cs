using System.Collections.Generic;
using System.Linq;

namespace KMD.Sites
{
    class Opex : MangaSite
    {
        protected override string Identifier => "//img[@alt='One Piece EX']";
        protected override string GetMangaName() => "One Piece";
        protected override string GetMangaCover() => "/mangareader/sbs/capa/grande/Volume_1.jpg";

        protected override IEnumerable<Chapter> GetMangaChapters()
        {
            return base.GetMangaChapters(
                "//li[@class='volume-capitulo']//a",
                x =>
                {
                    var link = x.GetAttributeValue("href", null);
                    return
                        $"{link.Split('/')[3]} " +
                        $"{(link.Contains("especiais") ? "especial" : link.Contains("capa") ? "capa" : "")} " +
                        $"{(link.Contains("jump") ? "colorido" : "")}";
                })
                .Reverse();
        }

        protected override IEnumerable<string> GetChapterImages() => 
            GetImagesFromScriptTag("paginasLista", ":\\\\\"(.*?)\\\\\"").Select(x => $"/{x}");
    }
}