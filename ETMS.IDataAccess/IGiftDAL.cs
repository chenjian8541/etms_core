using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IGiftDAL : IBaseDAL
    {
        Task<bool> ExistGift(string name, long id = 0);

        Task<bool> AddGift(EtGift gift);

        Task<bool> EditGift(EtGift gift);

        Task<EtGift> GetGift(long id);

        Task<bool> DelGift(long id);

        Task<Tuple<IEnumerable<EtGift>, int>> GetPaging(IPagingRequest request);

        Task<bool> IsUserCanNotBeDelete(long id);

        Task<bool> AddGiftExchange(EtGiftExchangeLog giftExchangeLog, List<EtGiftExchangeLogDetail> giftExchangeLogDetails);

        Task<int> GetStudentExchangeNums(long studentId, long giftId);

        Task<bool> DeductionNums(long giftId, int deNums);

        Task<Tuple<IEnumerable<GiftExchangeLogView>, int>> GetExchangeLogPaging(RequestPagingBase request);

        Task<Tuple<IEnumerable<GiftExchangeLogDetailView>, int>> GetExchangeLogDetailPaging(IPagingRequest request);

        Task<IEnumerable<GiftExchangeLogDetailView>> GetExchangeLogDetail(long giftExchangeLogId);

        Task<bool> UpdateExchangeLogNewStatus(long giftExchangeLogId, byte newStatus);
    }
}
