using ETMS.Entity.CacheBucket.MicroWeb;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Temp.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.MicroWeb
{
    public interface IMicroWebBLL : IBaseBLL
    {
        Task<EtMicroWebColumn> GetMicroWebColumn(long id);

        Task<ResponseBase> MicroWebColumnGet(MicroWebColumnGetRequest request);

        Task<ResponseBase> MicroWebColumnGetList(MicroWebColumnGetListRequest request);

        Task<ResponseBase> MicroWebColumnChangeStatus(MicroWebColumnChangeStatusRequest request);

        Task<ResponseBase> MicroWebColumnAdd(MicroWebColumnAddRequest request);

        Task<ResponseBase> MicroWebColumnEdit(MicroWebColumnEditRequest request);

        Task<ResponseBase> MicroWebColumnDel(MicroWebColumnDelRequest request);

        Task<ResponseBase> MicroWebColumnSinglePageArticleGet(MicroWebColumnSinglePageGetRequest request);

        Task<ResponseBase> MicroWebColumnSinglePageArticleSave(MicroWebColumnSinglePageSaveRequest request);

        Task<ResponseBase> MicroWebColumnArticleGetPaging(MicroWebColumnArticleGetPagingRequest request);

        Task<ResponseBase> MicroWebColumnArticleGet(MicroWebColumnArticleGetRequest request);

        Task<ResponseBase> MicroWebColumnArticleAdd(MicroWebColumnArticleAddRequest request);

        Task<ResponseBase> MicroWebColumnArticleEdit(MicroWebColumnArticleEditRequest request);

        Task<ResponseBase> MicroWebColumnArticleDel(MicroWebColumnArticleDelRequest request);

        Task<ResponseBase> MicroWebColumnArticleChangeStatus(MicroWebColumnArticleChangeStatusRequest request);

        Task<ResponseBase> MicroWebHomeGet(RequestBase request);

        Task<MicroWebHomeView> GetMicroWebHome(int tenantId);
    }
}
