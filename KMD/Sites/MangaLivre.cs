using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    public class MangaLivre : LeitorNet
    {
        protected override string Identifier => "//a[@class='brand-expanded'][@title='MangaLivre']";
    }
}
