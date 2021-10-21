using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Output
{
    public class MallGoodsGetConfigOutput
    {
        public string TenantNo { get; set; }

        public string HomeShareUrl { get; set; }

        public string DetailShareUrl { get; set; }

        public Img HomeShareImgUrl { get; set; }
    }
}
