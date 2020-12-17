using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantTxCloudUCountDAL : ISysTenantTxCloudUCountDAL, IEtmsManage
    {
        public async Task AddTenantTxCloudUCount(int tenantId, DateTime ot, byte type, int addCount)
        {
            var year = ot.Year;
            var month = ot.Month;
            var log = await this.Find<SysTenantTxCloudUCount>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == tenantId
            && p.Type == type && p.Year == year && p.Month == month);
            if (log == null)
            {
                await this.Insert(new SysTenantTxCloudUCount()
                {
                    TenantId = tenantId,
                    IsDeleted = EmIsDeleted.Normal,
                    Year = year,
                    Month = month,
                    Ot = ot,
                    UseCount = addCount,
                    Type = type,
                    Remark = string.Empty
                });
            }
            else
            {
                log.UseCount += addCount;
                await this.Update(log);
            }
        }
    }
}
