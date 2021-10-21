using ETMS.Entity.Common;
using ETMS.Entity.Dto.Open2.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IOpenBLL : IBaseBLL
    {
        Task<ResponseBase> TenantInfoGet(Open2Base request);

        Task<ResponseBase> MicroWebHomeGet(Open2Base request);

        Task<ResponseBase> MicroWebColumnSingleArticleGet(MicroWebColumnSingleArticleGetRequest request);

        Task<ResponseBase> MicroWebArticleGet(MicroWebArticleGetRequest request);

        Task<ResponseBase> MicroWebArticleGetPaging(MicroWebArticleGetPagingRequest request);

        Task<ResponseBase> GetJsSdkUiPackage(GetJsSdkUiPackageRequest request);

        Task<ResponseBase> TryCalssApply(TryCalssApplyRequest request);

        Task<ResponseBase> TryCalssApplySupplement(TryCalssApplySupplementRequest request);

        Task<ResponseBase> MicroWebColumnGet(MicroWebColumnGetRequest request);

        Task<ResponseBase> MicroWebArticleReading(MicroWebArticleReadingRequest request);

        Task<ResponseBase> MallGoodsGetPaging(MallGoodsGetPagingRequest request);

        Task<ResponseBase> MallGoodsDetailGet(MallGoodsDetailGetRequest request);
    }
}
