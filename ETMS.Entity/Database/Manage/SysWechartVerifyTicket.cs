using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 微信开放平台component_verify_ticket
    /// </summary>
    [Table("SysWechartVerifyTicket")]
    public class SysWechartVerifyTicket: EManageEntity<int>
    {
        /// <summary>
        /// 第三方开放平台appid
        /// </summary>
        public string ComponentAppId { get; set; }

        /// <summary>
        /// VerifyTicket
        /// </summary>
        public string ComponentVerifyTicket { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyOt { get; set; }
    }
}
