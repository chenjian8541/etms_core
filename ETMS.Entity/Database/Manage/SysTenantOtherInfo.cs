using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantOtherInfo")]
    public class SysTenantOtherInfo : EManageEntity<long>
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public string HomeLogo1 { get; set; }

        public string HomeLogo2 { get; set; }

        public string LoginLogo1 { get; set; }

        public string LoginBg { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsHideKeFu { get; set; }
    }
}
