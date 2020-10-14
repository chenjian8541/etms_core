using ETMS.Entity.Config;
using ETMS.Entity.ExternalService.Dto.Output;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.ExternalService.Contract;
using ETMS.ExternalService.ExProtocol.ZhuTong.Request;
using ETMS.ExternalService.ExProtocol.ZhuTong.Request.TemplatesParms;
using ETMS.ExternalService.ExProtocol.ZhuTong.Response;
using ETMS.LOG;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.ExternalService.Implement
{
    public class SmsService : ISmsService
    {
        private readonly SmsConfig _smsConfig;

        private readonly IHttpClient _httpClient;

        public SmsService(IAppConfigurtaionServices appConfigurtaionServices, IHttpClient httpClient)
        {
            this._smsConfig = appConfigurtaionServices.AppSettings.SmsConfig;
            this._httpClient = httpClient;
        }

        private Tuple<string, string> GetTKeyAndPwd()
        {
            var key = DateTime.UtcNow.EtmsGetTimestamp().ToString();
            var pwd = CryptogramHelper.EncryptMD5($"{CryptogramHelper.EncryptMD5(_smsConfig.ZhuTong.Password)}{key}");
            return Tuple.Create(key, pwd);
        }

        public async Task<SmsOutput> UserLogin(SmsUserLoginRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var sendSmsRequest = new SendSmsTpRequest<ValidCode>()
                {
                    username = _smsConfig.ZhuTong.UserName,
                    password = tKeyAndPwd.Item2,
                    tKey = tKeyAndPwd.Item1,
                    signature = _smsConfig.ZhuTong.Signature,
                    tpId = _smsConfig.ZhuTong.TemplatesLogin.TpId,
                    records = new List<Records<ValidCode>>() {
                     new Records<ValidCode>(){
                         mobile = request.Phone,
                         tpContent = new ValidCode(){
                             valid_code = request.ValidCode
                       }
                     }
                    }
                };
                var res = await _httpClient.PostAsync<SendSmsTpRequest<ValidCode>, SendSmsTpRes>(_smsConfig.ZhuTong.SendSmsTpUrl, sendSmsRequest);
                if (!SendSmsTpRes.IsSuccess(res))
                {
                    Log.Error($"UserLogin发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(request)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    return SmsOutput.Fail(res.msg);
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"UserLogin发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
        }

        public async Task<SmsOutput> ParentLogin(SmsParentLoginRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var sendSmsRequest = new SendSmsTpRequest<ValidCode>()
                {
                    username = _smsConfig.ZhuTong.UserName,
                    password = tKeyAndPwd.Item2,
                    tKey = tKeyAndPwd.Item1,
                    signature = _smsConfig.ZhuTong.Signature,
                    tpId = _smsConfig.ZhuTong.TemplatesParentLogin.TpId,
                    records = new List<Records<ValidCode>>() {
                     new Records<ValidCode>(){
                         mobile = request.Phone,
                         tpContent = new ValidCode(){
                             valid_code = request.ValidCode
                       }
                     }
                    }
                };
                var res = await _httpClient.PostAsync<SendSmsTpRequest<ValidCode>, SendSmsTpRes>(_smsConfig.ZhuTong.SendSmsTpUrl, sendSmsRequest);
                if (!SendSmsTpRes.IsSuccess(res))
                {
                    Log.Error($"ParentLogin发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(request)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    return SmsOutput.Fail(res.msg);
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"ParentLogin发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
        }

        public async Task<SmsOutput> NoticeStudentsOfClassBeforeDay(NoticeStudentsOfClassBeforeDayRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                foreach (var student in request.Students)
                {
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = string.Empty;
                    if (string.IsNullOrEmpty(request.ClassRoom))
                    {
                        content = string.Format(_smsConfig.ZhuTong.NoticeStudentsOfClassBeforeDay.NoRoom, student.StudentName, request.ClassTimeDesc, student.CourseName);
                    }
                    else
                    {
                        content = string.Format(_smsConfig.ZhuTong.NoticeStudentsOfClassBeforeDay.HasRoom, student.StudentName, request.ClassTimeDesc, student.CourseName, request.ClassRoom);
                    }
                    content = $"{_smsConfig.ZhuTong.Signature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"NoticeStudentsOfClassBeforeDay发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"NoticeStudentsOfClassBeforeDay发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
        }

        public async Task<SmsOutput> NoticeStudentsOfClassToday(NoticeStudentsOfClassTodayRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                foreach (var student in request.Students)
                {
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = string.Empty;
                    if (string.IsNullOrEmpty(request.ClassRoom))
                    {
                        content = string.Format(_smsConfig.ZhuTong.NoticeStudentsOfClassToday.NoRoom, student.StudentName, student.CourseName, request.ClassTimeDesc);
                    }
                    else
                    {
                        content = string.Format(_smsConfig.ZhuTong.NoticeStudentsOfClassToday.HasRoom, student.StudentName, student.CourseName, request.ClassTimeDesc, request.ClassRoom);
                    }
                    content = $"{_smsConfig.ZhuTong.Signature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"NoticeStudentsOfClassToday发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"NoticeStudentsOfClassToday发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
        }

        public async Task<SmsOutput> NoticeClassCheckSign(NoticeClassCheckSignRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                foreach (var student in request.Students)
                {
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = string.Format(_smsConfig.ZhuTong.ClassCheckSign.Com, student.Name, request.ClassTimeDesc, student.CourseName, student.StudentCheckStatusDesc,
                        student.DeClassTimesDesc, student.SurplusClassTimesDesc);
                    content = $"{_smsConfig.ZhuTong.Signature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeClassCheckSign]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeClassCheckSign]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
        }

        public async Task<SmsOutput> NoticeStudentLeaveApply(NoticeStudentLeaveApplyRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                foreach (var student in request.Students)
                {
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = string.Format(_smsConfig.ZhuTong.StudentLeaveApply.Com, student.Name, request.TimeDesc, student.HandleStatusDesc);
                    content = $"{_smsConfig.ZhuTong.Signature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeStudentLeaveApply]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeStudentLeaveApply]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
        }

        public async Task<SmsOutput> NoticeStudentContracts(NoticeStudentContractsRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                foreach (var student in request.Students)
                {
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = string.Format(_smsConfig.ZhuTong.StudentContracts.Com, student.Name, request.TimeDedc, request.BuyDesc, request.AptSumDesc, request.PaySumDesc);
                    content = $"{_smsConfig.ZhuTong.Signature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeStudentContracts]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeStudentContracts]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
        }
    }
}
