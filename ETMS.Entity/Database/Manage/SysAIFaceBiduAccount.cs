using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Manage
{
    [Table("SysAIFaceBiduAccount")]
    public class SysAIFaceBiduAccount : EManageEntity<int>
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysAIInterfaceType"/>
        /// </summary>
        public int Type { get; set; }

        public string Appid { get; set; }

        public string ApiKey { get; set; }

        public string SecretKey { get; set; }
    }
}
