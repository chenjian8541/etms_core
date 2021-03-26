using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentDuplicateCheckRequest : RequestBase
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        public override string Validate()
        {
            Phone = Phone.Trim();
            Name = Name.Trim();
            if (string.IsNullOrEmpty(Name))
            {
                return "学员名称不能为空";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            return base.Validate();
        }
    }
}
