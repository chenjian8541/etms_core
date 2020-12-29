using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ICouponsDAL : IBaseDAL
    {
        Task<bool> AddCoupons(EtCoupons coupons);

        Task<bool> EditCoupons(EtCoupons coupons);

        Task<EtCoupons> GetCoupons(long id);

        Task<bool> DelCoupons(long id);

        Task<Tuple<IEnumerable<EtCoupons>, int>> GetPaging(RequestPagingBase request);

        Task<int> StudentTodayGetCount(long studentId, long couponsId);

        Task<int> StudentGetCount(long studentId, long couponsId);

        Task<bool> AddCouponsStudentGet(EtCouponsStudentGet etCouponsStudentGet);

        void AddCouponsStudentGet(List<EtCouponsStudentGet> etCouponsStudentGets);

        Task<bool> AddCouponsGetCount(long couponsId, int addCount);

        Task<bool> AddCouponsUseCount(long couponsId, int addCount);

        Task<Tuple<IEnumerable<CouponsStudentGetView>, int>> CouponsStudentGetPaging(IPagingRequest request);

        Task<EtCouponsStudentGet> CouponsStudentGet(long id);

        Task<bool> ChangeCouponsStudentGetStatus(long id, byte newStatus);

        Task<bool> AddCouponsStudentUse(EtCouponsStudentUse etCouponsStudentUse);

        Task<Tuple<IEnumerable<CouponsStudentUseView>, int>> CouponsStudentUsePaging(RequestPagingBase request);

        Task<IEnumerable<CouponsStudentGetView>> GetCouponsCanUse(long studentId);

        Task<List<EtCouponsStudentGet>> GetCouponsStudentGet(string generateNo);

        Task<IEnumerable<EtCouponsStudentGet>> GetCouponsStudentGetToExpire(DateTime minTime, DateTime maxTime);
    }
}
