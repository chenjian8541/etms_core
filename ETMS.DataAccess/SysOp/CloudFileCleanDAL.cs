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

        public async Task<bool> ExistStudentAvatar(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Avatar = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistUserAvatar(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtUser WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Avatar = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistStudentTrack(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtStudentTrackLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND TrackImg LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistEvaluateStudent(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtClassRecordEvaluateStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND EvaluateImg LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistGift(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtGift WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ImgPath LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistStudentLeave(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtStudentLeaveApplyLog WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND LeaveMedias LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistActiveWxMessage(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtActiveWxMessage WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND MessageContent LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistStudentHomework(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtActiveHomework WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND WorkMedias LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistStudentHomeworkAnswer(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtActiveHomeworkDetail WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND AnswerMedias LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistActiveGrowthRecord(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtActiveGrowthRecord WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND GrowthMedias LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistMicroWebColumn(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtMicroWebColumn WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ShowInMenuIcon = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistMicroWebColumnArticle(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtMicroWebColumnArticle WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ArCoverImg = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistMicroWebColumnArticleContent(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtMicroWebColumnArticle WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ArContent LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistMallGoods(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtMallGoods WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ImgCover = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistMallGoodsContent(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtMallGoods WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND GsContent LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistShareTemplate(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtShareTemplate WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ImgKey = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistAlbum(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtElectronicAlbum WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND (CoverKey = '{key}' OR RenderKey = '{key}')");
            return obj != null;
        }

        public async Task<bool> ExistAlbumImg(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtLibImages WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ImgKey = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistAlbumAudio(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtLibAudios WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND AudioKey = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistFaceKey(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND FaceKey = '{key}'");
            return obj != null;
        }

        public async Task<bool> ExistAppConfig(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtAppConfig WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ConfigValue LIKE '%{key}%'");
            return obj != null;
        }

        public async Task<bool> ExistMicroWebConfig(string key)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtMicroWebConfig WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ConfigValue LIKE '%{key}%'");
            return obj != null;
        }
    }
}
