using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KMD
{
    public static class Utils
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) =>
            source.Select((item, index) => (item, index));

        public static string AddLeadingZeroes(int current, int maxCount) =>
            current.ToString($"D{Math.Ceiling(Math.Log10(maxCount))}");

        public static string RemoveInvalidChars(string str)
        {
            str = Path.GetInvalidFileNameChars()
                .Aggregate(str, (current, invalidChar) => current.Replace(invalidChar.ToString(), ""));
            return str.Trim().TrimEnd('.');
        }

        public static async Task SaveImageFromUrl(this HttpClient client, string url, string fullPath,
            CancellationToken ct = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
            using var content = response.Content;
            using var imageFactory = new ImageFactory(true);
            var imageStream = await content.ReadAsStreamAsync();
            var format = new JpegFormat();
            imageFactory.Load(imageStream)
                .Format(format)
                .Quality(100)
                .Save(fullPath);
        }

        public static IEnumerable<string> Split(this string str, string separator)
        {
            return str.Split(new[] {separator}, StringSplitOptions.None);
        }

        public static async Task<T> GetJson<T>(
            this HttpClient client,
            HttpRequestMessage requestMessage,
            CancellationToken ct = default)
        {
            var response = await client.SendAsync(requestMessage, ct);
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static Task<T> GetJson<T>(this HttpClient client, string url, CancellationToken ct = default)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            return client.GetJson<T>(requestMessage, ct);
        }
    }
}