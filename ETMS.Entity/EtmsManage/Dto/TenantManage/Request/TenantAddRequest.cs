using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantAddRequest
    {
        public string TenantName { get; set; }

        public string TenantCode { get; set; }

        public string TenantPhone { get; set; }

        public string UserName { get; set; }

        public int ConnectionId { get; set; }

        public string Remark { get; set; }

        public string Check()
        {
            if (string.IsNullOrEmpty(TenantName))
            {
                return "数据不对";
            }
            if (string.IsNullOrEmpty(TenantCode))
            {
                return "数据不对";
            }
            if (string.IsNullOrEmpty(TenantPhone))
            {
                return "数据不对";
            }
            if (string.IsNullOrEmpty(UserName))
            {
                return "数据不对";
            }
            if (ConnectionId <= 0)
            {
                return "数据不对";
            }
            return string.Empty;
        }
    }
}
