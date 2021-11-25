using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveHomeworkAddClassInfo : IValidate
    {
        public long ClassId { get; set; }

        public List<long> StudentIds { get; set; }

        public string Validate()
        {
            if (ClassId <= 0)
            {
                return "班级信息不合法";
            }
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "所选班级至少要含有一位学员";
            }
            return string.Empty;
        }
    }
}
