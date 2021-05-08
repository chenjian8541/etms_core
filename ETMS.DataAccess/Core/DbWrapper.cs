using ETMS.DataAccess.Lib;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ETMS.Entity.Config;

namespace ETMS.DataAccess.Core
{
    public class DbWrapper : IDbWrapper
    {
        private string _connectionString;

        private static object LockConStrObj = new object();

        /// <summary>
        /// 连接字符串
        /// </summary>
        private string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    lock (LockConStrObj)
                    {
                        if (string.IsNullOrEmpty(_connectionString))
                        {
                            if (_tenantId == 0)
                            {
                                _connectionString = CustomServiceLocator.GetInstance<IAppConfigurtaionServices>().AppSettings.DatabseConfig.EtmsManageConnectionString;
                            }
                            else
                            {
                                _connectionString = CustomServiceLocator.GetInstance<ITenantConfigWrapper>().GetTenantConnectionString(_tenantId).Result;
                            }
                        }
                    }
                }
                return _connectionString;
            }
        }

        private int _tenantId;

        /// <summary>
        /// 初始化机构
        /// </summary>
        /// <param name="tenantId"></param>
        public void InitTenant(int tenantId)
        {
            this._tenantId = tenantId;
        }

        /// <summary>
        /// 重置机构
        /// </summary>
        /// <param name="tenantId"></param>
        public void ResetTenant(int tenantId)
        {
            if (_tenantId == tenantId)
            {
                return;
            }
            _connectionString = string.Empty;
            this._tenantId = tenantId;
        }

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="finishWork"></param>
        /// <returns></returns>
        public async Task<bool> Insert<T>(T entity, Action finishWork = null) where T : class
        {
            var result = false;
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                content.Add(entity);
                result = await content.SaveChangesAsync() > 0;
            }
            if (result)
            {
                finishWork?.Invoke();
            }
            return result;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="finishWork"></param>
        /// <returns></returns>
        public bool InsertRange<T>(IEnumerable<T> entitys, Action finishWork = null) where T : class
        {
            var result = false;
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                content.AddRange(entitys);
                result = content.SaveChanges() > 0;
            }
            if (result)
            {
                finishWork?.Invoke();
            }
            return result;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<bool> InsertRangeAsync<T>(IEnumerable<T> entitys) where T : class
        {
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                content.AddRange(entitys);
                return await content.SaveChangesAsync() > 0;
            }
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="finishWork"></param>
        /// <returns></returns>
        public async Task<bool> Delete<T>(T entity, Action finishWork = null) where T : class
        {
            var result = false;
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                content.Remove(entity);
                result = await content.SaveChangesAsync() > 0;
            }
            if (result)
            {
                finishWork?.Invoke();
            }
            return result;
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="finishWork"></param>
        /// <returns></returns>
        public async Task<bool> Update<T>(T entity, Action finishWork = null) where T : class
        {
            var result = false;
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                content.Update(entity);
                result = await content.SaveChangesAsync() > 0;
            }
            if (result)
            {
                finishWork?.Invoke();
            }
            return result;
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRange<T>(IEnumerable<T> entitys) where T : class
        {
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                content.UpdateRange(entitys);
                return await content.SaveChangesAsync() > 0;
            }
        }


        /// <summary>
        /// 通过主键查询记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pk"></param>
        /// <returns></returns>
        public async Task<T> Find<T>(long pk) where T : class
        {
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                return await content.Set<T>().FindAsync(pk);
            }
        }

        /// <summary>
        /// 通过查询条件查询记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<T> Find<T>(Expression<Func<T, bool>> condition) where T : class
        {
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                return await content.Set<T>().FirstOrDefaultAsync(condition);
            }
        }

        /// <summary>
        /// 获取所有记录集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<T>> FindList<T>() where T : class
        {
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                return await content.Set<T>().ToListAsync();
            }
        }

        /// <summary>
        /// 通过查询提交获取记录集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<List<T>> FindList<T>(Expression<Func<T, bool>> condition) where T : class
        {
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                return await content.Set<T>().Where(condition).ToListAsync();
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrderBy"></typeparam>
        /// <param name="this"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderby"></param>
        /// <param name="isDesc"></param>
        /// <returns></returns>
        public async Task<Tuple<List<T>, int>> FindPage<T, TOrderBy>(int pageIndex, int pageSize, Expression<Func<T, bool>> condition, Func<T, TOrderBy> orderby, bool isDesc = true) where T : class
        {
            using (var content = new EtmsSourceDbContext(ConnectionString))
            {
                var total = await content.Set<T>().Where(condition).CountAsync();
                List<T> data = null;
                if (isDesc)
                {
                    data = content.Set<T>().Where(condition).OrderByDescending(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    data = content.Set<T>().Where(condition).OrderBy(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                return Tuple.Create(data, total);
            }
        }

        /// <summary>
        /// 执行SQL返回第一行第一列结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public async Task<object> ExecuteScalar(string commandText)
        {
            return await DapperProvider.ExecuteScalar(ConnectionString, commandText);
        }

        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ExecuteObject<T>(string commandText) where T : class
        {
            return await DapperProvider.ExecuteObject<T>(ConnectionString, commandText);
        }

        /// <summary>
        /// 执行SQL返回受影响的行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public async Task<int> Execute(string commandText)
        {
            return await DapperProvider.Execute(ConnectionString, commandText);
        }

        /// <summary>
        /// 执行SQL返回受影响的行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteSync(string commandText)
        {
            return DapperProvider.ExecuteSync(ConnectionString, commandText);
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="fildName"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCurrent"></param>
        /// <param name="fildSort"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<T>, int>> ExecutePage<T>(string tableName, string fildName, int pageSize,
           int pageCurrent, string fildSort, string condition) where T : class
        {
            return await DapperProvider.ExecutePage<T>(ConnectionString, tableName, fildName, pageSize, pageCurrent, fildSort, condition);
        }
    }
}
