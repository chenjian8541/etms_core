using ETMS.Business.Common;
using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Marketing.Output;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class SmsLogBLL : ISmsLogBLL
    {
        private readonly ISmsLogDAL _studentSmsLogDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly SmsConfig _smsConfig;

        private readonly IDistributedLockDAL _distributedLockDAL;

        private readonly ISmsService _smsService;

        private readonly IUserOperationLogDAL _userOperationLogDAL;
        public SmsLogBLL(ISmsLogDAL studentSmsLogDAL, IStudentDAL studentDAL, ISysTenantDAL sysTenantDAL,
            IAppConfigurtaionServices appConfigurtaionServices, IDistributedLockDAL distributedLockDAL, ISmsService smsService,
            IUserOperationLogDAL userOperationLogDAL)
        {
            this._studentSmsLogDAL = studentSmsLogDAL;
            this._studentDAL = studentDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._smsConfig = appConfigurtaionServices.AppSettings.SmsConfig;
            this._distributedLockDAL = distributedLockDAL;
            this._smsService = smsService;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSmsLogDAL, _studentDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> StudentSmsLogGetPaging(StudentSmsLogGetPagingRequest request)
        {
            var pagingData = await _studentSmsLogDAL.GetStudentSmsLogPaging(request);
            var output = new List<StudentSmsLogGetPagingOutput>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var studetName = string.Empty;
                if (p.StudentId != null)
                {
                    var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId.Value);
                    if (student != null)
                    {
                        studetName = student.Name;
                    }
                }
                output.Add(new StudentSmsLogGetPagingOutput()
                {
                    CId = p.Id,
                    DeCount = p.DeCount,
                    Ot = p.Ot,
                    Phone = p.Phone,
                    SmsContent = p.SmsContent,
                    Status = p.Status,
                    StatusDesc = EmSmsLogStatus.GetSmsLogStatusDesc(p.Status),
                    StudentId = p.StudentId,
                    StudentName = studetName,
                    Type = p.Type,
                    TypeDesc = EmStudentSmsLogType.GetStudentSmsLogTypeDesc(p.Type)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentSmsLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentSmsBatchSend(StudentSmsSendRequest request)
        {
            var lockKey = new StudentSmsBatchSendToken(request.LoginTenantId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await ProcessStudentSmsBatchSend(request);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[StudentSmsBatchSend]短信发送失败:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    return ResponseBase.CommonError("短信发送失败");
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("系统正在处理其他短信发送任务，请稍后再试...");
        }

        private async Task<ResponseBase> ProcessStudentSmsBatchSend(StudentSmsSendRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant.SmsCount <= 0)
            {
                return ResponseBase.CommonError("短信剩余条数不足");
            }
            var smsSignature = _smsConfig.ZhuTong.Signature;
            if (!string.IsNullOrEmpty(myTenant.SmsSignature))
            {
                smsSignature = $"【{myTenant.SmsSignature}】";
            }
            var newSmsContent = $"{smsSignature}{request.SmsContent}";
            var deCount = EtmsHelper3.SmsCountCalculate(newSmsContent, request.Students.Count);
            if (myTenant.SmsCount < deCount)
            {
                return ResponseBase.CommonError("短信剩余条数不足，可尝试减少发送内容");
            }
            var smsRequest = new SmsBatchSendRequest(request.LoginTenantId)
            {
                UserId = request.LoginUserId,
                SmsBatch = new List<SmsBatchSendItem>()
            };
            foreach (var p in request.Students)
            {
                smsRequest.SmsBatch.Add(new SmsBatchSendItem()
                {
                    Phone = p.Phone,
                    StudentId = p.CId,
                    SmsContent = newSmsContent.Replace("{{学员名称}}", p.Name)
                });
            }
            var res = await _smsService.SmsBatchSend(smsRequest);
            if (!res.IsSuccess)
            {
                return ResponseBase.CommonError("短信发送失败");
            }
            deCount = res.ResultData;
            await _sysTenantDAL.TenantSmsDeduction(myTenant.Id, deCount);

            await _userOperationLogDAL.AddUserLog(request, $"给{request.Students.Count}位学员发送短信", EmUserOperationType.StudentSmsBatchSend);
            return ResponseBase.Success();
        }
    }
}
