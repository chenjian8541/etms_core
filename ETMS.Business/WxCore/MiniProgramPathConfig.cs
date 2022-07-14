using ETMS.Entity.Enum.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.WxCore
{
    public class MiniProgramPathConfig
    {
        public const string ActivityMainGroupPurchase = "pages/acdetail1/index";

        public const string ActivityMainHaggling = "pages/acdetail2/index";

        public static string GetMiniProgramPath(int activityType)
        {
            switch (activityType)
            {
                case EmActivityType.GroupPurchase:
                    return ActivityMainGroupPurchase;
                case EmActivityType.Haggling:
                    return ActivityMainHaggling;
            }
            return string.Empty;
        }
    }
}
