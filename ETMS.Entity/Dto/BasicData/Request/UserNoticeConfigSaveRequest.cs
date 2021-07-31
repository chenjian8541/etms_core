using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class UserNoticeConfigSaveRequest : RequestBase
    {
        public bool StartClassWeChat { get; set; }

        public bool StartClassSms { get; set; }

        public bool StudentHomeworkSubmitWeChat { get; set; }

        public bool StudentCheckOnWeChat { get; set; }

        /// <summary>
        /// 学员请假申请通知(微信)
        /// </summary>
        public bool StudentLeaveApplyWeChat { get; set; }
    }
}
