using ETMS.Entity.CacheBucket.Mall;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.MallGoodsDAL
{
    public interface IMallPrepayDAL : IBaseDAL
    {
        Task MallPrepayAdd(EtMallPrepay entity);

        Task<MallPrepayBucket> MallPrepayGetBucket(long lcsPayLogId);

        Task<EtMallPrepay> MallPrepayGet(long lcsPayLogId);

        Task UpdateMallPrepayStatus(long lcsPayLogId,byte newStatus);
    }
}
