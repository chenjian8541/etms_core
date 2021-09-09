using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Config
{
    /// <summary>
    /// 机构设置
    /// </summary>
    public class TenantConfig
    {
        public TenantConfig()
        {
            StudentConfig = new StudentConfig();
            ClassCheckSignConfig = new ClassCheckSignConfig();
            StudentNoticeConfig = new StudentNoticeConfig();
            StudentCourseRenewalConfig = new StudentCourseRenewalConfig();
            PrintConfig = new PrintConfig();
            ParentSetConfig = new ParentSetConfig();
            UserNoticeConfig = new UserNoticeConfig();
            TeacherSetConfig = new TeacherSetConfig();
            TenantInfoConfig = new TenantInfoConfig();
            StudentCheckInConfig = new StudentCheckInConfig();
            StudentRecommendConfig = new StudentRecommendConfig();
            TenantOtherConfig = new TenantOtherConfig();
        }

        /// <summary>
        /// 学员设置
        /// </summary>
        public StudentConfig StudentConfig { get; set; }

        /// <summary>
        /// 点名设置
        /// </summary>
        public ClassCheckSignConfig ClassCheckSignConfig { get; set; }

        /// <summary>
        /// 学员通知设置
        /// </summary>
        public StudentNoticeConfig StudentNoticeConfig { get; set; }

        /// <summary>
        /// 用户通知
        /// </summary>
        public UserNoticeConfig UserNoticeConfig { get; set; }

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

        /// <summary>
        /// 老师端设置
        /// </summary>
        public TeacherSetConfig TeacherSetConfig { get; set; }

        /// <summary>
        /// 机构详情
        /// </summary>
        public TenantInfoConfig TenantInfoConfig { get; set; }

        /// <summary>
        /// 学员考勤
        /// </summary>
        public StudentCheckInConfig StudentCheckInConfig { get; set; }

        /// <summary>
        /// 推荐有奖
        /// </summary>
        public StudentRecommendConfig StudentRecommendConfig { get; set; }

        /// <summary>
        /// 机构其它配置
        /// </summary>
        public TenantOtherConfig TenantOtherConfig { get; set; }
    }

    /// <summary>
    /// 学员设置
    /// </summary>
    public class StudentConfig
    {
        /// <summary>
        /// 初始密码
        /// </summary>
        public string InitialPassword { get; set; }
    }

    public class TenantOtherConfig
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmValidPhoneType"/>
        /// </summary>
        public byte ValidPhoneType { get; set; }

        /// <summary>
        /// 家长端是否展示已结课的课程
        /// </summary>
        public bool ParentIsShowEndOfClass { get; set; } = true;

        /// <summary>
        /// 自动结课
        /// </summary>
        public bool AutoMarkGraduation { get; set; }

        /// <summary>
        /// 学员请假次数限制类型
        /// <see cref=" ETMS.Entity.Enum.EmStudentLeaveApplyMonthLimitType"/>
        /// </summary>
        public byte StudentLeaveApplyMonthLimitType { get; set; } = EmStudentLeaveApplyMonthLimitType.Month;

        /// <summary>
        /// 学员请假次数限制
        /// </summary>
        public int StudentLeaveApplyMonthLimitCount { get; set; }

        /// <summary>
        /// 提前多少小时请假
        /// </summary>
        public int StudentLeaveApplyMustBeforeHour { get; set; }

        /// <summary>
        /// 老师端是否展示工资
        /// </summary>
        public bool TeacherIsShowSalary { get; set; } = true;
    }

    /// <summary>
    /// 推荐有奖
    /// </summary>
    public class StudentRecommendConfig
    {
        /// <summary>
        /// 注册
        /// </summary>
        public bool IsOpenRegistered { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int RegisteredGivePoints { get; set; }

        /// <summary>
        /// 奖励充值账户
        /// </summary>
        public decimal RegisteredGiveMoney { get; set; }

        /// <summary>
        /// 购课
        /// </summary>
        public bool IsOpenBuy { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int BuyGivePoints { get; set; }

        /// <summary>
        /// 奖励充值账户
        /// </summary>
        public decimal BuyGiveMoney { get; set; }

        /// <summary>
        /// 规则描述(文字)
        /// </summary>
        public string RecommendDesText { get; set; }

        /// <summary>
        /// 规则描述(图片)
        /// </summary>
        public string RecommendDesImg { get; set; }
    }

    public class StudentCheckInConfig
    {
        public StudentCheckInConfig()
        {
            StudentUseCardCheckIn = new StudentUseCardCheckIn();
            StudentUseFaceCheckIn = new StudentUseFaceCheckIn();
        }

        /// <summary>
        /// 考勤时间限制  <see cref="EmStudentCheckInLimitTimeType"/>
        /// </summary>
        public byte StudentCheckInLimitTimeType { get; set; }

        /// <summary>
        /// 考勤时间限制(开始时间)
        /// </summary>
        public int StudentCheckInLimitTimeStart { get; set; }

        /// <summary>
        /// 考勤时间限制(结束时间)
        /// </summary>
        public int StudentCheckInLimitTimeEnd { get; set; }

        /// <summary>
        /// 考勤记上课(关联课次)  是否自动确认 <see cref="EmBool"/>
        /// </summary>
        public byte IsRelationClassTimesAutoGenerateClassRecord { get; set; }

        /// <summary>
        /// 考勤记上课直接扣课时，学员存在多门课程时的处理方式
        /// <see cref="EmRelationClassTimesGoDeStudentCourseMulCourseType"/>
        /// </summary>
        public byte RelationClassTimesGoDeStudentCourseMulCourseType { get; set; }

        public StudentUseCardCheckIn StudentUseCardCheckIn { get; set; }

        public StudentUseFaceCheckIn StudentUseFaceCheckIn { get; set; }

    }

    public class StudentUseCardCheckIn
    {
        /// <summary>
        /// 学员刷卡间隔时间(单位秒)
        /// </summary>
        public int IntervalTimeCard { get; set; } = 30;

        /// <summary>
        /// 学员是否需要刷卡签退  <see cref="EmBool"/>
        /// </summary>
        public byte IsMustCheckOutCard { get; set; }

        /// <summary>
        /// 刷卡记上课
        /// </summary>
        public byte IsRelationClassTimesCard { get; set; }

        /// <summary>
        /// 关联的上课课次时间不超过的分钟数
        /// </summary>
        public int RelationClassTimesLimitMinuteCard { get; set; } = 10;

        /// <summary>
        /// 记上课类型 <see cref="EmAttendanceRelationClassTimesType"/>
        /// </summary>
        public byte RelationClassTimesCardType { get; set; }

        /// <summary>
        /// 每天限制记上课次数
        /// </summary>
        public byte RelationClassTimesCardType1DayLimitValue { get; set; }

        /// <summary>
        /// 是否展示快捷刷卡
        /// </summary>
        public byte IsShowQuickCardCheck { get; set; }
    }

    public class StudentUseFaceCheckIn
    {
        /// <summary>
        /// 学员刷脸间隔时间(单位秒)
        /// </summary>
        public int IntervalTimeFace { get; set; } = 30;

        /// <summary>
        /// 学员是否需要刷卡签退  <see cref="EmBool"/>
        /// </summary>
        public byte IsMustCheckOutFace { get; set; }

        /// <summary>
        /// 刷脸记上课
        /// </summary>
        public byte IsRelationClassTimesFace { get; set; }

        /// <summary>
        /// 关联的上课课次时间不超过的分钟数
        /// </summary>
        public int RelationClassTimesLimitMinuteFace { get; set; } = 10;

        /// <summary>
        /// 记上课类型 <see cref="EmAttendanceRelationClassTimesType"/>
        /// </summary>
        public byte RelationClassTimesFaceType { get; set; }

        /// <summary>
        /// 每天限制记上课次数
        /// </summary>
        public byte RelationClassTimesFaceType1DayLimitValue { get; set; }
    }

    public class UserNoticeConfig
    {
        public bool StartClassWeChat { get; set; }

        public bool StartClassSms { get; set; }

        public bool StudentHomeworkSubmitWeChat { get; set; } = true;

        /// <summary>
        /// 学员考勤通知（微信）
        /// </summary>
        public bool StudentCheckOnWeChat { get; set; } = true;

        public int StartClassBeforeMinuteValue { get; set; } = 30;

        /// <summary>
        /// 微信推送后缀
        /// </summary>
        public string WeChatNoticeRemark { get; set; }

        /// <summary>
        /// 学员请假申请通知(微信)
        /// </summary>
        public bool StudentLeaveApplyWeChat { get; set; } = true;

        /// <summary>
        /// 老师工资(微信)
        /// </summary>
        public bool TeacherSalaryPayrollWeChat { get; set; } = true;
    }

    public class TenantInfoConfig
    {
        /// <summary>
        /// 机构简介
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// 联系号码
        /// </summary>
        public string LinkPhone { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }
    }

    public class TeacherSetConfig
    {
        public string Title { get; set; }

        public string LoginImage { get; set; }
    }

    public class ParentSetConfig
    {
        public ParentSetConfig()
        {
            this.ParentBanners = new List<ParentBanner>();
            this.ParentMenus = new List<ParentMenuInfo>();
        }

        public List<ParentBanner> ParentBanners { get; set; }

        public string Title { get; set; }

        public string LoginImage { get; set; }

        public List<ParentMenuInfo> ParentMenus { get; set; }
    }

    public class ParentMenuInfo
    {
        public ParentMenuInfo() { }

        public ParentMenuInfo(int id, string title, int orderIndex)
        {
            this.Id = id;
            this.Title = title;
            this.OrderIndex = orderIndex;
            this.IcoKey = $"system/material/parent/menus/{id}.png";
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string IcoKey { get; set; }

        public bool IsShow { get; set; }

        public int OrderIndex { get; set; }
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

        /// <summary>
        /// 学员考勤通知（微信）
        /// </summary>
        public bool StudentCheckOnWeChat { get; set; } = true;

        /// <summary>
        /// 学员考勤通知（短信）
        /// </summary>
        public bool StudentCheckOnSms { get; set; }

        /// <summary>
        /// 优惠券提醒(微信)
        /// </summary>
        public bool StudentCouponsWeChat { get; set; } = true;

        /// <summary>
        /// 充值账户变动提醒
        /// </summary>
        public bool StudentAccountRechargeChangedWeChat { get; set; } = true;

        /// <summary>
        /// 充值账户变动提醒
        /// </summary>
        public bool StudentAccountRechargeChangedSms { get; set; } = true;

        /// <summary>
        /// 学员课时变动提醒
        /// </summary>
        public bool StudentCourseSurplusChangedWeChat { get; set; } = true;

        /// <summary>
        /// 学员课时变动提醒
        /// </summary>
        public bool StudentCourseSurplusChangedSms { get; set; } = true;
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
