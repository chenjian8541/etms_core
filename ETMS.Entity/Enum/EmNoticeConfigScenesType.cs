using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmNoticeConfigScenesType
    {
        /// <summary>
        /// 学员上课提醒（微信）
        /// </summary>
        public const int StartClass = 0;

        /// <summary>
        /// 点名提醒（微信）
        /// </summary>
        public const int ClassCheckSign = 1;

        /// <summary>
        /// 学员请假审核结果通知(微信)
        /// </summary>
        public const int StudentAskForLeaveCheck = 2;

        /// <summary>
        /// 订单购买
        /// </summary>
        public const int OrderBuy = 3;

        /// <summary>
        /// 老师点评提醒(微信)
        /// </summary>
        public const int TeacherClassEvaluate = 4;

        /// <summary>
        /// 成长档案(微信)
        /// </summary>
        public const int StudentGrowUpRecord = 5;

        /// <summary>
        /// 课后作业(微信)
        /// </summary>
        public const int StudentHomework = 6;

        /// <summary>
        /// 课后作业点评(微信)
        /// </summary>
        public const int StudentHomeworkComment = 7;

        /// <summary>
        /// 修改点名记录提醒(微信)
        /// </summary>
        public const int ClassRecordStudentChange = 8;

        /// <summary>
        /// 学员课时不足提醒（微信）
        /// </summary>
        public const int StudentCourseNotEnough = 9;

        /// <summary>
        /// 学员考勤通知（微信）
        /// </summary>
        public const int StudentCheckOn = 10;

        /// <summary>
        /// 优惠券提醒(微信)
        /// </summary>
        public const int StudentCoupons = 11;

        /// <summary>
        /// 充值账户变动提醒
        /// </summary>
        public const int StudentAccountRechargeChanged = 12;

        /// <summary>
        /// 学员课时变动提醒
        /// </summary>
        public const int StudentCourseSurplusChanged = 13;

        /// <summary>
        /// 电子相册发布
        /// </summary>
        public const int StudentAlbumPublish = 14;
    }
}
