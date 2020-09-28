using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ISubjectDAL : IBaseDAL
    {
        Task<bool> AddSubject(EtSubject entity);

        Task DelSubject(long id);

        Task<List<EtSubject>> GetAllSubject();
    }
}
