using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentNoticeConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// 学员上课提醒（微信）
        /// </summary>
        public bool StartClassWeChat { get; set; }

        /// <summary>
        /// 学员上课提醒（短信）
        /// </summary>
        public bool StartClassSms { get; set; }

        /// <summary>
        /// 点名提醒（微信）
        /// </summary>
        public bool ClassCheckSignWeChat { get; set; }

        /// <summary>
        /// 点名提醒(短信)
        /// </summary>
        public bool ClassCheckSignSms { get; set; }

        /// <summary>
        /// 订单购买
        /// </summary>
        public bool OrderByWeChat { get; set; }

        /// <summary>
        /// 订单购买
        /// </summary>
        public bool OrderBySms { get; set; }

        /// <summary>
        /// 老师点评提醒(微信)
        /// </summary>
        public bool TeacherClassEvaluateWeChat { get; set; }

        /// <summary>
        /// 成长档案(微信)
        /// </summary>
        public bool StudentGrowUpRecordWeChat { get; set; }

        /// <summary>
        /// 课后作业(微信)
        /// </summary>
        public bool StudentHomeworkWeChat { get; set; }

        /// <summary>
        /// 课后作业点评提醒(微信)
        /// </summary>
        public bool StudentHomeworkCommentWeChat { get; set; }

        /// <summary>
        /// 学员请假审核结果通知(短信)
        /// </summary>
        public bool StudentAskForLeaveCheckSms { get; set; }

        /// <summary>
        /// 学员请假审核结果通知(微信)
        /// </summary>
        public bool StudentAskForLeaveCheckWeChat { get; set; }

        public bool ClassRecordStudentChangeWeChat { get; set; }

        /// <summary>
        /// 学员课时不足提醒（微信）
        /// </summary>
        public bool StudentCourseNotEnoughWeChat { get; set; }

        /// <summary>
        /// 学员课时不足提醒（短信）
        /// </summary>
        public bool StudentCourseNotEnoughSms { get; set; }

        /// <summary>
        /// 学员考勤通知（微信）
        /// </summary>
        public bool StudentCheckOnWeChat { get; set; }

        /// <summary>
        /// 学员考勤通知（短信）
        /// </summary>
        public bool StudentCheckOnSms { get; set; }

        /// <summary>
        /// 优惠券提醒(微信)
        /// </summary>
        public bool StudentCouponsWeChat { get; set; }

        /// <summary>
        /// 充值账户变动提醒
        /// </summary>
        public bool StudentAccountRechargeChangedWeChat { get; set; }

        /// <summary>
        /// 充值账户变动提醒
        /// </summary>
        public bool StudentAccountRechargeChangedSms { get; set; }

        /// <summary>
        /// 学员课时变动提醒
        /// </summary>
        public bool StudentCourseSurplusChangedWeChat { get; set; }

        /// <summary>
        /// 学员课时变动提醒
        /// </summary>
        public bool StudentCourseSurplusChangedSms { get; set; }

        /// <summary>
        /// 电子相册发布提醒
        /// </summary>
        public bool StudentAlbumPublishWeChat { get; set; }
    }
}
