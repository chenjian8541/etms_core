using ETMS.Authority;
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

        /// <summary>
        /// 通过权限过滤 得到新的菜单信息
        /// </summary>
        /// <param name="authorityCorePage"></param>
        /// <param name="authorityCoreAction"></param>
        /// <param name="childMenus"></param>
        /// <returns></returns>
        public static List<MenuConfig> GetChildMenus(AuthorityCore authorityCorePage, AuthorityCore authorityCoreAction, List<MenuConfig> childMenus)
        {
            var myMenuConfigs = new List<MenuConfig>();
            foreach (var p in childMenus)
            {
                if (p.Type == MenuType.Page)
                {
                    if (authorityCorePage.Validation(p.Id))
                    {
                        var myMenuConfig = new MenuConfig()
                        {
                            ActionId = p.ActionId,
                            Id = p.Id,
                            IsOwner = p.IsOwner,
                            Name = p.Name,
                            Type = p.Type
                        };
                        if (p.ChildrenPage != null && p.ChildrenPage.Count > 0)
                        {
                            myMenuConfig.ChildrenPage = GetChildMenus(authorityCorePage, authorityCoreAction, p.ChildrenPage);
                        }
                        if (p.ChildrenAction != null && p.ChildrenAction.Count > 0)
                        {
                            myMenuConfig.ChildrenAction = GetChildMenus(authorityCorePage, authorityCoreAction, p.ChildrenAction);
                        }
                        myMenuConfigs.Add(myMenuConfig);
                    }
                }
                else
                {
                    if (authorityCoreAction.Validation(p.ActionId))
                    {
                        var myMenuConfig = new MenuConfig()
                        {
                            ActionId = p.ActionId,
                            Id = p.Id,
                            IsOwner = p.IsOwner,
                            Name = p.Name,
                            Type = p.Type
                        };
                        if (p.ChildrenPage != null && p.ChildrenPage.Count > 0)
                        {
                            myMenuConfig.ChildrenPage = GetChildMenus(authorityCorePage, authorityCoreAction, p.ChildrenPage);
                        }
                        if (p.ChildrenAction != null && p.ChildrenAction.Count > 0)
                        {
                            myMenuConfig.ChildrenAction = GetChildMenus(authorityCorePage, authorityCoreAction, p.ChildrenAction);
                        }
                        myMenuConfigs.Add(myMenuConfig);
                    }
                }
            }
            return myMenuConfigs;
        }
    }
}
