using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.DataLog.Output;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysTryApplyLogBLL : ISysTryApplyLogBLL
    {
        private readonly ISysTryApplyLogDAL _sysTryApplyLogDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        public SysTryApplyLogBLL(ISysTryApplyLogDAL sysTryApplyLogDAL, ISysAgentLogDAL sysAgentLogDAL)
        {
            this._sysTryApplyLogDAL = sysTryApplyLogDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
        }

        public async Task<ResponseBase> TryApplyLogGetPaging(TryApplyLogGetPagingRequest request)
        {
            var pagingData = await _sysTryApplyLogDAL.GetPaging(request);
            var output = new List<TryApplyLogGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new TryApplyLogGetPagingOutput()
                {
                    CId = p.Id,
                    HandleRemark = p.HandleRemark,
                    LinkPhone = p.LinkPhone,
                    Name = p.Name,
                    Ot = p.Ot,
                    Status = p.Status,
                    ClientType = p.ClientType,
                    HandleOt = p.HandleOt,
                    ClientTypeDesc = EmUserOperationLogClientType.GetClientTypeDesc(p.ClientType),
                    Remark = p.Remark,
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TryApplyLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TryApplyLogHandle(TryApplyLogHandleRequest request)
        {
            var log = await _sysTryApplyLogDAL.SysTryApplyLogGet(request.Id);
            if (log == null)
            {
                return ResponseBase.CommonError("试听记录不存在");
            }
            log.HandleRemark = request.HandleContent;
            log.Status = EmSysTryApplyLogStatus.Processed;
            log.HandleUserId = request.LoginUserId;
            log.HandleOt = DateTime.Now;
            await _sysTryApplyLogDAL.EditSysTryApplyLog(log);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"标记试听记录已处理：{log.Name}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.TryMgr
            }, request.LoginUserId);
            return ResponseBase.Success();
        }
    }
}
