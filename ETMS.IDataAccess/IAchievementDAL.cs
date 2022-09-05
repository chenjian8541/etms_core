using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Entity.View.OnlyOneFiled;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IAchievementDAL: IBaseDAL
    {
        Task<EtAchievement> GetAchievement(long id);

        Task AddAchievement(EtAchievement entity,List<EtAchievementDetail> details);

        Task AddAchievementDetails(List<EtAchievementDetail> entitys);

        Task EditAchievement(EtAchievement entity);

        Task EditAchievementDetails(List<EtAchievementDetail> details);

        Task UpdateAchievementDetail(long achievementId, string name, byte showRankParent,byte showParent);

        Task SetAchievementDetailIsRead(long achievementId,long achievementDetailId);

        Task DelAchievementDetail(List<long> ids);

        Task DelAchievement(long id);

        Task PushAchievement(long id);

        Task<List<EtAchievementDetail>> GetAchievementDetail(long achievementId);

        Task<EtAchievementDetail> GetAchievementDetailById(long id);

        Task<Tuple<IEnumerable<EtAchievement>, int>> GetPaging(IPagingRequest request);

        Task<Tuple<IEnumerable<EtAchievementDetail>, int>> GetPagingDetail(IPagingRequest request);

        Task<List<EtAchievementDetail>> GetAchievementDetail(long studentId, long subjectId, DateTime startTime, DateTime endTime);
    }
}
