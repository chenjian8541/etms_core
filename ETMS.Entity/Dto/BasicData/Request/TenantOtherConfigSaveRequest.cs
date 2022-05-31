using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class TenantOtherConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmValidPhoneType"/>
        /// </summary>
        public byte ValidPhoneType { get; set; }

        /// <summary>
        /// 学员端是否展示已结课的课程
        /// </summary>
        public bool ParentIsShowEndOfClass { get; set; }

        /// <summary>
        /// 自动结课
        /// </summary>
        public bool AutoMarkGraduation { get; set; }

        /// <summary>
        /// 老师端是否展示工资
        /// </summary>
        public bool TeacherIsShowSalary { get; set; }

        /// <summary>
        /// 是否开放注册
        /// </summary>
        public bool IsOpenStudentRegister { get; set; }

        /// <summary>
        /// 是否在学员端展示课时单位（节课）
        /// </summary>
        public bool IsStudentShowClassTimesUnit { get; set; }

        /// <summary>
        /// 学员端展示课时单位
        /// </summary>
        public decimal StudentShowClassTimesUnitValue { get; set; } = 1;

        /// <summary>
        /// 自动点名
        /// </summary>
        public bool IsAutoCheckSign { get; set; }

        /// <summary>
        /// 自动点名课次超时分钟
        /// </summary>
        public int AutoCheckSignLimitMinute { get; set; }
    }
}
