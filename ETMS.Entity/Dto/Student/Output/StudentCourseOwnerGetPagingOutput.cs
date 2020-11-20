using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCourseOwnerGetPagingOutput
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarUrl { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }
    }
}
