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
    }
}
