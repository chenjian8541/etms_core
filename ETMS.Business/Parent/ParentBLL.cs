using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;
using ETMS.Entity.Config;
using ETMS.ExternalService.Contract;
using ETMS.Business.Common;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.ExternalService.Dto.Request;
using WxApi;
using WxApi.UserManager;
using ETMS.LOG;
using WxApi.ReceiveEntity;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IBusiness.Wechart;
using Senparc.Weixin.Open.OAuthAPIs;
using Senparc.Weixin.Open.Containers;
using Microsoft.AspNetCore.Http;

namespace ETMS.Business
{
    public class ParentBLL : IParentBLL
    {
        private readonly IParentLoginSmsCodeDAL _parentLoginSmsCodeDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IParentStudentDAL _parentStudentDAL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly ISmsService _smsService;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        private readonly IStudentWechatDAL _studentWechatDAL;

        private readonly ISysStudentWechartDAL _sysStudentWechartDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IComponentAccessBLL _componentAccessBLL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ISysTenantStudentDAL _sysTenantStudentDAL;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        public ParentBLL(IParentLoginSmsCodeDAL parentLoginSmsCodeDAL, ISysTenantDAL sysTenantDAL, IParentStudentDAL parentStudentDAL,
            IAppConfigurtaionServices appConfigurtaionServices, ISmsService smsService, IStudentOperationLogDAL studentOperationLogDAL,
            IStudentWechatDAL studentWechatDAL, ISysStudentWechartDAL sysStudentWechartDAL, IStudentDAL studentDAL, IComponentAccessBLL componentAccessBLL,
            ITenantConfigDAL tenantConfigDAL, IHttpContextAccessor httpContextAccessor, ISysTenantStudentDAL sysTenantStudentDAL,
            IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL)
        {
            this._parentLoginSmsCodeDAL = parentLoginSmsCodeDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._parentStudentDAL = parentStudentDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._smsService = smsService;
            this._studentOperationLogDAL = studentOperationLogDAL;
            this._studentWechatDAL = studentWechatDAL;
            this._sysStudentWechartDAL = sysStudentWechartDAL;
            this._studentDAL = studentDAL;
            this._componentAccessBLL = componentAccessBLL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._sysTenantStudentDAL = sysTenantStudentDAL;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
        }

        public async Task<IEnumerable<ParentStudentInfo>> GetMyStudent(ParentRequestBase request)
        {
            _parentStudentDAL.InitTenantId(request.LoginTenantId);
            return await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
        }

        public async Task<Tuple<string, SysTenant>> GetLoginTenant(string tenantNo, string code)
        {
            var tenantId = TenantLib.GetTenantDecrypt(tenantNo);
            SysTenant sysTenantInfo = null;
            if (tenantId == 0 && string.IsNullOrEmpty(code))
            {
                return Tuple.Create("机构编码不能为空", sysTenantInfo);
            }
            if (tenantId == 0)
            {
                sysTenantInfo = await _sysTenantDAL.GetTenant(code);
                if (sysTenantInfo == null)
                {
                    return Tuple.Create("机构不存在", sysTenantInfo);
                }
            }
            else
            {
                sysTenantInfo = await _sysTenantDAL.GetTenant(tenantId);
                if (sysTenantInfo == null)
                {
                    return Tuple.Create("机构不存在", sysTenantInfo);
                }
            }
            return Tuple.Create(string.Empty, sysTenantInfo);
        }

        public async Task<ResponseBase> ParentLoginSendSms(ParentLoginSendSmsRequest request)
        {
            var loginTenantResult = await GetLoginTenant(request.TenantNo, request.Code);
            if (!string.IsNullOrEmpty(loginTenantResult.Item1) || loginTenantResult.Item2 == null)
            {
                return ResponseBase.CommonError(loginTenantResult.Item1);
            }
            var sysTenantInfo = loginTenantResult.Item2;

            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            _parentStudentDAL.InitTenantId(sysTenantInfo.Id);
            var students = await _parentStudentDAL.UpdateCacheAndGetParentStudents(sysTenantInfo.Id, request.Phone);
            if (students == null || !students.Any())
            {
                return ResponseBase.CommonError("未找到此手机号绑定的学员信息");
            }
            var smsCode = RandomHelper.GetSmsCode();
            var sendSmsRes = await _smsService.ParentLogin(new SmsParentLoginRequest(sysTenantInfo.Id)
            {
                Phone = request.Phone,
                ValidCode = smsCode
            });
            if (!sendSmsRes.IsSuccess)
            {
                return ResponseBase.CommonError("发送短信失败,请稍后再试");
            }
            this._parentLoginSmsCodeDAL.AddParentLoginSmsCode(request.Code, request.Phone, smsCode);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ParentLoginBySms(ParentLoginBySmsRequest request)
        {
            var loginTenantResult = await GetLoginTenant(request.TenantNo, request.Code);
            if (!string.IsNullOrEmpty(loginTenantResult.Item1) || loginTenantResult.Item2 == null)
            {
                return ResponseBase.CommonError(loginTenantResult.Item1);
            }
            var sysTenantInfo = loginTenantResult.Item2;

            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            var loginSms = _parentLoginSmsCodeDAL.GetParentLoginSmsCode(request.Code, request.Phone);
            if (loginSms == null || loginSms.ExpireAtTime < DateTime.Now || loginSms.SmsCode != request.SmsCode)
            {
                return ResponseBase.CommonError("验证码错误");
            }
            _parentLoginSmsCodeDAL.RemoveParentLoginSmsCode(request.Code, request.Phone);
            return await GetParentLoginResult(sysTenantInfo.Id, request.Phone, request.StudentWechartId);
        }

        private async Task<ResponseBase> GetParentLoginResult(int tenantId, string phone, string studentWechartId)
        {
            _parentStudentDAL.InitTenantId(tenantId);
            var students = await _parentStudentDAL.UpdateCacheAndGetParentStudents(tenantId, phone);
            if (students == null || !students.Any())
            {
                return ResponseBase.CommonError("未找到手机号绑定的学员信息");
            }
            if (!string.IsNullOrEmpty(studentWechartId))
            {
                _studentWechatDAL.InitTenantId(tenantId);
                _studentDAL.InitTenantId(tenantId);
                var sysStudentWechartLog = await _sysStudentWechartDAL.GetSysStudentWechart(studentWechartId.ToLong());
                sysStudentWechartLog.TenantId = tenantId;
                await _sysStudentWechartDAL.EditSysStudentWechart(sysStudentWechartLog);
                await _studentWechatDAL.DelStudentWechat(phone, sysStudentWechartLog.WechatOpenid);
                await _studentWechatDAL.AddStudentWechat(new Entity.Database.Source.EtStudentWechat()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    Nickname = sysStudentWechartLog.Nickname,
                    Headimgurl = sysStudentWechartLog.Headimgurl,
                    Phone = phone,
                    Remark = string.Empty,
                    StudentId = students.First().Id,
                    TenantId = tenantId,
                    WechatOpenid = sysStudentWechartLog.WechatOpenid,
                    WechatUnionid = sysStudentWechartLog.WechatUnionid
                });
                await _studentDAL.UpdateStudentIsBindingWechat(students.Select(p => p.Id).ToList());
            }
            var exTime = DateTime.Now.Date.AddDays(_appConfigurtaionServices.AppSettings.ParentConfig.TokenExpiredDay).EtmsGetTimestamp().ToString();
            var parentTokenConfig = new ParentTokenConfig()
            {
                ExTimestamp = exTime,
                Phone = phone,
                TenantId = tenantId
            };
            var signatureInfo = ParentSignatureLib.GetSignature(parentTokenConfig);
            _studentOperationLogDAL.InitTenantId(tenantId);
            await _studentOperationLogDAL.AddStudentLog(students.First().Id, tenantId, $"家长端登录：{phone}", Entity.Enum.EmStudentOperationLogType.Login);
            return ResponseBase.Success(new ParentLoginBySmsOutput()
            {
                L = signatureInfo.Item1,
                S = signatureInfo.Item2,
                ExpiresIn = exTime
            });
        }

        public ResponseBase ParentRefreshToken(ParentRefreshTokenRequest request)
        {
            if (!ParentSignatureLib.CheckSignature(request.StrLoginInfo, request.StrSignature))
            {
                return ResponseBase.CommonError("凭证验证错误");
            }
            var loginInfo = ParentSignatureLib.GetParentLoginInfo(request.StrLoginInfo);
            var exTime = DateTime.Now.Date.AddDays(_appConfigurtaionServices.AppSettings.ParentConfig.TokenExpiredDay).EtmsGetTimestamp().ToString();
            var newParentTokenConfig = new ParentTokenConfig()
            {
                ExTimestamp = exTime,
                Phone = loginInfo.Phone,
                TenantId = loginInfo.TenantId
            };
            var signatureInfo = ParentSignatureLib.GetSignature(newParentTokenConfig);
            return ResponseBase.Success(new ParentLoginBySmsOutput()
            {
                L = signatureInfo.Item1,
                S = signatureInfo.Item2,
                ExpiresIn = exTime
            });
        }

        public async Task<ResponseBase> ParentGetAuthorizeUrl(ParentGetAuthorizeUrlRequest request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            return await GetAuthorizeUrl(tenantId, request.SourceUrl);
        }

        public async Task<ResponseBase> ParentLoginByCode(ParentLoginByCodeRequest request)
        {
            Log.Info($"家长端通过code登录请求:{request.Code}", this.GetType());
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[ParentGetAuthorizeUrl]未找到机构授权信息,tenantId:{tenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var componentAppid = _appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
            var componentAccessToken = ComponentContainer.GetComponentAccessToken(componentAppid);
            var authToken = OAuthApi.GetAccessToken(tenantWechartAuth.AuthorizerAppid, componentAppid, componentAccessToken, request.Code);

            var sysStudentWechartLog = await _sysStudentWechartDAL.GetSysStudentWechart(authToken.openid);
            if (sysStudentWechartLog == null || sysStudentWechartLog.TenantId == 0)
            {
                return await ResetSysStudentWechart(authToken, tenantId, tenantWechartAuth.AuthorizerAppid);
            }
            var sysTenantInfo = await _sysTenantDAL.GetTenant(sysStudentWechartLog.TenantId);
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                await _sysStudentWechartDAL.DelSysStudentWechart(authToken.openid); //修复bug 机构无法登录时 删除微信绑定信息
                return ResponseBase.CommonError(myMsg);
            }
            _studentWechatDAL.InitTenantId(sysStudentWechartLog.TenantId);
            var myStudentWechatLog = await _studentWechatDAL.GetStudentWechat(authToken.openid);
            if (myStudentWechatLog == null)
            {
                return await ResetSysStudentWechart(authToken, tenantId, tenantWechartAuth.AuthorizerAppid);
            }
            var result = await GetParentLoginResult(sysStudentWechartLog.TenantId, myStudentWechatLog.Phone, "");
            if (result.IsResponseSuccess())
            {
                return ResponseBase.Success(new ParentLoginByCodeOutput()
                {
                    Type = ParentLoginByCodeType.Success,
                    LoginInfo = (ParentLoginBySmsOutput)result.resultData
                });
            }
            else
            {
                await _sysStudentWechartDAL.DelSysStudentWechart(authToken.openid); //修复bug 机构无法登录时 删除微信绑定信息
                return result;
            }
        }

        private async Task<ResponseBase> ResetSysStudentWechart(OAuthAccessTokenResult authToken, int tenantId, string authorizerAppid)
        {
            await _sysStudentWechartDAL.DelSysStudentWechart(authToken.openid);
            var userInfo = OAuthApi.GetUserInfo(authToken.access_token, authToken.openid);
            var sysStudentWechart = new SysStudentWechart()
            {
                Headimgurl = userInfo.headimgurl,
                IsDeleted = EmIsDeleted.Normal,
                Nickname = userInfo.nickname,
                Remark = authorizerAppid,
                TenantId = tenantId,
                WechatOpenid = userInfo.openid,
                WechatUnionid = userInfo.unionid
            };
            await _sysStudentWechartDAL.AddSysStudentWechart(sysStudentWechart);
            return ResponseBase.Success(new ParentLoginByCodeOutput()
            {
                Type = ParentLoginByCodeType.Failure,
                StudentWechartId = sysStudentWechart.Id
            });
        }

        public async Task<ResponseBase> ParentGetAuthorizeUrl2(ParentGetAuthorizeUrl2Request request)
        {
            return await GetAuthorizeUrl(request.LoginTenantId, request.SourceUrl);
        }

        private async Task<ResponseBase> GetAuthorizeUrl(int tenantId, string sourceUrl)
        {
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(tenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[GetAuthorizeUrl]未找到机构授权信息,tenantId:{tenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var componentAppid = _appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
            var url = OAuthApi.GetAuthorizeUrl(tenantWechartAuth.AuthorizerAppid, componentAppid, sourceUrl, tenantId.ToString(),
                new[] { Senparc.Weixin.Open.OAuthScope.snsapi_userinfo, Senparc.Weixin.Open.OAuthScope.snsapi_base });
            Log.Info($"[家长端获取授权地址]{url}", this.GetType());
            return ResponseBase.Success(url);
        }

        public async Task<ResponseBase> ParentBindingWeChat(ParentBindingWeChatRequest request)
        {
            var tenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuth(request.LoginTenantId);
            if (tenantWechartAuth == null)
            {
                Log.Error($"[ParentBindingWeChat]未找到机构授权信息,tenantId:{request.LoginTenantId}", this.GetType());
                return ResponseBase.CommonError("机构绑定的微信公众号无权限");
            }
            var componentAppid = _appConfigurtaionServices.AppSettings.SenparcConfig.SenparcWeixinSetting.ComponentConfig.ComponentAppid;
            var componentAccessToken = ComponentContainer.GetComponentAccessToken(componentAppid);
            var authToken = OAuthApi.GetAccessToken(tenantWechartAuth.AuthorizerAppid, componentAppid, componentAccessToken, request.Code);
            var userInfo = OAuthApi.GetUserInfo(authToken.access_token, authToken.openid);
            await _sysStudentWechartDAL.DelSysStudentWechart(authToken.openid);
            var sysStudentWechart = new SysStudentWechart()
            {
                Headimgurl = userInfo.headimgurl,
                IsDeleted = EmIsDeleted.Normal,
                Nickname = userInfo.nickname,
                Remark = tenantWechartAuth.AuthorizerAppid,
                TenantId = request.LoginTenantId,
                WechatOpenid = userInfo.openid,
                WechatUnionid = userInfo.unionid
            };
            await _sysStudentWechartDAL.AddSysStudentWechart(sysStudentWechart);

            _studentWechatDAL.InitTenantId(request.LoginTenantId);
            _studentDAL.InitTenantId(request.LoginTenantId);
            await _studentWechatDAL.DelStudentWechat(request.LoginPhone, userInfo.openid);
            await _studentWechatDAL.AddStudentWechat(new Entity.Database.Source.EtStudentWechat()
            {
                IsDeleted = EmIsDeleted.Normal,
                Nickname = userInfo.nickname,
                Headimgurl = userInfo.headimgurl,
                Phone = request.LoginPhone,
                Remark = string.Empty,
                StudentId = request.ParentStudentIds.First(),
                TenantId = request.LoginTenantId,
                WechatOpenid = userInfo.openid,
                WechatUnionid = userInfo.unionid
            });
            await _studentDAL.UpdateStudentIsBindingWechat(request.ParentStudentIds);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ParentInfoGet(ParentInfoGetRequest request)
        {
            _studentWechatDAL.InitTenantId(request.LoginTenantId);
            _tenantConfigDAL.InitTenantId(request.LoginTenantId);
            _studentAccountRechargeCoreBLL.InitTenantId(request.LoginTenantId);
            var myStudentWechat = await _studentWechatDAL.GetStudentWechatByPhone(request.LoginPhone);
            var myTenantWechartAuth = await _componentAccessBLL.GetTenantWechartAuthSelf(request.LoginTenantId);
            var output = new ParentInfoGetOutput();
            if (myStudentWechat != null)
            {
                output.Nickname = myStudentWechat.Nickname;
                output.Headimgurl = myStudentWechat.Headimgurl;
                output.Phone = myStudentWechat.Phone;
            }
            output.IsShowLoginout = myTenantWechartAuth == null;

            //充值账户
            var studentAccountRechargeInfo = await _studentAccountRechargeCoreBLL.GetStudentAccountRechargeByPhone2(request.LoginPhone);
            if (studentAccountRechargeInfo != null)
            {
                output.StudentAccountRechargeId = studentAccountRechargeInfo.Id;
            }

            //推荐有奖
            var config = await _tenantConfigDAL.GetTenantConfig();
            var studentRecommendConfig = config.StudentRecommendConfig;
            if (studentRecommendConfig.IsOpenBuy || studentRecommendConfig.IsOpenRegistered)
            {
                if (!string.IsNullOrEmpty(studentRecommendConfig.RecommendDesText) || !string.IsNullOrEmpty(studentRecommendConfig.RecommendDesImg))
                {
                    output.IsShowStudentRecommend = true;
                }
            }

            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> CheckParentCanLogin(ParentRequestBase request)
        {
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ParentLoginout(ParentLoginoutRequest request)
        {
            _studentWechatDAL.InitTenantId(request.LoginTenantId);
            var myStudentWechat = await _studentWechatDAL.GetStudentWechatByPhone(request.LoginPhone);
            if (myStudentWechat != null)
            {
                await _sysStudentWechartDAL.DelSysStudentWechart(myStudentWechat.WechatOpenid);
                await _studentWechatDAL.DelStudentWechat(request.LoginPhone, myStudentWechat.WechatOpenid);
                _studentDAL.InitTenantId(request.LoginTenantId);
                await _studentDAL.UpdateStudentIsNotBindingWechat(request.ParentStudentIds);
            }
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> GetTenantInfo(int tenantId)
        {
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            _tenantConfigDAL.InitTenantId(tenantId);
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var parentLoginImage = string.Empty;
            if (!string.IsNullOrEmpty(tenantConfig.ParentSetConfig.LoginImage))
            {
                parentLoginImage = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, tenantConfig.ParentSetConfig.LoginImage);
            }
            return ResponseBase.Success(new GetTenantInfoOutput()
            {
                ParentHtmlTitle = tenantConfig.ParentSetConfig.Title,
                ParentLoginImage = parentLoginImage,
                TenantAddress = tenantConfig.TenantInfoConfig.Address,
                TenantDescribe = tenantConfig.TenantInfoConfig.Describe,
                TenantLinkName = tenantConfig.TenantInfoConfig.LinkName,
                TenantLinkPhone = tenantConfig.TenantInfoConfig.LinkPhone,
                TenantName = myTenant.Name,
                TenantNickName = myTenant.SmsSignature
            });
        }

        public async Task<ResponseBase> GetTenantInfoByNo(GetTenantInfoByNoRequest request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            if (tenantId == 0)
            {
                return ResponseBase.Success(new GetTenantInfoOutput());
            }
            return await GetTenantInfo(tenantId);
        }

        public async Task<ResponseBase> GetTenantInfo(GetTenantInfoRequest request)
        {
            return await GetTenantInfo(request.LoginTenantId);
        }

        public async Task<ResponseBase> ParentGetCurrentTenant(ParentRequestBase request)
        {
            var myTenants = await _sysTenantStudentDAL.GetTenantStudent(request.LoginPhone);
            var thisTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return ResponseBase.Success(new ParentGetCurrentTenantOutput()
            {
                CurrentTenantCode = thisTenant.TenantCode,
                CurrentTenantId = thisTenant.Id,
                CurrentTenantName = thisTenant.Name,
                IsHasMultipleTenant = myTenants.Count > 1
            });
        }

        public async Task<ResponseBase> ParentGetTenants(ParentRequestBase request)
        {
            var myTenants = await _sysTenantStudentDAL.GetTenantStudent(request.LoginPhone);
            var output = new List<ParentGetTenantsOutput>();
            foreach (var p in myTenants)
            {
                var thisTenant = await _sysTenantDAL.GetTenant(p.TenantId);
                if (thisTenant == null)
                {
                    Log.Error($"[ParentGetTenants]机构不存在，TenantId:{p.TenantId}", this.GetType());
                    continue;
                }
                if (!ComBusiness2.CheckTenantCanLogin(thisTenant, out var myMsg))
                {
                    continue;
                }
                output.Add(new ParentGetTenantsOutput()
                {
                    TenantCode = thisTenant.TenantCode,
                    TenantName = thisTenant.Name,
                    TenantId = thisTenant.Id,
                    IsCurrentLogin = p.TenantId == request.LoginTenantId
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ParentTenantEntrance(ParentTenantEntranceRequest request)
        {
            if (request.TenantId == request.LoginTenantId)
            {
                return ResponseBase.CommonError("已登录此机构");
            }
            var thisTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (thisTenant == null)
            {
                Log.Error($"[ParentTenantEntrance]机构不存在，TenantId:{request.TenantId}", this.GetType());
                return ResponseBase.CommonError("机构不存在");
            }
            if (!ComBusiness2.CheckTenantCanLogin(thisTenant, out var myMsg))
            {
                return ResponseBase.CommonError(myMsg);
            }
            return await GetParentLoginResult(thisTenant.Id, request.LoginPhone, null);
        }

        public async Task<ResponseBase> StudentRecommendRuleGet(ParentRequestBase request)
        {
            _tenantConfigDAL.InitTenantId(request.LoginTenantId);
            var config = await _tenantConfigDAL.GetTenantConfig();
            var studentRecommendConfig = config.StudentRecommendConfig;
            return ResponseBase.Success(new StudentRecommendRuleGetOutput()
            {
                RecommendDesImgUrl = UrlHelper.GetUrl(studentRecommendConfig.RecommendDesImg),
                RecommendDesText = studentRecommendConfig.RecommendDesText
            });
        }
    }
}
