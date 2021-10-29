using ETMS.Entity.Dto.Common.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassBascGetOutput
    {
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        public string ClassName { get; set; }

        public string DefaultClassTimes { get; set; }

        /// <summary>
        /// 请假是否收费
        /// </summary>
        public bool IsLeaveCharge { get; set; }

        /// <summary>
        /// 未到是否收费
        /// </summary>
        public bool IsNotComeCharge { get; set; }

        public bool IsCanOnlineSelClass { get; set; }

        public List<SelectItem> ClassRooms { get; set; }

        public List<SelectItem> ClassCourses { get; set; }

        public List<SelectItem> ClassTeachers { get; set; }
    }
}
