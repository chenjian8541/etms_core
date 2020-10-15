using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Core
{
    public interface IDbWrapper
    {
        /// <summary>
        /// 初始化机构
        /// </summary>
        /// <param name="tenantId"></param>
        void InitTenant(int tenantId);

        /// <summary>
        /// 重置机构
        /// </summary>
        /// <param name="tenantId"></param>
        void ResetTenant(int tenantId);

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="finishWork"></param>
        /// <returns></returns>
        Task<bool> Insert<T>(T entity, Action finishWork = null) where T : class;

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="finishWork"></param>
        /// <returns></returns>
        bool InsertRange<T>(IEnumerable<T> entitys, Action finishWork = null) where T : class;

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="finishWork"></param>
        /// <returns></returns>
        Task<bool> Delete<T>(T entity, Action finishWork = null) where T : class;

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="finishWork"></param>
        /// <returns></returns>
        Task<bool> Update<T>(T entity, Action finishWork = null) where T : class;

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<bool> UpdateRange<T>(IEnumerable<T> entitys) where T : class;

        /// <summary>
        /// 通过主键查询记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pk"></param>
        /// <returns></returns>
        Task<T> Find<T>(long pk) where T : class;

        /// <summary>
        /// 通过查询条件查询记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<T> Find<T>(Expression<Func<T, bool>> condition) where T : class;

        /// <summary>
        /// 获取所有记录集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<List<T>> FindList<T>() where T : class;

        /// <summary>
        /// 通过查询提交获取记录集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        Task<List<T>> FindList<T>(Expression<Func<T, bool>> condition) where T : class;

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
        Task<Tuple<List<T>, int>> FindPage<T, TOrderBy>(int pageIndex, int pageSize, Expression<Func<T, bool>> condition, Func<T, TOrderBy> orderby, bool isDesc = true) where T : class;

        /// <summary>
        /// 执行SQL返回第一行第一列结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        Task<object> ExecuteScalar(string commandText);

        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ExecuteObject<T>(string commandText) where T : class;

        /// <summary>
        /// 执行SQL返回受影响的行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        Task<int> Execute(string commandText);

        /// <summary>
        /// 执行SQL返回受影响的行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        int ExecuteSync(string commandText);

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
        Task<Tuple<IEnumerable<T>, int>> ExecutePage<T>(string tableName, string fildName, int pageSize,
           int pageCurrent, string fildSort, string condition) where T : class;
    }
}
