using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentRelationshipDAL : IBaseDAL
    {
        Task<bool> AddStudentRelationship(EtStudentRelationship entity);

        Task DelStudentRelationship(long id);

        Task<List<EtStudentRelationship>> GetAllStudentRelationship();
    }
}
