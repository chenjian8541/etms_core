using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStudentCourseOpLog")]
    public class EtStudentCourseOpLog : Entity<long>
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public string OpContent { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmStudentCourseOpLogType"/>
        /// </summary>
        public int OpType { get; set; }

        public DateTime OpTime { get; set; }

        public long OpUser { get; set; }

        public string Remark { get; set; }
    }
}
