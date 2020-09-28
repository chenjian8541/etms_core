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

        public ParentBLL(IParentLoginSmsCodeDAL parentLoginSmsCodeDAL, ISysTenantDAL sysTenantDAL, IParentStudentDAL parentStudentDAL,
            IAppConfigurtaionServices appConfigurtaionServices, ISmsService smsService, IStudentOperationLogDAL studentOperationLogDAL)
        {
            this._parentLoginSmsCodeDAL = parentLoginSmsCodeDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._parentStudentDAL = parentStudentDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._smsService = smsService;
            this._studentOperationLogDAL = studentOperationLogDAL;
        }

        public async Task<IEnumerable<ParentStudentInfo>> GetMyStudent(ParentRequestBase request)
        {
            _parentStudentDAL.InitTenantId(request.LoginTenantId);
            return await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
        }

        public async Task<ResponseBase> ParentLoginSendSms(ParentLoginSendSmsRequest request)
        {
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.Code);
            if (sysTenantInfo == null)
            {
                return ResponseBase.CommonError("机构不存在");
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
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.Code);
            if (sysTenantInfo == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            _parentStudentDAL.InitTenantId(sysTenantInfo.Id);
            var students = await _parentStudentDAL.UpdateCacheAndGetParentStudents(sysTenantInfo.Id, request.Phone);
            if (students == null || !students.Any())
            {
                return ResponseBase.CommonError("未找到此手机号绑定的学员信息");
            }
            var loginSms = _parentLoginSmsCodeDAL.GetParentLoginSmsCode(request.Code, request.Phone);
            if (loginSms == null || loginSms.ExpireAtTime < DateTime.Now || loginSms.SmsCode != request.SmsCode)
            {
                return ResponseBase.CommonError("验证码错误");
            }
            var exTime = DateTime.Now.Date.AddDays(_appConfigurtaionServices.AppSettings.ParentConfig.TokenExpiredDay).EtmsGetTimestamp().ToString();
            var parentTokenConfig = new ParentTokenConfig()
            {
                ExTimestamp = exTime,
                Phone = request.Phone,
                TenantId = sysTenantInfo.Id
            };
            var signatureInfo = ParentSignatureLib.GetSignature(parentTokenConfig);
            _parentLoginSmsCodeDAL.RemoveParentLoginSmsCode(request.Code, request.Phone);
            _studentOperationLogDAL.InitTenantId(sysTenantInfo.Id);
            await _studentOperationLogDAL.AddStudentLog(students.First().Id, sysTenantInfo.Id, $"家长端登录：{request.Phone}", Entity.Enum.EmStudentOperationLogType.Login);
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
    }
}
