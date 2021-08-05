using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.TeacherSalary
{
    public interface ITeacherSalaryFundsItemsDAL : IBaseDAL
    {
        Task<bool> AddTeacherSalaryFundsItems(EtTeacherSalaryFundsItems entity);

        Task<List<EtTeacherSalaryFundsItems>> GetTeacherSalaryFundsItems();

        Task<bool> DelTeacherSalaryFundsItems(long id);
    }
}
