using ETMS.Entity.Alien.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.SysCom.Output;
using ETMS.IBusiness.Alien;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public class AlienBascBLL : IAlienBascBLL
    {
        public void InitHeadId(int headId)
        {
        }

        public ResponseBase UploadConfigGet(AlienRequestBase request)
        {
            var aliyunOssSTS = AliyunOssSTSUtil.GetSTSAccessToken2(request.LoginHeadId);
            return ResponseBase.Success(new UploadConfigGetOutput()
            {
                AccessKeyId = aliyunOssSTS.Credentials.AccessKeyId,
                AccessKeySecret = aliyunOssSTS.Credentials.AccessKeySecret,
                Bucket = AliyunOssUtil.BucketName,
                Region = AliyunOssSTSUtil.STSRegion,
                Basckey = AliyunOssUtil.GetBascKeyPrefix(request.LoginHeadId, AliyunOssFileTypeEnum.Alien),
                ExTime = aliyunOssSTS.Credentials.Expiration.AddMinutes(-5),
                BascAccessUrlHttps = AliyunOssUtil.OssAccessUrlHttps,
                SecurityToken = aliyunOssSTS.Credentials.SecurityToken
            });
        }
    }
}
