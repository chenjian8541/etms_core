using ETMS.Entity.Config;
using ETMS.ICache;
using ETMS.LOG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Lib
{
    /// <summary>
    /// 数据缓存操作基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseCacheDAL<T> where T : class, ICacheDataContract, new()
    {

        /// <summary>
        /// 缓存提供者
        /// </summary>
        protected ICacheProvider _cacheProvider;

        /// <summary>
        /// 机构ID
        /// </summary>
        protected int _tenantId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheProvider"></param>
        public BaseCacheDAL(ICacheProvider cacheProvider)
        {
            this._cacheProvider = cacheProvider;
        }

        /// <summary>
        /// 更新缓存信息
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual async Task<T> UpdateCache(params object[] keys)
        {
            T data = null;
            try
            {
                data = await GetDb(keys);
            }
            catch (Exception ex)
            {
                Log.Error($"[BaseCacheDAL]获取缓存信息异常{keys}", ex, this.GetType());
            }
            if (data == null)
            {
                return null;
            }
            return UpdateCache(data, keys);
        }

        /// <summary>
        /// 更新缓存信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual T UpdateCache(T data, params object[] keys)
        {
            _cacheProvider.Set(_tenantId, data.GetKeyFormat(keys), data, data.TimeOut);
            return data;
        }

        /// <summary>
        /// 获取缓存信息
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual async Task<T> GetCache(params object[] keys)
        {
            var key = new T().GetKeyFormat(keys);
            var data = _cacheProvider.Get<T>(_tenantId, key);
            if (data == null)
            {
                Log.Warn($"[BaseCacheDAL]未从缓存得到数据，_tenantId={_tenantId} , key={key}", this.GetType());
                data = await UpdateCache(keys);
            }
            return data;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="keys"></param>
        protected virtual void RemoveCache(params object[] keys)
        {
            _cacheProvider.Remove(_tenantId, new T().GetKeyFormat(keys));
        }

        /// <summary>
        /// 从数据库中获取需要缓存的信息
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected abstract Task<T> GetDb(params object[] keys);

    }
}
