using ETMS.Entity.CacheBucket;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess
{
    public class UserLoginFailedRecordDAL : IUserLoginFailedRecordDAL
    {
        /// <summary>
        /// 缓存访问提供器
        /// </summary>
        private ICacheProvider _cacheProvider;

        protected int _tenantId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheProvider"></param>
        public UserLoginFailedRecordDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public UserLoginFailedBucket GetUserLoginFailedRecord(string code, string phone)
        {
            return _cacheProvider.Get<UserLoginFailedBucket>(_tenantId, new UserLoginFailedBucket().GetKeyFormat(code, phone));
        }

        public UserLoginFailedBucket AddUserLoginFailedRecord(string code, string phone)
        {
            var key = new UserLoginFailedBucket().GetKeyFormat(code, phone);
            var record = _cacheProvider.Get<UserLoginFailedBucket>(_tenantId, key);
            if (record == null)
            {
                record = new UserLoginFailedBucket()
                {
                    FailedCount = 1,
                    Phone = phone,
                    TenantCode = code
                };
            }
            else
            {
                if (record.FailedCount < SystemConfig.UserLoginConfig.LoginFailedMaxCount)
                {
                    record.FailedCount++;
                }
                else
                {
                    record.ExpireAtTime = DateTime.Now.AddMinutes(SystemConfig.UserLoginConfig.LoginFailedTimeOut);
                }
            }
            _cacheProvider.Set(_tenantId, key, record, record.TimeOut);
            return record;
        }

        public void RemoveUserLoginFailedRecord(string code, string phone)
        {
            var key = new UserLoginFailedBucket().GetKeyFormat(code, phone);
            _cacheProvider.Remove(_tenantId, key);
        }
    }
}
