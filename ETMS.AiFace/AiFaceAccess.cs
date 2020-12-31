using ETMS.IAiFace;
using ETMS.LOG;
using System;
using System.Collections.Generic;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Iai.V20200303;
using TencentCloud.Iai.V20200303.Models;

namespace ETMS.AiFace
{
    public class AiFaceAccess : IAiFaceAccess
    {
        private int _tenantId;

        private string _secretId;

        private string _secretKey;

        private string _endpoint;

        private string _region;

        private string _tenantGroupId;

        private IaiClient _client;

        public void InitTencentCloudConfig(int tenantId, string secretId, string secretKey, string endpoint, string region)
        {
            this._tenantId = tenantId;
            this._secretId = secretId;
            this._secretKey = secretKey;
            if (!string.IsNullOrEmpty(endpoint))
            {
                this._endpoint = endpoint;
            }
            else
            {
                this._endpoint = "iai.tencentcloudapi.com";
            }
            if (!string.IsNullOrEmpty(region))
            {
                this._region = region;
            }
            else
            {
                this._region = "ap-beijing";
            }
            this._tenantGroupId = $"tenant_{tenantId}";
            var cred = new Credential
            {
                SecretId = _secretId,
                SecretKey = _secretKey
            };
            var clientProfile = new ClientProfile();
            var httpProfile = new HttpProfile();
            httpProfile.Endpoint = _endpoint;
            clientProfile.HttpProfile = httpProfile;
            this._client = new IaiClient(cred, _region, clientProfile);
            this.InitTenantGroup();
        }

        private string GetPersonId(long studentId)
        {
            return $"{_tenantId}_{studentId}";
        }

        private long GetStudentId(string personId)
        {
            var tempInfo = personId.Split('_');
            return Convert.ToInt64(tempInfo[1]);
        }

        /// <summary>
        /// 判断机构人脸库是否存在
        /// </summary>
        /// <returns></returns>
        private bool ExistTenantGroup()
        {
            try
            {
                var req = new GetGroupInfoRequest();
                req.GroupId = _tenantGroupId;
                _client.GetGroupInfoSync(req);
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal($"[腾讯云人脸识别GetGroupInfoSync]人员库信息不存在,{_tenantId},{_tenantGroupId}", ex, this.GetType());
                return false;
            }
        }

        /// <summary>
        /// 初始化机构人员库
        /// 返回值：是否为刚初始化的人员库
        /// </summary>
        private bool InitTenantGroup()
        {
            if (!ExistTenantGroup())
            {
                Log.Info($"[腾讯云人脸识别InitTenantGroup]创建人员库,{_tenantGroupId}", this.GetType());
                var req = new CreateGroupRequest();
                req.GroupName = _tenantGroupId;
                req.GroupId = _tenantGroupId;
                _client.CreateGroupSync(req);
                return true;
            }
            return false;
        }

        public void StudentDel(long studentId)
        {
            try
            {
                var req = new DeletePersonRequest();
                req.PersonId = GetPersonId(studentId);
                _client.DeletePersonSync(req);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[腾讯云人脸识别DeletePersonSync]删除人员信息，{_tenantId}，{studentId}", ex, this.GetType());
            }
        }

        private GetPersonBaseInfoResponse StudentGetPersonInfo(long studentId)
        {
            try
            {
                var req = new GetPersonBaseInfoRequest();
                req.PersonId = GetPersonId(studentId);
                return _client.GetPersonBaseInfoSync(req);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[腾讯云人脸识别GetPersonBaseInfo]获取人员信息，{_tenantId}，{studentId}", ex, this.GetType());
                return null;
            }
        }

        private void StudentDelFaceIds(long studentId, string[] faceIds)
        {
            try
            {
                var req = new DeleteFaceRequest();
                req.FaceIds = faceIds;
                req.PersonId = GetPersonId(studentId);
                _client.DeleteFaceSync(req);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[腾讯云人脸识别DeleteFaceSync]删除人脸信息，{_tenantId}，{studentId}", ex, this.GetType());
            }
        }

        private void StudentCreate(long studentId, string faceGreyKeyUrl)
        {
            var req = new CreatePersonRequest();
            var personId = GetPersonId(studentId);
            req.GroupId = _tenantGroupId;
            req.PersonName = personId;
            req.PersonId = personId;
            req.Url = faceGreyKeyUrl;
            req.QualityControl = 4;
            _client.CreatePersonSync(req);
        }

        public bool StudentInitFace(long studentId, string faceGreyKeyUrl)
        {
            try
            {
                var personBaseInfo = StudentGetPersonInfo(studentId);
                if (personBaseInfo != null)//删除旧的人员  增加新的人脸
                {
                    //if (personBaseInfo.FaceIds != null && personBaseInfo.FaceIds.Length > 1)
                    //{
                    //    var dyFace = new List<string>();
                    //    for (var i = 1; i < personBaseInfo.FaceIds.Length; i++)
                    //    {
                    //        dyFace.Add(personBaseInfo.FaceIds[i]);
                    //    }
                    //    StudentDelFaceIds(studentId, dyFace.ToArray());
                    //}
                    //StudentAddFace(studentId, faceGreyKeyUrl);
                    StudentDel(studentId);
                }
                //创建人员 并设置人脸
                StudentCreate(studentId, faceGreyKeyUrl);
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal($"[腾讯云人脸识别]人脸采集，{_tenantId}，{studentId},{faceGreyKeyUrl}", ex, this.GetType());
                return false;
            }
        }

        public bool StudentClearFace(long studentId)
        {
            try
            {
                var personBaseInfo = StudentGetPersonInfo(studentId);
                if (personBaseInfo != null)
                {
                    StudentDel(studentId);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal($"[腾讯云人脸识别]删除人脸，{_tenantId}，{studentId}", ex, this.GetType());
                return false;
            }
        }

        public long SearchPerson(string imageBase64)
        {
            try
            {
                var req = new SearchPersonsRequest();
                req.GroupIds = new string[] { _tenantGroupId };
                req.Image = imageBase64;
                var resp = _client.SearchPersonsSync(req);
                var myCandidates = resp.Results[0].Candidates[0];
                LOG.Log.Debug($"[腾讯云人脸识别]人脸搜索结果:PersonId:{myCandidates.PersonId},Score:{myCandidates.Score}", this.GetType());
                if (myCandidates.Score != null && myCandidates.Score.Value >= 80)
                {
                    return GetStudentId(myCandidates.PersonId);
                }
                else
                {
                    Log.Fatal($"[腾讯云人脸识别]人脸搜索结果,分数太低:{Newtonsoft.Json.JsonConvert.SerializeObject(myCandidates)}", this.GetType());
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal($"[腾讯云人脸识别]人脸搜索失败，{_tenantId}", ex, this.GetType());
                return 0;
            }
        }
    }
}
