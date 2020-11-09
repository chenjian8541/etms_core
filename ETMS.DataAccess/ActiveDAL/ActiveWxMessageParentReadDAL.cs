using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class ActiveWxMessageParentReadDAL : DataAccessBase<WxMessageParentReadBucket>, IActiveWxMessageParentReadDAL
    {
        public ActiveWxMessageParentReadDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<WxMessageParentReadBucket> GetDb(params object[] keys)
        {
            var studentIds = (List<long>)keys[2];
            if (studentIds.Count == 0)
            {
                return null;
            }
            var sql = string.Empty;
            if (studentIds.Count == 1)
            {
                sql = $"select COUNT(0) from EtActiveWxMessageDetail where TenantId = {_tenantId} AND  StudentId = {studentIds[0]} AND IsRead = {EmBool.False} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            else
            {
                sql = $"select COUNT(0) from EtActiveWxMessageDetail where TenantId = {_tenantId} AND  StudentId IN ({string.Join(',', studentIds)}) AND IsRead = {EmBool.False} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return new WxMessageParentReadBucket()
            {
                UnreadCount = obj.ToInt()
            };
        }

        public async Task UpdateParentRead(string phone, List<long> studentIds)
        {
            await UpdateCache(_tenantId, phone, studentIds);
        }

        public async Task<int> GetParentUnreadCount(string phone, List<long> studentIds)
        {
            var bucket = await GetCache(_tenantId, phone, studentIds);
            if (bucket == null)
            {
                return 0;
            }
            return bucket.UnreadCount;
        }
    }
}
