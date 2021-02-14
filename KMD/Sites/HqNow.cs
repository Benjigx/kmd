using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KMD.Sites
{
    public class HqNow : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'HQ Now')]";
        private Dto1 _response;
        protected override async Task<string> GetMangaNameAsync(CancellationToken ct)
        {
            using var client = DefaultClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://admin.hq-now.com/graphql");
            var mangaNum = Url.Split('/')[4];
            var json = $"{{\"operationName\":\"getHqsById\",\"variables\":{{\"id\":{mangaNum}}},\"query\":\"query getHqsById($id: Int!) {{\\n getHqsById(id: $id) {{\\n name\\n hqCover\\n capitulos {{\\n number\\n id\\n }}\\n }}\\n}}\\n\"}}";
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            _response = await client.GetJson<Dto1>(request, ct);
            return _response.Data.GetHqsById.First().Name;
        }

        protected override string GetMangaCover() => _response.Data.GetHqsById.First().HqCover;
        protected override IEnumerable<Chapter> GetMangaChapters() => 
            _response
                .Data
                .GetHqsById
                .First()
                .Capitulos
                .Select(x => new Chapter(x.Number, $"{BaseUri}/hq-reader/{x.Id}/kmd/chapter/{x.Number}/page/1"))
                .Reverse();

        protected override async Task<IEnumerable<string>> GetChapterImagesAsync(CancellationToken ct)
        {
            using var client = DefaultClient();
            var chNum = Url.Split('/')[4];
            var request = new HttpRequestMessage(HttpMethod.Post, "http://admin.hq-now.com/graphql");
            var json =
                $"{{\n  \"operationName\": \"getChapterById\",\n  \"variables\": {{\n    \"chapterId\": {chNum}\n  }},\n  \"query\": \"query getChapterById($chapterId: Int!) {{\\n  getChapterById(chapterId: $chapterId) {{\\n    pictures {{\\n      pictureUrl\\n    }}\\n  }}\\n}}\\n\"\n}}";
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.GetJson<Dto5>(request, ct);
            return response.Data.GetChapterById.Pictures.Select(x => x.PictureUrl);
        }

        private class Dto1
        {
            public Dto2 Data { get; set; }
        }

        private class Dto2
        {
            public ICollection<Dto3> GetHqsById { get; set; }
        }

        private class Dto3
        {
            public string Name { get; set; }
            public string HqCover { get; set; }
            public ICollection<Dto4> Capitulos { get; set; }
        }

        private class Dto4
        {
            public string Id { get; set; }
            public string Number { get; set; }
        }

        private class Dto5
        {
            public Dto6 Data { get; set; }
        }

        private class Dto6
        {
            public Dto7 GetChapterById { get; set; }
        }

        private class Dto7
        {
            public ICollection<Dto8> Pictures { get; set; }
        }

        private class Dto8
        {
            public string PictureUrl { get; set; }
        }
    }
}
