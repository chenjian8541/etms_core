using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Config
{
    public class TenantConfig2
    {
        public TenantConfig2()
        {
            MallGoodsConfig = new MallGoodsConfig();
        }
        public MallGoodsConfig MallGoodsConfig { get; set; }
    }

    public class MallGoodsConfig
    {
        public string Title { get; set; }

        public string HomeShareImgKey { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallGoodsStatus"/>
        /// </summary>
        public byte MallGoodsStatus { get; set; }
    }
}
