using ETMS.Entity.Config;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IBusiness.MicroWeb;
using ETMS.IDataAccess.SysOp;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class CloudFileAutoDelDayBLL : ICloudFileAutoDelDayBLL
    {
        private readonly ICloudFileCleanDAL _cloudFileCleanDAL;

        private readonly IMicroWebConfigBLL _microWebConfigBLL;

        public CloudFileAutoDelDayBLL(ICloudFileCleanDAL cloudFileCleanDAL, IMicroWebConfigBLL microWebConfigBLL)
        {
            this._cloudFileCleanDAL = cloudFileCleanDAL;
            this._microWebConfigBLL = microWebConfigBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this._microWebConfigBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _cloudFileCleanDAL);
        }

        public async Task CloudFileAutoDelDayConsumerEvent(CloudFileAutoDelDayEvent request)
        {
            var itemPrefix = EmTenantCloudStorageType.GetOssKeyPrefix(request.FileTag, AliyunOssUtil.RootFolder, request.TenantId, request.DelDate);
            switch (request.FileTag)
            {
                case EmTenantCloudStorageType.sysConfig:
                    ProcessSysConfig(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentAvatar:
                    ProcessStudentAvatar(itemPrefix); ;
                    break;
                case EmTenantCloudStorageType.userAvatar:
                    ProcessUserAvatar(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentTrack:
                    ProcessStudentTrack(itemPrefix);
                    break;
                case EmTenantCloudStorageType.evaluateStudent:
                    ProcessEvaluateStudent(itemPrefix);
                    break;
                case EmTenantCloudStorageType.gift:
                    ProcessGift(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentLeave:
                    ProcessStudentLeave(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentNotice:
                    ProcessStudentNotice(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentHomework:
                    ProcessStudentHomework(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentHomeworkAnswer:
                    ProcessStudentHomeworkAnswer(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentGrowthRecord:
                    ProcessStudentGrowthRecord(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentWxMessage:
                    ProcessStudentWxMessage(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentcheckOnLog:
                    ProcessStudentcheckOnLog(itemPrefix);
                    break;
                case EmTenantCloudStorageType.micwebColumn:
                    ProcessMicwebColumn(itemPrefix);
                    break;
                case EmTenantCloudStorageType.micwebArticle:
                    ProcessMicwebArticle(itemPrefix);
                    break;
                case EmTenantCloudStorageType.mallGoods:
                    ProcessMallGoods(itemPrefix);
                    break;
                case EmTenantCloudStorageType.shareTemplate:
                    ProcessShareTemplate(itemPrefix);
                    break;
                case EmTenantCloudStorageType.album:
                    ProcessAlbum(itemPrefix);
                    break;
                case EmTenantCloudStorageType.agtPay:
                    ProcessAgtPay(itemPrefix);
                    break;
                case EmTenantCloudStorageType.studentFaceKey:
                    ProcessStudentFaceKey(itemPrefix);
                    break;
                case EmTenantCloudStorageType.mallGoodsContent:
                    ProcessMallGoodsContent(itemPrefix);
                    break;
                case EmTenantCloudStorageType.micwebArticleContent:
                    ProcessMicwebArticleContent(itemPrefix);
                    break;
                case EmTenantCloudStorageType.albumImg:
                    ProcessAlbumImg(itemPrefix);
                    break;
                case EmTenantCloudStorageType.albumAudio:
                    ProcessAlbumAudio(itemPrefix);
                    break;
                case EmTenantCloudStorageType.appConfig:
                    ProcessAppConfig(itemPrefix);
                    break;
                case EmTenantCloudStorageType.microWebConfig:
                    ProcessMicroWebConfig(itemPrefix);
                    break;
            }
        }

        private void ProcessSysConfig(string itemPrefix)
        { }

        /// <summary>
        /// 学员头像
        /// </summary>
        /// <param name="request"></param>
        /// <param name="itemPrefix"></param>
        /// <returns></returns>
        private void ProcessStudentAvatar(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistStudentAvatar(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);

        }

        /// <summary>
        /// 员工头像
        /// </summary>
        /// <param name="request"></param>
        /// <param name="itemPrefix"></param>
        /// <returns></returns>
        private void ProcessUserAvatar(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistUserAvatar(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        /// <summary>
        /// 学员跟进图片
        /// </summary>
        /// <param name="itemPrefix"></param>
        private void ProcessStudentTrack(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistStudentTrack(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        /// <summary>
        /// 课后点评图片
        /// </summary>
        /// <param name="request"></param>
        /// <param name="itemPrefix"></param>
        /// <returns></returns>
        private void ProcessEvaluateStudent(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistEvaluateStudent(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        /// <summary>
        /// 礼品
        /// </summary>
        /// <param name="itemPrefix"></param>
        /// <returns></returns>
        private void ProcessGift(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistGift(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        /// <summary>
        ///学员请假
        /// </summary>
        /// <param name="itemPrefix"></param>
        private void ProcessStudentLeave(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistStudentLeave(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessStudentNotice(string itemPrefix)
        {

        }

        private void ProcessStudentHomework(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistStudentHomework(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessStudentHomeworkAnswer(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistStudentHomeworkAnswer(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessStudentGrowthRecord(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistActiveGrowthRecord(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessStudentWxMessage(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistActiveWxMessage(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessStudentcheckOnLog(string itemPrefix)
        {

        }

        private void ProcessMicwebColumn(string itemPrefix)
        {
            var allDefault = _microWebConfigBLL.MicroWebDefaultColumnGet().Result;
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistMicroWebColumn(fileKey).Result;
                if (!isExist)
                {
                    if (allDefault.Any() && allDefault.Exists(j => j.ShowInMenuIcon == fileKey))
                    {
                        return;
                    }
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessMicwebArticle(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistMicroWebColumnArticle(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessMicwebArticleContent(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistMicroWebColumnArticleContent(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessMallGoods(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistMallGoods(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessMallGoodsContent(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistMallGoodsContent(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessShareTemplate(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistShareTemplate(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessAlbum(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistAlbum(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessAlbumImg(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistAlbumImg(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessAlbumAudio(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistAlbumAudio(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessAgtPay(string itemPrefix)
        {
        }

        private void ProcessStudentFaceKey(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistFaceKey(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessAppConfig(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistAppConfig(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }

        private void ProcessMicroWebConfig(string itemPrefix)
        {
            var aliyunOssCall = new AliyunOssCall();
            aliyunOssCall.FinishEachFile += (fileKey) =>
            {
                var isExist = _cloudFileCleanDAL.ExistMicroWebConfig(fileKey).Result;
                if (!isExist)
                {
                    aliyunOssCall.DelObject(fileKey);
                }
            };
            aliyunOssCall.ProcessEachFile(itemPrefix);
        }
    }
}
