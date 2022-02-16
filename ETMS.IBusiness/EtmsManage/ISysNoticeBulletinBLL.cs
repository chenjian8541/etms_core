using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.Agent3.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysNoticeBulletinBLL
    {
        Task<ResponseBase> SysSysNoticeBulletinAdd(SysSysNoticeBulletinAddRequest request);

        Task<ResponseBase> SysSysNoticeBulletinDel(SysSysNoticeBulletinDelRequest request);

        Task<ResponseBase> SysSysNoticeBulletinPaging(SysSysNoticeBulletinPagingRequest request);
    }
}
