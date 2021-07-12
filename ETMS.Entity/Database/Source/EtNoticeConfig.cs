using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 通知配置
    /// </summary>
    [Table("EtNoticeConfig")]
    public class EtNoticeConfig : Entity<long>
    {
        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmNoticeConfigType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmPeopleType"/>
        /// </summary>
        public byte PeopleType { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmNoticeConfigScenesType"/>
        /// </summary>
        public int ScenesType { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmNoticeConfigExType"/>
        /// </summary>
        public int ExType { get; set; }

        public string ConfigValue { get; set; }
    }
}
