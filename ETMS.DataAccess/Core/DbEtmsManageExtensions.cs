using ETMS.DataAccess.Lib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess.Core
{
    internal static class DbEtmsManageExtensions
    {
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
    }
}
