using ETMS.DataAccess.Lib;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Output;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp.View;
using ETMS.IBusiness.MicroWeb;
using ETMS.IDataAccess;
using ETMS.IDataAccess.MicroWeb;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Config;
using ETMS.Business.Common;

namespace ETMS.Business.MicroWeb
{
    public class MicroWebConfigBLL : IMicroWebConfigBLL
    {
        private readonly IMicroWebConfigDAL _microWebConfigDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        public MicroWebConfigBLL(IMicroWebConfigDAL microWebConfigDAL, IUserOperationLogDAL userOperationLogDAL,
            IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._microWebConfigDAL = microWebConfigDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _microWebConfigDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> MicroWebBannerGet(RequestBase request)
        {
            var log = await _microWebConfigDAL.GetMicroWebConfig(EmMicroWebConfigType.BannerSet);
            var output = new MicroWebBannerGetOutput()
            {
                IsShowInHome = false,
                Images = new List<MicroWebBannerGetImage>()
            };
            if (log == null || string.IsNullOrEmpty(log.ConfigValue))
            {
                return ResponseBase.Success(output);
            }
            var setValue = JsonConvert.DeserializeObject<MicroWebConfigBannerSetView>(log.ConfigValue);
            output.IsShowInHome = setValue.IsShowInHome;
            if (setValue.Images != null && setValue.Images.Count > 0)
            {
                foreach (var p in setValue.Images)
                {
                    output.Images.Add(new MicroWebBannerGetImage()
                    {
                        ImgKey = p,
                        ImgUrl = AliyunOssUtil.GetAccessUrlHttps(p)
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> MicroWebBannerSave(MicroWebBannerSaveRequest request)
        {
            var setValue = new MicroWebConfigBannerSetView()
            {
                IsShowInHome = request.IsShowInHome,
                Images = new List<string>()
            };
            if (request.BannerSets != null && request.BannerSets.Count > 0)
            {
                foreach (var p in request.BannerSets)
                {
                    setValue.Images.Add(p.ImgKey);
                }
            }
            var entity = new EtMicroWebConfig()
            {
                ConfigValue = JsonConvert.SerializeObject(setValue),
                IsDeleted = EmIsDeleted.Normal,
                TenantId = request.LoginTenantId,
                Type = EmMicroWebConfigType.BannerSet
            };
            await _microWebConfigDAL.SaveMicroWebConfig(entity);

            await _userOperationLogDAL.AddUserLog(request, "设置banner", EmUserOperationType.MicroWebManage);
            return ResponseBase.Success();
        }

        private List<EtMicroWebColumn> GetMicroWebDefaultColumnData()
        {
            var so = MicroWebDefaultColumnData.MicroWebColumnDefault;
            return EtmsHelper.DeepCopy(so);
        }

        private async Task<bool> SaveMicroWebDefaultColumn(int tenantId, List<EtMicroWebColumn> entitys)
        {
            var setValue = JsonConvert.SerializeObject(entitys);
            await _microWebConfigDAL.SaveMicroWebConfig(new EtMicroWebConfig()
            {
                ConfigValue = setValue,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = tenantId,
                Type = EmMicroWebConfigType.MicroWebDefaultColumnSet
            });
            return true;
        }

        public async Task<List<EtMicroWebColumn>> MicroWebDefaultColumnGet()
        {
            var log = await _microWebConfigDAL.GetMicroWebConfig(EmMicroWebConfigType.MicroWebDefaultColumnSet);
            if (log == null || string.IsNullOrEmpty(log.ConfigValue))
            {
                return GetMicroWebDefaultColumnData();
            }
            var setValue = JsonConvert.DeserializeObject<List<EtMicroWebColumn>>(log.ConfigValue);
            return setValue;
        }

        public async Task<EtMicroWebColumn> MicroWebDefaultColumnGet(long id)
        {
            var allDefaultMicroWebColumn = await MicroWebDefaultColumnGet();
            return allDefaultMicroWebColumn.FirstOrDefault(p => p.Id == id);
        }

        public async Task<bool> MicroWebDefaultColumnChangeStatus(int tenantId, long id, byte newStatus)
        {
            var myData = await MicroWebDefaultColumnGet();
            var thisData = myData.First(p => p.Id == id);
            thisData.Status = newStatus;
            await SaveMicroWebDefaultColumn(tenantId, myData);
            return true;
        }

        public async Task<bool> MicroWebDefaultColumnSave(MicroWebColumnEditRequest request)
        {
            var myData = await MicroWebDefaultColumnGet();
            var thisData = myData.First(p => p.Id == request.Id);
            thisData.Name = request.Name;
            thisData.OrderIndex = request.OrderIndex;
            if (thisData.Type == EmMicroWebColumnType.ListPage)
            {
                thisData.ShowStyle = request.ShowStyle;
            }
            thisData.IsShowInMenu = request.IsShowInMenu;
            thisData.ShowInMenuIcon = request.ShowInMenuIcon;
            thisData.IsShowInHome = request.IsShowInHome;
            thisData.IsShowYuYue = request.IsShowYuYue;

            await SaveMicroWebDefaultColumn(request.LoginTenantId, myData);
            return true;
        }

        public async Task<ResponseBase> MicroWebTenantAddressGet(RequestBase request)
        {
            var log = await _microWebConfigDAL.GetMicroWebConfig(EmMicroWebConfigType.TenantAddressSet);
            if (log == null || string.IsNullOrEmpty(log.ConfigValue))
            {
                return ResponseBase.Success(new MicroWebTenantAddressGetOutput());
            }
            var setValue = JsonConvert.DeserializeObject<MicroWebTenantAddressView>(log.ConfigValue);
            return ResponseBase.Success(new MicroWebTenantAddressGetOutput()
            {
                Address = setValue.Address,
                CoverIcon = setValue.CoverIcon,
                IsShowInHome = setValue.IsShowInHome,
                Latitude = setValue.Latitude,
                Longitude = setValue.Longitude,
                Name = setValue.Name,
                CoverIconUrl = AliyunOssUtil.GetAccessUrlHttps(setValue.CoverIcon),
                MicroWebHomeUrl = string.Format(_appConfigurtaionServices.AppSettings.SysAddressConfig.MicroWebHomeUrl, TenantLib.GetTenantEncrypt(request.LoginTenantId))
            });
        }

        public async Task<ResponseBase> MicroWebTenantAddressSave(MicroWebTenantAddressSaveRequest request)
        {
            var setValue = new MicroWebTenantAddressView()
            {
                Name = request.Name,
                Longitude = request.Longitude,
                Latitude = request.Latitude,
                IsShowInHome = request.IsShowInHome,
                CoverIcon = request.CoverIcon,
                Address = request.Address
            };
            var entity = new EtMicroWebConfig()
            {
                ConfigValue = JsonConvert.SerializeObject(setValue),
                IsDeleted = EmIsDeleted.Normal,
                TenantId = request.LoginTenantId,
                Type = EmMicroWebConfigType.TenantAddressSet
            };
            await _microWebConfigDAL.SaveMicroWebConfig(entity);

            await _userOperationLogDAL.AddUserLog(request, "设置地址", EmUserOperationType.MicroWebManage);
            return ResponseBase.Success();
        }
    }
}
