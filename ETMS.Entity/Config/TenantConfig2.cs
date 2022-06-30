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
            ActivityConfig = new ActivityConfig();
        }
        public MallGoodsConfig MallGoodsConfig { get; set; }

        public ActivityConfig ActivityConfig { get; set; }
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

    public class ActivityConfig
    {
        public bool IsAutoRefund { get; set; }

        public string PayTp { get; set; } = "注：您将购买课程服务，购买之后不支持退订、转让、退款";
    }
}
