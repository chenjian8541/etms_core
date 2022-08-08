using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Marketing.Output;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ETMS.Utility;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.LOG;
using ETMS.Entity.View;
using ETMS.Business.Common;
using ETMS.Entity.Dto.Parent.Output;

namespace ETMS.Business
{
    public class CouponsBLL : ICouponsBLL
    {
        private readonly ICouponsDAL _couponsDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        private readonly INoticeBLL _noticeBLL;

        public CouponsBLL(ICouponsDAL couponsDAL, IUserOperationLogDAL userOperationLogDAL, IEventPublisher eventPublisher,
            IStudentOperationLogDAL studentOperationLogDAL, INoticeBLL noticeBLL)
        {
            this._couponsDAL = couponsDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._eventPublisher = eventPublisher;
            this._studentOperationLogDAL = studentOperationLogDAL;
            this._noticeBLL = noticeBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _couponsDAL, _userOperationLogDAL, _studentOperationLogDAL);
            this._noticeBLL.InitTenantId(tenantId);
        }

        public async Task<ResponseBase> CouponsAdd(CouponsAddRequest request)
        {
            var coupons = new EtCoupons()
            {
                Remark = request.Remark,
                Ot = DateTime.Now,
                ExpiredType = request.ExpiredType,
                GetCount = 0,
                IsDeleted = EmIsDeleted.Normal,
                Status = EmCouponsStatus.Enabled,
                TenantId = request.LoginTenantId,
                Title = request.Title,
                TotalCount = request.TotalCount,
                Type = request.Type,
                UsedCount = 0,
                UseExplain = request.UseExplain,
                MinLimit = null,
                LimitGetSingle = 0,
                LimitGetAll = 0
            };
            switch (request.Type)
            {
                case EmCouponsType.Cash:
                    coupons.Value = Convert.ToDecimal(request.CashValue);
                    break;
                case EmCouponsType.Discount:
                    coupons.Value = Convert.ToDecimal(request.DiscountValue);
                    break;
                case EmCouponsType.ClassTimes:
                    coupons.Value = Convert.ToDecimal(request.ClassTimesValue);
                    break;
            }
            switch (request.ExpiredType)
            {
                case EmCouponsExpiredType.Unexpected:
                    break;
                case EmCouponsExpiredType.FixedTime:
                    coupons.StartTime = Convert.ToDateTime(request.ExpiredTimeDesc[0]);
                    coupons.EndTime = Convert.ToDateTime(request.ExpiredTimeDesc[1]);
                    break;
                case EmCouponsExpiredType.AfterGet:
                    coupons.EndOffset = Convert.ToInt32(request.EndOffset);
                    break;
            }
            if (request.LimitType == 1)
            {
                coupons.MinLimit = Convert.ToDecimal(request.MinLimit);
            }
            if (!string.IsNullOrEmpty(request.LimitGetSingle))
            {
                coupons.LimitGetSingle = Convert.ToInt32(request.LimitGetSingle);
            }
            if (!string.IsNullOrEmpty(request.LimitGetAll))
            {
                coupons.LimitGetAll = Convert.ToInt32(request.LimitGetAll);
            }
            await _couponsDAL.AddCoupons(coupons);
            await _userOperationLogDAL.AddUserLog(request, $"添加优惠券-{request.Title}", EmUserOperationType.CouponsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CouponsStatusChange(CouponsStatusChangeRequest request)
        {
            var coupons = await _couponsDAL.GetCoupons(request.CId);
            if (coupons == null)
            {
                return ResponseBase.CommonError("优惠券不存在");
            }
            coupons.Status = request.NewStatus;
            await _couponsDAL.EditCoupons(coupons);
            var tag = request.NewStatus == EmCouponsStatus.Enabled ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{tag}优惠券-{coupons.Title}", EmUserOperationType.CouponsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CouponsDel(CouponsDelRequest request)
        {
            var coupons = await _couponsDAL.GetCoupons(request.CId);
            if (coupons == null)
            {
                return ResponseBase.CommonError("优惠券不存在");
            }
            if (coupons.GetCount > 0 || coupons.UsedCount > 0)
            {
                return ResponseBase.CommonError("优惠券已使用，无法删除");
            }
            await _couponsDAL.DelCoupons(request.CId);
            await _userOperationLogDAL.AddUserLog(request, $"删除优惠券-{coupons.Title}", EmUserOperationType.CouponsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CouponsGetPaging(CouponsGetPagingRequest request)
        {
            var pagingData = await _couponsDAL.GetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<CouponsGetPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p => new CouponsGetPagingOutput()
            {
                CId = p.Id,
                ExpiredType = p.ExpiredType,
                ExpiredDesc = GetExpiredTypeDesc(p),
                GetCount = p.GetCount,
                LimitGetAll = p.LimitGetAll,
                LimitGetSingle = p.LimitGetSingle,
                MinLimit = p.MinLimit ?? 0,
                Remark = p.Remark,
                Status = p.Status,
                Title = p.Title,
                TotalCount = p.TotalCount,
                Type = p.Type,
                UsedCount = p.UsedCount,
                UseExplain = p.UseExplain,
                MyValue = p.Value,
                TypeDesc = EmCouponsType.GetCouponsTypeDesc(p.Type),
                ValueDesc = ComBusiness.GetCouponsValueDesc(p.Type, p.Value),
                MinLimitDesc = p.MinLimit == null || p.MinLimit == 0 ? "无门槛" : $"消费满{p.MinLimit.Value.ToDecimalDesc()}元可用",
                LimitGetSingleDesc = p.LimitGetSingle == 0 ? "无限制" : $"{ p.LimitGetSingle}张",
                LimitGetAllDesc = p.LimitGetAll == 0 ? "无限制" : $"{ p.LimitGetAll}张",
                Label = p.Title,
                Value = p.Id,
                IsExpired = p.ExpiredType == EmCouponsExpiredType.FixedTime && p.EndTime != null && DateTime.Now.Date > p.EndTime.Value
            })));
        }

        private string GetExpiredTypeDesc(EtCoupons coupons)
        {
            switch (coupons.ExpiredType)
            {
                case EmCouponsExpiredType.Unexpected:
                    return "不限制";
                case EmCouponsExpiredType.AfterGet:
                    return $"领取后{coupons.EndOffset}天";
                default:
                    var desc = $"{coupons.StartTime.EtmsToDateString2()}-{coupons.EndTime.EtmsToDateString2()}";
                    if (coupons.EndTime != null && DateTime.Now.Date > coupons.EndTime.Value)
                    {
                        desc = $"{desc}[已过期]";
                    }
                    return desc;
            }
        }

        public async Task<ResponseBase> ParentCouponsReceive(ParentCouponsReceiveRequest request)
        {
            var coupons = await _couponsDAL.GetCoupons(request.CId);
            var response = await CheckParentCouponsReceive(coupons, request.StudentId, request.CId);
            if (response.IsResponseSuccess())
            {
                _eventPublisher.Publish(new ParentCouponsReceiveEvent(request.LoginTenantId)
                {
                    StudentId = request.StudentId,
                    CouponsId = request.CId
                });
            }
            return response;
        }

        private async Task<ResponseBase> CheckParentCouponsReceive(EtCoupons coupons, long studentId, long couponsId)
        {
            if (coupons == null)
            {
                return ResponseBase.CommonError("优惠券不存在");
            }
            if (coupons.Status == EmCouponsStatus.Disabled)
            {
                return ResponseBase.CommonError("优惠券已禁用");
            }
            if (coupons.GetCount >= coupons.TotalCount)
            {
                return ResponseBase.CommonError("晚来一步，优惠券已领完");
            }
            if (coupons.LimitGetSingle != 0)
            {
                var todayGetCount = await _couponsDAL.StudentTodayGetCount(studentId, couponsId);
                if (todayGetCount >= coupons.LimitGetSingle)
                {
                    return ResponseBase.CommonError("今日已领取，明天再来吧");
                }
            }
            if (coupons.LimitGetAll != 0)
            {
                var totalGetCount = await _couponsDAL.StudentGetCount(studentId, couponsId);
                if (totalGetCount >= coupons.LimitGetAll)
                {
                    return ResponseBase.CommonError("您已领取，快去使用吧");
                }
            }
            return ResponseBase.Success();
        }

        public async Task ParentCouponsReceiveEvent(ParentCouponsReceiveEvent request)
        {
            var coupons = await _couponsDAL.GetCoupons(request.CouponsId);
            var response = await CheckParentCouponsReceive(coupons, request.StudentId, request.CouponsId);
            if (!response.IsResponseSuccess())
            {
                Log.Info($"{request.Id},{response.message}", this.GetType());
                return;
            }
            await _couponsDAL.AddCouponsGetCount(request.CouponsId, 1);
            var getTime = DateTime.Now;
            DateTime? limitUseTime = null;
            DateTime? expiredTime = null;
            switch (coupons.ExpiredType)
            {
                case EmCouponsExpiredType.FixedTime:
                    limitUseTime = coupons.StartTime;
                    expiredTime = coupons.EndTime;
                    break;
                case EmCouponsExpiredType.AfterGet:
                    expiredTime = getTime.AddDays(coupons.EndOffset);
                    break;
            }
            await _couponsDAL.AddCouponsStudentGet(new EtCouponsStudentGet()
            {
                CouponsId = request.CouponsId,
                GetTime = DateTime.Now,
                IsDeleted = EmIsDeleted.Normal,
                OrderId = null,
                OrderNo = string.Empty,
                Remark = string.Empty,
                Status = EmCouponsStudentStatus.Unused,
                StudentId = request.StudentId,
                TenantId = request.TenantId,
                ExpiredTime = expiredTime,
                LimitUseTime = limitUseTime
            });
            await _studentOperationLogDAL.AddStudentLog(request.StudentId, request.TenantId,
              $"领取[{coupons.Title}]优惠券", EmStudentOperationLogType.CouponsReceive);
        }

        public async Task<ResponseBase> CouponsStudentGetPaging(CouponsStudentGetPagingRequest request)
        {
            var pagingData = await _couponsDAL.CouponsStudentGetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<CouponsStudentGetPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                var logStatusDesc = ComBusiness.GetCouponsLogStatusDesc(p);
                return new CouponsStudentGetPagingOutput()
                {
                    CId = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsStatus = p.CouponsStatus,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    GetTime = p.GetTime,
                    OrderNo = p.OrderNo,
                    LogStatus = logStatusDesc.Item1,
                    LogStatusDesc = logStatusDesc.Item2,
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType, request.SecrecyDataBag),
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用",
                };
            })));
        }

        public async Task<ResponseBase> CouponsStudentRevoked(CouponsStudentRevokedRequest request)
        {
            var getLog = await _couponsDAL.CouponsStudentGet(request.CId);
            if (getLog == null)
            {
                return ResponseBase.CommonError("未找到此优惠券");
            }
            if (getLog.Status == EmCouponsStudentStatus.Used)
            {
                return ResponseBase.CommonError("此优惠券已核销");
            }
            await _couponsDAL.DelCouponsStudentGet(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "撤销优惠券", EmUserOperationType.CouponsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CouponsStudentWriteOff(CouponsStudentWriteOffRequest request)
        {
            var getLog = await _couponsDAL.CouponsStudentGet(request.CId);
            if (getLog == null)
            {
                return ResponseBase.CommonError("未找到此优惠券");
            }
            if (getLog.Status == EmCouponsStudentStatus.Used)
            {
                return ResponseBase.CommonError("此优惠券已核销");
            }
            if (getLog.LimitUseTime != null && getLog.LimitUseTime.Value.Date > DateTime.Now)
            {
                return ResponseBase.CommonError("此优惠券还未开放使用");
            }
            if (getLog.ExpiredTime != null && DateTime.Now.Date > getLog.ExpiredTime.Value)
            {
                return ResponseBase.CommonError("此优惠券已过期");
            }
            await _couponsDAL.ChangeCouponsStudentGetStatus(request.CId, EmCouponsStudentStatus.Used);
            await _couponsDAL.AddCouponsUseCount(getLog.CouponsId, 1);
            await _couponsDAL.AddCouponsStudentUse(new EtCouponsStudentUse()
            {
                CouponsId = getLog.CouponsId,
                IsDeleted = EmIsDeleted.Normal,
                OrderId = null,
                OrderNo = string.Empty,
                Ot = DateTime.Now,
                Remark = "手动核销",
                StudentId = getLog.StudentId,
                TenantId = getLog.TenantId
            });
            await _userOperationLogDAL.AddUserLog(request, "核销优惠券", EmUserOperationType.CouponsManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CouponsStudentUsePaging(CouponsStudentUsrPagingRequest request)
        {
            var pagingData = await _couponsDAL.CouponsStudentUsePaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<CouponsStudentUsrPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                return new CouponsStudentUsrPagingOutput()
                {
                    CId = p.Id,
                    OrderNo = p.OrderNo,
                    StudentName = p.StudentName,
                    StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType, request.SecrecyDataBag),
                    CouponsTitle = p.CouponsTitle,
                    Ot = p.Ot,
                    Remark = p.Remark,
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                };
            })));
        }

        public async Task<ResponseBase> CouponsStudentGetCanUse(CouponsStudentGetCanUseRequest request)
        {
            var canUseCoupons = await _couponsDAL.GetCouponsCanUse(request.StudentId);
            var output = new List<CouponsStudentGetCanUseOutput>();
            foreach (var p in canUseCoupons)
            {
                var logStatusDesc = ComBusiness.GetCouponsLogStatusDesc(p);
                output.Add(new CouponsStudentGetCanUseOutput()
                {
                    CId = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsStatus = p.CouponsStatus,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    GetTime = p.GetTime,
                    OrderNo = p.OrderNo,
                    LogStatus = logStatusDesc.Item1,
                    LogStatusDesc = logStatusDesc.Item2,
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType, request.SecrecyDataBag),
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc2(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用",
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> CouponsStudentSend(CouponsStudentSendRequest request)
        {
            var coupons = await _couponsDAL.GetCoupons(request.CouponsId);
            if (coupons.Status == EmCouponsStatus.Disabled)
            {
                return ResponseBase.CommonError("优惠券已禁用");
            }
            var surplusCount = coupons.TotalCount - coupons.GetCount;
            if (request.StudentIds.Count > surplusCount)
            {
                return ResponseBase.CommonError("优惠券剩余数量不足");
            }
            await _couponsDAL.AddCouponsGetCount(request.CouponsId, request.StudentIds.Count);
            var getTime = DateTime.Now;
            DateTime? limitUseTime = null;
            DateTime? expiredTime = null;
            switch (coupons.ExpiredType)
            {
                case EmCouponsExpiredType.FixedTime:
                    limitUseTime = coupons.StartTime;
                    expiredTime = coupons.EndTime;
                    break;
                case EmCouponsExpiredType.AfterGet:
                    expiredTime = getTime.AddDays(coupons.EndOffset);
                    break;
            }
            var couponsStudentGets = new List<EtCouponsStudentGet>();
            var now = DateTime.Now;
            var generateNo = OrderNumberLib.CouponsGenerateNo();
            foreach (var studentId in request.StudentIds)
            {
                couponsStudentGets.Add(new EtCouponsStudentGet()
                {
                    CouponsId = request.CouponsId,
                    GetTime = now,
                    IsDeleted = EmIsDeleted.Normal,
                    OrderId = null,
                    OrderNo = string.Empty,
                    Remark = string.Empty,
                    Status = EmCouponsStudentStatus.Unused,
                    StudentId = studentId,
                    TenantId = request.LoginTenantId,
                    ExpiredTime = expiredTime,
                    LimitUseTime = limitUseTime,
                    GenerateNo = generateNo,
                    IsRemindExpired = EmBool.False
                });
                await _noticeBLL.SendCoupons();
            }
            _couponsDAL.AddCouponsStudentGet(couponsStudentGets);
            _eventPublisher.Publish(new NoticeStudentCouponsGetEvent(request.LoginTenantId)
            {
                GenerateNo = generateNo
            });

            await _userOperationLogDAL.AddUserLog(request, $"发放优惠券，优惠券:{coupons.Title},学员数量：{request.StudentIds.Count}", EmUserOperationType.CouponsStudentSend);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCouponsNormalGet2(StudentCouponsNormalGet2Request request)
        {
            var pagingData = await _couponsDAL.CouponsStudentGetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCouponsNormalGetOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                return new StudentCouponsNormalGetOutput()
                {
                    Id = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用"
                };
            })));
        }

        public async Task<ResponseBase> StudentCouponsUsedGet2(StudentCouponsUsedGet2Request request)
        {
            var pagingData = await _couponsDAL.CouponsStudentGetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCouponsUsedGetOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                return new StudentCouponsUsedGetOutput()
                {
                    Id = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用"
                };
            })));
        }

        public async Task<ResponseBase> StudentCouponsExpiredGet2(StudentCouponsExpiredGet2Request request)
        {
            var pagingData = await _couponsDAL.CouponsStudentGetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCouponsExpiredGetOutput>(pagingData.Item2, pagingData.Item1.Select(p =>
            {
                return new StudentCouponsExpiredGetOutput()
                {
                    Id = p.Id,
                    CouponsMinLimit = p.CouponsMinLimit,
                    CouponsTitle = p.CouponsTitle,
                    CouponsType = p.CouponsType,
                    CouponsValue = p.CouponsValue,
                    CouponsTypeDesc = EmCouponsType.GetCouponsTypeDesc(p.CouponsType),
                    EffectiveTimeDesc = ComBusiness.GetCouponsEffectiveTimeDesc(p),
                    StudentName = p.StudentName,
                    CouponsValueDesc = ComBusiness.GetCouponsValueDesc(p.CouponsType, p.CouponsValue),
                    MinLimitDesc = p.CouponsMinLimit == null || p.CouponsMinLimit == 0 ? "无门槛" : $"消费满{p.CouponsMinLimit.Value.ToDecimalDesc()}元可用"
                };
            })));
        }
    }
}
