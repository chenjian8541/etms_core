using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Output
{
    public class CheckOnLog2Get
    {
        /// <summary>
        /// 考勤介质
        /// 磁卡卡号/人脸图片key
        /// </summary>
        public string CheckMediumUrl { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        public DateTime CheckOt { get; set; }

        /// <summary>
        /// 考勤类型  <see cref="ETMS.Entity.Enum.EmStudentCheckOnLogCheckType"/>
        /// </summary>
        public byte CheckType { get; set; }

        /// <summary>
        /// 考勤类型  
        /// </summary>
        public string CheckTypeDesc { get; set; }
    }
}
