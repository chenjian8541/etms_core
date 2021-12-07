using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.CoreBusiness.Output
{
    public abstract class AgtPayServiceOutputBase
    {
        public bool IsSuccess { get; set; }

        public string ErrMsg { get; set; }
    }
}
