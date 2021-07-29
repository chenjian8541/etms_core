using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmRelationClassTimesGoDeStudentCourseMulCourseType
    {
        /// <summary>
        /// 请先在学员详情->报读课程中设置“考勤记上课课程”
        /// </summary>
        public const byte NeedSetStudentCoueseCheckDefault = 0;

        /// <summary>
        /// 弹出选择框
        /// </summary>
        public const byte PopupsChooseStudentCouese = 1;
    }
}
