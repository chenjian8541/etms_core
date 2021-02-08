using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantStudentDAL : BaseCacheDAL<SysTenantStudentBucket>, ISysTenantStudentDAL, IEtmsManage
    {
        public SysTenantStudentDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysTenantStudentBucket> GetDb(params object[] keys)
        {
            var phone = keys[0].ToString();
            var myTenants = await this.FindList<SysTenantStudent>(p => p.Phone == phone
            && p.Status == EmSysTenantPeopleStatus.Normal && p.IsDeleted == EmIsDeleted.Normal);
            return new SysTenantStudentBucket()
            {
                SysTenantStudents = myTenants
            };
        }

        public async Task ResetTenantAllStudent(int tenantId)
        {
            await this.Execute($"UPDATE SysTenantStudent SET [Status] = {EmSysTenantPeopleStatus.Lock} WHERE TenantId = {tenantId} ");
        }

        public async Task AddTenantStudent(int tenantId, long StudentId, string phone, bool isRefreshCache)
        {
            var log = await this.Find<SysTenantStudent>(p => p.TenantId == tenantId && p.Phone == phone && p.IsDeleted == EmIsDeleted.Normal);
            if (log != null)
            {
                log.Status = EmSysTenantPeopleStatus.Normal;
                await this.Update(log);
            }
            else
            {
                await this.Insert(new SysTenantStudent()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    LastOpTime = null,
                    Phone = phone,
                    Remark = string.Empty,
                    Status = EmSysTenantPeopleStatus.Normal,
                    TenantId = tenantId,
                    StudentId = StudentId
                });
            }
            if (isRefreshCache)
            {
                await UpdateCache(phone);
            }
        }

        public async Task RemoveTenantStudent(int tenantId, string phone)
        {
            await this.Execute($"DELETE SysTenantStudent WHERE TenantId = {tenantId} AND Phone = '{phone}' ");
            await UpdateCache(phone);
        }

        public async Task UpdateTenantStudentOpTime(int tenantId, string phone, DateTime opTime)
        {
            await this.Execute($"UPDATE SysTenantStudent SET LastOpTime = '{opTime.EtmsToString()}' WHERE TenantId = {tenantId} AND Phone = '{phone}' ");
            await UpdateCache(phone);
        }

        public async Task<List<SysTenantStudent>> GetTenantStudent(string phone)
        {
            var bucket = await GetCache(phone);
            return bucket?.SysTenantStudents;
        }
    }
}
