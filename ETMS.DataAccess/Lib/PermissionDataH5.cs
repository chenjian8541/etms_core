using ETMS.Entity.Config.Menu;
using ETMS.Entity.Enum;
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
                            foreach (var p in _menuConfigs)
                            {
                                p.CategoryName = EmSystemMenusCategory.GetSystemMenusCategoryName(p.CategoryId);
                                p.Icon = AliyunOssUtil.GetAccessUrlHttps(p.Icon);
                            }
                            return _menuConfigs;
                        }
                    }
                }
                return _menuConfigs;
            }
        }
    }
}
