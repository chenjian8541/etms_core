using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentWechatDAL : DataAccessBase<StudentWechatBucket>, IStudentWechatDAL
    {
        public StudentWechatDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected async override Task<StudentWechatBucket> GetDb(params object[] keys)
        {
            var phone = keys[1].ToString();
            var studentWechat = await _dbWrapper.Find<EtStudentWechat>(p => p.TenantId == _tenantId && p.Phone == phone && p.IsDeleted == EmIsDeleted.Normal);
            return new StudentWechatBucket()
            {
                StudentWechat = studentWechat
            };
        }

        public async Task<EtStudentWechat> GetStudentWechat(string opendId)
        {
            return await this._dbWrapper.Find<EtStudentWechat>(p => p.TenantId == _tenantId && p.WechatOpenid == opendId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<EtStudentWechat> GetStudentWechatByPhone(string phone)
        {
            var bucket = await this.GetCache(_tenantId, phone);
            return bucket?.StudentWechat;
        }

        public async Task AddStudentWechat(EtStudentWechat entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.Phone);
        }

        public async Task DelStudentWechat(string phone, string openId)
        {
            await _dbWrapper.Execute($"DELETE EtStudentWechat WHERE WechatOpenid = '{openId}' OR Phone = '{phone}'");
            RemoveCache(_tenantId, phone);
        }
    }
}
