using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Utility;

namespace ETMS.Business.Common
{
    public static class ConvertLib
    {
        /// <summary>
        /// 获取以","隔开的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="allValues"></param>
        /// <param name="filterPredicate"></param>
        /// <param name="getDescPredicate"></param>
        /// <returns></returns>
        public static string ToItemDesc<T>(this string @this, List<T> allValues)
            where T : Entity<long>, IHasName
        {
            if (string.IsNullOrEmpty(@this))
            {
                return string.Empty;
            }
            var sbItem = new StringBuilder();
            var valueItems = @this.Split(",");
            foreach (var s in valueItems)
            {
                var myItem = allValues.FirstOrDefault(p => p.Id == s.ToLong());
                if (myItem != null)
                {
                    sbItem.Append($"{myItem.Name},");
                }
            }
            return sbItem.ToString().TrimEnd(',');
        }
    }
}
