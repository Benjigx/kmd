using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace KMD
{
    public abstract class MangaSite
    {
        protected HtmlDocument Doc;
        protected string Url;
        protected string BaseUri => new Uri(Url).GetLeftPart(UriPartial.Authority);
        protected abstract string Identifier { get; }

        public static async Task<Manga> GetMangaInfo(string url, CancellationToken ct = default)
        {
            var doc = await GetPageSource(url, ct);
            var mangaSite = GetMangaSiteInstance(url, doc);
            var name = await mangaSite.GetMangaNameAsync(ct);
            var coverUrl = await mangaSite.GetMangaCoverAsync(ct);
            var chapters = (await mangaSite.GetMangaChaptersAsync(ct)).ToList();
            name = Regex.Replace(name, @"(?i)\s?(?:\(br|\(pt).*?\)", "").Trim();
            coverUrl = mangaSite.FixUrl(coverUrl);
            chapters = chapters
                .Select((x, i) =>
                {
                    x.Index = chapters.Count - i;
                    x.Name = x.Name.Trim();
                    x.Url = mangaSite.FixUrl(x.Url);
                    return x;
                })
                .ToList();
            return new Manga {Name = name, Cover = coverUrl, Chapters = chapters};
        }

        public static async Task<ICollection<string>> GetChapterImages(Chapter chapter, CancellationToken ct = default)
        {
            var doc = await GetPageSource(chapter.Url, ct);
            var mangaSite = GetMangaSiteInstance(chapter.Url, doc);
            return (await mangaSite.GetChapterImagesAsync(ct)).Select(mangaSite.FixUrl).ToList();
        }

        private static MangaSite GetMangaSiteInstance(string url, HtmlDocument doc)
        {
            var ts = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(MangaSite)))
                .OrderBy(x => x.BaseType!.Name == nameof(MangaSite));
            foreach (var t in ts)
            {
                var instance = (MangaSite) Activator.CreateInstance(t);
                if (doc.DocumentNode.SelectSingleNode(instance.Identifier) == null) continue;
                instance.Doc = doc;
                instance.Url = url;
                return instance;
            }

            throw new Exception("Site não suportado");
        }

        private string FixUrl(string url)
        {
            url = Regex.Unescape(url).Trim();
            var protocol = UrlProtocol(Url);
            if (url.StartsWith("//"))
            {
                return $"{protocol}:{url}";
            }

            if (url.StartsWith("/"))
            {
                return BaseUri + url;
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                return $"{protocol}://{url}";
            }

            return url;
        }

        private static string UrlProtocol(string url) => new Uri(url).Scheme;


        protected virtual string MangaNameXpath { get; } = "//h1";
        protected virtual string MangaCoverXpath => throw new NotImplementedException();
        protected virtual string MangaCoverImgAttribute { get; } = "src";
        protected virtual string ChapterImagesXpath => throw new NotImplementedException();
        protected virtual string ChapterImagesImgAttribute { get; } = "src";

        protected virtual string GetMangaName()
        {
            return Doc
                .DocumentNode
                .SelectSingleNode(MangaNameXpath)
                ?.InnerText;
        }

        protected virtual Task<string> GetMangaNameAsync(CancellationToken ct) => Task.FromResult(GetMangaName());

        protected virtual string GetMangaCover()
        {
            return Doc
                .DocumentNode
                .SelectSingleNode(MangaCoverXpath)
                ?.GetAttributeValue(MangaCoverImgAttribute, "");
        }

        protected virtual Task<string> GetMangaCoverAsync(CancellationToken ct) => Task.FromResult(GetMangaCover());

        protected virtual IEnumerable<Chapter> GetMangaChapters() => throw new NotImplementedException();

        protected virtual Task<IEnumerable<Chapter>> GetMangaChaptersAsync(CancellationToken ct) =>
            Task.FromResult(GetMangaChapters());

        protected virtual IEnumerable<string> GetChapterImages() =>
            Doc
                .DocumentNode
                .SelectNodes(ChapterImagesXpath)
                .Select(x => x.GetAttributeValue(ChapterImagesImgAttribute, null));

        protected virtual Task<IEnumerable<string>> GetChapterImagesAsync(CancellationToken ct) =>
            Task.FromResult(GetChapterImages());

        protected static async Task<HtmlDocument> GetPageSource(string url, CancellationToken ct)
        {
            using var client = DefaultClient();
            var response = await client.GetAsync(url, ct);
            var html = Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        protected static HttpClient DefaultClient(string url = null)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0");
            if (url != null) client.DefaultRequestHeaders.Add("Referer", url);
            client.Timeout = TimeSpan.FromSeconds(60);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 |
                                                   SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            return client;
        }

        protected IEnumerable<Chapter> GetMangaChapters(
            string xpath,
            Func<HtmlNode, string> name = null,
            Func<HtmlNode, HtmlNode> url = null)
        {
            return GetMangaChapters(
                xpath,
                x => name?.Invoke(x) ?? x.InnerText,
                x => (url?.Invoke(x) ?? x).GetAttributeValue("href", null));
        }

        protected IEnumerable<Chapter> GetMangaChapters(
            string xpath,
            Func<HtmlNode, string> name,
            Func<HtmlNode, string> url) =>
            Doc
                .DocumentNode
                .SelectNodes(xpath)
                .Select(x =>
                    new Chapter(
                        name.Invoke(x),
                        url.Invoke(x)));


        protected IEnumerable<string> GetImagesFromScriptTag(string tagIdentifier, string pattern)
        {
            var scriptTag = Doc.DocumentNode
                .SelectNodes("//script")
                .Select(x => x.InnerText)
                .First(x => x.Contains(tagIdentifier));
            return Regex.Matches(scriptTag, pattern).Cast<Match>().Select(x => x.Groups[1].Value);
        }
    }
}