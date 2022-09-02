using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentTodayWorkPlanGetOutput
    {
        public bool IsShow { get; set; }

        public List<StudentTodayWorkPlanTrack> Tracks { get; set; }
    }

    public class StudentTodayWorkPlanTrack {

        public long CId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? AgeMonth { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }
      
        public string LastTrackTimeDesc { get; set; }
    }
}
