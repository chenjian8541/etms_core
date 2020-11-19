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
        private static string BucketName { get; set; }

        /// <summary>
        /// AccessKeyId
        /// </summary>
        private static string AccessKeyId { get; set; }

        /// <summary>
        /// AccessKeySecret
        /// </summary>
        private static string AccessKeySecret { get; set; }

        /// <summary>
        /// Endpoint
        /// </summary>
        private static string Endpoint { get; set; }

        /// <summary>
        /// 外网访问地址(http)
        /// </summary>
        private static string OssAccessUrlHttp { get; set; }

        /// <summary>
        /// 外网访问地址(https)
        /// </summary>
        private static string OssAccessUrlHttps { get; set; }

        /// <summary>
        /// 根文件夹
        /// </summary>
        private static string RootFolder { get; set; }

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

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteObject(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return;
                }
                var client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
                client.DeleteObject(BucketName, key);
            }
            catch (Exception ex)
            {
                Log.Error($"【删除OSS文件出错】==> 参数：key：{key}", ex, typeof(AliyunOssUtil));
            }
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
    }
}
