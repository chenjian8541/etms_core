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
using ETMS.Entity.CacheBucket.MicroWeb;

namespace ETMS.Business.MicroWeb
{
    public class MicroWebBLL : IMicroWebBLL
    {
        private readonly IMicroWebConfigBLL _microWebConfigBLL;

        private readonly IMicroWebColumnDAL _microWebColumnDAL;

        private readonly IMicroWebColumnArticleDAL _microWebColumnArticleDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        public MicroWebBLL(IMicroWebConfigBLL microWebConfigBLL, IMicroWebColumnDAL microWebColumnDAL,
            IMicroWebColumnArticleDAL microWebColumnArticleDAL, IUserOperationLogDAL userOperationLogDAL, ITempDataCacheDAL tempDataCacheDAL)
        {
            this._microWebConfigBLL = microWebConfigBLL;
            this._microWebColumnDAL = microWebColumnDAL;
            this._microWebColumnArticleDAL = microWebColumnArticleDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._microWebConfigBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _microWebColumnDAL, _microWebColumnArticleDAL, _userOperationLogDAL);
        }

        public async Task<EtMicroWebColumn> GetMicroWebColumn(long id)
        {
            if (id > 0)
            {
                return await _microWebColumnDAL.GetMicroWebColumn(id);
            }
            else
            {
                return await _microWebConfigBLL.MicroWebDefaultColumnGet(id);
            }
        }

        public async Task<ResponseBase> MicroWebColumnGet(MicroWebColumnGetRequest request)
        {
            var p = await GetMicroWebColumn(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            return ResponseBase.Success(new MicroWebColumnGetOutput()
            {
                Id = p.Id,
                IsShowInHome = p.IsShowInHome,
                IsShowInMenu = p.IsShowInMenu,
                IsShowYuYue = p.IsShowYuYue,
                Name = p.Name,
                OrderIndex = p.OrderIndex,
                ShowInMenuIcon = p.ShowInMenuIcon,
                ShowInMenuIconUrl = AliyunOssUtil.GetAccessUrlHttps(p.ShowInMenuIcon),
                ShowStyle = p.ShowStyle,
                Status = p.Status,
                Type = p.Type,
                TypeDesc = EmMicroWebColumnType.GetMicroWebColumnTypeDesc(p.Type),
                ShowInHomeTopIndex = p.ShowInHomeTopIndex
            });
        }

        private async Task<List<MicroWebColumnGetListOutput>> MicroWebColumnGetList(bool isOnlyEnable)
        {
            var output = new List<MicroWebColumnGetListOutput>();
            var defaultColumn = await _microWebConfigBLL.MicroWebDefaultColumnGet();
            var customColumn = await _microWebColumnDAL.GetMicroWebColumn();
            if (customColumn != null && customColumn.Count > 0)
            {
                defaultColumn.AddRange(customColumn);
            }
            if (isOnlyEnable)
            {
                defaultColumn = defaultColumn.Where(p => p.Status == EmMicroWebStatus.Enable).ToList();
            }
            var allColumn = defaultColumn.OrderBy(p => p.OrderIndex);
            foreach (var p in allColumn)
            {
                output.Add(new MicroWebColumnGetListOutput()
                {
                    Id = p.Id,
                    OrderIndex = p.OrderIndex,
                    IsShowInHome = p.IsShowInHome,
                    IsShowInHomeDesc = EmBool.GetBoolDesc(p.IsShowInHome),
                    IsShowInMenu = p.IsShowInMenu,
                    IsShowInMenuDesc = EmBool.GetBoolDesc(p.IsShowInMenu),
                    IsShowYuYue = p.IsShowYuYue,
                    IsShowYuYueDesc = EmBool.GetBoolDesc(p.IsShowYuYue),
                    Name = p.Name,
                    ShowInMenuIcon = p.ShowInMenuIcon,
                    ShowInMenuIconUrl = AliyunOssUtil.GetAccessUrlHttps(p.ShowInMenuIcon),
                    ShowStyle = p.ShowStyle,
                    Status = p.Status,
                    Type = p.Type,
                    StatusDesc = EmMicroWebStatus.GetMicroWebStatusDesc(p.Status),
                    TypeDesc = EmMicroWebColumnType.GetMicroWebColumnTypeDesc(p.Type),
                    ShowInHomeTopIndex = p.ShowInHomeTopIndex
                });
            }
            return output;
        }

        public async Task<ResponseBase> MicroWebColumnGetList(MicroWebColumnGetListRequest request)
        {
            return ResponseBase.Success(await MicroWebColumnGetList(request.IsOnlyEnable));
        }

        public async Task<ResponseBase> MicroWebColumnChangeStatus(MicroWebColumnChangeStatusRequest request)
        {
            var p = await GetMicroWebColumn(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            if (request.Id > 0)
            {
                await _microWebColumnDAL.ChangeStatus(request.Id, request.NewStatus);
            }
            else
            {
                await _microWebConfigBLL.MicroWebDefaultColumnChangeStatus(request.LoginTenantId, request.Id, request.NewStatus);
            }
            var desc = request.NewStatus == EmMicroWebStatus.Enable ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{desc}栏目-{p.Name}", EmUserOperationType.MicroWebManage);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnAdd(MicroWebColumnAddRequest request)
        {
            var allColumn = await _microWebColumnDAL.GetMicroWebColumn();
            if (allColumn != null && allColumn.Count >= 5)
            {
                return ResponseBase.CommonError("最多添加5个栏目");
            }
            if (request.Type == EmMicroWebColumnType.SinglePage)
            {
                request.ShowStyle = EmMicroWebStyle.Style1;
            }
            await _microWebColumnDAL.AddMicroWebColumn(new EtMicroWebColumn()
            {
                Name = request.Name,
                IsDeleted = EmIsDeleted.Normal,
                IsShowInHome = request.IsShowInHome,
                IsShowInMenu = request.IsShowInMenu,
                IsShowYuYue = request.IsShowYuYue,
                OrderIndex = request.OrderIndex,
                ShowInMenuIcon = request.ShowInMenuIcon,
                ShowStyle = request.ShowStyle,
                Status = EmMicroWebStatus.Enable,
                TenantId = request.LoginTenantId,
                Type = request.Type,
                ShowInHomeTopIndex = request.ShowInHomeTopIndex
            });

            await _userOperationLogDAL.AddUserLog(request, $"添加栏目-{request.Name}", EmUserOperationType.MicroWebManage);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnEdit(MicroWebColumnEditRequest request)
        {
            var thisData = await GetMicroWebColumn(request.Id);
            if (thisData == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            if (request.Id > 0)
            {
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
                thisData.ShowInHomeTopIndex = request.ShowInHomeTopIndex;
                await _microWebColumnDAL.EditMicroWebColumn(thisData);
            }
            else
            {
                await _microWebConfigBLL.MicroWebDefaultColumnSave(request);
            }

            await _userOperationLogDAL.AddUserLog(request, $"编辑栏目-{request.Name}", EmUserOperationType.MicroWebManage);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnDel(MicroWebColumnDelRequest request)
        {
            if (request.Id <= 0)
            {
                return ResponseBase.CommonError("系统栏目无法删除");
            }
            var thisData = await GetMicroWebColumn(request.Id);
            if (thisData == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            await _microWebColumnDAL.DelMicroWebColumn(request.Id);

            await _userOperationLogDAL.AddUserLog(request, $"删除栏目-{thisData.Name}", EmUserOperationType.MicroWebManage);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnSinglePageArticleGet(MicroWebColumnSinglePageGetRequest request)
        {
            var p = await GetMicroWebColumn(request.ColumnId);
            if (p == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            var myData = await _microWebColumnArticleDAL.GetMicroWebColumnSinglePageArticle(request.ColumnId);
            if (myData == null)
            {
                return ResponseBase.Success(new MicroWebColumnArticleGetOutput()
                {
                    ColumnId = request.ColumnId
                });
            }
            return ResponseBase.Success(new MicroWebColumnArticleGetOutput()
            {
                ColumnId = request.ColumnId,
                ArContent = myData.ArContent,
                ArCoverImg = myData.ArCoverImg,
                ArCoverImgUrl = AliyunOssUtil.GetAccessUrlHttps(myData.ArCoverImg),
                ArSummary = myData.ArSummary,
                ArTitile = myData.ArTitile,
                Id = myData.Id,
                Status = myData.Status,
                IsShowYuYue = p.IsShowYuYue,
                ShowStyle = p.ShowStyle,
                ColumnName = p.Name
            });
        }

        public async Task<ResponseBase> MicroWebColumnSinglePageArticleSave(MicroWebColumnSinglePageSaveRequest request)
        {
            var myData = await _microWebColumnArticleDAL.GetMicroWebColumnSinglePageArticle(request.ColumnId);
            var now = DateTime.Now;
            if (myData == null)
            {
                myData = new EtMicroWebColumnArticle()
                {
                    TenantId = request.LoginTenantId,
                    CreateUserId = request.LoginUserId,
                    ColumnId = request.ColumnId,
                    Status = EmMicroWebStatus.Enable,
                    CreateTime = now,
                    IsDeleted = EmIsDeleted.Normal,
                    OrderIndex = 0,
                    ReadCount = 0
                };
            }
            myData.ArTitile = request.ArTitile;
            myData.ArCoverImg = request.ArCoverImg;
            myData.ArSummary = request.ArSummary;
            myData.ArContent = request.ArContent;
            myData.UpdateTime = now;
            await _microWebColumnArticleDAL.SaveMicroWebColumnSinglePageArticle(myData);

            await _userOperationLogDAL.AddUserLog(request, $"修改栏目内容-{myData.ArTitile}", EmUserOperationType.MicroWebManage);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnArticleGetPaging(MicroWebColumnArticleGetPagingRequest request)
        {
            var pagingData = await _microWebColumnArticleDAL.GetPaging(request);
            var output = new List<MicroWebColumnArticleGetPagingOutput>();
            if (pagingData.Item1 != null && pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new MicroWebColumnArticleGetPagingOutput()
                    {
                        ArCoverImg = p.ArCoverImg,
                        ArCoverImgUrl = AliyunOssUtil.GetAccessUrlHttps(p.ArCoverImg),
                        ArSummary = p.ArSummary,
                        ArTitile = p.ArTitile,
                        ColumnId = p.ColumnId,
                        CreateTime = p.CreateTime,
                        Id = p.Id,
                        OrderIndex = p.OrderIndex,
                        ReadCount = p.ReadCount,
                        Status = p.Status,
                        UpdateTime = p.UpdateTime,
                        StatusDesc = EmMicroWebStatus.GetMicroWebStatusDesc(p.Status)
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<MicroWebColumnArticleGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> MicroWebColumnArticleGet(MicroWebColumnArticleGetRequest request)
        {
            var myData = await _microWebColumnArticleDAL.GetMicroWebColumnArticle(request.Id);
            if (myData == null)
            {
                return ResponseBase.CommonError("内容记录不存在");
            }
            var p = await GetMicroWebColumn(myData.ColumnId);
            if (p == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            return ResponseBase.Success(new MicroWebColumnArticleGetOutput()
            {
                ColumnId = myData.ColumnId,
                ArContent = myData.ArContent,
                ArCoverImg = myData.ArCoverImg,
                ArCoverImgUrl = AliyunOssUtil.GetAccessUrlHttps(myData.ArCoverImg),
                ArSummary = myData.ArSummary,
                ArTitile = myData.ArTitile,
                Id = myData.Id,
                Status = myData.Status,
                IsShowYuYue = p.IsShowYuYue,
                ShowStyle = p.ShowStyle,
                ColumnName = p.Name
            });
        }

        public async Task<ResponseBase> MicroWebColumnArticleAdd(MicroWebColumnArticleAddRequest request)
        {
            var now = DateTime.Now;
            await _microWebColumnArticleDAL.AddMicroWebColumnArticle(new EtMicroWebColumnArticle()
            {
                TenantId = request.LoginTenantId,
                CreateUserId = request.LoginUserId,
                ColumnId = request.ColumnId,
                Status = EmMicroWebStatus.Enable,
                CreateTime = now,
                IsDeleted = EmIsDeleted.Normal,
                OrderIndex = 0,
                ReadCount = 0,
                ArContent = request.ArContent,
                ArCoverImg = request.ArCoverImg,
                ArSummary = request.ArSummary,
                ArTitile = request.ArTitile,
                UpdateTime = now
            });

            await _userOperationLogDAL.AddUserLog(request, $"添加栏目内容-{request.ArTitile}", EmUserOperationType.MicroWebManage, now);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnArticleEdit(MicroWebColumnArticleEditRequest request)
        {
            var myData = await _microWebColumnArticleDAL.GetMicroWebColumnArticle(request.Id);
            if (myData == null)
            {
                return ResponseBase.CommonError("内容记录不存在");
            }
            myData.ArTitile = request.ArTitile;
            myData.ArCoverImg = request.ArCoverImg;
            myData.ArSummary = request.ArSummary;
            myData.ArContent = request.ArContent;
            myData.UpdateTime = DateTime.Now;
            await _microWebColumnArticleDAL.EditMicroWebColumnArticle(myData);

            await _userOperationLogDAL.AddUserLog(request, $"编辑栏目内容-{request.ArTitile}", EmUserOperationType.MicroWebManage);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnArticleDel(MicroWebColumnArticleDelRequest request)
        {
            var myData = await _microWebColumnArticleDAL.GetMicroWebColumnArticle(request.Id);
            if (myData == null)
            {
                return ResponseBase.CommonError("内容记录不存在");
            }
            await _microWebColumnArticleDAL.DelMicroWebColumnArticle(request.Id);

            await _userOperationLogDAL.AddUserLog(request, $"删除栏目内容-{myData.ArTitile}", EmUserOperationType.MicroWebManage);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnArticleChangeStatus(MicroWebColumnArticleChangeStatusRequest request)
        {
            var myData = await _microWebColumnArticleDAL.GetMicroWebColumnArticle(request.Id);
            if (myData == null)
            {
                return ResponseBase.CommonError("内容记录不存在");
            }
            await _microWebColumnArticleDAL.ChangeMicroWebColumnArticleStatus(request.Id, request.NewStatus);

            var desc = request.NewStatus == EmMicroWebStatus.Enable ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{desc}栏目内容-{myData.ArTitile}", EmUserOperationType.MicroWebManage);
            RemoveMicroWebHomeBucket(request.LoginTenantId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebHomeGet(RequestBase request)
        {
            return ResponseBase.Success(await GetMicroWebHome(request.LoginTenantId));
        }

        private void RemoveMicroWebHomeBucket(int tenantId)
        {
            _tempDataCacheDAL.RemoveMicroWebHomeBucket(tenantId);
        }

        public async Task<MicroWebHomeView> GetMicroWebHome(int tenantId)
        {
            var bucket = _tempDataCacheDAL.GetMicroWebHomeBucket(tenantId);
            if (bucket == null)
            {
                bucket = await UpdateMicroWebHomeBucket(tenantId);
            }
            return bucket.MicroWebHomeView;
        }

        private async Task<MicroWebHomeBucket> UpdateMicroWebHomeBucket(int tenantId)
        {
            var bucket = await GetMicroWebHomeBucketDb(tenantId);
            _tempDataCacheDAL.SetMicroWebHomeBucket(tenantId, bucket);
            return bucket;
        }

        private async Task<MicroWebHomeBucket> GetMicroWebHomeBucketDb(int tenantId)
        {
            var result = new MicroWebHomeView()
            {
                Banners = new List<MicroWebHomeBanner>(),
                Columns = new List<MicroWebHomeColumn>(),
                Menus = new List<MicroWebHomeMenus>()
            };
            var microWebBanner = await _microWebConfigBLL.MicroWebBannerGet(tenantId);
            if (microWebBanner != null && microWebBanner.Images != null && microWebBanner.Images.Count > 0)
            {
                foreach (var p in microWebBanner.Images)
                {
                    result.Banners.Add(new MicroWebHomeBanner() { ImgUrl = p.ImgUrl });
                }
            }

            var allEnableColumnGetList = await MicroWebColumnGetList(true);
            var microMenus = allEnableColumnGetList.Where(p => p.IsShowInMenu == EmBool.True);
            if (microMenus.Any())
            {
                foreach (var p in microMenus)
                {
                    result.Menus.Add(new MicroWebHomeMenus()
                    {
                        Id = p.Id,
                        ShowInMenuIconUrl = p.ShowInMenuIconUrl,
                        Type = p.Type,
                        Name = p.Name
                    });
                }
            }

            var microWebAddress = await _microWebConfigBLL.MicroWebTenantAddressGet(tenantId);
            if (microWebAddress != null && !string.IsNullOrEmpty(microWebAddress.Name))
            {
                result.Address = new MicroWebHomeAddress()
                {
                    Address = microWebAddress.Address,
                    Name = microWebAddress.Name,
                    CoverIconUrl = microWebAddress.CoverIconUrl,
                    Latitude = microWebAddress.Latitude,
                    Longitude = microWebAddress.Longitude
                };
            }

            var microHome = allEnableColumnGetList.Where(p => p.IsShowInHome == EmBool.True);
            if (microHome.Any())
            {
                foreach (var p in microHome)
                {
                    var myColumn = new MicroWebHomeColumn()
                    {
                        Articles = new List<MicroWebHomeArticle>(),
                        Id = p.Id,
                        IsShowYuYue = p.IsShowYuYue,
                        Name = p.Name,
                        ShowStyle = p.ShowStyle,
                        Type = p.Type
                    };
                    IEnumerable<EtMicroWebColumnArticle> articles;
                    if (myColumn.Type == EmMicroWebColumnType.SinglePage)
                    {
                        articles = await _microWebColumnArticleDAL.GetMicroWebColumnArticleTopLimit(p.Id, 1);
                    }
                    else
                    {
                        var topLimit = p.ShowInHomeTopIndex;
                        if (topLimit <= 0)
                        {
                            topLimit = 2;
                        }
                        if (topLimit > 10)
                        {
                            topLimit = 10;
                        }
                        articles = await _microWebColumnArticleDAL.GetMicroWebColumnArticleTopLimit(p.Id, topLimit);
                    }
                    if (articles != null && articles.Any())
                    {
                        foreach (var myContent in articles)
                        {
                            myColumn.Articles.Add(new MicroWebHomeArticle()
                            {
                                ArCoverImgUrl = AliyunOssUtil.GetAccessUrlHttps(myContent.ArCoverImg),
                                ArSummary = myContent.ArSummary,
                                ArTitile = myContent.ArTitile,
                                ColumnId = myContent.ColumnId,
                                Id = myContent.Id
                            });
                        }
                        result.Columns.Add(myColumn);
                    }
                }
            }

            return new MicroWebHomeBucket()
            {
                MicroWebHomeView = result
            };
        }

        public async Task<MicroWebTenantAddressGetOutput> MicroWebTenantAddressGet(int tenantId)
        {
            return await _microWebConfigBLL.MicroWebTenantAddressGet(tenantId);
        }
    }
}
