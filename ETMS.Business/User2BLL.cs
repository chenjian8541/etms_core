using ETMS.Authority;
using ETMS.Business.Common;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.Dto.User.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class User2BLL : IUser2BLL
    {
        private readonly IRoleDAL _roleDAL;

        private readonly IUserDAL _userDAL;

        public User2BLL(IRoleDAL roleDAL, IUserDAL userDAL)
        {
            this._roleDAL = roleDAL;
            this._userDAL = userDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _roleDAL, _userDAL);
        }

        public async Task<ResponseBase> GetAllMenusH5(RequestBase request)
        {
            var userInfo = await _userDAL.GetUser(request.LoginUserId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            var output = new GetAllMenusH5Output()
            {
                AllCategorys = new List<GetAllMenusH5Category>()
            };
            var myItems = ComBusiness.GetH5AllMenus(PermissionDataH5.MenuConfigs, role.AuthorityValueMenu, userInfo.IsAdmin);
            var allCategory = myItems.GroupBy(p => p.CategoryId).OrderBy(p => p.Key);
            foreach (var p in allCategory)
            {
                var thisItem = myItems.Where(j => j.CategoryId == p.Key).OrderBy(j => j.Sort);
                output.AllCategorys.Add(new GetAllMenusH5Category()
                {
                    CategoryId = p.Key,
                    CategoryName = thisItem.First().CategoryName,
                    MyMenus = thisItem
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> GetEditMenusH5(RequestBase request)
        {
            var userInfo = await _userDAL.GetUser(request.LoginUserId);
            var role = await _roleDAL.GetRole(userInfo.RoleId);
            return ResponseBase.Success(ComBusiness.GetEditMenusH5(PermissionDataH5.MenuConfigs, role.AuthorityValueMenu,
                userInfo.HomeMenu, userInfo.IsAdmin));
        }

        public async Task<ResponseBase> SaveHomeMenusH5(SaveHomeMenusH5Request request)
        {
            var authorityCore = new AuthorityCore();
            var weightSum = authorityCore.AuthoritySum(request.Ids);
            var newHomeMenu = weightSum.ToString();
            await _userDAL.UpdateUserHomeMenu(request.LoginUserId, newHomeMenu);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> UserAccountResign(RequestBase request)
        {
            var user = await _userDAL.GetUser(request.LoginUserId);
            if (user == null)
            {
                return ResponseBase.CommonError("用户不存在");
            }
            if (user.IsAdmin)
            {
                return ResponseBase.Success();
            }
            user.JobType = EmUserJobType.Resignation;
            await _userDAL.EditUser(user);
            return ResponseBase.Success();
        }
    }
}
