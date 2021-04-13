using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Output
{
    public class AIFaceTenantAccountGetGetPagingOutput
    {
        public int Id { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysAIInterfaceType"/>
        /// </summary>
        public int Type { get; set; }

        public string SecretId { get; set; }

        public string SecretKey { get; set; }

        public string Endpoint { get; set; }

        public string Region { get; set; }

        public string Remark { get; set; }
    }
}
