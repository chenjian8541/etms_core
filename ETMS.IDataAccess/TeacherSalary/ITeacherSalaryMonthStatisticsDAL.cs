using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.TeacherSalary
{
    public interface ITeacherSalaryMonthStatisticsDAL : IBaseDAL
    {
        Task<decimal> GetTeacherSalaryMonthStatistics(long userId, int year, int month);

        Task UpdateTeacherSalaryMonthStatistics(long userId, int year, int month);
    }
}
