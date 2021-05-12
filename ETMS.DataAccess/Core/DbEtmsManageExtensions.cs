using ETMS.DataAccess.Lib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IOC;
using ETMS.Entity.Config;

namespace ETMS.DataAccess.Core
{
    internal static class DbEtmsManageExtensions
    {
        public static string EtmsManageConnectionString;

        internal static async Task<T> Find<T>(this IEtmsManage @this, Expression<Func<T, bool>> condition) where T : class
        {
            using (var content = new EtmsManageDbContext())
            {
                return await content.Set<T>().FirstOrDefaultAsync(condition);
            }
        }
        internal static async Task<List<T>> FindList<T>(this IEtmsManage @this, Expression<Func<T, bool>> condition) where T : class
        {
            using (var content = new EtmsManageDbContext())
            {
                return await content.Set<T>().Where(condition).ToListAsync();
            }
        }

        internal static async Task<bool> Insert<T>(this IEtmsManage @this, T entity) where T : class
        {
            var result = false;
            using (var content = new EtmsManageDbContext())
            {
                content.Add(entity);
                result = await content.SaveChangesAsync() > 0;
            }
            return result;
        }

        internal static async Task<bool> InsertRange<T>(this IEtmsManage @this, List<T> entitys) where T : class
        {
            var result = false;
            using (var content = new EtmsManageDbContext())
            {
                content.AddRange(entitys);
                result = await content.SaveChangesAsync() > 0;
            }
            return result;
        }

        internal static async Task<bool> Update<T>(this IEtmsManage @this, T entity) where T : class
        {
            var result = false;
            using (var content = new EtmsManageDbContext())
            {
                content.Update(entity);
                result = await content.SaveChangesAsync() > 0;
            }
            return result;
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
        internal static async Task<Tuple<List<T>, int>> FindPage<T, TOrderBy>(this IEtmsManage @this, int pageIndex, int pageSize, Expression<Func<T, bool>> condition, Func<T, TOrderBy> orderby, bool isDesc = true) where T : class
        {
            using (var content = new EtmsManageDbContext())
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
        internal static async Task<object> ExecuteScalar(this IEtmsManage @this, string commandText)
        {
            return await DapperProvider.ExecuteScalar(EtmsManageConnectionString, commandText);
        }

        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static async Task<IEnumerable<T>> ExecuteObject<T>(this IEtmsManage @this, string commandText) where T : class
        {
            return await DapperProvider.ExecuteObject<T>(EtmsManageConnectionString, commandText);
        }

        /// <summary>
        /// 执行SQL返回受影响的行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static async Task<int> Execute(this IEtmsManage @this, string commandText)
        {
            return await DapperProvider.Execute(EtmsManageConnectionString, commandText);
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
        internal static async Task<Tuple<IEnumerable<T>, int>> ExecutePage<T>(this IEtmsManage @this, string tableName, string fildName, int pageSize,
           int pageCurrent, string fildSort, string condition) where T : class
        {
            return await DapperProvider.ExecutePage<T>(EtmsManageConnectionString, tableName, fildName, pageSize, pageCurrent, fildSort, condition);
        }

        static DbEtmsManageExtensions()
        {
            EtmsManageConnectionString = CustomServiceLocator.GetInstance<IAppConfigurtaionServices>().AppSettings.DatabseConfig.EtmsManageConnectionString;
        }
    }
}
