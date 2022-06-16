using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Output
{
    public class ActivityHaggleLogGetOutput
    {
        public long ItemId { get; set; }

        public long ActivityId { get; set; }

        public long ActivityRouteId { get; set; }

        public string AvatarUrl { get; set; }

        public string NickName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
