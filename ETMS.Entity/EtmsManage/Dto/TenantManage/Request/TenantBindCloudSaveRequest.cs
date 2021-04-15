using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantBindCloudSaveRequest : AgentRequestBase
    {
        public int Id { get; set; }

        /// <summary>
        /// 云AI类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantAICloudType"/>
        /// </summary>
        public int AICloudType { get; set; }

        /// <summary>
        /// 腾讯云账号ID 
        /// </summary>
        public int TencentCloudId { get; set; }

        /// <summary>
        /// 百度云账号ID 
        /// </summary>
        public int BaiduCloudId { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "机构Id不能为空";
            }
            if (AICloudType == EmSysTenantAICloudType.BaiduCloud && BaiduCloudId < 0)
            {
                return "请选择百度云账户";
            }
            if (AICloudType == EmSysTenantAICloudType.TencentCloud && TencentCloudId < 0)
            {
                return "请选择腾讯云账户";
            }
            return base.Validate();
        }
    }
}
