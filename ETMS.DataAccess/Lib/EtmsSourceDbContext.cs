using ETMS.Entity.Database.Source;
using ETMS.IOC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess.Lib
{
    /// <summary>
    /// EtmsSource数据访问
    /// </summary>
    public class EtmsSourceDbContext : DbContext
    {
        private readonly string _connectionString;

        public EtmsSourceDbContext(string connectionString)
        {
            this._connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<EtAppConfig> EtAppConfigs { get; set; }
        public DbSet<EtClass> EtClasss { get; set; }
        public DbSet<EtClassCategory> EtClassCategorys { get; set; }
        public DbSet<EtClassRecord> EtClassRecords { get; set; }
        public DbSet<EtClassRecordAbsenceLog> EtClassRecordAbsenceLogs { get; set; }
        public DbSet<EtClassRecordEvaluateStudent> EtClassRecordEvaluateStudents { get; set; }
        public DbSet<EtClassRecordEvaluateTeacher> EtClassRecordEvaluateTeachers { get; set; }
        public DbSet<EtClassRecordOperationLog> EtClassRecordOperationLogs { get; set; }
        public DbSet<EtClassRecordPointsApplyLog> EtClassRecordPointsApplyLogs { get; set; }
        public DbSet<EtClassRecordStudent> EtClassRecordStudents { get; set; }
        public DbSet<EtClassRoom> EtClassRooms { get; set; }
        public DbSet<EtClassSet> EtClassSets { get; set; }
        public DbSet<EtClassStudent> EtClassStudents { get; set; }
        public DbSet<EtClassTimes> EtClassTimess { get; set; }
        public DbSet<EtClassTimesRule> EtClassTimesRules { get; set; }
        public DbSet<EtClassTimesStudent> EtClassTimesStudents { get; set; }
        public DbSet<EtCost> EtCosts { get; set; }
        public DbSet<EtCoupons> EtCouponss { get; set; }
        public DbSet<EtCouponsStudentGet> EtCouponsStudentGets { get; set; }
        public DbSet<EtCouponsStudentUse> EtCouponsStudentUses { get; set; }
        public DbSet<EtCourse> EtCourses { get; set; }
        public DbSet<EtCoursePriceRule> EtCoursePriceRules { get; set; }
        public DbSet<EtGift> EtGifts { get; set; }
        public DbSet<EtGiftExchangeLog> EtGiftExchangeLogs { get; set; }
        public DbSet<EtGiftExchangeLogDetail> EtGiftExchangeLogDetails { get; set; }
        public DbSet<EtGoods> EtGoodss { get; set; }
        public DbSet<EtGoodsInventoryLog> EtGoodsInventoryLogs { get; set; }
        public DbSet<EtGrade> EtGrades { get; set; }
        public DbSet<EtHolidaySetting> EtHolidaySettings { get; set; }
        public DbSet<EtIncomeLog> EtIncomeLogs { get; set; }
        public DbSet<EtOrder> EtOrders { get; set; }
        public DbSet<EtOrderDetail> EtOrderDetails { get; set; }
        public DbSet<EtOrderOperationLog> EtOrderOperationLogs { get; set; }
        public DbSet<EtRole> EtRoles { get; set; }
        public DbSet<EtStudent> EtStudents { get; set; }
        public DbSet<EtStudentCourse> EtStudentCourses { get; set; }
        public DbSet<EtStudentCourseConsumeLog> EtStudentCourseConsumeLogs { get; set; }
        public DbSet<EtStudentCourseDetail> EtStudentCourseDetails { get; set; }
        public DbSet<EtStudentCourseStopLog> EtStudentCourseStopLogs { get; set; }
        public DbSet<EtStudentExtendField> EtStudentExtendFields { get; set; }
        public DbSet<EtStudentExtendInfo> EtStudentExtendInfos { get; set; }
        public DbSet<EtStudentGrowingTag> EtStudentGrowingTags { get; set; }
        public DbSet<EtStudentLeaveApplyLog> EtStudentLeaveApplyLogs { get; set; }
        public DbSet<EtStudentNotice> EtStudentNotices { get; set; }
        public DbSet<EtStudentPointsLog> EtStudentPointsLogs { get; set; }
        public DbSet<EtStudentRelationship> EtStudentRelationships { get; set; }
        public DbSet<EtStudentSource> EtStudentSources { get; set; }
        public DbSet<EtStudentTag> EtStudentTags { get; set; }
        public DbSet<EtStudentTrackLog> EtStudentTrackLogs { get; set; }
        public DbSet<EtStudentWechat> EtStudentWechats { get; set; }
        public DbSet<EtSubject> EtSubjectss { get; set; }
        public DbSet<EtTeacherClassTimes> EtTeacherClassHours { get; set; }
        public DbSet<EtTryCalssApplyLog> EtTryCalssApplyLogs { get; set; }
        public DbSet<EtTryCalssLog> EtTryCalssLogs { get; set; }
        public DbSet<EtUser> EtUsers { get; set; }
        public DbSet<EtUserOperationLog> EtUserOperationLogs { get; set; }
        public DbSet<EtGiftCategory> EtGiftCategorys { get; set; }
        public DbSet<EtStudentOperationLog> EtStudentOperationLogs { get; set; }
        public DbSet<EtStatisticsStudent> EtStatisticsStudents { get; set; }
        public DbSet<EtStatisticsStudentCount> EtStatisticsStudentCounts { get; set; }
        public DbSet<EtStatisticsStudentTrackCount> EtStatisticsStudentTrackCounts { get; set; }
        public DbSet<EtStatisticsSalesProduct> EtStatisticsSalesProducts { get; set; }
        public DbSet<EtIncomeProjectType> EtIncomeProjectTypes { get; set; }
        public DbSet<EtStatisticsFinanceIncome> EtStatisticsFinanceIncomes { get; set; }
        public DbSet<EtStatisticsClassAttendance> EtStatisticsClassAttendances { get; set; }
        public DbSet<EtStatisticsClassTimes> EtStatisticsClassTimess { get; set; }
        public DbSet<EtStatisticsClassCourse> EtStatisticsClassCourses { get; set; }
        public DbSet<EtStatisticsClassTeacher> EtStatisticsClassTeachers { get; set; }
        public DbSet<EtStatisticsSalesCourse> EtStatisticsSalesCourses { get; set; }
        public DbSet<EtStatisticsStudentSource> EtStatisticsStudentSource { get; set; }
        public DbSet<EtStatisticsClassAttendanceTag> EtStatisticsClassAttendanceTags { get; set; }
        public DbSet<EtTempStudentClassNotice> EtTempStudentClassNotices { get; set; }
        public DbSet<EtStudentSmsLog> EtStudentSmsLogs { get; set; }
        public DbSet<EtActiveHomework> EtActiveHomeworks { get; set; }
        public DbSet<EtActiveHomeworkDetail> EtActiveHomeworkDetails { get; set; }
        public DbSet<EtActiveHomeworkDetailComment> EtActiveHomeworkDetailComments { get; set; }
        public DbSet<EtActiveGrowthRecord> EtActiveGrowthRecords { get; set; }
        public DbSet<EtActiveGrowthRecordDetail> EtActiveGrowthRecordDetails { get; set; }
        public DbSet<EtActiveGrowthRecordDetailComment> EtActiveGrowthRecordDetailComments { get; set; }
        public DbSet<EtActiveWxMessage> EtActiveWxMessages { get; set; }
        public DbSet<EtActiveWxMessageDetail> EtActiveWxMessageDetails { get; set; }
        public DbSet<EtUserWechat> EtUserWechats { get; set; }
        public DbSet<EtTempUserClassNotice> EtTempUserClassNotices { get; set; }
        public DbSet<EtUserSmsLog> EtUserSmsLogs { get; set; }
        public DbSet<EtUserNotice> EtUserNotices { get; set; }
        public DbSet<EtStudentCheckOnLog> EtStudentCheckOnLogs { get; set; }
        public DbSet<EtTempStudentNeedCheck> EtTempStudentNeedChecks { get; set; }
        public DbSet<EtTempStudentNeedCheckClass> EtTempStudentNeedCheckClasss { get; set; }
        public DbSet<EtStudentCourseOpLog> EtStudentCourseOpLogs { get; set; }
        public DbSet<EtStatisticsStudentAccountRecharge> EtStatisticsStudentAccountRecharges { get; set; }
        public DbSet<EtStudentAccountRecharge> EtStudentAccountRecharges { get; set; }
        public DbSet<EtStudentAccountRechargeLog> EtStudentAccountRechargeLogs { get; set; }

        public DbSet<EtClassTimesReservationLog> EtClassTimesReservationLogs { get; set; }

        public DbSet<EtStudentAccountRechargeBinder> EtStudentAccountRechargeBinders { get; set; }

        public DbSet<EtStatisticsSalesTenant> EtStatisticsSalesTenants { get; set; }

        public DbSet<EtStatisticsSalesUser> EtStatisticsSalesUsers { get; set; }

        public DbSet<EtSuit> EtSuits { get; set; }

        public DbSet<EtSuitDetail> EtSuitDetails { get; set; }

        public DbSet<EtMicroWebConfig> EtMicroWebConfigs { get; set; }

        public DbSet<EtMicroWebColumn> EtMicroWebColumns { get; set; }

        public DbSet<EtMicroWebColumnArticle> EtMicroWebColumnArticles { get; set; }

        public DbSet<EtStatisticsStudentCountMonth> EtStatisticsStudentCountMonths { get; set; }

        public DbSet<EtStatisticsSalesProductMonth> EtStatisticsSalesProductMonths { get; set; }

        public DbSet<EtStatisticsEducationMonth> EtStatisticsEducationMonths { get; set; }

        public DbSet<EtStatisticsEducationClassMonth> EtStatisticsEducationClassMonths { get; set; }

        public DbSet<EtStatisticsEducationTeacherMonth> EtStatisticsEducationTeacherMonths { get; set; }

        public DbSet<EtStatisticsEducationCourseMonth> EtStatisticsEducationCourseMonths { get; set; }

        public DbSet<EtStatisticsEducationStudentMonth> EtStatisticsEducationStudentMonths { get; set; }

        public DbSet<EtStatisticsFinanceIncomeMonth> EtStatisticsFinanceIncomeMonths { get; set; }

        public DbSet<EtNoticeConfig> EtNoticeConfigs { get; set; }
    }
}
