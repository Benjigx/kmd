using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace KMD.Sites
{
    public class AnimaRegia : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'AnimaRegia')]";

        protected override string MangaNameXpath => "//h1[@class='widget-title']";
        protected override string MangaCoverXpath => "//img[@class='img-thumbnail']";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//h5[@class='chapter-title-rtl']//a", 
                x => x.GetAttributeValue("href", "").Split('/').Last());

        protected override IEnumerable<string> GetChapterImages()
        {
            return GetImagesFromScriptTag("var pages", ":\"(.*?)\",");
        }
    }
}