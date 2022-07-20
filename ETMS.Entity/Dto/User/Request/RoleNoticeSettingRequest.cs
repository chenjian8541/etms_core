using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class RoleNoticeSettingRequest
    {
        /// <summary>
        /// 试听申请
        /// </summary>
        public bool IsTryCalssApply { get; set; }

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

        public bool IsOpenStudentLeaveApply { get; set; }

        public int OpenStudentLeaveApplyType { get; set; }

        public bool IsOpenStudentContractsNotArrived { get; set; }

        public int OpenStudentContractsNotArrivedType { get; set; }

        public bool IsOpenReceiveInteractiveStudent { get; set; }

        public int OpenReceiveInteractiveStudentType { get; set; }

        public bool IsOpenStudentCheckOnWeChat { get; set; }

        public int OpenStudentCheckOnWeChatType { get; set; }

        public bool IsEverydayBusinessStatistics { get; set; }
    }
}
