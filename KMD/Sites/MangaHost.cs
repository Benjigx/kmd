using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace KMD.Sites
{
    public class MangaHost : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'Mangá Host')]";
        protected override string MangaCoverXpath => "//img[@class='image-3']";
        protected override string ChapterImagesXpath => "//div[@id='slider']//img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters("//div[@class='chapters']/div[@class='cap']",
                x => x.SelectSingleNode(".//span[@class='btn-caps']").InnerText,
                x => x.SelectSingleNode(".//div[@class='pop-content']//a"));


        protected override IEnumerable<string> GetChapterImages()
        {
            return base.GetChapterImages()
                .Select(x =>
                    x.Replace("images", "mangas_files")
                        .Replace(".webp", ""));
        }
    }
}