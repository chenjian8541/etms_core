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
using ETMS.IBusiness;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.Open2.Output;

namespace ETMS.Business
{
    public class OpenBLL : IOpenBLL
    {
        private readonly IMicroWebBLL _microWebBLL;

        private readonly IAppConfigBLL _appConfigBLL;

        private readonly IMicroWebColumnArticleDAL _microWebColumnArticleDAL;

        public OpenBLL(IMicroWebBLL microWebBLL, IAppConfigBLL appConfigBLL, IMicroWebColumnArticleDAL microWebColumnArticleDAL)
        {
            this._microWebBLL = microWebBLL;
            this._appConfigBLL = appConfigBLL;
            this._microWebColumnArticleDAL = microWebColumnArticleDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._microWebBLL.InitTenantId(tenantId);
            this._appConfigBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _microWebColumnArticleDAL);
        }

        public async Task<ResponseBase> TenantInfoGet(Open2Base request)
        {
            return await _appConfigBLL.GetTenantInfoH5(request.LoginTenantId);
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
                ShowStyle = myColumnInfo.ShowStyle
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
                ShowStyle = myColumnInfo.ShowStyle
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
    }
}
