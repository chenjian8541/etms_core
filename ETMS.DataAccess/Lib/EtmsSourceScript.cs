using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ETMS.DataAccess.Lib
{
    public class EtmsSourceScript
    {
        private static string _initSql;

        private static object LockInitSql = new object();

        internal static string InitSql
        {
            get
            {
                if (string.IsNullOrEmpty(_initSql))
                {
                    lock (LockInitSql)
                    {
                        if (string.IsNullOrEmpty(_initSql))
                        {
                            _initSql = File.ReadAllText(FileHelper.GetFilePath("init.sql")).Trim();
                            return _initSql;
                        }
                    }
                }
                return _initSql;
            }
        }
    }
}
