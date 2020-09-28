using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class TeacherClassTimesGetPagingOutput
    {
        public string DateDesc { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public int ClassTimes { get; set; }

        /// <summary>
        /// 课次
        /// </summary>
        public int ClassCount { get; set; }

        public string UserName { get; set; }

        public string UserPhone { get; set; }
    }
}
