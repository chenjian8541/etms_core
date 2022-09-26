using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Output
{
    public class UserEnterH5Output
    {
        public bool IsOtherLogin { get; set; }

        public UserLoginBySmsH5Output LoginIngo { get; set; }
    }
}
