using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class UserGetPagingOutput
    {
        public long Cid { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public string LastLoginTimeDesc { get; set; }

        /// <summary>
        /// 是否为老师
        /// </summary>
        public bool IsTeacher { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string AvatarUrl { get; set; }

        public string JobTypeDesc { get; set; }

        public string IsTeacherDesc { get; set; }

        public int JobType { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }

        /// <summary> 
        /// 是否绑定微信  <see cref="ETMS.Entity.Enum.EmIsBindingWechat"/>
        /// </summary>
        public byte IsBindingWechat { get; set; }
    }
}
