﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class RoleNoticeSettingOutput
    {
        /// <summary>
        ///接收学员请假申请提醒
        /// </summary>
        public bool IsStudentLeaveApply { get; set; }

        /// <summary>
        ///接收上课点名未到学员提醒
        /// </summary>
        public bool IsStudentContractsNotArrived { get; set; }

        /// <summary>
        /// 试听申请
        /// </summary>
        public bool IsTryCalssApply { get; set; }

        /// <summary>
        /// 接收学员互动消息提醒
        /// </summary>
        public bool IsReceiveInteractiveStudent { get; set; }
    }
}
