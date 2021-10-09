using ETMS.Entity.Config.Menu;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ETMS.DataAccess.Lib
{
    public class PermissionDataH5
    {
        private static List<MenuConfigH5> _menuConfigs;

        private static object LockMenuObj = new object();

        public static List<MenuConfigH5> MenuConfigs
        {
            get
            {
                if (_menuConfigs == null)
                {
                    lock (LockMenuObj)
                    {
                        if (_menuConfigs == null)
                        {
                            var json = File.ReadAllText(FileHelper.GetFilePath("menusettingH5.json"));
                            _menuConfigs = JsonConvert.DeserializeObject<List<MenuConfigH5>>(json);
                            return _menuConfigs;
                        }
                    }
                }
                return _menuConfigs;
            }
        }
    }
}
