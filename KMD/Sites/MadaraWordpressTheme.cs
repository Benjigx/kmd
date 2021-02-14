using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace KMD.Sites
{
    class MadaraWordpressTheme : MangaSite
    {
        protected override string Identifier => "//style[@id='madara-css-inline-css']";
        protected override string MangaNameXpath => "(//ol[@class='breadcrumb']//li)[last()]";
        protected override string MangaCoverXpath => "//div[@class='summary_image']//img";
        protected override string MangaCoverImgAttribute => "data-src";
        protected virtual string BasePath => "";

        protected override async Task<IEnumerable<Chapter>> GetMangaChaptersAsync(CancellationToken ct)
        {
            var idChapter = Doc
                .DocumentNode
                .SelectSingleNode("//div[@id='manga-chapters-holder']")
                ?.GetAttributeValue("data-id", null);
            if (idChapter != null) // if chapters are lazy loaded
            {
                using var client = DefaultClient(Url);
                var content = new Dictionary<string, string>
                {
                    {"action", "manga_get_chapters"},
                    {"manga", idChapter}
                };
                var response = await client.SendAsync(
                    new HttpRequestMessage(
                        HttpMethod.Post,
                        $"{BaseUri}/{BasePath}/wp-admin/admin-ajax.php") {Content = new FormUrlEncodedContent(content)}, ct);
                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());
                Doc = doc;
            }
            return GetMangaChapters(
                "//li[contains(@class, 'wp-manga-chapter')]//a",
                x => x.InnerText,
                x => $"{x.GetAttributeValue("href", null)}"); // explicitly open chapter in paged mode using query param "?style=paged"
        }

        protected override IEnumerable<string> GetChapterImages() =>
            GetImagesFromScriptTag("chapter_preloaded_images", "\"(http.*?)\"");
    }
}