using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IUserDAL : IBaseDAL
    {
        Task<EtUser> GetUser(long userId);

        Task<EtUser> GetUser(string phone);

        Task UpdateUserLastLoginTime(long userId, DateTime lastLoginTime);

        Task<bool> ExistUserPhone(string phone, long userId = 0);

        Task<bool> AddUser(EtUser user);

        Task<bool> EditUser(EtUser user);

        Task<bool> ExistRole(long roleId);

        Task<bool> DelUser(long id);

        Task<Tuple<IEnumerable<UserView>, int>> GetUserPaging(IPagingRequest request);

        Task<bool> UpdateTeacherClassTimesInfo(long teacherId, decimal addClassTimes, int addClassCount);

        Task<bool> DeTeacherClassTimesInfo(long teacherId, decimal deClassTimes, int deClassCount);

        Task<bool> AddTeacherMonthClassTimes(long teacherId, DateTime classTime, decimal addClassTimes, int addClassCount);

        Task<bool> DeTeacherMonthClassTimes(long teacherId, DateTime classTime, decimal deClassTimes, int deClassCount);

        Task<Tuple<IEnumerable<TeacherClassTimesView>, int>> GetTeacherClassTimesPaging(RequestPagingBase request);

        Task<int> GetUserCount();

        Task<bool> UserEditWx(long userId, string wechatOpenid, string wechatUnionid);
    }
}
