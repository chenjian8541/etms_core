using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
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

        Task<Tuple<EtCourse, List<EtCoursePriceRule>>> GetCourse(long id);

        Task<EtCourse> GetCourse(string name);

        Task<bool> DelCourse(long id);

        Task<Tuple<IEnumerable<EtCourse>, int>> GetPaging(RequestPagingBase request);

        Task<bool> IsCanNotDelete(long id);
    }
}
