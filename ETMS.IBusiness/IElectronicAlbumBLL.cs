using ETMS.Entity.Common;
using ETMS.Entity.Dto.Interaction.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IElectronicAlbumBLL : IBaseBLL
    {
        Task<ResponseBase> SysElectronicAlbumGetPaging(SysElectronicAlbumGetPagingRequest request);

        Task<ResponseBase> ElectronicAlbumGetPaging(ElectronicAlbumGetPagingRequest request);

        Task<ResponseBase> ElectronicAlbumCreateInit(ElectronicAlbumCreateInitRequest request);

        Task<ResponseBase> ElectronicAlbumPageInit(ElectronicAlbumPageInitRequest request);

        Task<ResponseBase> ElectronicAlbumSave(ElectronicAlbumSaveRequest request);

        Task<ResponseBase> ElectronicAlbumPublish(ElectronicAlbumPublishRequest request);

        Task<ResponseBase> ElectronicAlbumDel(ElectronicAlbumDelRequest request);
    }
}
