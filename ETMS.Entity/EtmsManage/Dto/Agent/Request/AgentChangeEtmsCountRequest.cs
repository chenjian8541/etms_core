using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentChangeEtmsCountRequest : AgentRequestBase
    {
        public int Id { get; set; }

        /// <summary>
        ///  <see cref="AgentChangeEnum"/>
        /// </summary>
        public int ChangeType { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int VersionId { get; set; }

        public int ChangeCount { get; set; }

        public string Remark { get; set; }

        public decimal Sum { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "代理商Id不能为空";
            }
            if (ChangeCount <= 0)
            {
                return "数量必须大于0";
            }
            return base.Validate();
        }
    }
}
