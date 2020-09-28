using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentGrowingTagDAL : IBaseDAL
    {
        Task<bool> AddStudentGrowingTag(EtStudentGrowingTag entity);

        Task DelStudentGrowingTag(long id);

        Task<List<EtStudentGrowingTag>> GetAllStudentGrowingTag();
    }
}
