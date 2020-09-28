using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentSourceDAL : IBaseDAL
    {
        Task<bool> AddStudentSource(EtStudentSource entity);

        Task DelStudentSource(long id);

        Task<List<EtStudentSource>> GetAllStudentSource();
    }
}
