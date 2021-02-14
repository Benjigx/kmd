using System.Collections.Generic;

namespace KMD
{
    public class Manga
    {
        public string Name { get; set; }
        public string Cover { get; set; }
        public ICollection<Chapter> Chapters { get; set; }
    }

    public class Chapter
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Index { get; set; }
        public string DisplayName => $"({Index}) {Name}";

        public Chapter(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
