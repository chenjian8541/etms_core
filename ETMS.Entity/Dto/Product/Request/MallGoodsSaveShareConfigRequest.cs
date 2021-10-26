using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Request
{
    public class MallGoodsSaveShareConfigRequest : RequestBase
    {
        public string Title { get; set; }

        public string HomeShareImgKey { get; set; }

        public override string Validate()
        {
            return base.Validate();
        }
    }
}

