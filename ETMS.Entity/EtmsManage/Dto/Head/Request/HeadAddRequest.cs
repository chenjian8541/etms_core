using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Head.Request
{
    public class HeadAddRequest : AgentRequestBase
    {
        public string HeadCode { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string LinkMan { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(HeadCode))
            {
                return "请输入企业编码";
            }
            if (HeadCode.Length < 3 || HeadCode.Length > 12)
            {
                return "企业编码长度必须在3~11范围内";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入企业名称";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "请输入联系电话";
            }
            return base.Validate();
        }
    }
}
