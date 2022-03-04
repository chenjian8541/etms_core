using ETMS.AiFace.Common;
using ETMS.AiFace.Dto.Baidu.Output;
using ETMS.Entity.CacheBucket.Other;
using ETMS.Entity.Config;
using ETMS.IAiFace;
using ETMS.IDataAccess;
using ETMS.LOG;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.AiFace
{
    public class AiBaiduFaceAccess : IAiBaiduFaceAccess
    {
        private int _tenantId;

        private string _appid;

        private string _apiKey;

        private string _secretKey;

        private string _access_token;

        private string _tenantGroupId;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly CloudBaiduConfig _cloudBaiduConfig;

        public AiBaiduFaceAccess(ITempDataCacheDAL tempDataCacheDAL, IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._cloudBaiduConfig = appConfigurtaionServices.AppSettings.CloudBaiduConfig;
        }

        private string GetUserId(long studentId)
        {
            return $"{_tenantId}_{studentId}";
        }

        private long GetStudentId(string userId)
        {
            var tempInfo = userId.Split('_');
            return Convert.ToInt64(tempInfo[1]);
        }

        public void InitBaiduCloudConfig(int tenantId, string appid, string apiKey, string secretKey)
        {
            this._tenantId = tenantId;
            this._appid = appid;
            this._apiKey = apiKey;
            this._secretKey = secretKey;
            this._tenantGroupId = $"tenant_{tenantId}";
            this.InitAccessToken();
        }

        private void InitAccessToken()
        {
            var now = DateTime.Now;
            var bucket = _tempDataCacheDAL.GetBaiduCloudAccessTokenBucket(_appid);
            if (bucket != null && bucket.ExTime > now)
            {
                this._access_token = bucket.AccessToken;
            }
            else
            {
                var tokenResult = this.AiBaiduGetAccessToken();
                this._access_token = tokenResult.access_token;
                var exTime = now.AddSeconds(tokenResult.expires_in / 2);
                _tempDataCacheDAL.SetBaiduCloudAccessTokenBucket(new BaiduCloudAccessTokenBucket()
                {
                    AccessToken = tokenResult.access_token,
                    Appid = this._appid,
                    ExTime = exTime
                });
            }
            this.AiBaiduInitGroup();
        }

        private TokenOutput AiBaiduGetAccessToken()
        {
            var paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", this._apiKey));
            paraList.Add(new KeyValuePair<string, string>("client_secret", this._secretKey));
            return HttpLib.BaiduGetTokenPost(_cloudBaiduConfig.Token, paraList);
        }

        private void AiBaiduInitGroup()
        {
            var paraList = new
            {
                group_id = this._tenantGroupId
            };
            try
            {
                HttpLib.BaiduAPISendPost<OutputBase<ComOutput>>(_cloudBaiduConfig.FaceGroupAdd, paraList, this._access_token);
            }
            catch
            {
            }
        }

        public void StudentDel(long studentId)
        {
            var paraList = new
            {
                group_id = this._tenantGroupId,
                user_id = this.GetUserId(studentId)
            };
            try
            {
                HttpLib.BaiduAPISendPost<OutputBase<ComOutput>>(_cloudBaiduConfig.FaceUserDelete, paraList, this._access_token);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[百度云人脸识别StudentDel]删除人员信息，{_tenantId}，{studentId}", ex, this.GetType());
            }
        }

        public string CheckFace(OutputBase<DetectOutput> output)
        {
            if (output.result.face_list == null || output.result.face_list.Count == 0)
            {
                return "人脸图像过于模糊";
            }
            var faceQualityInfo = output.result.face_list[0].quality;
            if (faceQualityInfo.blur >= 0.7)
            {
                return "人脸图像过于模糊";
            }
            if (faceQualityInfo.illumination <= 40)
            {
                return "人脸图像过于模糊";
            }
            if (faceQualityInfo.occlusion.left_eye > 0.6)
            {
                return "请勿遮挡眼睛";
            }
            if (faceQualityInfo.occlusion.right_eye > 0.6)
            {
                return "请勿遮挡眼睛";
            }
            if (faceQualityInfo.occlusion.nose > 0.7)
            {
                return "请勿遮挡鼻子";
            }
            if (faceQualityInfo.occlusion.mouth > 0.7)
            {
                return "请勿遮挡嘴巴";
            }
            if (faceQualityInfo.occlusion.left_cheek > 0.8)
            {
                return "请勿遮挡脸颊";
            }
            if (faceQualityInfo.occlusion.right_cheek > 0.8)
            {
                return "请勿遮挡脸颊";
            }
            if (faceQualityInfo.occlusion.chin > 0.6)
            {
                return "请勿遮挡下巴";
            }
            if (faceQualityInfo.completeness == 0)
            {
                return "请将脸部置于采集框内";
            }
            return string.Empty;
        }

        public string DetectFace2(string base64Img)
        {
            var paraList = new
            {
                image_type = "BASE64",
                image = base64Img,
                face_field = "quality,beauty,face_shape,eye_status,emotion,face_type,mask,spoofing",
                max_face_num = 1
            };
            var result = HttpLib.BaiduAPISendPost<OutputBase<DetectOutput>>(_cloudBaiduConfig.FaceDetect, paraList, this._access_token);
            return CheckFace(result);
        }

        public void ReplaceUser(long studentId, string base64Img)
        {
            var userId = GetUserId(studentId);
            var paraList = new
            {
                image_type = "BASE64",
                image = base64Img,
                group_id = this._tenantGroupId,
                user_id = userId,
                user_info = userId,
                quality_control = "NORMAL",
                liveness_control = "NORMAL",
                action_type = "REPLACE"
            };
            HttpLib.BaiduAPISendPost<OutputBase<UserUpdateOutput>>(_cloudBaiduConfig.FaceUserUpdate, paraList, this._access_token);
        }

        public Tuple<bool, string> StudentInitFace(long studentId, string imageBase64)
        {
            try
            {
                var errMsg = DetectFace2(imageBase64);
                if (!string.IsNullOrEmpty(errMsg))
                {
                    return Tuple.Create(false, errMsg);
                }
                ReplaceUser(studentId, imageBase64);
                return Tuple.Create(true, string.Empty);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[百度云人脸识别]人脸采集，{_tenantId}，{studentId}", ex, this.GetType());
                return Tuple.Create(false, "人脸图像质量不符合要求，请重新采集"); ;
            }
        }

        public bool StudentClearFace(long studentId)
        {
            StudentDel(studentId);
            return true;
        }

        public Tuple<long, string> SearchPerson(string imageBase64)
        {
            try
            {
                var checkImg = DetectFace2(imageBase64);
                if (!string.IsNullOrEmpty(checkImg))
                {
                    return Tuple.Create(0L, checkImg);
                }
                var paraList = new
                {
                    image_type = "BASE64",
                    image = imageBase64,
                    group_id_list = this._tenantGroupId
                };
                var result = HttpLib.BaiduAPISendPost<OutputBase<SearchOutput>>(_cloudBaiduConfig.FaceSearch, paraList, this._access_token);
                var myCandidates = result.result.user_list[0];
                if (myCandidates.score >= 80)
                {
                    var id = GetStudentId(myCandidates.user_id);
                    return Tuple.Create(id, string.Empty);
                }
                else
                {
                    Log.Fatal($"[百度云人脸识别]人脸搜索结果,分数太低:{Newtonsoft.Json.JsonConvert.SerializeObject(result)}", this.GetType());
                    return Tuple.Create(0L, string.Empty);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal($"[百度云人脸识别]人脸搜索失败，{_tenantId}", ex, this.GetType());
                return Tuple.Create(0L, string.Empty);
            }
        }
    }
}
