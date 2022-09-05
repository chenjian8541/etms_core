using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational3.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IAchievementBLL : IBaseBLL
    {
        Task<ResponseBase> AchievementGetPaging(AchievementGetPagingRequest request);

        Task<ResponseBase> AchievementDetailGetPaging(AchievementDetailGetPagingRequest request);

        Task<ResponseBase> AchievementGet(AchievementGetRequest request);

        Task<ResponseBase> AchievementAdd(AchievementAddRequest request);

        Task<ResponseBase> AchievementDel(AchievementDelRequest request);

        Task<ResponseBase> AchievementEdit(AchievementEditRequest request);

        Task<ResponseBase> AchievementPush(AchievementPushRequest request);

        Task<ResponseBase> AchievementStudentIncreaseGet(AchievementStudentIncreaseGetRequest request);
    }
}
