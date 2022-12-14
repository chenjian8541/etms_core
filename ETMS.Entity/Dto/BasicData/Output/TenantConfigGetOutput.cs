using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class TenantConfigGetOutput
    {
        /// <summary>
        /// 学员设置
        /// </summary>
        public StudentConfig StudentConfig { get; set; }

        /// <summary>
        /// 点名设置
        /// </summary>
        public ClassCheckSignConfig ClassCheckSignConfig { get; set; }

        /// <summary>
        /// 通知设置
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
        /// 学员端设置
        /// </summary>
        public ParentSetConfig ParentSetConfig { get; set; }

        public OtherOutput OtherOutput { get; set; }

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
    /// 需要处理的数据
    /// </summary>
    public class OtherOutput
    {
        /// <summary>
        /// 提前一天提醒,默认晚上7点(学员上课提醒)
        /// </summary>
        public string StartClassDayBeforeTimeValueDesc { get; set; }

        public string ParentLoginImageUrl { get; set; }

        public string TeacherLoginImageUrl { get; set; }

        public string RecommendDesImgUrl { get; set; }

        /// <summary>
        /// 考勤时间限制(开始时间)
        /// </summary>
        public string StudentCheckInLimitTimeStart { get; set; }

        /// <summary>
        /// 考勤时间限制(结束时间)
        /// </summary>
        public string StudentCheckInLimitTimeEnd { get; set; }
    }
}
