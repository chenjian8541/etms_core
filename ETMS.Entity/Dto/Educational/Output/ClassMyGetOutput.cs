using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassMyGetOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 班级类型  <see cref="ETMS.Entity.Enum.EmClassType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 课程列表，各课程Id之间以“,”隔开
        /// </summary>
        public string CourseDesc { get; set; }

        /// <summary>
        /// 学员个数
        /// </summary>
        public int StudentNums { get; set; }

        /// <summary>
        /// 班级容量
        /// </summary>
        public int? LimitStudentNums { get; set; }

        public string LimitStudentNumsDesc { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string ClassRoomDesc { get; set; }

        /// <summary>
        /// 老师，各老师Id之间以“,”隔开
        /// </summary>
        public string TeachersDesc { get; set; }

        public string CourseList { get; set; }

    }
}
