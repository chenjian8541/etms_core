using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ICostDAL : IBaseDAL
    {
        Task<bool> ExistCost(string name, long id = 0);

        Task<bool> AddCost(EtCost cost);

        Task<bool> EditCost(EtCost cost);

        Task<EtCost> GetCost(long id);

        Task<bool> DelCost(long id);

        Task<Tuple<IEnumerable<EtCost>, int>> GetPaging(RequestPagingBase request);

        Task<bool> AddSaleQuantity(long id, int count);

        Task<bool> DeductioneSaleQuantity(long id, int count);
    }
}
