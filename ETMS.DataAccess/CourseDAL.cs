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
            var updatePriceRule = coursePriceRules.Where(p => p.Id > 0).ToList();
            var InsertPriceRule = coursePriceRules.Where(p => p.Id == 0).ToList();
            if (updatePriceRule.Any())
            {
                var notIds = string.Join(',', updatePriceRule.Select(p => p.Id));
                await _dbWrapper.Execute($"DELETE EtCoursePriceRule WHERE TenantId = {_tenantId} AND CourseId = {course.Id} AND Id NOT IN ({notIds})");
            }
            else
            {
                await _dbWrapper.Execute($"DELETE EtCoursePriceRule WHERE TenantId = {_tenantId} AND CourseId = {course.Id}");
            }
            if (updatePriceRule.Any()) //只能编辑名称
            {
                var sql = new StringBuilder();
                foreach (var p in updatePriceRule)
                {
                    sql.Append($"UPDATE EtCoursePriceRule SET Name = '{p.Name}' WHERE Id = {p.Id} AND TenantId = {p.TenantId} ;");
                }
                await _dbWrapper.Execute(sql.ToString());
            }
            if (InsertPriceRule.Any())
            {
                _dbWrapper.InsertRange(InsertPriceRule);
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

        public async Task<EtCourse> GetCourse(string name)
        {
            return await _dbWrapper.Find<EtCourse>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Name == name);
        }

        public async Task<EtCourse> GetCourse(string name, byte courseType)
        {
            return await _dbWrapper.Find<EtCourse>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Name == name && p.Type == courseType);
        }

        public async Task<bool> DelCourse(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id};DELETE EtCoursePriceRule WHERE CourseId = {id}");
            base.RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<bool> DelCourseDepth(long id)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtCoursePriceRule SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtOrderDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE ProductType = {EmProductType.Course} and  TenantId = {_tenantId} and ProductId = {id} ;");
            sql.Append($"UPDATE EtOrder SET IsDeleted = {EmIsDeleted.Deleted} WHERE id in (select OrderId from EtOrderDetail where ProductType = {EmProductType.Course} and  TenantId = {_tenantId} and ProductId = {id}) and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCourseOpLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCourseDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassTimesStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassTimesReservationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordPointsApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordAbsenceLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCourseConsumeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTryCalssApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTryCalssLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStatisticsSalesCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStatisticsClassCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTempStudentNeedCheckClass SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCheckOnLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE CourseId = {id} AND TenantId = {_tenantId} ;");
            var tempSql = sql.ToString();
            LOG.Log.Info($"[DelCourseDepth]执行深度删除:{tempSql}", this.GetType());
            await _dbWrapper.Execute(tempSql);
            base.RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtCourse>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtCourse>("EtCourse", "*", request.PageSize, request.PageCurrent, "[Status] ASC,Id DESC", request.ToString());
        }

        public async Task<bool> IsCanNotDelete(long id)
        {
            var myClassTimes = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM EtClass WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND CourseList LIKE '%,{id},%'");
            return myClassTimes != null;
        }
    }
}
