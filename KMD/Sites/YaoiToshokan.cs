using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class YaoiToshokan : MadaraWordpressTheme
    {
        protected override string Identifier => "//title[contains(text(), 'Yaoi Toshokan')]";
        protected override string MangaNameXpath => "//h1";
    }
}
