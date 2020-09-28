using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class UserGetOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

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
        /// 是否老师
        /// </summary>
        public bool IsTeacher { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarUrl { get; set; }

        public string AvatarKey { get; set; }

        /// <summary>
        /// 昵称 （家长端显示）
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// 擅长科目 科目之间以”,”隔开
        /// </summary>
        public string SubjectsGoodAt { get; set; }

        public string[] SubjectsGoodAtItems { get; set; }

        /// <summary>
        /// 在职类型  <see cref="ETMS.Entity.Enum.EmUserJobType"/>
        /// </summary>
        public int JobType { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public string JobAddTimeDesc { get; set; }

        /// <summary>
        /// 教师资格证
        /// </summary>
        public string TeacherCertification { get; set; }

        /// <summary>
        /// 已授课时
        /// </summary>
        public int TotalClassTimes { get; set; }

        public int TotalClassCount { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public string LastLoginTimeDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
