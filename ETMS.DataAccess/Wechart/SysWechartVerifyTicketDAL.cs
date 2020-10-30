using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Wechart;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Wechart
{
    public class SysWechartVerifyTicketDAL : BaseCacheDAL<SysWechartVerifyTicketBucket>, ISysWechartVerifyTicketDAL, IEtmsManage
    {
        public SysWechartVerifyTicketDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysWechartVerifyTicketBucket> GetDb(params object[] keys)
        {
            var componentAppId = keys[0].ToString();
            var db = await this.Find<SysWechartVerifyTicket>(p => p.ComponentAppId == componentAppId && p.IsDeleted == EmIsDeleted.Normal);
            if (db == null)
            {
                return null;
            }
            return new SysWechartVerifyTicketBucket()
            {
                SysWechartVerifyTicket = db
            };
        }

        public async Task<SysWechartVerifyTicket> GetSysWechartVerifyTicket(string componentAppId)
        {
            var bucket = await GetCache(componentAppId);
            return bucket?.SysWechartVerifyTicket;
        }

        public async Task<bool> SaveSysWechartVerifyTicket(string componentAppId, string componentVerifyTicket)
        {
            var oldLog = await GetSysWechartVerifyTicket(componentAppId);
            if (oldLog != null)
            {
                oldLog.ComponentVerifyTicket = componentVerifyTicket;
                oldLog.ModifyOt = DateTime.Now;
                await this.Update(oldLog);
            }
            else
            {
                oldLog = new SysWechartVerifyTicket()
                {
                    ComponentAppId = componentAppId,
                    ComponentVerifyTicket = componentVerifyTicket,
                    IsDeleted = EmIsDeleted.Normal,
                    ModifyOt = DateTime.Now,
                    Remark = string.Empty
                };
                await this.Insert(oldLog);
            }
            await UpdateCache(componentAppId);
            return true;
        }
    }
}
