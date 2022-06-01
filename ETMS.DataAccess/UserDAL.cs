using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class UserDAL : DataAccessBase<UserBucket>, IUserDAL
    {
        public UserDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected async override Task<UserBucket> GetDb(params object[] keys)
        {
            var userId = keys[1].ToLong();
            var user = await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.Id == userId && p.IsDeleted == EmIsDeleted.Normal);
            if (user == null)
            {
                return null;
            }
            if (user.TotalClassCount < 0 || user.TotalClassTimes < 0) //小于0处理
            {
                if (user.TotalClassCount < 0)
                {
                    user.TotalClassCount = 0;
                }
                if (user.TotalClassTimes < 0)
                {
                    user.TotalClassTimes = 0;
                }
                await _dbWrapper.Execute($"UPDATE EtUser SET TotalClassTimes = {user.TotalClassTimes} ,TotalClassCount = {user.TotalClassCount} WHERE Id = {userId} AND TenantId = {_tenantId}");
            }

            return new UserBucket()
            {
                User = user
            };
        }

        public async Task<EtUser> GetAdminUser()
        {
            var adminUser = await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.IsAdmin == true);
            if (adminUser == null)
            {
                //防止admin账号被删除，正常情况下administrator是不允许被删除的
                adminUser = await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            }
            return adminUser;
        }

        public async Task<EtUser> GetUser(long userId)
        {
            var userBucket = await base.GetCache(_tenantId, userId);
            return userBucket?.User;
        }

        public async Task<EtUser> GetUser(string phone)
        {
            return await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.Phone == phone && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task UpdateUserLastLoginTime(long userId, DateTime lastLoginTime)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtUser SET LastLoginTime = '{lastLoginTime.EtmsToString()}' WHERE Id = {userId}");
            await UpdateCache(_tenantId, userId);
        }

        public async Task<bool> ExistUserPhone(string phone, long userId = 0)
        {
            var user = await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.Phone == phone && p.Id != userId && p.IsDeleted == EmIsDeleted.Normal);
            return user != null;
        }

        public async Task<bool> AddUser(EtUser user)
        {
            return await _dbWrapper.Insert(user, async () => { await UpdateCache(_tenantId, user.Id); });
        }

        public async Task<bool> EditUser(EtUser user)
        {
            if (user.TotalClassTimes < 0)
            {
                user.TotalClassTimes = 0;
            }
            if (user.TotalClassCount < 0)
            {
                user.TotalClassCount = 0;
            }
            return await _dbWrapper.Update(user, async () => { await UpdateCache(_tenantId, user.Id); });
        }

        public async Task<bool> ExistRole(long roleId)
        {
            var user = await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.RoleId == roleId && p.IsDeleted == EmIsDeleted.Normal);
            return user != null;
        }

        public async Task<bool> DelUser(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtUser SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id}");
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<UserView>, int>> GetUserPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<UserView>("UserView", "*", request.PageSize, request.PageCurrent, "jobtype asc,Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<UserSimpleView>, int>> GetUserSimplePaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<UserSimpleView>("EtUser", "Id,Name,Phone,IsTeacher", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> AddTeacherClassTimesInfo(long teacherId, decimal addClassTimes, int addClassCount)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtUser SET TotalClassTimes = TotalClassTimes + {addClassTimes} , TotalClassCount = TotalClassCount + {addClassCount} WHERE Id = {teacherId}");
            await UpdateCache(_tenantId, teacherId);
            return count > 0;
        }

        public async Task<bool> DeTeacherClassTimesInfo(long teacherId, decimal deClassTimes, int deClassCount)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtUser SET TotalClassTimes = TotalClassTimes - {deClassTimes} , TotalClassCount = TotalClassCount - {deClassCount} WHERE Id = {teacherId}");
            await UpdateCache(_tenantId, teacherId);
            return count > 0;
        }

        public async Task<bool> AddTeacherMonthClassTimes(long teacherId, DateTime classTime, decimal addClassTimes, int addClassCount)
        {
            var year = classTime.Year;
            var moth = classTime.Month;
            var teacherClassTimesData = await this._dbWrapper.Find<EtTeacherClassTimes>(p => p.TenantId == _tenantId && p.UserId == teacherId
             && p.Year == year && p.Month == moth && p.IsDeleted == EmIsDeleted.Normal);
            if (teacherClassTimesData != null)
            {
                await _dbWrapper.Execute($"UPDATE EtTeacherClassTimes SET ClassTimes = ClassTimes + {addClassTimes} ,ClassCount = ClassCount + {addClassCount} WHERE id = {teacherClassTimesData.Id}");
            }
            else
            {
                var firstDate = new DateTime(classTime.Year, classTime.Month, 1);
                await this._dbWrapper.Insert(new EtTeacherClassTimes()
                {
                    TenantId = _tenantId,
                    ClassCount = addClassCount,
                    ClassTimes = addClassTimes,
                    FirstTime = firstDate,
                    IsDeleted = EmIsDeleted.Normal,
                    Month = moth,
                    UserId = teacherId,
                    Year = year
                });
            }
            return true;
        }

        public async Task<bool> DeTeacherMonthClassTimes(long teacherId, DateTime classTime, decimal deClassTimes, int deClassCount)
        {
            var year = classTime.Year;
            var moth = classTime.Month;
            var teacherClassTimesData = await this._dbWrapper.Find<EtTeacherClassTimes>(p => p.TenantId == _tenantId && p.UserId == teacherId
             && p.Year == year && p.Month == moth && p.IsDeleted == EmIsDeleted.Normal);
            if (teacherClassTimesData != null)
            {
                await _dbWrapper.Execute($"UPDATE EtTeacherClassTimes SET ClassTimes = ClassTimes - {deClassTimes} ,ClassCount = ClassCount - {deClassCount} WHERE id = {teacherClassTimesData.Id}");
            }
            return true;
        }

        public async Task UpdateTeacherMonthClassTimes(long teacherId, DateTime classTime, decimal newClassTimes, int newClassCount)
        {
            var year = classTime.Year;
            var moth = classTime.Month;
            var teacherClassTimesData = await this._dbWrapper.Find<EtTeacherClassTimes>(p => p.TenantId == _tenantId && p.UserId == teacherId
            && p.Year == year && p.Month == moth && p.IsDeleted == EmIsDeleted.Normal);
            if (teacherClassTimesData != null)
            {
                if (newClassCount == 0)
                {
                    await _dbWrapper.Execute($"DELETE EtTeacherClassTimes WHERE id = {teacherClassTimesData.Id} ");
                }
                else
                {
                    await _dbWrapper.Execute($"UPDATE EtTeacherClassTimes SET ClassTimes = {newClassTimes} ,ClassCount = {newClassCount} WHERE id = {teacherClassTimesData.Id}");
                }
            }
            else
            {
                if (newClassCount == 0)
                {
                    return;
                }
                var firstDate = new DateTime(classTime.Year, classTime.Month, 1);
                await this._dbWrapper.Insert(new EtTeacherClassTimes()
                {
                    TenantId = _tenantId,
                    ClassCount = newClassCount,
                    ClassTimes = newClassTimes,
                    FirstTime = firstDate,
                    IsDeleted = EmIsDeleted.Normal,
                    Month = moth,
                    UserId = teacherId,
                    Year = year
                });
            }
        }

        public async Task UpdateTeacherClassTimes(long teacherId)
        {
            var sql = $"SELECT SUM(ClassTimes) AS TotalClassTimes,SUM(ClassCount) AS TotalClassCount FROM EtTeacherClassTimes WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND UserId = {teacherId}";
            var db = await _dbWrapper.ExecuteObject<MyTeacherClassTimesView>(sql);
            var totalClassTimes = 0M;
            var totalClassCount = 0;
            var myLog = db.FirstOrDefault();
            if (myLog != null)
            {
                totalClassTimes = myLog.TotalClassTimes;
                totalClassCount = myLog.TotalClassCount;
            }
            await _dbWrapper.Execute($"UPDATE EtUser SET TotalClassTimes = {totalClassTimes} , TotalClassCount = {totalClassCount} WHERE Id = {teacherId}");
            await UpdateCache(_tenantId, teacherId);
        }

        public async Task<Tuple<IEnumerable<TeacherClassTimesView>, int>> GetTeacherClassTimesPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<TeacherClassTimesView>("TeacherClassTimesView", "*", request.PageSize, request.PageCurrent, "FirstTime DESC", request.ToString());
        }

        public async Task<int> GetUserCount()
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT COUNT(0) FROM EtUser WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal}");
            if (obj == null)
            {
                return 0;
            }
            return obj.ToInt();
        }

        public async Task<bool> UserEditWx(long userId, string wechatOpenid, string wechatUnionid)
        {
            var sql = $"update EtUser set WechatOpenid = '{wechatOpenid}',WechatUnionid = '{wechatUnionid}' where TenantId = {_tenantId} and Id = {userId}";
            await _dbWrapper.Execute(sql);
            await UpdateCache(_tenantId, userId);
            return true;
        }

        public async Task<IEnumerable<NoticeUserView>> GetUserAboutNotice(int roleNoticeType)
        {
            var sql = $"SELECT TOP 100 Id,Name,NickName,Phone FROM UserView WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND JobType <> {EmUserJobType.Resignation} AND NoticeSetting LIKE '%,{roleNoticeType},%'";
            return await _dbWrapper.ExecuteObject<NoticeUserView>(sql);
        }

        public async Task<IEnumerable<NoticeUserView>> GetUserAboutNotice(int roleNoticeType, IEnumerable<long> relationUserIds)
        {
            if (relationUserIds == null || !relationUserIds.Any())
            {
                return null;
            }
            relationUserIds = relationUserIds.Distinct();
            var sql = string.Empty;
            if (relationUserIds.Count() == 1)
            {
                sql = $"SELECT TOP 100 Id,Name,NickName,Phone FROM UserView WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Id = {relationUserIds.First()} AND JobType <> {EmUserJobType.Resignation} AND NoticeSetting LIKE '%,{roleNoticeType},%'";
            }
            else
            {
                sql = $"SELECT TOP 100 Id,Name,NickName,Phone FROM UserView WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Id IN ({SQLHelper.GetInIds(relationUserIds)}) AND JobType <> {EmUserJobType.Resignation} AND NoticeSetting LIKE '%,{roleNoticeType},%'";
            }
            return await _dbWrapper.ExecuteObject<NoticeUserView>(sql);
        }

        public async Task UpdateUserHomeMenu(long userId, string homeMenus)
        {
            await _dbWrapper.Execute($"UPDATE EtUser SET HomeMenu = '{homeMenus}' WHERE Id = {userId}");
            await UpdateCache(_tenantId, userId);
        }
    }
}
