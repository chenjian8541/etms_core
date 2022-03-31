using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Utility
{
    /// <summary>
    /// 获取阿里云OSS临时授权
    /// https://help.aliyun.com/document_detail/112718.html?spm=a2c4g.11174283.6.1746.6e267da2pjoSTh
    /// https://help.aliyun.com/document_detail/100624.htm?spm=a2c4g.11186623.2.5.f23bca85eauXGV#title-884-tmc-thk
    /// </summary>
    public static class AliyunOssSTSUtil
    {
        public static string STSAccessKeyId { get; set; }

        public static string STSAccessKeySecret { get; set; }

        public static string STSRoleArn { get; set; }

        public static string STSRegion { get; set; } = "oss-cn-beijing";

        public static string STSEndpoint { get; set; }

        public static STSAccessTokenRes GetSTSAccessToken(int tenantId)
        {
            var config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                AccessKeyId = STSAccessKeyId,
                AccessKeySecret = STSAccessKeySecret,
            };
            config.Endpoint = STSEndpoint;
            var assumeRoleRequest = new AlibabaCloud.SDK.Sts20150401.Models.AssumeRoleRequest
            {
                DurationSeconds = 3000,
                Policy = "{ \"Version\": \"1\", \"Statement\": [  {    \"Effect\": \"Allow\",        \"Action\": [          \"oss:PutObject\"        ],    \"Resource\": [      \"acs:oss:*:*:*\"    ]  } ] }",
                RoleArn = STSRoleArn,
                RoleSessionName = $"etms_xiaohebang_{tenantId}"
            };
            var client = new AlibabaCloud.SDK.Sts20150401.Client(config);
            var res = client.AssumeRole(assumeRoleRequest);
            return new STSAccessTokenRes()
            {
                RequestId = res.Body.RequestId,
                AssumedRoleUser = new STSAccessTokenAssumedRoleUserRes()
                {
                    Arn = res.Body.AssumedRoleUser.Arn,
                    AssumedRoleId = res.Body.AssumedRoleUser.AssumedRoleId
                },
                Credentials = new STSAccessTokenCredentialsRes()
                {
                    AccessKeyId = res.Body.Credentials.AccessKeyId,
                    AccessKeySecret = res.Body.Credentials.AccessKeySecret,
                    Expiration = Convert.ToDateTime(res.Body.Credentials.Expiration).ToBeijingTime(),
                    SecurityToken = res.Body.Credentials.SecurityToken
                }
            };
        }

        public static STSAccessTokenRes GetSTSAccessToken2(int headId)
        {
            var config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                AccessKeyId = STSAccessKeyId,
                AccessKeySecret = STSAccessKeySecret,
            };
            config.Endpoint = STSEndpoint;
            var assumeRoleRequest = new AlibabaCloud.SDK.Sts20150401.Models.AssumeRoleRequest
            {
                DurationSeconds = 3000,
                Policy = "{ \"Version\": \"1\", \"Statement\": [  {    \"Effect\": \"Allow\",        \"Action\": [          \"oss:PutObject\"        ],    \"Resource\": [      \"acs:oss:*:*:*\"    ]  } ] }",
                RoleArn = STSRoleArn,
                RoleSessionName = $"etms_xiaohebang_head_{headId}"
            };
            var client = new AlibabaCloud.SDK.Sts20150401.Client(config);
            var res = client.AssumeRole(assumeRoleRequest);
            return new STSAccessTokenRes()
            {
                RequestId = res.Body.RequestId,
                AssumedRoleUser = new STSAccessTokenAssumedRoleUserRes()
                {
                    Arn = res.Body.AssumedRoleUser.Arn,
                    AssumedRoleId = res.Body.AssumedRoleUser.AssumedRoleId
                },
                Credentials = new STSAccessTokenCredentialsRes()
                {
                    AccessKeyId = res.Body.Credentials.AccessKeyId,
                    AccessKeySecret = res.Body.Credentials.AccessKeySecret,
                    Expiration = Convert.ToDateTime(res.Body.Credentials.Expiration).ToBeijingTime(),
                    SecurityToken = res.Body.Credentials.SecurityToken
                }
            };
        }

        public static void InitAliyunSTSConfig(string stsAccessKeyId, string stsAccessKeySecret, string stsRoleArn,
            string endpoint)
        {
            STSAccessKeyId = stsAccessKeyId;
            STSAccessKeySecret = stsAccessKeySecret;
            STSRoleArn = stsRoleArn;
            STSEndpoint = endpoint;
        }
    }

    public class STSAccessTokenRes
    {
        public string RequestId { get; set; }

        public STSAccessTokenAssumedRoleUserRes AssumedRoleUser { get; set; }

        public STSAccessTokenCredentialsRes Credentials { get; set; }
    }

    public class STSAccessTokenAssumedRoleUserRes
    {
        public string Arn { get; set; }

        public string AssumedRoleId { get; set; }
    }

    public class STSAccessTokenCredentialsRes
    {
        public string SecurityToken { get; set; }

        public string AccessKeyId { get; set; }

        public string AccessKeySecret { get; set; }

        public DateTime Expiration { get; set; }
    }
}
