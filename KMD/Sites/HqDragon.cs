using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class HqDragon : MangaSite
    {
        protected override string Identifier => "//meta[@content='HQ Dragon']";
        protected override string MangaNameXpath => "//h3";
        protected override string MangaCoverXpath => "//img[@class='img-fluid']";
        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters(
                "//table[@class='table table-bordered']//a",
                x => x.GetAttributeValue("href", null).Split('/').Last());

        protected override string ChapterImagesXpath => "//article[@id='leitor']//img";
    }
}
