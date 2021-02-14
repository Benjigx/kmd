using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KMD.Sites
{
    public class TsukiMangas : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'Tsuki Mangás')]";
        private string MangaNum => Url.Split('/')[4];

        protected override async Task<string> GetMangaNameAsync(CancellationToken ct)
        {
            using var client = DefaultClient();
            var response = await client.GetJson<ApiResponseManga>($"{BaseUri}/api/v2/mangas/{MangaNum}", ct);
            return response.Title;
        }

        protected override async Task<string> GetMangaCoverAsync(CancellationToken ct)
        {
            using var client = DefaultClient();
            var response = await client.GetJson<ApiResponseManga>($"{BaseUri}/api/v2/mangas/{MangaNum}", ct);
            return $"/imgs/{response.Poster}";
        }

        protected override async Task<IEnumerable<Chapter>> GetMangaChaptersAsync(CancellationToken ct)
        {
            var list = new List<Chapter>();
            using var client = DefaultClient();
            for (var i = 1; i < 50; i++)
            {
                var slug = Url.Split('/')[5];
                var response = await client.GetJson<ApiResponseChapter>($"{BaseUri}/api/v2/chapters?manga_id={MangaNum}&order=desc&page={i}");
                if (response.Data.Count == 0) break;
                list.AddRange(response.Data.Select(x =>
                    new Chapter(x.Number,
                        $"/leitor/{MangaNum}/{x.Versions.First().Id}/{slug}/{x.Number}")));
            }

            return list;
        }

        protected override async Task<IEnumerable<string>> GetChapterImagesAsync(CancellationToken ct)
        {
            using var client = DefaultClient();
            var version = Url.Split('/')[5];
            var response = await client.GetJson<ApiResponseImage>($"{BaseUri}/api/v2/chapter/versions/{version}");
            return response.Pages.Select(x =>
                $"{new Uri(Url).Scheme}://cdn{x.Server}.{new Uri(Url).Host}{x.Url}");
        }

        private class ApiResponseManga
        {
            public string Poster { get; set; }
            public string Title { get; set; }
        }

        private class ApiResponseChapter
        {
            public ICollection<ChapterDto> Data { get; set; }
        }

        private class ChapterDto
        {
            public string Number { get; set; }
            public ICollection<VersionDto> Versions { get; set; }
        }

        private class VersionDto
        {
            public string Id { get; set; }
        }

        private class ApiResponseImage
        {
            public ICollection<PageDto> Pages { get; set; }
        }

        private class PageDto
        {
            public string Url { get; set; }
            public string Server { get; set; }
        }
    }
}