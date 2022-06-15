using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActivityHaggleLog")]
    public class EtActivityHaggleLog : Entity<long>
    {
        public long ActivityId { get; set; }

        public long ActivityRouteId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string OpenId { get; set; }

        public string Unionid { get; set; }

        public string AvatarUrl { get; set; }

        public string NickName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
