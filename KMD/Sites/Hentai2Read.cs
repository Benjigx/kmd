using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class Hentai2Read : MangaSite
    {
        protected override string Identifier => "//meta[contains(@content, 'hentai2read.com')]";
        protected override string MangaNameXpath => "(//span[@property='name'])[last()]";
        protected override string MangaCoverXpath => "//img[@class='img-responsive border-black-op']";
        protected override IEnumerable<Chapter> GetMangaChapters() => 
            GetMangaChapters(
                "//ul[@class='nav-chapters']/li/div/a", 
                x => x.InnerText.Split('/').First());

        protected override IEnumerable<string> GetChapterImages() => 
            GetImagesFromScriptTag("var gData", @"""(\\\/.*?)""").Select(x => $"https://static.hentaicdn.com/hentai{x}");
    }
}
