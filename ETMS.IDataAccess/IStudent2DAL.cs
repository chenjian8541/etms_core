using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudent2DAL : IBaseDAL
    {
        Task<bool> UpdateCache(string cardNo);

        bool RemoveCache(string cardNo);

        Task<EtStudent> GetStudent(string cardNo);

        Task<EtStudent> GetStudentByDb(string cardNo);
    }
}
