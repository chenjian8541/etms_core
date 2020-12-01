using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtUserWechat")]
    public class EtUserWechat : Entity<long>
    {
        public long UserId { get; set; }

        public string Phone { get; set; }

        public string WechatUnionid { get; set; }

        public string WechatOpenid { get; set; }

        public string Nickname { get; set; }

        public string Headimgurl { get; set; }

        public string Remark { get; set; }
    }
}
