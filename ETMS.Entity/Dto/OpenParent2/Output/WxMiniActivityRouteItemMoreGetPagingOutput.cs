using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniActivityRouteItemMoreGetPagingOutput
    {
        public long ActivityRouteId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string AvatarUrl { get; set; }

        public string StudentNameDesc { get; set; }

        public int CountLimit { get; set; }

        public int CountFinish { get; set; }

        public int CountShort { get; set; }

        public byte CountShortStatus { get; set; }

        public string CurrentAvatarUrl { get; set; }

        public string CurrentStudentNameDesc { get; set; }

        public int CurrentIndex { get; set; }

        public List<WxMiniActivityHomeJoinRouteItem> JoinRouteItems { get; set; }
    }
}
