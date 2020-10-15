using ETMS.Authority;
using ETMS.DataAccess.EtmsManage.Lib;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Config.Router;
using ETMS.Entity.Dto.User.Output;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Business.EtmsManage.Common
{
    internal static class AgentComBusiness
    {
        internal static List<RouteConfig> GetRouteConfigs(string roleAuthorityValueMenu)
        {
            var pageWeight = roleAuthorityValueMenu.Split('|')[2].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var pageRoute = EtmsHelper.DeepCopy(AgentPermissionData.RouteConfigs);
            PageRouteHandle(pageRoute, authorityCorePage);
            return pageRoute;
        }

        private static void PageRouteHandle(List<RouteConfig> pageRoute, AuthorityCore authorityCorePage)
        {
            foreach (var p in pageRoute)
            {
                if (p.Id == 0 || !authorityCorePage.Validation(p.Id))
                {
                    p.Hidden = true;
                }
                if (p.Children != null && p.Children.Any())
                {
                    PageRouteHandle(p.Children, authorityCorePage);
                }
            }
        }

        internal static PermissionOutput GetPermissionOutput(string roleAuthorityValueMenu)
        {
            var strMenuCategory = roleAuthorityValueMenu.Split('|');
            var pageWeight = strMenuCategory[2].ToBigInteger();
            var actionWeight = strMenuCategory[1].ToBigInteger();
            var authorityCoreLeafPage = new AuthorityCore(pageWeight);
            var authorityCoreActionPage = new AuthorityCore(actionWeight);
            var menus = EtmsHelper.DeepCopy(AgentPermissionData.MenuConfigs);
            var output = new PermissionOutput()
            {
                Action = new List<int>(),
                Page = new List<int>()
            };
            foreach (var p in menus)
            {
                if (authorityCoreLeafPage.Validation(p.Id))
                {
                    output.Page.Add(p.Id);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    GetPermissionActionHandle(p.ChildrenAction, authorityCoreActionPage, output);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    GetPermissionPageHandle(p.ChildrenPage, authorityCoreLeafPage, authorityCoreActionPage, output);
                }
            }
            return output;
        }

        internal static void GetPermissionPageHandle(List<MenuConfig> menuConfigs, AuthorityCore authorityCoreLeafPage, AuthorityCore authorityCoreActionPage, PermissionOutput output)
        {
            foreach (var p in menuConfigs)
            {
                if (authorityCoreLeafPage.Validation(p.Id))
                {
                    output.Page.Add(p.Id);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    GetPermissionActionHandle(p.ChildrenAction, authorityCoreActionPage, output);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    GetPermissionPageHandle(p.ChildrenPage, authorityCoreLeafPage, authorityCoreActionPage, output);
                }
            }
        }

        internal static void GetPermissionActionHandle(List<MenuConfig> menuConfigs, AuthorityCore authorityCoreActionPage, PermissionOutput output)
        {
            foreach (var p in menuConfigs)
            {
                if (authorityCoreActionPage.Validation(p.ActionId))
                {
                    output.Action.Add(p.ActionId);
                }
            }
        }
    }
}
