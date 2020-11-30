using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    /// <summary>
    /// 机构设置
    /// </summary>
    public class TenantConfig
    {
        public TenantConfig()
        {
            ClassCheckSignConfig = new ClassCheckSignConfig();
            StudentNoticeConfig = new StudentNoticeConfig();
            StudentCourseRenewalConfig = new StudentCourseRenewalConfig();
            PrintConfig = new PrintConfig();
            ParentSetConfig = new ParentSetConfig();
        }

        /// <summary>
        /// 点名设置
        /// </summary>
        public ClassCheckSignConfig ClassCheckSignConfig { get; set; }

        /// <summary>
        /// 通知设置
        /// </summary>
        public StudentNoticeConfig StudentNoticeConfig { get; set; }

        /// <summary>
        /// 续费预警设置
        /// </summary>
        public StudentCourseRenewalConfig StudentCourseRenewalConfig { get; set; }

        /// <summary>
        /// 打印设置
        /// </summary>
        public PrintConfig PrintConfig { get; set; }

        /// <summary>
        /// 家长端设置
        /// </summary>
        public ParentSetConfig ParentSetConfig { get; set; }
    }

    public class ParentSetConfig
    {
        public ParentSetConfig()
        {
            this.ParentBanners = new List<ParentBanner>();
        }

        public List<ParentBanner> ParentBanners { get; set; }
    }

    public class ParentBanner
    {
        public string ImgKey { get; set; }

        public string UrlKey { get; set; }
    }

    public class PrintConfig
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 宣传语
        /// </summary>
        public string BottomDesc { get; set; }

        /// <summary>
        /// 打印类型  <see cref=" ETMS.Entity.Enum.EmPrintType"/>
        /// </summary>
        public int PrintType { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        public string TagImgKey { get; set; }
    }

    public class StudentCourseRenewalConfig
    {
        /// <summary>
        /// 剩余课时
        /// </summary>
        public int LimitClassTimes { get; set; }

        /// <summary>
        /// 剩余天数
        /// </summary>
        public int LimitDay { get; set; }
    }

    public class StudentNoticeConfig
    {

        /// <summary>
        /// 学员上课提醒（微信）
        /// </summary>
        public bool StartClassWeChat { get; set; } = true;

        /// <summary>
        /// 学员上课提醒（短信）
        /// </summary>
        public bool StartClassSms { get; set; } = false;

        /// <summary>
        /// 点名提醒（微信）
        /// </summary>
        public bool ClassCheckSignWeChat { get; set; } = true;

        /// <summary>
        /// 点名提醒(短信)
        /// </summary>
        public bool ClassCheckSignSms { get; set; } = false;

        /// <summary>
        /// 订单购买
        /// </summary>
        public bool OrderByWeChat { get; set; } = true;

        /// <summary>
        /// 订单购买
        /// </summary>
        public bool OrderBySms { get; set; } = false;

        /// <summary>
        /// 老师点评提醒(微信)
        /// </summary>
        public bool TeacherClassEvaluateWeChat { get; set; } = true;

        /// <summary>
        /// 成长档案(微信)
        /// </summary>
        public bool StudentGrowUpRecordWeChat { get; set; } = true;

        /// <summary>
        /// 课后作业(微信)
        /// </summary>
        public bool StudentHomeworkWeChat { get; set; } = true;

        /// <summary>
        /// 课后作业点评(微信)
        /// </summary>
        public bool StudentHomeworkCommentWeChat { get; set; } = true;

        /// <summary>
        /// 修改点名记录提醒(微信)
        /// </summary>
        public bool ClassRecordStudentChangeWeChat { get; set; } = true;

        /// <summary>
        /// 是否提前一天提醒（学员上课提醒）
        /// </summary>
        public bool StartClassDayBeforeIsOpen { get; set; } = true;

        /// <summary>
        /// 提前一天提醒,默认晚上7点(学员上课提醒)
        /// </summary>
        public int StartClassDayBeforeTimeValue { get; set; } = 1900;

        /// <summary>
        /// 是否提前几分钟提醒(学员上课提醒)
        /// </summary>
        public bool StartClassBeforeMinuteIsOpen { get; set; } = true;

        /// <summary>
        /// 前几分钟提醒，默认30分钟(学员上课提醒)
        /// </summary>
        public int StartClassBeforeMinuteValue { get; set; } = 30;

        /// <summary>
        /// 学员请假审核结果通知(短信)
        /// </summary>
        public bool StudentAskForLeaveCheckSms { get; set; } = false;

        /// <summary>
        /// 学员请假审核结果通知(微信)
        /// </summary>
        public bool StudentAskForLeaveCheckWeChat { get; set; } = true;

        /// <summary>
        /// 学员课时不足提醒（微信）
        /// </summary>
        public bool StudentCourseNotEnoughWeChat { get; set; } = true;

        /// <summary>
        /// 学员课时不足提醒（短信）
        /// </summary>
        public bool StudentCourseNotEnoughSms { get; set; }

        /// <summary>
        /// 学员课时不足提醒提醒次数
        /// </summary>
        public int StudentCourseNotEnoughCount { get; set; } = 1;

        /// <summary>
        /// 微信推送后缀
        /// </summary>
        public string WeChatNoticeRemark { get; set; }
    }

    public class ClassCheckSignConfig
    {
        /// <summary>
        /// 补课是否扣课时
        /// </summary>
        public bool MakeupIsDeClassTimes { get; set; } = false;

        /// <summary>
        /// 点名奖励的积分是否需要审核
        /// </summary>
        public bool RewardPointsMustApply { get; set; } = false;

        /// <summary>
        /// 点名时按时收费课程必须设置起止日期
        /// </summary>
        public bool DayCourseMustSetStartEndTime { get; set; } = true;

        /// <summary>
        /// 点名时学员必须购买过此课程
        /// </summary>
        public bool MustBuyCourse { get; set; } = false;

        /// <summary>
        /// 点名时 是否必须有足够的剩余课时
        /// </summary>
        public bool MustEnoughSurplusClassTimes { get; set; } = false;

        /// <summary>
        /// 学员试听结果 通知跟进人
        /// </summary>
        public bool TryCalssNoticeTrackUser { get; set; } = true;

        /// <summary>
        /// 点名时扣减课时是否允许输入小数
        /// </summary>
        public bool IsCanDeDecimal { get; set; } = false;
    }
}
