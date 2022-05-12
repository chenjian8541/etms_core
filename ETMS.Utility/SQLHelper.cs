using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Utility
{
    public class SQLHelper
    {
        public static List<string> _dangerStrList;

        public static bool SqlValidate(string str)
        {
            foreach (var p in _dangerStrList)
            {
                if (str.IndexOf(p, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetInIds(IEnumerable<long> ids)
        {
            if (ids.Count() > 50)
            {
                ids = ids.Take(50);
            }
            return string.Join(',', ids);
        }

        static SQLHelper()
        {
            _dangerStrList = new List<string>();
            _dangerStrList.Add(";");
            _dangerStrList.Add("--");
            _dangerStrList.Add("*/");
            _dangerStrList.Add("xp_");
            _dangerStrList.Add("/*");
            _dangerStrList.Add("master ");
            _dangerStrList.Add("drop ");
            _dangerStrList.Add("table ");
            _dangerStrList.Add("declare ");
            _dangerStrList.Add("delete ");
            _dangerStrList.Add("exec ");
            _dangerStrList.Add("insert ");
            _dangerStrList.Add("update ");
            _dangerStrList.Add("select ");
            _dangerStrList.Add("set ");
            _dangerStrList.Add("truncate ");
            _dangerStrList.Add("execute ");
            _dangerStrList.Add("into ");
        }
    }
}
