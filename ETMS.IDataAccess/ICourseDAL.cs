using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ICourseDAL : IBaseDAL
    {
        Task<bool> AddCourse(EtCourse course, List<EtCoursePriceRule> coursePriceRules);

        Task<bool> ExistCourse(string name, long id = 0);

        Task<bool> EditCourse(EtCourse course, List<EtCoursePriceRule> coursePriceRules);

        Task<bool> EditCourse(EtCourse course);

        Task<EtCourse> GetCourse(string name, byte courseType);

        Task<Tuple<EtCourse, List<EtCoursePriceRule>>> GetCourse(long id);

        Task<EtCourse> GetCourse(string name);

        Task<bool> DelCourse(long id);

        Task<IEnumerable<OrderStudentOt>> GetCourseRelatedOrder(long courseId);

        Task<bool> DelCourseDepth(long id);

        Task<Tuple<IEnumerable<EtCourse>, int>> GetPaging(IPagingRequest request);

        Task<bool> IsCanNotDelete(long id);
    }
}
