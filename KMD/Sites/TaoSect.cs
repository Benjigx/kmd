using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace KMD.Sites
{
    public class TaoSect : MangaSite
    {
        protected override string Identifier => "//title[contains(text(), 'Tao Sect')]";
        protected override string MangaCoverXpath => "//div[@class='imagens-projeto']/img";

        protected override IEnumerable<Chapter> GetMangaChapters() =>
            GetMangaChapters(
                    "//div[@class='volumes_capitulos']//td[@align='left']/a",
                    x => x.InnerText.Split(new[] {"Capítulo"}, StringSplitOptions.None).Last())
                .Reverse();

        protected override string ChapterImagesXpath => "//select[@id='leitor_pagina_projeto']/option";
        protected override string ChapterImagesImgAttribute => "value";
    }
}