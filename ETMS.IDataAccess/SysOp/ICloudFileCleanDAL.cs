using ETMS.Entity.Temp.CloudFileClean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.SysOp
{
    public interface ICloudFileCleanDAL : IBaseDAL
    {
        Task<IEnumerable<HomeworkDetailView>> GetActiveHomeworkDetail(long homeworkId);

        Task<Tuple<IEnumerable<MicroWebColumnArticleView>, int>> GetMicroWebColumnArticlePaging(long columnId, int pageSize, int pageCurrent);
    }
}
