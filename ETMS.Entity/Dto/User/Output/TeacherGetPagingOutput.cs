using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class TeacherGetPagingOutput
    {
        public long Cid { get; set; }

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
        /// 昵称 （家长端显示）
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 擅长科目 科目之间以”,”隔开
        /// </summary>
        public string SubjectsGoodAtDesc { get; set; }

        /// <summary>
        /// 在职类型  <see cref="ETMS.Entity.Enum.EmUserJobType"/>
        /// </summary>
        public string JobTypeDesc { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public string JobAddTimeDesc { get; set; }

        /// <summary>
        /// 教师资格证
        /// </summary>
        public string TeacherCertification { get; set; }

        /// <summary>
        /// 已授次数
        /// </summary>
        public int TotalClassCount { get; set; }

        /// <summary>
        /// 已授课时
        /// </summary>
        public string TotalClassTimes { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        public string GenderDesc { get; set; }

        public string AvatarUrl { get; set; }

        /// <summary>
        /// 在职类型  <see cref="ETMS.Entity.Enum.EmUserJobType"/>
        /// </summary>
        public int JobType { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }

        /// <summary> 
        /// 是否绑定微信  <see cref="ETMS.Entity.Enum.EmIsBindingWechat"/>
        /// </summary>
        public byte IsBindingWechat { get; set; }
    }
}
