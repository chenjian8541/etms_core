using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantEtmsAccountLog")]
    public class SysTenantEtmsAccountLog: EManageEntity<int>
    {
        public int TenantId { get; set; }

        public int AgentId { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public long UserId { get; set; }

        public int VersionId { get; set; }

        /// <summary>
        /// 类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmTenantEtmsAccountLogChangeType"/>
        /// </summary>
        public int ChangeType { get; set; }

        public int ChangeCount { get; set; }

        public decimal Sum { get; set; }

        public DateTime Ot { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantEtmsAccountLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public long? RelatedId { get; set; }

        /// <summary>
        /// 场景类型
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantEtmsAccountLogSceneType"/>
        /// </summary>
        public int SceneType { get; set; }
    }
}
