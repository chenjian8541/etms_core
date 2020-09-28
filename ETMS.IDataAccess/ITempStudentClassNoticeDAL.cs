using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITempStudentClassNoticeDAL : IBaseDAL
    {
        Task<List<EtTempStudentClassNotice>> GetTempStudentClassNotice(DateTime classOt);

        Task GenerateTempStudentClassNotice(DateTime classOt);

        Task SetProcessed(List<long> ids, DateTime classOt);
    }
}
