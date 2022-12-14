using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class CouponsStudentRevokedRequest : RequestBase
    {
        /// <summary>
        /// CouponsStudentGet Id
        /// </summary>
        public long CId { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
