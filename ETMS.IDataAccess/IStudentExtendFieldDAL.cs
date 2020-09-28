using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentExtendFieldDAL : IBaseDAL
    {
        Task<bool> AddStudentExtendField(EtStudentExtendField entity);

        Task DelStudentExtendField(long id);

        Task<List<EtStudentExtendField>> GetAllStudentExtendField();
    }
}
