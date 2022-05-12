using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    public struct RoleOtherSetting
    {
        /// <summary>
        ///接收学员请假申请提醒
        /// </summary>
        public const int StudentLeaveApply = 1;

        /// <summary>
        ///接收上课点名未到学员提醒
        /// </summary>
        public const int StudentContractsNotArrived = 2;

        /// <summary>
        ///试听申请
        /// </summary>
        public const int TryCalssApply = 3;

        /// <summary>
        /// 接收学员互动提醒
        /// </summary>
        public const int ReceiveInteractiveStudent = 4;

        /// <summary>
        /// 允许使用APP登录(人脸考勤)
        /// </summary>
        public const int AllowAppLogin = 5;

        /// <summary>
        /// 允许查看机构统计信息
        /// </summary>
        public const int AllowLookStatistics = 6;

        /// <summary>
        /// 允许在PC端登录
        /// </summary>
        public const int AllowPCLogin = 7;

        /// <summary>
        /// 允许在微信公众号登录
        /// </summary>
        public const int AllowWebchatLogin = 8;

        /// <summary>
        /// 接收学员考勤通知
        /// </summary>
        public const int StudentCheckOnWeChat = 9;

        /// <summary>
        /// 接收学员请假申请提醒
        /// </summary>
        public const int StudentLeaveApplyMy = 10;

        /// <summary>
        /// 接收上课点名未到学员提醒
        /// </summary>
        public const int StudentContractsNotArrivedMy = 11;

        /// <summary>
        /// 接收学员互动提醒
        /// </summary>
        public const int ReceiveInteractiveStudentMy = 12;

        /// <summary>
        /// 接收学员考勤通知
        /// </summary>
        public const int StudentCheckOnWeChatMy = 13;
    }
}
