using System;
using System.Threading.Tasks;

namespace ETMS.ICache
{
    /// <summary>
    /// 缓存使用约定
    /// </summary>
    public interface ICacheProvider
    {
        bool Set<T>(int tenantId, string key, T t) where T : class;

        bool Set<T>(int tenantId, string key, T t, TimeSpan timeSpan) where T : class;

        T Get<T>(int tenantId, string key) where T : class;

        bool Remove(int tenantId, string key);

        bool LockTake(int tenantId, string key, string value, TimeSpan timeSpan);

        bool LockRelease(int tenantId, string key);
    }
}
