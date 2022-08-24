using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysExternalConfig")]
    public class SysExternalConfig : EManageEntity<int>
    {
        /// <summary>
        /// 1:百度语音合成token
        /// 2:百度地图开放平台（AK）
        /// <see cref="ETMS.Entity.Enum.EmSysExternalConfigType"/>
        /// </summary>
        public int Type { get; set; }

        public string Name { get; set; }

        public string Data1 { get; set; }

        public string Data2 { get; set; }
    }
}
