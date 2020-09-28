using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IIncomeProjectTypeDAL : IBaseDAL
    {
        Task<bool> AddIncomeProjectType(EtIncomeProjectType entity);

        Task DelIncomeProjectType(long id);

        Task<List<EtIncomeProjectType>> GetAllIncomeProjectType();
    }
}
