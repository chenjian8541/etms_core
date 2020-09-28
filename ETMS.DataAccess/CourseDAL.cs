using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class CourseDAL : DataAccessBase<CourseBucket>, ICourseDAL
    {
        public CourseDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }
        protected override async Task<CourseBucket> GetDb(params object[] keys)
        {
            var course = await _dbWrapper.Find<EtCourse>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (course == null)
            {
                return null;
            }
            var coursePriceRules = await _dbWrapper.FindList<EtCoursePriceRule>(p => p.CourseId == course.Id && p.IsDeleted == EmIsDeleted.Normal);
            return new CourseBucket()
            {
                Course = course,
                CoursePriceRules = coursePriceRules
            };
        }

        public async Task<bool> AddCourse(EtCourse course, List<EtCoursePriceRule> coursePriceRules)
        {
            await _dbWrapper.Insert(course);
            if (coursePriceRules != null && coursePriceRules.Any())
            {
                foreach (var s in coursePriceRules)
                {
                    s.CourseId = course.Id;
                }
                _dbWrapper.InsertRange(coursePriceRules);
            }
            await base.UpdateCache(_tenantId, course.Id);
            return true;
        }

        public async Task<bool> ExistCourse(string name, long id = 0)
        {
            var course = await _dbWrapper.Find<EtCourse>(p => p.TenantId == _tenantId && p.Name == name && p.Id != id && p.IsDeleted == EmIsDeleted.Normal);
            return course != null;
        }

        public async Task<bool> EditCourse(EtCourse course, List<EtCoursePriceRule> coursePriceRules)
        {
            await _dbWrapper.Update(course);
            await _dbWrapper.Execute($"DELETE EtCoursePriceRule WHERE CourseId = {course.Id}");
            if (coursePriceRules != null && coursePriceRules.Any())
            {
                _dbWrapper.InsertRange(coursePriceRules);
            }
            await base.UpdateCache(_tenantId, course.Id);
            return true;
        }

        public async Task<bool> EditCourse(EtCourse course)
        {
            await _dbWrapper.Update(course);
            await base.UpdateCache(_tenantId, course.Id);
            return true;
        }

        public async Task<Tuple<EtCourse, List<EtCoursePriceRule>>> GetCourse(long id)
        {
            var courseBucket = await base.GetCache(_tenantId, id);
            if (courseBucket == null)
            {
                return null;
            }
            return Tuple.Create(courseBucket.Course, courseBucket.CoursePriceRules);
        }

        public async Task<bool> DelCourse(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id};DELETE EtCoursePriceRule WHERE CourseId = {id}");
            base.RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtCourse>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtCourse>("EtCourse", "*", request.PageSize, request.PageCurrent, "[Status] ASC,Id DESC", request.ToString());
        }
    }
}
