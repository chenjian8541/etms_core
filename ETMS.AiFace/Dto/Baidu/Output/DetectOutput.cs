using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.AiFace.Dto.Baidu.Output
{
    public class DetectOutput
    {
        public int face_num { get; set; }

        public List<DetectFace> face_list { get; set; }
    }

    public class DetectFace {
        public string face_token { get; set; }

        public DetectFaceLocation location { get; set; }

        public double face_probability { get; set; }

        public double beauty { get; set; }

        public DetectFaceQuality quality { get; set; }
    }

    public class DetectFaceQuality
    {
        public DetectFaceQualityOcclusion occlusion { get; set; }

        public double blur { get; set; }

        public double illumination { get; set; }

        public double completeness { get; set; }
    }

    public class DetectFaceQualityOcclusion
    {
        public double left_eye { get; set; }

        public double right_eye { get; set; }

        public double nose { get; set; }

        public double mouth { get; set; }

        public double left_cheek { get; set; }

        public double right_cheek { get; set; }

        public double chin { get; set; }
    }

    public class DetectFaceLocation {
        public double left { get; set; }

        public double top { get; set; }

        public double width { get; set; }

        public double height { get; set; }

        public double rotation { get; set; }
    }
}
