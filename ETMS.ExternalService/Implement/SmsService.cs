using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Output;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Entity.ExternalService.Dto.Request.User;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
using ETMS.ExternalService.ExProtocol.ZhuTong.Request;
using ETMS.ExternalService.ExProtocol.ZhuTong.Request.TemplatesParms;
using ETMS.ExternalService.ExProtocol.ZhuTong.Response;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.LOG;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.ExternalService.Implement
{
    public class SmsService : ISmsService
    {
        private readonly SmsConfig _smsConfig;

        private readonly IHttpClient _httpClient;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IEventPublisher _eventPublisher;

        public SmsService(IAppConfigurtaionServices appConfigurtaionServices, IHttpClient httpClient, ISysTenantDAL sysTenantDAL,
            IEventPublisher eventPublisher)
        {
            this._smsConfig = appConfigurtaionServices.AppSettings.SmsConfig;
            this._httpClient = httpClient;
            this._sysTenantDAL = sysTenantDAL;
            this._eventPublisher = eventPublisher;
        }

        private Tuple<string, string> GetTKeyAndPwd()
        {
            var key = DateTime.UtcNow.EtmsGetTimestamp().ToString();
            var pwd = CryptogramHelper.EncryptMD5($"{CryptogramHelper.EncryptMD5(_smsConfig.ZhuTong.Password)}{key}");
            return Tuple.Create(key, pwd);
        }

        public async Task<SmsOutput> AddSmsSign(AddSmsSignRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                var sendSmsRequest = new AddSignRequest()
                {
                    username = _smsConfig.ZhuTong.UserName,
                    password = tKeyAndPwd.Item2,
                    tKey = tKeyAndPwd.Item1,
                    sign = new List<string>() { $"【{request.SmsSignature}】" },
                    remark = string.Empty
                };
                var res = await _httpClient.PostAsync<AddSignRequest, SendSmsTpRes>(_smsConfig.ZhuTong.SmsSign, sendSmsRequest);
                if (!SendSmsTpRes.IsSuccess(res))
                {
                    Log.Fatal($"AddSmsSign新增短信签名,请求参数:{EtmsHelper.EtmsSerializeObject(request)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    return SmsOutput.Fail(res.msg);
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Fatal($"AddSmsSign新增短信签名:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
        }

        public async Task<SmsOutput> UserLogin(SmsUserLoginRequest request)
        {
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var sendSmsRequest = new SendSmsTpRequest<ValidCode>()
                {
                    username = _smsConfig.ZhuTong.UserName,
                    password = tKeyAndPwd.Item2,
                    tKey = tKeyAndPwd.Item1,
                    signature = smsSignature,
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
                var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var sendSmsRequest = new SendSmsTpRequest<ValidCode>()
                {
                    username = _smsConfig.ZhuTong.UserName,
                    password = tKeyAndPwd.Item2,
                    tKey = tKeyAndPwd.Item1,
                    signature = smsSignature,
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

        public async Task<SmsOutput> SysSafe(SmsSysSafeRequest request)
        {
            return await UserLogin(new SmsUserLoginRequest(request.LoginTenantId)
            {
                LoginTenantId = request.LoginTenantId,
                Phone = request.Phone,
                ValidCode = request.ValidCode
            });
        }

        public async Task<SmsOutput> NoticeStudentsOfClassBeforeDay(NoticeStudentsOfClassBeforeDayRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[SmsService]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.NoticeStudentsOfClassBeforeDay).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>() {
                     new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value = student.StudentName  },
                     new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value =  request.ClassTimeDesc  },
                     new SmsReplaceWord(){  Wildcard =mySmsTemplateWildcard[2],Value =student.CourseName },
                     new SmsReplaceWord(){  Wildcard =mySmsTemplateWildcard[3],Value =request.ClassRoom },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"NoticeStudentsOfClassBeforeDay发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.NoticeStudentsOfClassBeforeDay
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"NoticeStudentsOfClassBeforeDay发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeStudentsOfClassToday(NoticeStudentsOfClassTodayRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[SmsService]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.NoticeStudentsOfClassToday).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>(){
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value = student.StudentName  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value =  request.ClassTimeDesc  },
                        new SmsReplaceWord(){  Wildcard =mySmsTemplateWildcard[2],Value =student.CourseName },
                        new SmsReplaceWord(){  Wildcard =mySmsTemplateWildcard[3],Value =request.ClassRoom },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"NoticeStudentsOfClassToday发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.NoticeStudentsOfClassToday
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"NoticeStudentsOfClassToday发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeClassCheckSign(NoticeClassCheckSignRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[SmsService]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.ClassCheckSign).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>() {
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value =  student.Name  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value = request.ClassTimeDesc },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[2], Value = student.CourseName  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[3], Value = student.StudentCheckStatusDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[4], Value = student.DeClassTimesDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[5], Value = student.SurplusClassTimesDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[6], Value = student.ExTimeDesc  },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeClassCheckSign]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.NoticeClassCheckSign
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeClassCheckSign]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeStudentLeaveApply(NoticeStudentLeaveApplyRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[SmsService]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.StudentLeaveApply).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };

                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>() {
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value = student.Name  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value = request.TimeDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[2], Value = student.HandleStatusDesc  },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeStudentLeaveApply]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.NoticeStudentLeaveApply
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeStudentLeaveApply]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeStudentContracts(NoticeStudentContractsRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[SmsService]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.StudentContracts).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>()
                    {
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value = student.Name  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value = request.TimeDedc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[2], Value = request.BuyDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[3], Value = request.AptSumDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[4], Value = request.PaySumDesc  },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeStudentContracts]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.NoticeStudentContracts
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeStudentContracts]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeStudentCourseNotEnough(NoticeStudentCourseNotEnoughRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[NoticeStudentCourseNotEnough]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.StudentCourseNotEnough).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>()
                    {
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value = student.StudentName  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value = request.CourseName  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[2], Value = request.NotEnoughDesc  },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeStudentCourseNotEnough]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.StudentCourseNotEnough
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeStudentCourseNotEnough]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeUserOfClassToday(NoticeUserOfClassTodayRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Users.Count)
            {
                Log.Warn($"[NoticeUserOfClassToday]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtUserSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.NoticeUserOfClassToday).Wildcards;
                foreach (var user in request.Users)
                {
                    if (!EtmsHelper.IsMobilePhone(user.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = user.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>() {
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value = user.UserName  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value = user.CourseName  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[2], Value = request.ClassTimeDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[3], Value = request.ClassRoom  },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeUserOfClassToday]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtUserSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = user.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            UserId = user.UserId,
                            TenantId = request.LoginTenantId,
                            Type = EmUserSmsLogType.NoticeOfClassToday
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeUserOfClassToday]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { UserSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeStudentCheckIn(NoticeStudentCheckInRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[NoticeStudentCheckIn]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var deClassTimesDesc = string.Empty;
                if (!string.IsNullOrEmpty(request.DeClassTimesDesc))
                {
                    deClassTimesDesc = $"{request.DeClassTimesDesc}，";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.StudentCheckOnLogCheckIn).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>() {
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value = student.Name  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value =  request.CheckOtDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[2], Value = deClassTimesDesc  },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeStudentCheckIn]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.StudentCheckOnLog
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeStudentCheckIn]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeStudentCheckOut(NoticeStudentCheckOutRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[NoticeStudentCheckOut]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.StudentCheckOnLogCheckOut).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>() {
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value =  student.Name  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value = request.CheckOtDesc  },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[NoticeStudentCheckOut]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.StudentCheckOnLog
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeStudentCheckOut]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> NoticeStudentAccountRechargeChanged(NoticeStudentAccountRechargeChangedRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[NoticeStudentAccountRechargeChanged]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.StudentAccountRechargeChanged).Wildcards;
                var student = request.Students.First();
                var sendSmsRequest = new SendSmsRequest()
                {
                    mobile = request.AccountRechargePhone,
                    password = tKeyAndPwd.Item2,
                    tKey = tKeyAndPwd.Item1,
                    time = string.Empty,
                    username = _smsConfig.ZhuTong.UserName
                };
                var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>() {
                    new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value = student.Name  },
                    new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value = request.BalanceDesc  },
                });
                content = $"{smsSignature}{content}";
                sendSmsRequest.content = content;
                var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                if (!SendSmsRes.IsSuccess(res))
                {
                    Log.Info($"[NoticeStudentAccountRechargeChanged]发送短信失败,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                }
                else
                {
                    smsLog.Add(new EtStudentSmsLog()
                    {
                        DeCount = res.contNum,
                        IsDeleted = EmIsDeleted.Normal,
                        Ot = now,
                        Phone = request.AccountRechargePhone,
                        SmsContent = content,
                        Status = EmSmsLogStatus.Finish,
                        StudentId = student.StudentId,
                        TenantId = request.LoginTenantId,
                        Type = EmStudentSmsLogType.StudentAccountRechargeChanged
                    });
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[NoticeStudentAccountRechargeChanged]发送短信失败:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }

        public async Task<SmsOutput> StudentCourseSurplus(StudentCourseSurplusRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount < request.Students.Count)
            {
                Log.Warn($"[StudentCourseSurplus]机构短信剩余数量不足，无法发送短信,TenantId:{request.LoginTenantId}", this.GetType());
                return SmsOutput.Fail();
            }
            var smsLog = new List<EtStudentSmsLog>();
            var now = DateTime.Now;
            try
            {
                var tKeyAndPwd = GetTKeyAndPwd();
                var smsSignature = _smsConfig.ZhuTong.Signature;
                if (!string.IsNullOrEmpty(myTenant.SmsSignature))
                {
                    smsSignature = $"【{myTenant.SmsSignature}】";
                }
                var mySmsTemplateWildcard = SmsTemplateAnalyze.SmsTemplate.First(p => p.Type == EmSysSmsTemplateType.StudentCourseSurplus).Wildcards;
                foreach (var student in request.Students)
                {
                    if (!EtmsHelper.IsMobilePhone(student.Phone))
                    {
                        continue;
                    }
                    var sendSmsRequest = new SendSmsRequest()
                    {
                        mobile = student.Phone,
                        password = tKeyAndPwd.Item2,
                        tKey = tKeyAndPwd.Item1,
                        time = string.Empty,
                        username = _smsConfig.ZhuTong.UserName
                    };
                    var content = SmsTemplateAnalyze.GetSmsContent(request.SmsTemplate, new List<SmsReplaceWord>() {
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[0], Value =  student.Name  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[1], Value = student.CourseName  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[2], Value = student.SurplusQuantityDesc  },
                        new SmsReplaceWord(){ Wildcard =mySmsTemplateWildcard[3], Value = student.ExTimeDesc  },
                    });
                    content = $"{smsSignature}{content}";
                    sendSmsRequest.content = content;
                    var res = await _httpClient.PostAsync<SendSmsRequest, SendSmsRes>(_smsConfig.ZhuTong.SendSms, sendSmsRequest);
                    if (!SendSmsRes.IsSuccess(res))
                    {
                        Log.Info($"[StudentCourseSurplus]学员剩余课程,请求参数:{EtmsHelper.EtmsSerializeObject(sendSmsRequest)},返回值:{EtmsHelper.EtmsSerializeObject(res)}", this.GetType());
                    }
                    else
                    {
                        smsLog.Add(new EtStudentSmsLog()
                        {
                            DeCount = res.contNum,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            Phone = student.Phone,
                            SmsContent = content,
                            Status = EmSmsLogStatus.Finish,
                            StudentId = student.StudentId,
                            TenantId = request.LoginTenantId,
                            Type = EmStudentSmsLogType.StudentCheckOnLog
                        });
                    }
                }
                return SmsOutput.Success();
            }
            catch (Exception ex)
            {
                Log.Error($"[StudentCourseSurplus]学员剩余课程:{EtmsHelper.EtmsSerializeObject(request)}", ex, this.GetType());
                return SmsOutput.Fail();
            }
            finally
            {
                if (smsLog.Count > 0)
                {
                    _eventPublisher.Publish(new TenantSmsDeductionEvent(request.LoginTenantId) { StudentSmsLogs = smsLog });
                }
            }
        }
    }
}
