using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Request
{
    public class TryApplyLogGetPagingRequest : AgentPagingBase
    {
        public byte? HandleStatus { get; set; }

        public byte? ClientType { get; set; }

        public string HandleRemark { get; set; }

        public string Remark { get; set; }
        public override string ToString()
        {
            var condition = new StringBuilder($"IsDeleted = {EmIsDeleted.Normal}");
            if (HandleStatus != null)
            {
                condition.Append($" AND Status = {HandleStatus.Value}");
            }
            if (ClientType != null)
            {
                condition.Append($" AND ClientType = {ClientType.Value}");
            }
            if (!string.IsNullOrEmpty(HandleRemark))
            {
                condition.Append($" AND HandleRemark LIKE '%{HandleRemark}%'");
            }
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND Remark LIKE '%{Remark}%'");
            }
            return condition.ToString();
        }
    }
}
