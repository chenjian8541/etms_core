using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantChangeFileLimitMBRequest : AgentRequestBase
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int Id { get; set; }

        public int FileLimitMB { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "机构Id不能为空";
            }
            if (FileLimitMB < 0)
            {
                return "请设置存储";
            }
            return base.Validate();
        }
    }
}
