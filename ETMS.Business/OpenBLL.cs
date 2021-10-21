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
using ETMS.IBusiness;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.Open2.Output;
using ETMS.IBusiness.Wechart;
using ETMS.IEventProvider;
using ETMS.Business.Common;
using ETMS.Event.DataContract;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.Entity.Dto.Product.Output;
using ETMS.Business.WxCore;
using ETMS.Entity.Config;
using ETMS.LOG;

namespace ETMS.Business
{
    public class OpenBLL : WeChatAccessBLL, IOpenBLL
    {
        private readonly IMicroWebBLL _microWebBLL;

        private readonly IAppConfigBLL _appConfigBLL;

        private readonly IMicroWebColumnArticleDAL _microWebColumnArticleDAL;

        private readonly ITryCalssApplyLogDAL _tryCalssApplyLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IParentStudentDAL _parentStudentDAL;

        private readonly IMallGoodsDAL _mallGoodsDAL;

        public OpenBLL(IMicroWebBLL microWebBLL, IAppConfigBLL appConfigBLL, IMicroWebColumnArticleDAL microWebColumnArticleDAL,
            IComponentAccessBLL componentAccessBLL, ITryCalssApplyLogDAL tryCalssApplyLogDAL, IEventPublisher eventPublisher,
            IParentStudentDAL parentStudentDAL, IMallGoodsDAL mallGoodsDAL, IAppConfigurtaionServices appConfigurtaionServices)
            : base(componentAccessBLL, appConfigurtaionServices)
        {
            this._microWebBLL = microWebBLL;
            this._appConfigBLL = appConfigBLL;
            this._microWebColumnArticleDAL = microWebColumnArticleDAL;
            this._tryCalssApplyLogDAL = tryCalssApplyLogDAL;
            this._eventPublisher = eventPublisher;
            this._parentStudentDAL = parentStudentDAL;
            this._mallGoodsDAL = mallGoodsDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._microWebBLL.InitTenantId(tenantId);
            this._appConfigBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _microWebColumnArticleDAL, _tryCalssApplyLogDAL, _parentStudentDAL, _mallGoodsDAL);
        }

        public async Task<ResponseBase> TenantInfoGet(Open2Base request)
        {
            var tenantInfo = await _appConfigBLL.GetTenantInfoH52(request.LoginTenantId);
            var addressConfig = await _microWebBLL.MicroWebTenantAddressGet(request.LoginTenantId);
            return ResponseBase.Success(new TenantInfoGetOutput()
            {
                TenantAddressInfo = addressConfig,
                TenantInfo = tenantInfo
            });
        }

        public async Task<ResponseBase> MicroWebHomeGet(Open2Base request)
        {
            return ResponseBase.Success(await _microWebBLL.GetMicroWebHome(request.LoginTenantId));
        }

        public async Task<ResponseBase> MicroWebColumnSingleArticleGet(MicroWebColumnSingleArticleGetRequest request)
        {
            var myColumnInfo = await _microWebBLL.GetMicroWebColumn(request.ColumnId);
            if (myColumnInfo == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            var myData = await _microWebColumnArticleDAL.GetMicroWebColumnSinglePageArticle(request.ColumnId);
            if (myData == null)
            {
                return ResponseBase.Success(new MicroWebArticleGetOutput()
                {
                    ColumnId = request.ColumnId
                });
            }
            return ResponseBase.Success(new MicroWebArticleGetOutput()
            {
                ColumnId = request.ColumnId,
                ArContent = myData.ArContent,
                ArCoverImgUrl = AliyunOssUtil.GetAccessUrlHttps(myData.ArCoverImg),
                ArSummary = myData.ArSummary,
                ArTitile = myData.ArTitile,
                Id = myData.Id,
                IsShowYuYue = myColumnInfo.IsShowYuYue,
                ShowStyle = myColumnInfo.ShowStyle,
                ColumnName = myColumnInfo.Name
            });
        }

        public async Task<ResponseBase> MicroWebArticleGet(MicroWebArticleGetRequest request)
        {
            var myData = await _microWebColumnArticleDAL.GetMicroWebColumnArticle(request.ArticleId);
            if (myData == null)
            {
                return ResponseBase.CommonError("内容记录不存在");
            }
            var myColumnInfo = await _microWebBLL.GetMicroWebColumn(myData.ColumnId);
            if (myColumnInfo == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            return ResponseBase.Success(new MicroWebArticleGetOutput()
            {
                ColumnId = myData.ColumnId,
                ArContent = myData.ArContent,
                ArCoverImgUrl = AliyunOssUtil.GetAccessUrlHttps(myData.ArCoverImg),
                ArSummary = myData.ArSummary,
                ArTitile = myData.ArTitile,
                Id = myData.Id,
                IsShowYuYue = myColumnInfo.IsShowYuYue,
                ShowStyle = myColumnInfo.ShowStyle,
                ColumnName = myColumnInfo.Name
            });
        }

        public async Task<ResponseBase> MicroWebArticleGetPaging(MicroWebArticleGetPagingRequest request)
        {
            var myColumnInfo = await _microWebBLL.GetMicroWebColumn(request.ColumnId);
            if (myColumnInfo == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            var pagingData = await _microWebColumnArticleDAL.GetPaging(request);
            var output = new List<MicroWebArticleGetPagingOutput>();
            if (pagingData.Item1 != null && pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new MicroWebArticleGetPagingOutput()
                    {
                        ArCoverImgUrl = AliyunOssUtil.GetAccessUrlHttps(p.ArCoverImg),
                        ArSummary = p.ArSummary,
                        ArTitile = p.ArTitile,
                        ColumnId = p.ColumnId,
                        Id = p.Id,
                        ShowStyle = myColumnInfo.ShowStyle
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<MicroWebArticleGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> GetJsSdkUiPackage(GetJsSdkUiPackageRequest request)
        {
            return await base.GetJsSdkUiPackage(request.LoginTenantId, request.Url);
        }

        public async Task<ResponseBase> GetAuthorizeUrl(GetAuthorizeUrlRequest request)
        {
            return await this.GetAuthorizeUrl2(request.LoginTenantId, request.SourceUrl);
        }

        public async Task<ResponseBase> GetWxOpenId(GetWxOpenIdRequest request)
        {
            Log.Info($"GetWxOpenId:{request.Code}", this.GetType());
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(request.LoginTenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[GetWxOpenId]未找到机构授权信息,tenantId:{request.LoginTenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var authToken = GetAuthAccessToken(tenantWechartAuth.AuthorizerAppid, request.Code);
            return ResponseBase.Success(new GetWxOpenIdOutput()
            {
                OpenId = authToken.openid
            });
        }

        public async Task<ResponseBase> TryCalssApply(TryCalssApplyRequest request)
        {
            long? recommandStudentId = null;
            if (!string.IsNullOrEmpty(request.StuNo))
            {
                var phone = TenantLib.GetPhoneDecrypt(request.StuNo);
                var students = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, phone);
                if (students != null && students.Any())
                {
                    recommandStudentId = students.First().Id;
                }
            }
            var applyLog = new EtTryCalssApplyLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                Phone = request.Phone,
                RecommandStudentId = recommandStudentId,
                SourceType = EmTryCalssSourceType.WeChat,
                StudentId = null,
                TenantId = request.LoginTenantId,
                TouristName = string.Empty,
                ClassTime = string.Empty,
                ClassOt = null,
                CourseDesc = string.Empty,
                ApplyOt = DateTime.Now,
                CourseId = null,
                HandleOt = null,
                HandleRemark = null,
                HandleStatus = EmTryCalssApplyHandleStatus.Unreviewed,
                HandleUser = null,
                TouristRemark = string.Empty
            };
            await _tryCalssApplyLogDAL.AddTryCalssApplyLog(applyLog);

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            _eventPublisher.Publish(new NoticeUserTryCalssApplyEvent(request.LoginTenantId)
            {
                TryCalssApplyLog = applyLog
            });

            return ResponseBase.Success(new TryCalssApplyOutput()
            {
                TryClassId = applyLog.Id
            });
        }

        public async Task<ResponseBase> TryCalssApplySupplement(TryCalssApplySupplementRequest request)
        {
            var log = await _tryCalssApplyLogDAL.GetTryCalssApplyLog(request.TryClassId);
            if (log == null)
            {
                return ResponseBase.CommonError("试听记录不存在");
            }
            log.TouristName = request.Name;
            log.CourseDesc = request.TryCourse;
            await _tryCalssApplyLogDAL.EditTryCalssApplyLog(log);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MicroWebColumnGet(ETMS.Entity.Dto.Open2.Request.MicroWebColumnGetRequest request)
        {
            var p = await _microWebBLL.GetMicroWebColumn(request.ColumnId);
            if (p == null)
            {
                return ResponseBase.CommonError("栏目不存在");
            }
            return ResponseBase.Success(new ETMS.Entity.Dto.Open2.Output.MicroWebColumnGetOutput()
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

        public async Task<ResponseBase> MicroWebArticleReading(MicroWebArticleReadingRequest request)
        {
            await _microWebColumnArticleDAL.AddReadCount(request.ArticleId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MallGoodsGetPaging(MallGoodsGetPagingRequest request)
        {
            var pagingData = await _mallGoodsDAL.GetPagingComplex(request);
            var output = new List<MallGoodsOpenGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    List<PriceRuleDesc> myPriceRuleDesc = null;
                    if (p.ProductType == EmProductType.Course)
                    {
                        var myMallGoodsBucket = await _mallGoodsDAL.GetMallGoods(p.Id);
                        if (myMallGoodsBucket == null || myMallGoodsBucket.MallGoods == null)
                        {
                            continue;
                        }
                        myPriceRuleDesc = ComBusiness3.GetPriceRuleDescs(myMallGoodsBucket.MallCoursePriceRules);
                    }
                    output.Add(new MallGoodsOpenGetPagingOutput()
                    {
                        GId = p.Id,
                        ImgCoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.ImgCover),
                        Name = p.Name,
                        OriginalPrice = p.OriginalPrice == 0 ? string.Empty : p.OriginalPrice.EtmsToString2(),
                        Price = p.Price,
                        PriceDesc = p.PriceDesc,
                        ProductType = p.ProductType,
                        ProductTypeDesc = p.ProductTypeDesc,
                        SpecItems = ComBusiness4.GetSpecView(p.SpecContent),
                        TagItems = ComBusiness4.GetTagView(p.TagContent),
                        CoursePriceRules = myPriceRuleDesc
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<MallGoodsOpenGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> MallGoodsDetailGet(MallGoodsDetailGetRequest request)
        {
            var mallGoodsBucket = await _mallGoodsDAL.GetMallGoods(request.GId);
            if (mallGoodsBucket == null || mallGoodsBucket.MallGoods == null)
            {
                return ResponseBase.CommonError("商品不存在");
            }
            var p = mallGoodsBucket.MallGoods;
            List<PriceRuleDesc> myPriceRuleDesc = null;
            if (p.ProductType == EmProductType.Course)
            {
                myPriceRuleDesc = ComBusiness3.GetPriceRuleDescs(mallGoodsBucket.MallCoursePriceRules);
            }
            var output = new MallGoodsDetailGetOutput()
            {
                GsContent = p.GsContent,
                GId = p.Id,
                Name = p.Name,
                Price = p.Price,
                PriceDesc = p.PriceDesc,
                ProductType = p.ProductType,
                OriginalPrice = p.OriginalPrice == 0 ? string.Empty : p.OriginalPrice.EtmsToString2(),
                ImgCoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.ImgCover),
                ImgDetailUrL = EtmsHelper2.GetImgUrl(p.ImgDetail),
                ProductTypeDesc = p.ProductTypeDesc,
                SpecItems = ComBusiness4.GetSpecView(p.SpecContent),
                TagItems = ComBusiness4.GetTagView(p.TagContent),
                CoursePriceRules = myPriceRuleDesc
            };
            return ResponseBase.Success(output);
        }
    }
}
