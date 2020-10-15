using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 版本信息
    /// </summary>
    [Table("SysVersion")]
    public class SysVersion: EManageEntity<int>
    {
        public string Name { get; set; }

        public string Img { get; set; }

        public string DetailInfo { get; set; }

        public string EtmsAuthorityValue { get; set; }
    }
}
