using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp.CloudFileClean;
using ETMS.IDataAccess.SysOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.SysOp
{
    public class CloudFileCleanDAL : DataAccessBase, ICloudFileCleanDAL
    {
        public CloudFileCleanDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<IEnumerable<HomeworkDetailView>> GetActiveHomeworkDetail(long homeworkId)
        {
            return await this._dbWrapper.ExecuteObject<HomeworkDetailView>(
                $"SELECT AnswerMedias FROM EtActiveHomeworkDetail WHERE HomeworkId = {homeworkId} AND TenantId = {_tenantId} AND AnswerStatus = {EmActiveHomeworkDetailAnswerStatus.Answered}");
        }

        public async Task<Tuple<IEnumerable<MicroWebColumnArticleView>, int>> GetMicroWebColumnArticlePaging(long columnId, int pageSize, int pageCurrent)
        {
            return await _dbWrapper.ExecutePage<MicroWebColumnArticleView>(
                "EtMicroWebColumnArticle", "ArCoverImg", pageSize, pageCurrent, "Id DESC", $" TenantId = {_tenantId} AND ColumnId = {columnId} ");
        }
    }
}
