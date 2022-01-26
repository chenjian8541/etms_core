using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ElectronicAlbumDetailGetPagingOutput
    {
        public string StudentName { get; set; }

        public long StudentId { get; set; }

        public int ReadCount { get; set; }

        public int ShareCount { get; set; }
    }
}
