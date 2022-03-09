using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantMqScheduleDAL : ISysTenantMqScheduleDAL, IEtmsManage
    {
        public async Task AddSysTenantMqSchedule<T>(T entity, int tenantId, int type, TimeSpan timeSpan) where T : Event.DataContract.Event
        {
            var now = DateTime.Now;
            await this.Insert(new SysTenantMqSchedule()
            {
                CreateTime = now,
                ExTime = now.Add(timeSpan),
                IsDeleted = EmIsDeleted.Normal,
                Remark = null,
                SendContent = Newtonsoft.Json.JsonConvert.SerializeObject(entity),
                TenantId = tenantId,
                Type = type
            });
        }

        public async Task<Tuple<IEnumerable<SysTenantMqSchedule>, int>> GetTenantMqSchedule(int pageSize, int pageCurrent, DateTime maxExTime)
        {
            return await this.ExecutePage<SysTenantMqSchedule>("SysTenantMqSchedule", "*", pageSize, pageCurrent, "Id DESC",
                $"IsDeleted = {EmIsDeleted.Normal} AND ExTime <= '{maxExTime.EtmsToString()}'");
        }

        public async Task ClearSysTenantMqSchedule(DateTime maxExTime)
        {
            await this.Execute($"DELETE SysTenantMqSchedule WHERE ExTime <= '{maxExTime.EtmsToString()}'");
        }
    }
}
