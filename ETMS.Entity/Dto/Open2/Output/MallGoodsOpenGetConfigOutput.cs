using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Output
{
    public class MallGoodsOpenGetConfigOutput
    {
        public string HomeShareImgUrl { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallGoodsStatus"/>
        /// </summary>
        public byte MallGoodsStatus { get; set; }

        public string Title { get; set; }
    }
}
