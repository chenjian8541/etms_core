using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActiveHomeworkStudent")]
    public class EtActiveHomeworkStudent : Entity<long>
    {
        public long HomeworkId { get; set; }

        public long StudentId { get; set; }

        public long CreateUserId { get; set; }

        /// <summary>
        ///作答状态   <see cref="ETMS.Entity.Enum.EmActiveHomeworkDetailAnswerStatus"/>
        /// </summary>
        public byte AnswerStatus { get; set; }

        /// <summary>
        /// 阅读状态  <see cref="ETMS.Entity.Enum.EmActiveHomeworkDetailReadStatus"/>
        /// </summary>
        public byte ReadStatus { get; set; }
    }
}
