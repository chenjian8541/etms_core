using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class StudentDetailInfoOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarKey { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string BirthdayDesc { get; set; }

        /// <summary>
        /// 手机号码描述
        /// </summary>
        public long PhoneRelationship { get; set; }

        public string PhoneRelationshipDesc { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 备用号码描述
        /// </summary>
        public long? PhoneBakRelationship { get; set; }

        public string PhoneBakRelationshipDesc { get; set; }

        /// <summary>
        /// 备用号码
        /// </summary>
        public string PhoneBak { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string HomeAddress { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Points { get; set; }
    }
}
