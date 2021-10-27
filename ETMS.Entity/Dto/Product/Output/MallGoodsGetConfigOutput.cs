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
        public string TenantName { get; set; }
        public string TenantNo { get; set; }

        public string HomeShareUrl { get; set; }

        public string DetailShareUrl { get; set; }

        public string HomeShareImgUrl { get; set; }

        public string HomeShareImgKey { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallGoodsStatus"/>
        /// </summary>
        public byte MallGoodsStatus { get; set; }

        public string Title { get; set; }
    }
}
