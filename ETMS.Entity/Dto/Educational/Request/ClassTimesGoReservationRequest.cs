using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesGoReservationRequest : RequestBase
    {
        public List<long> ClassTimesIds { get; set; }

        public override string Validate()
        {
            if (ClassTimesIds == null || ClassTimesIds.Count == 0)
            {
                return "请选择课次";
            }
            if (ClassTimesIds.Count > 50)
            {
                return "一次性最多处理50节课次";
            }
            return base.Validate();
        }
    }
}
