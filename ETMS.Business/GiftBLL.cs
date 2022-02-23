using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Common;
using ETMS.Entity.Dto.Marketing.Output;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Business.Common;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Parent.Output;

namespace ETMS.Business
{
    public class GiftBLL : IGiftBLL
    {
        private readonly IGiftDAL _giftDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IStudentDAL _studentDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentPointsLogDAL _studentPointsLog;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        private readonly IGiftCategoryDAL _giftCategoryDAL;

        public GiftBLL(IGiftDAL giftDAL, IUserOperationLogDAL userOperationLogDAL, IHttpContextAccessor httpContextAccessor,
            IAppConfigurtaionServices appConfigurtaionServices, IStudentDAL studentDAL, IEventPublisher eventPublisher, IStudentPointsLogDAL studentPointsLog,
            IStudentOperationLogDAL studentOperationLogDAL, IGiftCategoryDAL giftCategoryDAL)
        {
            this._giftDAL = giftDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._studentDAL = studentDAL;
            this._eventPublisher = eventPublisher;
            this._studentPointsLog = studentPointsLog;
            this._studentOperationLogDAL = studentOperationLogDAL;
            this._giftCategoryDAL = giftCategoryDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _giftDAL, _userOperationLogDAL, _studentDAL, _studentPointsLog, _studentOperationLogDAL, _giftCategoryDAL);
        }

        public async Task<ResponseBase> GiftAdd(GiftAddRequest request)
        {
            if (await _giftDAL.ExistGift(request.Name))
            {
                return ResponseBase.CommonError("已存在相同名称的礼品");
            }
            await _giftDAL.AddGift(new EtGift()
            {
                ImgPath = request.ImgPathKeys,
                GiftContent = request.GiftContent,
                GiftCategoryId = request.GiftCategoryId,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Nums = request.Nums,
                NumsLimit = request.NumsLimit,
                Points = request.Points,
                Remark = request.Remark,
                TenantId = request.LoginTenantId,
                IsLimitNums = request.IsLimitNums
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加礼品-{request.Name}", EmUserOperationType.GiftManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GiftEdit(GiftEditRequest request)
        {
            var gift = await _giftDAL.GetGift(request.CId);
            if (gift == null)
            {
                return ResponseBase.CommonError("礼品不存在");
            }
            if (await _giftDAL.ExistGift(request.Name, request.CId))
            {
                return ResponseBase.CommonError("已存在相同名称的礼品");
            }
            gift.ImgPath = request.ImgPathKeys;
            gift.GiftContent = request.GiftContent;
            gift.GiftCategoryId = request.GiftCategoryId;
            gift.Name = request.Name;
            gift.Nums = request.Nums;
            gift.NumsLimit = request.NumsLimit;
            gift.Points = request.Points;
            gift.Remark = request.Remark;
            gift.IsLimitNums = request.IsLimitNums;
            await _giftDAL.EditGift(gift);
            await _userOperationLogDAL.AddUserLog(request, $"编辑礼品-{request.Name}", EmUserOperationType.GiftManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GiftGet(GiftGetRequest request)
        {
            var gift = await _giftDAL.GetGift(request.CId);
            if (gift == null)
            {
                return ResponseBase.CommonError("礼品不存在");
            }
            return ResponseBase.Success(new GiftGetOutput()
            {
                GiftCategoryId = gift.GiftCategoryId,
                GiftContent = gift.GiftContent,
                Imgs = GetImgs(gift.ImgPath),
                Name = gift.Name,
                Nums = gift.Nums,
                NumsLimit = gift.NumsLimit,
                Points = gift.Points,
                Remark = gift.Remark,
                IsLimitNums = gift.IsLimitNums
            });
        }

        private List<Img> GetImgs(string imgPath)
        {
            var imgs = new List<Img>();
            if (!string.IsNullOrEmpty(imgPath))
            {
                var strImgs = imgPath.Split('|');
                foreach (var s in strImgs)
                {
                    imgs.Add(new Img()
                    {
                        Key = s,
                        Url = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, s),
                    });
                }
            }
            return imgs;
        }

        public async Task<ResponseBase> GiftDel(GiftDelRequest request)
        {
            var gift = await _giftDAL.GetGift(request.CId);
            if (gift == null)
            {
                return ResponseBase.CommonError("礼品不存在");
            }
            if (await _giftDAL.IsUserCanNotBeDelete(request.CId))
            {
                return ResponseBase.CommonError("此礼品存在兑换记录，无法删除");
            }
            await _giftDAL.DelGift(request.CId);
            AliyunOssUtil.DeleteObject2(gift.ImgPath);

            await _userOperationLogDAL.AddUserLog(request, $"删除礼品-{gift.Name}", EmUserOperationType.GiftManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GiftGetPaging(GiftGetPagingRequest request)
        {
            var giftGetPagingInfo = await _giftDAL.GetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<GiftGetPagingOutput>(giftGetPagingInfo.Item2, giftGetPagingInfo.Item1.Select(p => new GiftGetPagingOutput()
            {
                Imgs = GetImgs(p.ImgPath),
                Name = p.Name,
                Nums = p.Nums,
                IsLimitNums = p.IsLimitNums,
                Points = p.Points,
                CId = p.Id,
                Value = p.Id,
                Label = p.Name
            })));
        }

        public async Task<ResponseBase> GetExchangeLogPaging(GetExchangeLogPagingRequest request)
        {
            var pagingData = await _giftDAL.GetExchangeLogPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<GetExchangeLogPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p => new GetExchangeLogPagingOutput()
            {
                CId = p.Id,
                No = p.No,
                Ot = p.Ot,
                Remark = p.Remark,
                Status = p.Status,
                StatusDesc = EmGiftExchangeLogStatus.GetStatusDesc(p.Status),
                StudentName = p.StudentName,
                StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType),
                TotalCount = p.TotalCount,
                TotalPoints = p.TotalPoints,
                ExchangeType = p.ExchangeType,
                ExchangeTypeDesc = EmExchangeType.GetExchangeType(p.ExchangeType)
            })));
        }

        public async Task<ResponseBase> GetExchangeLogDetailPaging(GetExchangeLogDetailPagingRequest request)
        {
            var pagingData = await _giftDAL.GetExchangeLogDetailPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<GetExchangeLogDetailPagingOutput>(pagingData.Item2, pagingData.Item1.Select(p => new GetExchangeLogDetailPagingOutput()
            {
                No = p.No,
                Status = p.Status,
                StatusDesc = EmGiftExchangeLogStatus.GetStatusDesc(p.Status),
                Count = p.Count,
                GiftName = p.GiftName,
                GiftImgPath = GetImgs(p.GiftImgPath),
                ItemPoints = p.ItemPoints,
                Ot = p.Ot,
                StudentName = p.StudentName,
                StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType)
            })));
        }

        public async Task<ResponseBase> GetExchangeLogDetail(GetExchangeLogDetailRequest request)
        {
            var details = await _giftDAL.GetExchangeLogDetail(request.CId);
            return ResponseBase.Success(details.Select(p => new GetExchangeLogDetailOutput()
            {
                No = p.No,
                Status = p.Status,
                StatusDesc = EmGiftExchangeLogStatus.GetStatusDesc(p.Status),
                Count = p.Count,
                GiftName = p.GiftName,
                GiftImgPath = GetImgs(p.GiftImgPath),
                ItemPoints = p.ItemPoints,
                Ot = p.Ot,
                StudentName = p.StudentName,
                StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType)
            }));
        }

        public async Task<ResponseBase> ExchangeLogHandle(ExchangeLogHandleRequest request)
        {
            await _giftDAL.UpdateExchangeLogNewStatus(request.CId, EmGiftExchangeLogStatus.Processed);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GiftExchangeReception(GiftExchangeReceptionRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            var student = studentBucket.Student;
            if (student.Points < request.TotalPoint)
            {
                return ResponseBase.CommonError("积分不足");
            }
            var logDetails = new List<EtGiftExchangeLogDetail>();
            var totalPoint = 0;
            var totalCount = 0;
            var no = OrderNumberLib.GiftExchangeOrderNumber();
            var time = DateTime.Now;
            foreach (var exGift in request.ExchangeGiftInfos)
            {
                var gift = await _giftDAL.GetGift(exGift.GiftId);
                if (gift == null)
                {
                    return ResponseBase.CommonError("礼品不存在");
                }
                if (!gift.IsLimitNums && exGift.Count > gift.Nums)
                {
                    return ResponseBase.CommonError($"[{gift.Name}]库存不足");
                }
                if (gift.NumsLimit != 0)
                {
                    var exCount = await _giftDAL.GetStudentExchangeNums(request.StudentId, gift.Id);
                    if ((exCount + exGift.Count) > gift.NumsLimit)
                    {
                        return ResponseBase.CommonError($"[{gift.Name}]已超过可兑换的数量");
                    }
                }
                var itemPoint = gift.Points * exGift.Count;
                totalCount += exGift.Count;
                totalPoint += itemPoint;
                logDetails.Add(new EtGiftExchangeLogDetail()
                {
                    GiftId = gift.Id,
                    Count = exGift.Count,
                    IsDeleted = EmIsDeleted.Normal,
                    ItemPoints = itemPoint,
                    No = no,
                    StudentId = request.StudentId,
                    Status = EmGiftExchangeLogStatus.Unprocessed,
                    TenantId = request.LoginTenantId,
                    Ot = time
                });
            }
            if (student.Points < totalPoint)
            {
                return ResponseBase.CommonError("积分不足");
            }
            var giftExchangeLog = new EtGiftExchangeLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                Ot = time,
                No = no,
                Remark = request.Remark,
                Status = EmGiftExchangeLogStatus.Unprocessed,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                TotalCount = totalCount,
                TotalPoints = totalPoint,
                UserId = request.LoginUserId,
                ExchangeType = EmExchangeType.Reception
            };
            _eventPublisher.Publish(new GiftExchangeEvent(request.LoginTenantId)
            {
                giftExchangeLog = giftExchangeLog,
                GiftExchangeLogDetails = logDetails,
                LoginClientType = request.LoginClientType
            });
            return ResponseBase.Success();
        }

        public async Task GiftExchangeEvent(GiftExchangeEvent giftExchange)
        {
            await _studentDAL.DeductionPoint(giftExchange.giftExchangeLog.StudentId, giftExchange.giftExchangeLog.TotalPoints);
            await _studentPointsLog.AddStudentPointsLog(new EtStudentPointsLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                No = giftExchange.giftExchangeLog.No,
                Ot = giftExchange.giftExchangeLog.Ot,
                Points = giftExchange.giftExchangeLog.TotalPoints,
                Remark = giftExchange.giftExchangeLog.Remark,
                StudentId = giftExchange.giftExchangeLog.StudentId,
                TenantId = giftExchange.giftExchangeLog.TenantId,
                Type = EmStudentPointsLogType.GiftExchange
            });
            foreach (var detail in giftExchange.GiftExchangeLogDetails)
            {
                await _giftDAL.DeductionNums(detail.GiftId, detail.Count);
            }
            await _giftDAL.AddGiftExchange(giftExchange.giftExchangeLog, giftExchange.GiftExchangeLogDetails);
            if (giftExchange.giftExchangeLog.ExchangeType == EmExchangeType.WeChat)
            {
                await _studentOperationLogDAL.AddStudentLog(giftExchange.giftExchangeLog.StudentId, giftExchange.giftExchangeLog.TenantId,
                    $"兑换了{giftExchange.giftExchangeLog.TotalCount}件礼品", EmStudentOperationLogType.GiftExchange);
            }
            else
            {
                await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
                {
                    IpAddress = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    OpContent = $"礼品兑换-兑换了{giftExchange.giftExchangeLog.TotalCount}件礼品",
                    Ot = giftExchange.giftExchangeLog.Ot,
                    Remark = giftExchange.giftExchangeLog.Remark,
                    TenantId = giftExchange.giftExchangeLog.TenantId,
                    Type = (int)EmUserOperationType.GiftExchange,
                    UserId = giftExchange.giftExchangeLog.UserId.Value,
                    ClientType = giftExchange.LoginClientType
                });
            }
        }

        public async Task<ResponseBase> GiftCategoryGetParent(GiftCategoryGetParentRequest request)
        {
            var allCategory = await _giftCategoryDAL.GetAllGiftCategory();
            var outPut = new List<GiftCategoryGetParentOutput>();
            if (allCategory != null && allCategory.Any())
            {
                foreach (var p in allCategory)
                {
                    outPut.Add(new GiftCategoryGetParentOutput()
                    {
                        Id = p.Id,
                        Name = p.Name
                    });
                }
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> GiftGetParent(GiftGetParentRequest request)
        {
            var giftGetPagingInfo = await _giftDAL.GetPaging(request);
            var outPut = new List<GiftGetParentOutput>();
            foreach (var p in giftGetPagingInfo.Item1)
            {
                var images = GetImgs(p.ImgPath);
                outPut.Add(new GiftGetParentOutput()
                {
                    GiftCategoryId = p.GiftCategoryId,
                    Id = p.Id,
                    Name = p.Name,
                    Points = p.Points,
                    Nums = p.Nums,
                    IsAllowLackNums = p.IsLimitNums,
                    Img = images == null || !images.Any() ? string.Empty : images.First().Url
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<GiftGetParentOutput>(giftGetPagingInfo.Item2, outPut));
        }

        public async Task<ResponseBase> GiftDetailGetParent(GiftDetailGetParentRequest request)
        {
            var p = await _giftDAL.GetGift(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("礼品不存在");
            }
            var images = GetImgs(p.ImgPath);
            return ResponseBase.Success(new GiftDetailGetParentOutput()
            {
                GiftCategoryId = p.GiftCategoryId,
                Id = p.Id,
                Name = p.Name,
                Points = p.Points,
                Img = images == null || !images.Any() ? string.Empty : images.First().Url,
                GiftContent = p.GiftContent,
                Nums = p.Nums,
                IsAllowLackNums = p.IsLimitNums
            });
        }

        public async Task<ResponseBase> GiftExchangeSelfHelp(GiftExchangeRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            var student = studentBucket.Student;
            if (student.Points < request.TotalPoint)
            {
                return ResponseBase.CommonError("积分不足");
            }
            var logDetails = new List<EtGiftExchangeLogDetail>();
            var totalPoint = 0;
            var totalCount = 0;
            var no = OrderNumberLib.GiftExchangeOrderNumber();
            var time = DateTime.Now;
            foreach (var exGift in request.ExchangeGiftInfos)
            {
                var gift = await _giftDAL.GetGift(exGift.GiftId);
                if (gift == null)
                {
                    return ResponseBase.CommonError("礼品不存在");
                }
                if (!gift.IsLimitNums && exGift.Count > gift.Nums)
                {
                    return ResponseBase.CommonError($"[{gift.Name}]库存不足");
                }
                if (gift.NumsLimit != 0)
                {
                    var exCount = await _giftDAL.GetStudentExchangeNums(request.StudentId, gift.Id);
                    if ((exCount + exGift.Count) > gift.NumsLimit)
                    {
                        return ResponseBase.CommonError($"[{gift.Name}]已超过可兑换的数量");
                    }
                }
                var itemPoint = gift.Points * exGift.Count;
                totalCount += exGift.Count;
                totalPoint += itemPoint;
                logDetails.Add(new EtGiftExchangeLogDetail()
                {
                    GiftId = gift.Id,
                    Count = exGift.Count,
                    IsDeleted = EmIsDeleted.Normal,
                    ItemPoints = itemPoint,
                    No = no,
                    StudentId = request.StudentId,
                    Status = EmGiftExchangeLogStatus.Unprocessed,
                    TenantId = request.LoginTenantId,
                    Ot = time
                });
            }
            if (student.Points < totalPoint)
            {
                return ResponseBase.CommonError("积分不足");
            }
            var giftExchangeLog = new EtGiftExchangeLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                Ot = time,
                No = no,
                Remark = request.Remark,
                Status = EmGiftExchangeLogStatus.Unprocessed,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                TotalCount = totalCount,
                TotalPoints = totalPoint,
                UserId = null,
                ExchangeType = EmExchangeType.WeChat
            };
            _eventPublisher.Publish(new GiftExchangeEvent(request.LoginTenantId)
            {
                giftExchangeLog = giftExchangeLog,
                GiftExchangeLogDetails = logDetails
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> GetExchangeLogDetailParentPaging(StudentGiftExchangeLogGetReqeust request)
        {
            var pagingData = await _giftDAL.GetExchangeLogDetailPaging(request);
            var output = new List<GetExchangeLogDetailParentPagingOuput>();
            foreach (var p in pagingData.Item1)
            {
                var images = GetImgs(p.GiftImgPath);
                output.Add(new GetExchangeLogDetailParentPagingOuput()
                {
                    No = p.No,
                    Count = p.Count,
                    GiftName = p.GiftName,
                    Img = images == null || !images.Any() ? string.Empty : images.First().Url,
                    ItemPoints = p.ItemPoints,
                    Ot = p.Ot,
                    StudentName = p.StudentName
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<GetExchangeLogDetailParentPagingOuput>(pagingData.Item2, output));
        }
    }
}
