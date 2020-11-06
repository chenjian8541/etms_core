﻿using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeStudentsOfHomeworkAddQueue")]
    public class NoticeStudentsOfHomeworkAddEventConsumer : ConsumerBase<NoticeStudentsOfHomeworkAddEvent>
    {
        protected override async Task Receive(NoticeStudentsOfHomeworkAddEvent eEvent)
        {
            var studentSendNoticeBLL = CustomServiceLocator.GetInstance<IStudentSendNotice2BLL>();
            studentSendNoticeBLL.InitTenantId(eEvent.TenantId);
            await studentSendNoticeBLL.NoticeStudentsOfHomeworkAddConsumeEvent(eEvent);
        }
    }
}
