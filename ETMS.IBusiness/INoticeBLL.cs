using ETMS.Entity.Temp.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface INoticeBLL : IBaseBLL
    {
        Task TryCalssNoticeTrackUser(TryCalssNoticeTrackUserRequest request);

        Task SendCoupons();

        Task TryCalssApplyLogHandle();
    }
}
