using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IReservationCourseSetDAL : IBaseDAL
    {
        Task<bool> ExistReservationCourse(long courseId);

        Task<List<EtReservationCourseSet>> GetReservationCourseSet();

        Task AddReservationCourseSet(EtReservationCourseSet entity);

        Task UpdateReservationCourseSet(long id,int limitCount);

        Task DelReservationCourseSet(long id);
    }
}
