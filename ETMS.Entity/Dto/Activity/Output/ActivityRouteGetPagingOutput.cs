using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Output
{
    public class ActivityRouteGetPagingOutput
    {
        public long ActivityRouteId { get; set; }

        public long ActivityId { get; set; }

        public string AvatarUrl { get; set; }

        public string NickName { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentFieldValue1 { get; set; }

        public string StudentFieldValue2 { get; set; }

        public int CountLimit { get; set; }

        public int CountFinish { get; set; }

        public decimal PaySum { get; set; }

        public DateTime? PayFinishTime { get; set; }

        public string ShareQRCode { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
