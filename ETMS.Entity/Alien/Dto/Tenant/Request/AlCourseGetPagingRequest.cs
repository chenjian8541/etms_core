using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.Tenant.Request
{
    public class AlCourseGetPagingRequest : AlienTenantRequestPagingBase
    {
        public string Name { get; set; }

        public byte? Status { get; set; }

        public byte? Type { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(TenantDataFilterWhere());
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name like '%{Name}%'");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            if (Type != null)
            {
                condition.Append($" AND [Type] = {Type.Value}");
            }
            return condition.ToString();
        }
    }
}
