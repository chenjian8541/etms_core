using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Common.Output
{
    public class RenderContent
    {
        public ShareContent ShareContent { get; set; }

        public ShowContent ShowContent { get; set; }
    }

    public class ShareContent
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string ImgUrl { get; set; }
    }

    public class ShowContent
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string ImgUrl { get; set; }
    }
}
