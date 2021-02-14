using System;
using System.Collections.Generic;
using System.Linq;

namespace KMD.Sites
{
    public class ReadComicOnline : MangaSite
    {
        protected override string Identifier => "//meta[@content='130397647116314']";
        protected override string MangaNameXpath => "//a[@class='bigChar']";
        protected override string MangaCoverXpath => "//div[@class='rightBox']//img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters(
                "//table[@class='listing']//a",
                x => x.InnerText.Split(new[] {GetMangaName()}, StringSplitOptions.None).Last());

        protected override IEnumerable<string> GetChapterImages() =>
            GetImagesFromScriptTag("var lstImages", @"push\(""(.*?)""\)");
    }
}
