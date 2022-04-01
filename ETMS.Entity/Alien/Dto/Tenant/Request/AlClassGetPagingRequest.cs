using ETMS.Entity.Alien.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.Tenant.Request
{
    public class AlClassGetPagingRequest : AlienTenantRequestPagingBase
    {
        public byte? Type { get; set; }

        public string Name { get; set; }

        public byte? CompleteStatus { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(TenantDataFilterWhere());
            condition.Append($" AND DataType = {EmClassDataType.Normal}");
            if (Type != null)
            {
                condition.Append($" AND Type = {Type}");
            }
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name LIKE '%{Name}%'");
            }
            if (CompleteStatus != null)
            {
                condition.Append($" AND CompleteStatus = {CompleteStatus.Value}");
            }
            return condition.ToString();
        }
    }
}
