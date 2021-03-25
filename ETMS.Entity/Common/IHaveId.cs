﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    public interface IHaveId<T>
    {
        T Id { get; set; }
    }
}
