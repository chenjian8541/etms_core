using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 账户日志
    /// </summary>
    [Table("SysAgentEtmsAccountLog")]
    public class SysAgentEtmsAccountLog : EManageEntity<int>
    {
        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public long UserId { get; set; }


        /// <summary>
        /// 系统版本ID
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        ///变动类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmSysAgentEtmsAccountLogChangeType"/>
        /// </summary>
        public int ChangeType { get; set; }

        /// <summary>
        /// 变动数量
        /// </summary>
        public int ChangeCount { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// 变动时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 场景
        /// </summary>
        public int SceneType { get; set; }
    }
}
