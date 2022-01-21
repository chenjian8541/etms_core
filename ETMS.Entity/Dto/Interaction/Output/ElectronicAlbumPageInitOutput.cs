using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ElectronicAlbumPageInitOutput
    {
        public string RenderKey { get; set; }

        public string RenderUrl { get; set; }

        public string NewRenderKey { get; set; }

        public string NewCoverKey { get; set; }

        public string Name { get; set; }

        public List<AlbumLibImg> ImgList { get; set; }

        public List<AlbumLibAudio> AudioList { get; set; }
    }

    public class AlbumLibImg
    {
        public string ImgUrl { get; set; }
    }

    public class AlbumLibAudio
    {
        public string AudioUrl { get; set; }

        public string Name { get; set; }
    }
}
