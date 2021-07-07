using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentTrackLogGetListOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 跟进时间
        /// </summary>
        public string TrackTimeDesc { get; set; }

        /// <summary>
        /// 下一次跟进时间
        /// </summary>
        public string NextTrackTimeDesc { get; set; }

        /// <summary>
        /// 跟进内容
        /// </summary>
        public string TrackContent { get; set; }

        public string TrackUserName { get; set; }

        public string TrackUserAvatarUrl { get; set; }

        public List<string> TrackImgUrl { get; set; }
    }
}
