using ETMS.Entity.CacheBucket.Alien;
using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.IDataAccess.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Alien
{
    public class MgUserLoginFailedRecordDAL: IMgUserLoginFailedRecordDAL
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
        public MgUserLoginFailedRecordDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        public MgUserLoginFailedBucket GetUserLoginFailedRecord(string code, string phone)
        {
            return _cacheProvider.Get<MgUserLoginFailedBucket>(_tenantId, new MgUserLoginFailedBucket().GetKeyFormat(code, phone));
        }

        public MgUserLoginFailedBucket AddUserLoginFailedRecord(string code, string phone)
        {
            var key = new MgUserLoginFailedBucket().GetKeyFormat(code, phone);
            var record = _cacheProvider.Get<MgUserLoginFailedBucket>(_tenantId, key);
            if (record == null)
            {
                record = new MgUserLoginFailedBucket()
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
            var key = new MgUserLoginFailedBucket().GetKeyFormat(code, phone);
            _cacheProvider.Remove(_tenantId, key);
        }
    }
}
