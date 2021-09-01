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
            var user = await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (user == null)
            {
                return null;
            }
            return new UserBucket()
            {
                User = user
            };
        }

        public async Task<EtUser> GetAdminUser()
        {
            return await _dbWrapper.Find<EtUser>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.IsAdmin == true);
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
            var user = await GetUser(teacherId);
            if (user == null)
            {
                return false;
            }
            var newTotalClassTimes = user.TotalClassTimes - deClassTimes;
            var newTotalClassCount = user.TotalClassCount - deClassCount;
            if (newTotalClassTimes < 0)
            {
                newTotalClassTimes = 0;
            }
            if (newTotalClassCount < 0)
            {
                newTotalClassCount = 0;
            }
            // var count = await _dbWrapper.Execute($"UPDATE EtUser SET TotalClassTimes = {newTotalClassTimes} , TotalClassCount = {newTotalClassCount} WHERE Id = {teacherId}");
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
                await _dbWrapper.Execute($"UPDATE EtTeacherClassTimes SET ClassTimes = {newClassTimes} ,ClassCount = {newClassCount} WHERE id = {teacherClassTimesData.Id}");
            }
            else
            {
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

        public async Task<List<EtUser>> GetUserAboutNotice(int roleNoticeType)
        {
            var sltRule = $"SELECT TOP 50 Id FROM EtRole WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND NoticeSetting LIKE '%,{roleNoticeType},%'";
            var myRoleId = (await _dbWrapper.ExecuteObject<OnlyId>(sltRule)).ToList();
            if (myRoleId.Count == 0)
            {
                return null;
            }
            var sql = string.Empty;
            if (myRoleId.Count == 1)
            {
                sql = $"SELECT TOP 100 * FROM EtUser WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND RoleId = {myRoleId[0].Id} ";
            }
            else
            {
                var ids = myRoleId.Select(p => p.Id);
                sql = $"SELECT TOP 100 * FROM EtUser WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND RoleId IN ({string.Join(',', ids)}) ";
            }
            return (await _dbWrapper.ExecuteObject<EtUser>(sql)).ToList();
        }
    }
}
