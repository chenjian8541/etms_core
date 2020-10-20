using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员短信记录
    /// </summary>
    [Table("EtStudentSmsLog")]
    public class EtStudentSmsLog : Entity<long>
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long? StudentId { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 类型 <see cref="ETMS.Entity.Enum.EmStudentSmsLogType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 扣减数量
        /// </summary>
        public int DeCount { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string SmsContent { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 状态   <see cref="ETMS.Entity.Enum.EmSmsLogStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
