using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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

        public async Task<TencentCloudAccountView> GetTencentCloudAccount(int tencentCloudId)
        {
            var log = await _sysAppsettingsDAL.GetAppsettings(EmSysAppsettingsType.TencentCloudAccount);
            if (log == null || string.IsNullOrEmpty(log.Data))
            {
                LOG.Log.Error($"[GetTencentCloudAccount]获取腾讯云账户信息错误,tencentCloudId:{tencentCloudId}", this.GetType());
                throw new Exception("获取腾讯云账户信息错误");
            }
            var listData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TencentCloudAccountView>>(log.Data);
            var mydata = listData.FirstOrDefault(p => p.TencentCloudId == tencentCloudId);
            if (mydata == null)
            {
                LOG.Log.Error($"[GetTencentCloudAccount]获取腾讯云账户信息错误2,tencentCloudId:{tencentCloudId}", this.GetType());
                throw new Exception("获取腾讯云账户信息错误");
            }
            return mydata;
        }
    }
}
