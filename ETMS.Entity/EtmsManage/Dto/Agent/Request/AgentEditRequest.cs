using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentEditRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        //public string TagKey { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string IdCard { get; set; }

        public string Address { get; set; }

        public bool IsLock { get; set; }

        public string Remark { get; set; }

        public string KefuQQ { get; set; }

        public string KefuPhone { get; set; }

        public override string Validate()
        {
            Code = Code.Trim();
            Phone = Phone.Trim();
            if (Id <= 0)
            {
                return "代理商Id不能为空";
            }
            if (RoleId <= 0)
            {
                return "角色不能为空";
            }
            //if (string.IsNullOrEmpty(TagKey) && TagKey.Length != 3)
            //{
            //    return "代理商标识不能为空";
            //}
            if (string.IsNullOrEmpty(Code))
            {
                return "编码不能为空";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "名称不能为空";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            if (string.IsNullOrEmpty(Address))
            {
                return "联系地址不能为空";
            }
            return string.Empty;
        }
    }
}