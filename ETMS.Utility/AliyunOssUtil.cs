using Aliyun.OSS;
using ETMS.LOG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ETMS.Utility
{
    public static class AliyunOssUtil
    {
        /// <summary>
        /// Bucket名称
        /// </summary>
        public static string BucketName { get; set; }

        /// <summary>
        /// AccessKeyId
        /// </summary>
        internal static string AccessKeyId { get; set; }

        /// <summary>
        /// AccessKeySecret
        /// </summary>
        internal static string AccessKeySecret { get; set; }

        /// <summary>
        /// Endpoint
        /// </summary>
        internal static string Endpoint { get; set; }

        /// <summary>
        /// 外网访问地址(http)
        /// </summary>
        internal static string OssAccessUrlHttp { get; set; }

        /// <summary>
        /// 外网访问地址(https)
        /// </summary>
        public static string OssAccessUrlHttps { get; set; }

        /// <summary>
        /// 根文件夹
        /// </summary>
        public static string RootFolder { get; set; }

        /// <summary>
        /// 临时文件
        /// </summary>
        public const string TempFolder = "temporary";

        /// <summary>
        /// 固定的正式环境 根文件夹
        /// </summary>
        public const string RootFolderProd = "etms_prod";

        public static string GetBascKeyPrefix(int tenantId, string fileType)
        {
            return $"{RootFolder}/{tenantId}/{fileType}/";
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="key"></param>
        /// <param name="fileType"><see cref="AliyunOssFileTypeEnum"/></param>
        /// <param name="content"></param>
        public static string PutObject(int tenantId, string key, string fileType, Stream content)
        {
            key = $"{RootFolder}/{tenantId}/{fileType}/{key}";
            var client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
            client.PutObject(BucketName, key, content);
            return key;
        }

        /// <summary>
        /// 上传临时文件
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="key"></param>
        /// <param name="fileType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string PutObjectTemp(int tenantId, string key, string fileType, Stream content)
        {
            key = $"{RootFolder}/{TempFolder}/{fileType}/{tenantId}/{key}";
            var client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
            client.PutObject(BucketName, key, content);
            return key;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="key"></param>
        /// <param name="fileType"></param>
        /// <param name="bytes"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string PutObject(int tenantId, string key, string fileType, byte[] bytes, int count)
        {
            using (var cutContent = new MemoryStream(bytes, 0, count))
            {
                return PutObject(tenantId, key, fileType, cutContent);
            }
        }

        public static void PutObject2(string fullKey, byte[] bytes, int count)
        {
            using (var cutContent = new MemoryStream(bytes, 0, count))
            {
                var client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
                client.PutObject(BucketName, fullKey, cutContent);
            }
        }

        /// <summary>
        /// 上传临时文件
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="key"></param>
        /// <param name="fileType"></param>
        /// <param name="bytes"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string PutObjectTemp(int tenantId, string key, string fileType, byte[] bytes, int count)
        {
            using (var cutContent = new MemoryStream(bytes, 0, count))
            {
                return PutObjectTemp(tenantId, key, fileType, cutContent);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteObject(params string[] keys)
        {
            if (keys == null || keys.Length == 0)
            {
                return;
            }
            foreach (var key in keys)
            {
                try
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }
                    var client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
                    client.DeleteObject(BucketName, key);
                }
                catch (Exception ex)
                {
                    Log.Error($"【删除OSS文件出错】==> 参数：key：{key}", ex, typeof(AliyunOssUtil));
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="medias"></param>
        public static void DeleteObject2(string medias)
        {
            LOG.Log.Debug($"oss删除:{medias}", typeof(AliyunOssUtil));
            var myMedias = medias.Split('|');
            if (myMedias.Length == 0)
            {
                return;
            }
            DeleteObject(myMedias);
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Stream GetObject(string key)
        {
            var client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
            return client.GetObject(BucketName, key).Content;
        }

        public static void DelTenant(int tenantId)
        {
            var id = $"{RootFolderProd}_{tenantId}";
            var prefix = $"{RootFolderProd}/{tenantId}/";
            AddBucketLifecycle(id, prefix, 1);
        }

        #region 生命周期管理

        /// <summary>
        /// 添加生命周期规则
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prefix"></param>
        /// <param name="expriationDays"></param>
        private static void AddBucketLifecycle(string id, string prefix, int expriationDays)
        {
            var client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
            var rules = client.GetBucketLifecycle(BucketName);
            try
            {
                rules.Add(new LifecycleRule()
                {
                    ID = id,
                    Prefix = prefix,
                    Status = RuleStatus.Enabled,
                    ExpriationDays = expriationDays
                });
                var setBucketLifecycleRequest = new SetBucketLifecycleRequest(BucketName);
                setBucketLifecycleRequest.LifecycleRules = rules;
                client.SetBucketLifecycle(setBucketLifecycleRequest);
            }
            catch (Exception ex)
            {
                Log.Error($"【OSS添加生命周期规则出错】==> 参数：prefix：{prefix}", ex, typeof(AliyunOssUtil));
            }
        }

        /// <summary>
        /// 设置生命周期
        /// </summary>
        public static void InitBucketLifecycle()
        {
            var setBucketLifecycleRequest = new SetBucketLifecycleRequest(BucketName);
            //学员考勤照片 保留七天
            var myFaceStudentCheckOnId = $"{RootFolderProd}_{TempFolder}_{AliyunOssTempFileTypeEnum.FaceStudentCheckOn}";
            setBucketLifecycleRequest.AddLifecycleRule(new LifecycleRule()
            {
                ID = myFaceStudentCheckOnId,
                Prefix = $"{RootFolderProd}/{TempFolder}/{AliyunOssTempFileTypeEnum.FaceStudentCheckOn}/",
                Status = RuleStatus.Enabled,
                ExpriationDays = 7
            });
            //学员人脸考勤黑名单 保留2天
            var myFaceBlacklistId = $"{RootFolderProd}_{TempFolder}_{AliyunOssTempFileTypeEnum.FaceBlacklist}";
            setBucketLifecycleRequest.AddLifecycleRule(new LifecycleRule()
            {
                ID = myFaceBlacklistId,
                Prefix = $"{RootFolderProd}/{TempFolder}/{AliyunOssTempFileTypeEnum.FaceBlacklist}/",
                Status = RuleStatus.Enabled,
                ExpriationDays = 2
            });
            var client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
            client.SetBucketLifecycle(setBucketLifecycleRequest);
        }

        #endregion

        /// <summary>
        /// 通过KEY获取外网访问地址（http）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAccessUrlHttp(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            return $"{OssAccessUrlHttp}/{key}";
        }

        /// <summary>
        /// 通过KEY获取外网访问地址（https）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAccessUrlHttps(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            return $"{OssAccessUrlHttps}/{key}";
        }

        /// <summary>
        /// 通过访问地址获取key值
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetKeyByUrl(string url)
        {
            if (url.IndexOf(OssAccessUrlHttps) != -1)
            {
                return url.Replace($"{OssAccessUrlHttps}/", "");
            }
            if (url.IndexOf(OssAccessUrlHttp) != -1)
            {
                return url.Replace($"{OssAccessUrlHttp}/", "");
            }
            return url;
        }

        /// <summary>
        /// 判断是否是OSSkey  
        /// 判断依据：key中是否存在oss RootFolder字符串（不太严谨）
        /// </summary>
        /// <param name="key"></param>
        public static bool IsOssKey(string key)
        {
            return key.IndexOf(RootFolder) != -1;
        }

        /// <summary>
        /// 获取一个新的文件名
        /// </summary>
        /// <returns></returns>
        public static string GetOneNewFileName()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// 初始化OSS配置
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="accessKeyId"></param>
        /// <param name="accessKeySecret"></param>
        /// <param name="endpoint"></param>
        /// <param name="ossAccessUrlHttp"></param>
        /// <param name="ossAccessUrlHttps"></param>
        /// <param name="rootFolder"></param>
        public static void InitAliyunOssConfig(
            string bucketName,
            string accessKeyId,
            string accessKeySecret,
            string endpoint,
            string ossAccessUrlHttp,
            string ossAccessUrlHttps,
            string rootFolder)
        {
            BucketName = bucketName;
            AccessKeyId = accessKeyId;
            AccessKeySecret = accessKeySecret;
            Endpoint = endpoint;
            OssAccessUrlHttp = ossAccessUrlHttp;
            OssAccessUrlHttps = ossAccessUrlHttps;
            RootFolder = rootFolder;
        }
    }

    /// <summary>
    /// 阿里云OSS 存储文件类型
    /// </summary>
    public struct AliyunOssFileTypeEnum
    {
        /// <summary>
        /// 图片
        /// </summary>
        public const string Image = "image";

        /// <summary>
        /// Audio
        /// </summary>
        public const string Audio = "audio";

        /// <summary>
        /// Video
        /// </summary>
        public const string Video = "video";

        /// <summary>
        /// sts临时授权上传
        /// </summary>
        public const string STS = "sts";

        /// <summary>
        /// 学生人脸
        /// </summary>
        public const string ImageStudentFace = "image_s_face";

        /// <summary>
        /// 电子相册相关
        /// </summary>
        public const string AlbumLb = "album";
    }

    /// <summary>
    /// 临时图片文件夹
    /// </summary>
    public struct AliyunOssTempFileTypeEnum
    {
        /// <summary>
        /// 学员人脸考勤黑名单 
        /// 保留2天
        /// </summary>
        public const string FaceBlacklist = "face_b_lst";

        /// <summary>
        /// 学员考勤照片
        /// 保留7天
        /// </summary>
        public const string FaceStudentCheckOn = "fase_check";
    }
}
