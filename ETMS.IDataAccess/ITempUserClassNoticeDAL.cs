using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITempUserClassNoticeDAL : IBaseDAL
    {
        Task<List<EtTempUserClassNotice>> GetTempUserClassNotice(DateTime classOt);

        Task GenerateTempUserClassNotice(DateTime classOt);

        Task SetProcessed(List<long> ids, DateTime classOt);
    }
}
