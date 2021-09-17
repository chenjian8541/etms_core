using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Lcsw.Utility
{
    public class APICommonFun
    {
        /// <summary>
        /// 合并另一个集合
        /// </summary>
        /// <param name="firstMap">主集合</param>
        /// <param name="otherMap">其他集合</param>
        public static void DictionaryUnion(Dictionary<string, object> firstMap, Dictionary<string, object> otherMap)
        {
            if (firstMap == null)
            {
                return;
            }
            if (otherMap == null)
            {
                return;
            }
            foreach (var item in otherMap)
            {
                firstMap.Add(item.Key, item.Value);
            }
        }
    }
}
