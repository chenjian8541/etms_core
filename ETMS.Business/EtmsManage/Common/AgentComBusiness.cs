using ETMS.Authority;
using ETMS.Business.Common;
using ETMS.DataAccess.EtmsManage.Lib;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Config.Router;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.EtmsManage.Dto.Agent.Output;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage.Common
{
    internal static class AgentComBusiness
    {
        internal static List<RouteConfig> GetRouteConfigs(string roleAuthorityValueMenu, string userRoleAuthorityValueMenu)
        {
            var pageWeight = roleAuthorityValueMenu.Split('|')[2].ToBigInteger();
            if (!string.IsNullOrEmpty(userRoleAuthorityValueMenu))
            {
                var userPageWeight = userRoleAuthorityValueMenu.Split('|')[2].ToBigInteger();
                pageWeight = pageWeight & userPageWeight;
            }
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

        internal static PermissionOutput GetPermissionOutput(string roleAuthorityValueMenu, string userRoleAuthorityValueMenu)
        {
            var strMenuCategory = roleAuthorityValueMenu.Split('|');
            var pageWeight = strMenuCategory[2].ToBigInteger();
            var actionWeight = strMenuCategory[1].ToBigInteger();
            if (!string.IsNullOrEmpty(userRoleAuthorityValueMenu)) //代理商角色和用户角色做与运算,合并权限
            {
                var userStrMenuCategory = userRoleAuthorityValueMenu.Split('|');
                var userPageWeight = userStrMenuCategory[2].ToBigInteger();
                var userActionWeight = userStrMenuCategory[1].ToBigInteger();
                pageWeight = pageWeight & userPageWeight;
                actionWeight = actionWeight & userActionWeight;
            }
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

        private static void GetPermissionPageHandle(List<MenuConfig> menuConfigs, AuthorityCore authorityCoreLeafPage, AuthorityCore authorityCoreActionPage, PermissionOutput output)
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

        private static void GetPermissionActionHandle(List<MenuConfig> menuConfigs, AuthorityCore authorityCoreActionPage, PermissionOutput output)
        {
            foreach (var p in menuConfigs)
            {
                if (authorityCoreActionPage.Validation(p.ActionId))
                {
                    output.Action.Add(p.ActionId);
                }
            }
        }

        internal static async Task<SysAgent> GetAgent(AgentDataTempBox<SysAgent> tempBox, ISysAgentDAL sysAgentDAL, int agentId)
        {
            var agent = await tempBox.GetData(agentId, async () =>
            {
                var bucket = await sysAgentDAL.GetAgent(agentId);
                return bucket?.SysAgent;
            });
            return agent;
        }

        internal static async Task<SysTenant> GetTenant(AgentDataTempBox<SysTenant> tempBox, ISysTenantDAL sysTenantDAL, int tenantId)
        {
            var tenant = await tempBox.GetData(tenantId, async () =>
            {
                return await sysTenantDAL.GetTenant(tenantId);
            });
            return tenant;
        }

        internal static async Task<SysUser> GetUser(AgentDataTempBox2<SysUser> tempBox, ISysUserDAL sysUserDAL, long userId)
        {
            var user = await tempBox.GetData(userId, async () =>
            {
                return await sysUserDAL.GetUser(userId);
            });
            return user;
        }

        internal static async Task<SysUserRole> GetUserRole(AgentDataTempBox<SysUserRole> tempBox, ISysUserRoleDAL sysUserRoleDAL, int roleId)
        {
            var role = await tempBox.GetData(roleId, async () =>
            {
                return await sysUserRoleDAL.GetRole(roleId);
            });
            return role;
        }

        #region 角色

        internal static string GetAuthorityValueMenu(List<int> pageMenus, List<int> actionMenus, List<int> pageRouteIds)
        {
            return $"{GetAuthorityValue(pageMenus.ToArray())}|{GetAuthorityValue(actionMenus.ToArray())}|{GetAuthorityValue(pageRouteIds.ToArray())}";
        }

        /// <summary>
        /// 通过选择的菜单ID，计算权值
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        internal static string GetAuthorityValue(int[] ids)
        {
            if (ids == null || !ids.Any())
            {
                return string.Empty;
            }
            var authorityCore = new AuthorityCore();
            var weightSum = authorityCore.AuthoritySum(ids);
            return weightSum.ToString();
        }

        internal static List<SysMenuViewOutput> GetSysMenuViewOutputs(List<MenuConfig> menuConfigs)
        {
            var output = new List<SysMenuViewOutput>();
            var index = 1;
            foreach (var p in menuConfigs)
            {
                var item = new SysMenuViewOutput()
                {
                    ActionCheck = new List<int>(),
                    ActionItems = new List<SysMenuItem>(),
                    PageCheck = new List<int>(),
                    PageItems = new List<SysMenuItem>(),
                    Index = index
                };
                index++;
                var thisPageItem = new SysMenuItem()
                {
                    Children = new List<SysMenuItem>(),
                    Id = p.Id,
                    Label = p.Name,
                    Type = p.Type
                };
                item.PageItems.Add(thisPageItem);
                if (p.IsOwner)
                {
                    item.PageCheck.Add(p.Id);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    AddChildrenPage(p.ChildrenPage, thisPageItem, item);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    AddChildrenCheck(p.ChildrenAction, item);
                }
                output.Add(item);
            }
            return output;
        }

        internal static void AddChildrenCheck(List<MenuConfig> ChildrenAction, SysMenuViewOutput itemOutput)
        {
            if (itemOutput.ActionItems.Count == 0)
            {
                itemOutput.ActionItems.Add(new SysMenuItem()
                {
                    Children = new List<SysMenuItem>(),
                    Id = 0,
                    Label = "全选",
                    Type = MenuType.Action
                });
            }
            foreach (var p in ChildrenAction)
            {
                itemOutput.ActionItems[0].Children.Add(new SysMenuItem()
                {
                    Id = p.ActionId,
                    Label = p.Name,
                    Type = p.Type
                });
                if (p.IsOwner)
                {
                    itemOutput.ActionCheck.Add(p.ActionId);
                }
            }
        }

        internal static void AddChildrenPage(List<MenuConfig> childrenPage, SysMenuItem item, SysMenuViewOutput itemOutput)
        {
            foreach (var p in childrenPage)
            {
                var thisRoleMenuItem = new SysMenuItem()
                {
                    Children = new List<SysMenuItem>(),
                    Id = p.Id,
                    Label = p.Name,
                    Type = p.Type
                };
                item.Children.Add(thisRoleMenuItem);
                if (p.IsOwner)
                {
                    itemOutput.PageCheck.Add(p.Id);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    AddChildrenPage(p.ChildrenPage, thisRoleMenuItem, itemOutput);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    AddChildrenCheck(p.ChildrenAction, itemOutput);
                }
            }
        }

        internal static void MenuConfigsHandle(List<MenuConfig> myMenuConfigs, AuthorityCore authorityCorePage, AuthorityCore authorityCoreAction)
        {
            foreach (var p in myMenuConfigs)
            {
                if (p.Type == MenuType.Page)
                {
                    p.IsOwner = authorityCorePage.Validation(p.Id);
                }
                else
                {
                    p.IsOwner = authorityCoreAction.Validation(p.ActionId);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    MenuConfigsHandle(p.ChildrenPage, authorityCorePage, authorityCoreAction);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    MenuConfigsHandle(p.ChildrenAction, authorityCorePage, authorityCoreAction);
                }
            }
        }


        #endregion
    }
}
