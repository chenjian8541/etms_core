using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentRegister2Request
    {
        public int TenantId { get; set; }

        public long? TrackUser { get; set; }

        public string Phone { get; set; }

        public string Name { get; set; }

        public string SmsCode { get; set; }

        public string Address { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        public string RecommenderPhone { get; set; }

        public List<StudentExtendItem> StudentExtendItems { get; set; }
    }
}
