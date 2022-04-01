using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.Tenant.Output
{
    public class StudentGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? AgeMonth { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string BirthdayDesc { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? Gender { get; set; }

        public string GenderDesc { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

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

        /// <summary>
        /// 结课时间
        /// </summary>
        public string EndClassOtDesc { get; set; }

        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        public string StudentTypeDesc { get; set; }

        /// <summary> 
        /// 是否绑定微信  <see cref="ETMS.Entity.Enum.EmIsBindingWechat"/>
        /// </summary>
        public byte IsBindingWechat { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string OtDesc { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }
    }
}
