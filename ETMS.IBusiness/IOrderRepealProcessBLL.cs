﻿using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IOrderRepealProcessBLL : IBaseBLL
    {
        Task OrderStudentEnrolmentRepealEventProcess(OrderStudentEnrolmentRepealEvent request);
    }
}
