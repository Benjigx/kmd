using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMD.Sites
{
    class LimaScans : MadaraWordpressTheme
    {
        protected override string Identifier => "//title[contains(text(), 'Lima Scans')]";
        protected override string MangaCoverImgAttribute => "src";
        protected override string BasePath => "v2";
    }
}
