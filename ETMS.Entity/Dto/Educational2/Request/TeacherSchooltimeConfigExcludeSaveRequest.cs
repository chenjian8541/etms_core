using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Educational2.Request
{
    public class TeacherSchooltimeConfigExcludeSaveRequest : RequestBase
    {
        public long TeacherId { get; set; }

        public List<DateTime> ExcludeDate { get; set; }

        public override string Validate()
        {
            if (TeacherId <= 0)
            {
                return "请选择老师";
            }
            if (ExcludeDate != null)
            {
                var tempEffectiveExcludeDate = ExcludeDate.Where(p => p >= DateTime.Now.Date);
                if (tempEffectiveExcludeDate.Count() > 100)
                {
                    return "特定日期最多100天";
                }
            }
            return base.Validate();
        }
    }
}
