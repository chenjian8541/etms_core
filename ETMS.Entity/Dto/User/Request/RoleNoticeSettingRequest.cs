﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class RoleNoticeSettingRequest
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

        /// <summary>
        /// 允许使用APP登录(人脸考勤)
        /// </summary>
        public bool IsAllowAppLogin {get;set;}

        /// <summary>
        /// 允许查看机构统计信息
        /// </summary>
        public bool IsAllowLookStatistics { get; set; }

        /// <summary>
        /// 允许在PC端登录
        /// </summary>
        public bool IsAllowPCLogin { get; set; }

        /// <summary>
        /// 允许在微信公众号登录
        /// </summary>
        public bool IsAllowWebchatLogin { get; set; }

        /// <summary>
        /// 是否接收学员考勤通知
        /// </summary>
        public bool IsStudentCheckOnWeChat { get; set; }
    }
}
