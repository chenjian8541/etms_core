﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassTimesRuleDefiniteGetOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        public string StartTimeDesc { get; set; }

        public string EndTimeDesc { get; set; }

        /// <summary>
        /// 预约类型  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public List<long> TeacherIds { get; set; }

        public List<long> CourseIds { get; set; }

        public bool IsJumpHoliday { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
