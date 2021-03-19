using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantEditRequest : AgentRequestBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        public string SmsSignature { get; set; }


        /// <summary>
        /// 购买状态  <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantBuyStatus"/>
        /// </summary>
        public byte BuyStatus { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "数据格式错误";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "机构名称不能为空";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            if (string.IsNullOrEmpty(LinkMan))
            {
                return "联系人不能为空";
            }
            return base.Validate();
        }
    }
}
