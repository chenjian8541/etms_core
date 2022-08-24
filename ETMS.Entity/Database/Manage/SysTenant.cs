using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 机构
    /// </summary>
    [Table("SysTenant")]
    public class SysTenant : EManageEntity<int>
    {
        /// <summary>
        /// 所属代理商
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// etms系统版本
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime ExDate { get; set; }

        /// <summary>
        /// 短信数量
        /// </summary>
        public int SmsCount { get; set; }

        /// <summary>
        /// 限制用户数
        /// (0:不限制)
        /// </summary>
        public int MaxUserCount { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 购买状态  <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantBuyStatus"/>
        /// </summary>
        public byte BuyStatus { get; set; }

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

        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        /// <summary>
        /// 利楚扫呗申请状态
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcswApplyStatus"/>
        /// </summary>
        public int LcswApplyStatus { get; set; }

        /// <summary>
        /// 聚合支付(利楚扫呗)开启状态
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte LcswOpenStatus { get; set; }

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

        /// <summary>
        /// 短信签名
        /// </summary>
        public string SmsSignature { get; set; }

        /// <summary>
        /// 最后一次操作时间
        /// </summary>
        public DateTime? LastOpTime { get; set; }

        public decimal CloudStorageLimitGB { get; set; }

        public decimal CloudStorageValueGB { get; set; }

        public decimal CloudStorageValueMB { get; set; }

        public int FileLimitMB { get; set; }

        public DateTime? LastRenewalTime { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmPayUnionType"/>
        /// </summary>
        public int PayUnionType { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string IpAddress { get; set; }

        public DateTime? IpUpdateOt { get; set; }
    }
}
