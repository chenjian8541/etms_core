using Aliyun.OSS;
using ETMS.LOG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace ETMS.Utility
{
    public class AliyunOssCall
    {
        private OssClient client;
        public AliyunOssCall()
        {
            client = new OssClient(AliyunOssUtil.Endpoint, AliyunOssUtil.AccessKeyId, AliyunOssUtil.AccessKeySecret);
        }

        //public event Action<List<AliyunOssObjectView>> FinishGetEvent;

        public decimal Statistics(string prefix)
        {
            var nextMarker = string.Empty;
            ObjectListing result = null;
            var totalStorage = 0M;
            decimal unitCvtMB = 1048576;
            decimal itemStorage = 0M;
            do
            {
                //var aliyunOssObjects = new List<AliyunOssObjectView>();
                var listObjectsRequest = new ListObjectsRequest(AliyunOssUtil.BucketName)
                {
                    Marker = nextMarker,
                    MaxKeys = 500,
                    Prefix = prefix,
                };
                result = client.ListObjects(listObjectsRequest);
                foreach (var summary in result.ObjectSummaries)
                {
                    itemStorage = summary.Size / unitCvtMB;
                    totalStorage += itemStorage;
                    //aliyunOssObjects.Add(new AliyunOssObjectView()
                    //{
                    //    Key = summary.Key,
                    //    LastModified = summary.LastModified,
                    //    ValueMB = itemStorage
                    //});
                }
                //if (aliyunOssObjects.Any())
                //{
                //    FinishGetEvent(aliyunOssObjects);
                //}
                nextMarker = result.NextMarker;
            } while (result.IsTruncated);
            return totalStorage;
        }

        public event Action<string> FinishEachFile;

        public void ProcessEachFile(string prefix)
        {
            var nextMarker = string.Empty;
            ObjectListing result = null;
            do
            {
                var listObjectsRequest = new ListObjectsRequest(AliyunOssUtil.BucketName)
                {
                    Marker = nextMarker,
                    MaxKeys = 500,
                    Prefix = prefix,
                };
                result = client.ListObjects(listObjectsRequest);
                foreach (var summary in result.ObjectSummaries)
                {
                    FinishEachFile(summary.Key);
                }
                nextMarker = result.NextMarker;
            } while (result.IsTruncated);
        }

        public void DelObject(string key)
        {
            try
            {
                client.DeleteObject(AliyunOssUtil.BucketName, key);
            }
            catch (Exception ex)
            {
                Log.Error($"【定时删除OSS文件出错】==> 参数：key：{key}", ex, typeof(AliyunOssCall));
            }
        }
    }
}
