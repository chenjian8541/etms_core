using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserAddRequest : RequestBase
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsTeacher { get; set; }

        public string Address { get; set; }

        public string Remark { get; set; }

        public long RoleId { get; set; }

        /// <summary>
        /// 在职类型  <see cref="ETMS.Entity.Enum.EmUserJobType"/>
        /// </summary>
        public int JobType { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "请输入手机号码";
            }
            if (RoleId <= 0)
            {
                return "请选择角色";
            }
            return string.Empty;
        }
    }
}
