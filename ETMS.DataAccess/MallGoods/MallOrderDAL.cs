using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess.MallGoodsDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.MallGoods
{
    public class MallOrderDAL : DataAccessBase, IMallOrderDAL
    {
        public MallOrderDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task<long> AddMallOrder(EtMallOrder entity)
        {
            await _dbWrapper.Insert(entity);
            return entity.Id;
        }

        public async Task EditMallOrder(EtMallOrder entity)
        {
            await _dbWrapper.Update(entity);
        }

        public async Task<Tuple<IEnumerable<EtMallOrder>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtMallOrder>("EtMallOrder", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
