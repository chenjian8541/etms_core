using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class TeacherEditRequest : RequestBase
    {
        public long CId { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// 昵称 （家长端显示）
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 擅长科目 科目之间以”,”隔开
        /// </summary>
        public string[] SubjectsGoodAt { get; set; }

        /// <summary>
        /// 教师资格证
        /// </summary>
        public string TeacherCertification { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (SubjectsGoodAt != null && SubjectsGoodAt.Length > 10)
            {
                return "最多设置10个擅长的科目";
            }
            return string.Empty;
        }
    }
}
