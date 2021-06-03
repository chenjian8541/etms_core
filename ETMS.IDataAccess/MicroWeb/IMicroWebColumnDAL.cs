using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.MicroWeb
{
    public interface IMicroWebColumnDAL : IBaseDAL
    {
        Task<bool> AddMicroWebColumn(EtMicroWebColumn entity);

        Task<bool> EditMicroWebColumn(EtMicroWebColumn entity);

        Task<bool> ChangeStatus(long id,byte newStatus);

        Task<bool> DelMicroWebColumn(long id);

        Task<List<EtMicroWebColumn>> GetMicroWebColumn();

        Task<EtMicroWebColumn> GetMicroWebColumn(long id);
    }
}
