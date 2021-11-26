using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class CheckOnLogGetPagingOutput
    {
        public long StudentCheckOnLogId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        /// <summary> 
        /// 考勤形式  <see cref="ETMS.Entity.Enum.EmStudentCheckOnLogCheckForm"/>
        /// </summary>
        public byte CheckForm { get; set; }

        public string CheckFormDesc { get; set; }

        /// <summary>
        /// 考勤类型  <see cref="ETMS.Entity.Enum.EmStudentCheckOnLogCheckType"/>
        /// </summary>
        public byte CheckType { get; set; }

        public string CheckTypeDesc { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        public DateTime CheckOt { get; set; }

        public string CourseName { get; set; }

        public string DeClassTimesDesc { get; set; }

        public string CheckMedium { get; set; }
    }
}
