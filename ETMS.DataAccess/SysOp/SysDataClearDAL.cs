using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.IDataAccess.SysOp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.SysOp
{
    public class SysDataClearDAL : DataAccessBase, ISysDataClearDAL
    {
        private readonly ICacheProvider _cacheProvider;

        public SysDataClearDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper)
        {
            this._cacheProvider = cacheProvider;
        }

        #region 家校互动

        public async Task<bool> ClearActiveHomework()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtActiveHomework SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveHomeworkDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveHomeworkDetailComment SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} ;");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearGrowthRecord()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtActiveGrowthRecord SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtActiveGrowthRecordDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtActiveGrowthRecordDetailComment SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearWxMessage()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtActiveWxMessage SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtActiveWxMessageDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearStudentSmsLog()
        {
            await _dbWrapper.Execute($"UPDATE EtStudentSmsLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} ;");
            return true;
        }

        public async Task<bool> ClearClassRecordEvaluate()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassRecordEvaluateTeacher SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassRecordEvaluateStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        #endregion

        #region 营销中心

        public async Task<bool> ClearGiftExchange()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtGiftExchangeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtGiftExchangeLogDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentPointsLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearGift()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtGiftCategory SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtGift SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            await ClearGiftExchange();
            return true;
        }

        public async Task<bool> ClearCoupons()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtCoupons SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtCouponsStudentGet SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtCouponsStudentUse SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            await ClearGiftExchange();
            return true;
        }

        #endregion

        #region 机构设置

        public async Task<bool> ClearIncomeProjectType()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtIncomeProjectType SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new IncomeProjectTypeBucket<EtIncomeProjectType>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearStudentRelationship()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentRelationship SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new StudentRelationshipBucket<EtStudentRelationship>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearStudentTag()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentTag SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new StudentTagBucket<EtStudentTag>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearClassRoom()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassRoom SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new ClassRoomBucket<EtClassRoom>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearStudentGrowingTag()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentGrowingTag SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new StudentGrowingTagBucket<EtStudentGrowingTag>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearClassSet()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassSet SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new ClassSetBucket<EtClassSet>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearGrade()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtGrade SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new GradeBucket<EtGrade>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearStudentSource()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentSource SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new StudentSourceBucket<EtStudentSource>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearSubject()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtSubject SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new SubjectBucket<EtSubject>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearStudentExtendField()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentExtendField SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new StudentExtendFieldBucket<EtStudentExtendField>().GetKeyFormat(_tenantId));
            return true;
        }

        public async Task<bool> ClearHolidaySetting()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtHolidaySetting SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            _cacheProvider.Remove(_tenantId, new HolidaySettingBucket<EtHolidaySetting>().GetKeyFormat(_tenantId));
            return true;
        }

        #endregion

        #region 基础数据

        public async Task<bool> ClearStudentTrackLog()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentTrackLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        private async Task<IEnumerable<OnlyId>> GetStudentIds()
        {
            return await _dbWrapper.ExecuteObject<OnlyId>($"SELECT TOP 100 Id FROM EtStudent WHERE TenantId = {_tenantId} ");
        }

        public async Task<bool> ClearStudentCourse()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentCourseDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentCourseStopLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentCourseConsumeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentCourseOpLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            var tempStudentIds = await GetStudentIds();
            var keys = new List<string>();
            foreach (var student in tempStudentIds)
            {
                keys.Add(new StudentCourseBucket().GetKeyFormat(_tenantId, student.Id));
                keys.Add(new StudentCourseStopLogBucket().GetKeyFormat(_tenantId, student.Id));
            }
            if (keys.Count > 0)
            {
                _cacheProvider.Remove(_tenantId, keys.ToArray());
            }
            return true;
        }

        public async Task<bool> ClearOrder()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtOrder SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtOrderDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtOrderOperationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtIncomeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearClassTimes()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassTimesRule SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassTimes SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassTimesStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtTempStudentClassNotice SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassTimesReservationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            _cacheProvider.Remove(_tenantId, new TempStudentClassNoticeBucket().GetKeyFormat(_tenantId, DateTime.Now));
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearClassRecord()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassRecord SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassRecordStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassRecordPointsApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassRecordOperationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassRecordEvaluateTeacher SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassRecordEvaluateStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassRecordAbsenceLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtTryCalssLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentCourseConsumeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtTeacherClassTimes SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtTryCalssApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtUser SET TotalClassTimes = 0,TotalClassCount = 0 WHERE TenantId = {_tenantId} ;");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearStudentLeaveApplyLog()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentLeaveApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearCourse()
        {
            var courseIds = await _dbWrapper.ExecuteObject<OnlyId>($"SELECT TOP 100 Id FROM EtCourse WHERE TenantId = {_tenantId} ");
            var keys = new List<string>();
            foreach (var courseId in courseIds)
            {
                keys.Add(new CourseBucket().GetKeyFormat(_tenantId, courseId.Id));
            }
            if (keys.Count > 0)
            {
                _cacheProvider.Remove(_tenantId, keys.ToArray());
            }
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtCoursePriceRule SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtSuit SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtSuitDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearGoodsAndCost()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtGoods SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtGoodsInventoryLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtCost SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtOrderDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND ProductType <> {EmProductType.Course} ;");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearClass()
        {
            var classIds = await _dbWrapper.ExecuteObject<OnlyId>($"SELECT TOP 100 Id FROM EtClass WHERE TenantId = {_tenantId} ");
            var kys = new List<string>();
            kys.Add(new ClassCategoryBucket<EtClassCategory>().GetKeyFormat(_tenantId));
            foreach (var classId in classIds)
            {
                kys.Add(new ClassBucket().GetKeyFormat(_tenantId, classId.Id));
            }
            if (kys.Count > 0)
            {
                _cacheProvider.Remove(_tenantId, kys.ToArray());
            }
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassCategory SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClass SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtClassTimesRule SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearStudent()
        {
            var studentIds = await _dbWrapper.ExecuteObject<OnlyId>($"SELECT TOP 100 Id FROM EtStudent WHERE TenantId = {_tenantId} ");
            var kys = new List<string>();
            foreach (var studentId in studentIds)
            {
                kys.Add(new StudentBucket().GetKeyFormat(_tenantId, studentId.Id));
            }
            if (kys.Count > 0)
            {
                _cacheProvider.Remove(_tenantId, kys.ToArray());
            }
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentWechat SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentExtendInfo SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentTrackLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentOperationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentNotice SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearUser()
        {
            var userIds = await _dbWrapper.ExecuteObject<OnlyId>($"SELECT TOP 100 Id FROM EtUser WHERE TenantId = {_tenantId} AND IsAdmin = {EmBool.False}");
            var kys = new List<string>();
            var ids = new List<long>();
            foreach (var userId in userIds)
            {
                kys.Add(new UserBucket().GetKeyFormat(_tenantId, userId.Id));
                ids.Add(userId.Id);
            }
            if (kys.Count > 0)
            {
                _cacheProvider.Remove(_tenantId, kys.ToArray());
            }
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtUser SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND IsAdmin = {EmBool.False} ;");
            if (ids.Count > 0)
            {
                sql.Append($"UPDATE EtUserOperationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND UserId IN ({string.Join(',', ids)}) ;");
            }
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearUserOperationLog()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtUserOperationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        #endregion

        #region 统计数据

        public async Task<bool> ClearStatisticsStudent()
        {
            _cacheProvider.Remove(_tenantId, new StatisticsStudentBucket().GetKeyFormat(_tenantId));
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStatisticsStudentCount SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsStudentTrackCount SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsStudentSource SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearStatisticsSales()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStatisticsSalesProduct SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsSalesCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsFinanceIncome SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsSalesTenant SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsSalesUser SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearStatisticsClass()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStatisticsClassAttendance SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsClassTimes SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsClassCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsClassTeacher SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStatisticsClassAttendanceTag SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        #endregion

        #region 其它

        public async Task<bool> ClearUserLog()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtTempUserClassNotice SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtUserSmsLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtUserNotice SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearStudentCheckLog()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentCheckOnLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtTempStudentNeedCheck SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtTempStudentNeedCheckClass SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<bool> ClearStudentAccountRecharge()
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStatisticsStudentAccountRecharge SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentAccountRecharge SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentAccountRechargeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            sql.Append($"UPDATE EtStudentAccountRechargeBinder SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId};");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        #endregion
    }
}
