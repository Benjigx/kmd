using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class MangaKakalot : MangaSite
    {
        protected override string Identifier => "//meta[@content='TNfDROcsP2nhZRixOKhUq1_GZ0d_EgiCxb0eRRgfMHg']";
        protected override string MangaCoverXpath => "//div[@class='manga-info-pic']/img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters(
                "//div[@class='chapter-list']//a",
                x => x
                    .GetAttributeValue("href", "")
                    .Split(new[] {"chapter_"}, StringSplitOptions.None)
                    .Last());

        protected override string ChapterImagesXpath => "//div[@id='vungdoc']/img";
    }
}