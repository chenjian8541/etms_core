﻿using ETMS.Entity.Alien.Common;
using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Alien
{
    public interface IAlienBascBLL : IAlienBaseBLL
    {
        ResponseBase UploadConfigGet(AlienRequestBase request);
    }
}
