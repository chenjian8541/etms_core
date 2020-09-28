using ETMS.Entity.Config;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Config.Router;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ETMS.DataAccess.Lib
{
    public class PermissionData
    {
        private static List<MenuConfig> _menuConfigs;

        private static object LockMenuObj = new object();

        private static List<RouteConfig> _routeConfigs;

        private static object LockRouteConfig = new object();

        public static List<MenuConfig> MenuConfigs
        {
            get
            {
                if (_menuConfigs == null)
                {
                    lock (LockMenuObj)
                    {
                        if (_menuConfigs == null)
                        {
                            var json = File.ReadAllText(FileHelper.GetFilePath("menusetting.json"));
                            _menuConfigs = JsonConvert.DeserializeObject<List<MenuConfig>>(json);
                            return _menuConfigs;
                        }
                    }
                }
                return _menuConfigs;
            }
        }

        public static List<RouteConfig> RouteConfigs
        {
            get
            {
                if (_routeConfigs == null)
                {
                    lock (LockRouteConfig)
                    {
                        if (_routeConfigs == null)
                        {
                            var json = File.ReadAllText(FileHelper.GetFilePath("routesetting.json"));
                            _routeConfigs = JsonConvert.DeserializeObject<List<RouteConfig>>(json);
                            return _routeConfigs;
                        }
                    }
                }
                return _routeConfigs;
            }
        }
    }
}
