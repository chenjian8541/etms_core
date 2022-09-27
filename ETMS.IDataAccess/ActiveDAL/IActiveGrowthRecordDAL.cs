using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IActiveGrowthRecordDAL : IBaseDAL
    {
        Task<bool> AddActiveGrowthRecord(EtActiveGrowthRecord entity);

        Task EditActiveGrowthRecord(EtActiveGrowthRecord entity);

        Task UpdateActiveGrowthRecordTotalCount(long growthRecordId, int totalCount);

        Task<ActiveGrowthRecordBucket> GetActiveGrowthRecord(long id);

        Task<bool> DelActiveGrowthRecord(long id);

        Task<Tuple<IEnumerable<EtActiveGrowthRecord>, int>> GetPaging(IPagingRequest request);

        Task<bool> AddActiveGrowthRecordDetailComment(EtActiveGrowthRecordDetailComment entity);

        Task<bool> DelActiveGrowthRecordDetailComment(long growthRecordId, long commentId);

        bool AddActiveGrowthRecordDetail(List<EtActiveGrowthRecordDetail> entitys);

        Task AddActiveGrowthRecordDetail(EtActiveGrowthRecordDetail entity);

        Task<Tuple<IEnumerable<EtActiveGrowthRecordDetail>, int>> GetDetailPaging(IPagingRequest request);

        Task<EtActiveGrowthRecordDetail> GetActiveGrowthRecordDetail(long growthRecordDetailId);

        Task<bool> SetActiveGrowthRecordDetailNewFavoriteStatus(long growthRecordDetailId, byte newFavoriteStatus);

        Task<bool> SetActiveGrowthRecordNewFavoriteStatus(long growthRecordId, byte newFavoriteStatus);

        Task<bool> SetActiveGrowthRecordIsRead(long growthRecordId, long growthRecordDetailId);

        Task GrowthRecordAddReadCount(long growthRecordId);

        Task<IEnumerable<ActiveGrowthRecordDetailSimpleView>> GetActiveGrowthRecordDetailSimpleView(long growthRecordId);

        Task<IEnumerable<GrowthRecordDetailView>> GetGrowthRecordDetailView(long growthRecordId);

        Task DelActiveGrowthRecordDetailAboutRelatedInfo(int sceneType,long relatedId,long studentId);

        Task UpdateActiveGrowthRecordDetail(long growthRecordId,string growthContent,string growthMedias);
    }
}
