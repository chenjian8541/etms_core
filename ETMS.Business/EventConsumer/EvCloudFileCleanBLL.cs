using ETMS.Entity.Temp.CloudFileClean;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess.SysOp;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvCloudFileCleanBLL : IEvCloudFileCleanBLL
    {
        private readonly ICloudFileCleanDAL _cloudFileCleanDAL;

        public EvCloudFileCleanBLL(ICloudFileCleanDAL cloudFileCleanDAL)
        {
            this._cloudFileCleanDAL = cloudFileCleanDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _cloudFileCleanDAL);
        }

        public async Task CloudFileDelConsumerEvent(CloudFileDelEvent request)
        {
            switch (request.SceneType)
            {
                case CloudFileScenes.ActiveHomework:
                    await CleanActiveHomework(request);
                    break;
                case CloudFileScenes.MicroWebColumnArticle:
                    await CleanMicroWebColumnArticle(request);
                    break;
            }
        }

        private async Task CleanMicroWebColumnArticle(CloudFileDelEvent request)
        {
            var pageSize = 200;
            var pageCurrent = 1;
            var pagingData = await _cloudFileCleanDAL.GetMicroWebColumnArticlePaging(request.RelatedId, pageSize, pageCurrent);
            if (pagingData.Item2 == 0)
            {
                return;
            }
            ProcessMicroWebColumnArticle(pagingData.Item1);
            var totalPage = EtmsHelper.GetTotalPage(pagingData.Item2, pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                pagingData = await _cloudFileCleanDAL.GetMicroWebColumnArticlePaging(request.RelatedId, pageSize, pageCurrent);
                ProcessMicroWebColumnArticle(pagingData.Item1);
                pageCurrent++;
            }
        }

        private void ProcessMicroWebColumnArticle(IEnumerable<MicroWebColumnArticleView> datas)
        {
            foreach (var p in datas)
            {
                AliyunOssUtil.DeleteObject(p.ArCoverImg);
            }
        }

        private async Task CleanActiveHomework(CloudFileDelEvent request)
        {
            var homeworkDetailList = await _cloudFileCleanDAL.GetActiveHomeworkDetail(request.RelatedId);
            if (!homeworkDetailList.Any())
            {
                return;
            }
            foreach (var p in homeworkDetailList)
            {
                AliyunOssUtil.DeleteObject2(p.AnswerMedias);
            }
        }
    }
}
