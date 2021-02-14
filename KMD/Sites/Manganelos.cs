using System.Collections.Generic;
using System.Linq;

namespace KMD.Sites
{
    public class Manganelos : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'MangaNelos.com')]";
        protected override string MangaNameXpath => "(//ol[@class='breadcrumb']//li)[last()]";
        protected override string MangaCoverXpath => "//div[@class='media-left cover-detail']/img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//ul/li/div[contains(@class, 'chapter')]/h4/a",
                x => x.InnerText.Split("Chapter").Last());

        protected override IEnumerable<string> GetChapterImages() =>
            Doc.DocumentNode.SelectSingleNode("//p[@id='arraydata']").InnerText.Split(',');
    }
}