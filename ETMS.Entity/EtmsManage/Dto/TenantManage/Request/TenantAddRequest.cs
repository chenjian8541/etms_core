using ETMS.Entity.EtmsManage.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantAddRequest : AgentRequestBase
    {
        public string TenantCode { get; set; }

        public string Name { get; set; }

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

        public string Phone { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 系统版本
        /// </summary>
        public int VersionId { get; set; }

        public int EtmsCount { get; set; }

        public int SmsCount { get; set; }

        public decimal Sum { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(TenantCode))
            {
                return "机构编码不能为空";
            }
            if (TenantCode.Length < 3 || TenantCode.Length > 12)
            {
                return "机构编码长度必须在3~11范围内";
            }
            if (!EtmsHelper.CheckIsDigitOrLetter(TenantCode))
            {
                return "机构编码只能由字母或数字组成";
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
            if (VersionId <= 0 || EtmsCount <= 0)
            {
                return "请填写完整购买的系统版本信息";
            }
            return base.Validate();
        }
    }
}
