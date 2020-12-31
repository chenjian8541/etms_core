﻿using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.SendNotice
{
    public interface IStudentSendNotice3BLL : IBaseBLL
    {
        Task NoticeStudentCouponsGetConsumerEvent(NoticeStudentCouponsGetEvent request);

        Task NoticeStudentCouponsExplainConsumetEvent(NoticeStudentCouponsExplainEvent request);
    }
}