using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class IncomeLogBLL : IIncomeLogBLL
    {
        private readonly IIncomeLogDAL _incomeLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IIncomeProjectTypeDAL _incomeProjectTypeDAL;

        private readonly IEventPublisher _eventPublisher;

        public IncomeLogBLL(IIncomeLogDAL incomeLogDAL, IUserDAL userDAL, IUserOperationLogDAL userOperationLogDAL, IIncomeProjectTypeDAL incomeProjectTypeDAL, IEventPublisher eventPublisher)
        {
            this._incomeLogDAL = incomeLogDAL;
            this._userDAL = userDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._incomeProjectTypeDAL = incomeProjectTypeDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _incomeLogDAL, _userDAL, _userOperationLogDAL, _incomeProjectTypeDAL);
        }

        public async Task<ResponseBase> IncomeLogGetPaging(IncomeLogGetPagingPagingRequest request)
        {
            var pagingData = await _incomeLogDAL.GetIncomeLogPaging(request);
            var output = new List<IncomeLogGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var allProjectType = await _incomeProjectTypeDAL.GetAllIncomeProjectType();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new IncomeLogGetPagingOutput()
                {
                    AccountNo = p.AccountNo,
                    CId = p.Id,
                    CreateOt = p.CreateOt,
                    No = p.No,
                    OrderId = p.OrderId,
                    Ot = p.Ot,
                    OtDesc = p.Ot.EtmsToDateString(),
                    PayType = p.PayType,
                    PayTypeDesc = EmPayType.GetPayType(p.PayType),
                    ProjectType = p.ProjectType,
                    ProjectTypeDesc = EmIncomeLogProjectType.GetIncomeLogProjectType(allProjectType, p.ProjectType),
                    Remark = p.Remark,
                    Status = p.Status,
                    Sum = p.Sum,
                    Type = p.Type,
                    TypeDesc = EmIncomeLogType.GetIncomeLogType(p.Type),
                    UserId = p.UserId,
                    UserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId),
                    StatusDesc = EmIncomeLogStatus.GetIncomeLogStatusDesc(p.Status)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<IncomeLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> IncomeLogAdd(IncomeLogAddRequest request)
        {
            var now = DateTime.Now;
            var log = new EtIncomeLog()
            {
                AccountNo = request.AccountNo,
                CreateOt = now,
                IsDeleted = EmIsDeleted.Normal,
                No = string.Empty,
                OrderId = null,
                Ot = request.Ot,
                PayType = request.PayType,
                ProjectType = request.ProjectType,
                Remark = request.Remark,
                RepealOt = null,
                RepealUserId = null,
                Status = EmIncomeLogStatus.Normal,
                Sum = request.Sum,
                TenantId = request.LoginTenantId,
                Type = request.Type,
                UserId = request.LoginUserId
            };
            await _incomeLogDAL.AddIncomeLog(log);
            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
            {
                StatisticsDate = log.Ot
            });
            await _userOperationLogDAL.AddUserLog(request, $"新增{EmIncomeLogType.GetIncomeLogType(request.Type)},金额:{request.Sum},账户:{request.AccountNo},经办日期:{request.Ot.EtmsToDateString()},备注:{request.Remark}", EmUserOperationType.IncomeLogAdd, now);
            return ResponseBase.Success();
        }
    }
}
