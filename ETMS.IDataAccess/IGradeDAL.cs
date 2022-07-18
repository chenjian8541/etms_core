using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IGradeDAL : IBaseDAL
    {
        Task<bool> AddGrade(EtGrade entity);

        Task DelGrade(long id);

        Task<List<EtGrade>> GetAllGrade();

        Task<EtGrade> GetGrade(long id);

        Task EditGrade(EtGrade entity);
    }
}
