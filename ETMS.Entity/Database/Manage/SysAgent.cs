using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 代理商信息
    /// </summary>
    [Table("SysAgent")]
    public class SysAgent : EManageEntity<int>
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 作为机构编码的前缀
        /// </summary>
        public string TagKey { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///身份证号码
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 短信数量
        /// </summary>
        public int EtmsSmsCount { get; set; }

        /// <summary>
        /// 客服QQ
        /// </summary>
        public string KefuQQ { get; set; }

        /// <summary>
        /// 客服phone
        /// </summary>
        public string KefuPhone { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 是否锁定 <see cref=" ETMS.Entity.Enum.EtmsManage.EmSysAgentIsLock"/>
        /// </summary>
        public byte IsLock { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginOt { get; set; }

        /// <summary>
        /// 创建人 
        /// 0表示没有
        /// </summary>
        public int CreatedAgentId { get; set; }
    }
}
