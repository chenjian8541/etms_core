using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentListGetOutput
    {
        public long StudentId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// 头像key
        /// </summary>
        public string AvatarKey { get; set; }

        /// <summary>
        /// 头像url
        /// </summary>
        public string AvatarUrl { get; set; }
    }
}
