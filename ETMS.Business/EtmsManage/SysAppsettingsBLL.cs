using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysAppsettingsBLL : ISysAppsettingsBLL
    {
        private readonly ISysAppsettingsDAL _sysAppsettingsDAL;

        public SysAppsettingsBLL(ISysAppsettingsDAL sysAppsettingsDAL)
        {
            this._sysAppsettingsDAL = sysAppsettingsDAL;
        }

        public async Task<SysTenantWechartAuth> GetWechartAuthDefault()
        {
            var log = await _sysAppsettingsDAL.GetAppsettings(EmSysAppsettingsType.TenantDefaultWechartAuth);
            if (log == null || string.IsNullOrEmpty(log.Data))
            {
                LOG.Log.Error("[GetWechartAuthDefault]获取默认第三方授权信息失败", this.GetType());
                return null;
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SysTenantWechartAuth>(log.Data);
        }
    }
}
