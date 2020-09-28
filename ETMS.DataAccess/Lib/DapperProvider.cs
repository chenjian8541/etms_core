using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using ETMS.Utility;
using System.Data;

namespace ETMS.DataAccess.Lib
{
    /// <summary>
    /// Dapper提供数据访问
    /// </summary>
    public class DapperProvider
    {
        private const int commandTimeout = 60;

        /// <summary>
        /// 执行SQL返回第一行第一列结果
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static async Task<object> ExecuteScalar(string conStr, string commandText)
        {
            using (var connection = new SqlConnection(conStr))
            {
                return await connection.ExecuteScalarAsync(commandText, commandTimeout: commandTimeout);
            }
        }

        /// <summary>
        /// 执行SQL返回第一行第一列结果
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private static async Task<object> ExecuteScalar(DbConnection connection, string commandText)
        {
            return await connection.ExecuteScalarAsync(commandText, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conStr"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static async Task<IEnumerable<T>> ExecuteObject<T>(string conStr, string commandText) where T : class
        {
            using (var connection = new SqlConnection(conStr))
            {
                return await connection.QueryAsync<T>(commandText, commandTimeout: commandTimeout);
            }
        }

        /// <summary>
        /// 查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<T>> ExecuteObject<T>(DbConnection connection, string commandText) where T : class
        {
            return await connection.QueryAsync<T>(commandText, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 执行SQL返回受影响的行数
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        internal static async Task<int> Execute(string conStr, string commandText)
        {
            using (var connection = new SqlConnection(conStr))
            {
                return await connection.ExecuteAsync(commandText, commandTimeout: commandTimeout);
            }
        }

        /// <summary>
        /// 执行SQL返回受影响的行数
        /// </summary>
        /// <param name="conStr"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private static async Task<int> Execute(DbConnection connection, string commandText)
        {
            return await connection.ExecuteAsync(commandText, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conStr"></param>
        /// <param name="tableName"></param>
        /// <param name="fildName"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCurrent"></param>
        /// <param name="fildSort"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal static async Task<Tuple<IEnumerable<T>, int>> ExecutePage<T>(string conStr, string tableName, string fildName, int pageSize,
            int pageCurrent, string fildSort, string condition) where T : class
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TableName", tableName);
            parameters.Add("@FildName", fildName);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageCurrent", pageCurrent);
            parameters.Add("@FildSort", fildSort);
            parameters.Add("@Condition", condition);
            parameters.Add("@RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            using (var connection = new SqlConnection(conStr))
            {
                var record = await connection.QueryAsync<T>("[Etms_SP_Common_Paging]", parameters, commandType: CommandType.StoredProcedure);
                var recordCount = parameters.Get<int>("RecordCount");
                return Tuple.Create(record, recordCount);
            }
        }
    }
}
