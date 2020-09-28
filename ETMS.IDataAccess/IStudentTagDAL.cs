using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentTagDAL : IBaseDAL
    {
        Task<bool> AddStudentTag(EtStudentTag entity);

        Task DelStudentTag(long id);

        Task<List<EtStudentTag>> GetAllStudentTag();
    }
}
