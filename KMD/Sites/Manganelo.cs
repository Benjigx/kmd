using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class Manganelo : MangaSite
    {
        protected override string Identifier => "//meta[@content='Manganelo']";
        protected override string MangaCoverXpath => "//span[@class='info-image']/img";
        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters(
                "//ul[@class='row-content-chapter']/li/a",
                x => x
                    .GetAttributeValue("href", "")
                    .Split(new[] {"chapter_"}, StringSplitOptions.None)
                    .Last());

        protected override string ChapterImagesXpath => "//div[@class='container-chapter-reader']//img";
    }
}
