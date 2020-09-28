using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IHolidaySettingDAL : IBaseDAL
    {
        Task<bool> AddHolidaySetting(EtHolidaySetting entity);

        Task DelHolidaySetting(long id);

        Task<List<EtHolidaySetting>> GetAllHolidaySetting();
    }
}
