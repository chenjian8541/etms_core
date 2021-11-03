using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentTrackLogGetOutput
    {
        public long CId { get; set; }

        public List<Img> Imgs { get; set; }

        public string TrackContent { get; set; }

        public string NextTrackTime { get; set; }
    }
}
