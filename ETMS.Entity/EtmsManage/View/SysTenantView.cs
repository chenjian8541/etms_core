using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.View
{
    public class SysTenantView
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机构代码
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// 机构电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 数据库ID
        /// </summary>
        public int ConnectionId { get; set; }

        public int AgentId { get; set; }

        public int VersionId { get; set; }

        public DateTime ExDate { get; set; }

        public int SmsCount { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 购买状态  <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantBuyStatus"/>
        /// </summary>
        public byte BuyStatus { get; set; }

        public string AgentName { get; set; }

        public string AgentPhone { get; set; }

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
        /// 限制用户数
        /// (0:不限制)
        /// </summary>
        public int MaxUserCount { get; set; }

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

        public long UserId { get; set; }

        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        /// <summary>
        /// 最后一次操作时间
        /// </summary>
        public DateTime? LastOpTime { get; set; }

        public decimal CloudStorageLimitGB { get; set; }

        public decimal CloudStorageValueGB { get; set; }

        public decimal CloudStorageValueMB { get; set; }

        public DateTime? LastRenewalTime { get; set; }
    }
}
