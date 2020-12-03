using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 员工通知(PC端)
    /// </summary>
    [Table("EtUserNotice")]
    public class EtUserNotice : Entity<long>
    {
        public long UserId { get; set; }

        /// <summary>
        ///  场景  <see cref="ETMS.Entity.Enum.EmUserNoticeSceneType"/>
        /// </summary>
        public int SceneType { get; set; }

        public string NoticeTitle { get; set; }

        public string NoticeContent { get; set; }

        public string RelationLink { get; set; }

        public string RelationIds { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmUserNoticeStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public DateTime Ot { get; set; }
    }
}
