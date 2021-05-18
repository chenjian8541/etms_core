using ETMS.Business.EtmsManage.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Dto.DataLog.Output;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Enum;
using ETMS.Business.Common;
using ETMS.Utility;

namespace ETMS.Business.EtmsManage
{
    public class DataLogBLL : IDataLogBLL
    {
        private readonly ISysAgentDAL _sysAgentDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysSmsLogDAL sysSmsLogDAL;

        private readonly ISysTenantOperationLogDAL _sysTenantOperationLogDAL;

        private readonly ISysTenantLogDAL _sysTenantLogDAL;

        public DataLogBLL(ISysAgentDAL sysAgentDAL, ISysTenantDAL sysTenantDAL, ISysSmsLogDAL sysSmsLogDAL,
            ISysTenantOperationLogDAL sysTenantOperationLogDAL, ISysTenantLogDAL sysTenantLogDAL)
        {
            this._sysAgentDAL = sysAgentDAL;
            this._sysTenantDAL = sysTenantDAL;
            this.sysSmsLogDAL = sysSmsLogDAL;
            this._sysTenantOperationLogDAL = sysTenantOperationLogDAL;
            this._sysTenantLogDAL = sysTenantLogDAL;
        }

        public async Task<ResponseBase> SysSmsLogPaging(SysSmsLogPagingRequest request)
        {
            var output = new List<SysSmsLogPagingOutput>();
            var pagingData = await sysSmsLogDAL.GetPaging(request);
            var tempBoxAgent = new AgentDataTempBox<SysAgent>();
            var tempBoxTenant = new AgentDataTempBox<SysTenant>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    var agent = await AgentComBusiness.GetAgent(tempBoxAgent, _sysAgentDAL, p.AgentId);
                    var tenant = await AgentComBusiness.GetTenant(tempBoxTenant, _sysTenantDAL, p.TenantId);
                    var typedesc = string.Empty;
                    if (p.Type > 0)
                    {
                        if (p.RetType == EmPeopleType.User)
                        {
                            typedesc = EmUserSmsLogType.GetTypeDesc(p.Type);
                        }
                        else
                        {
                            typedesc = EmStudentSmsLogType.GetStudentSmsLogTypeDesc(p.Type);
                        }
                    }
                    output.Add(new SysSmsLogPagingOutput()
                    {
                        TenantId = p.TenantId,
                        AgentId = p.AgentId,
                        AgentName = agent?.Name,
                        AgentPhone = agent?.Phone,
                        Phone = p.Phone,
                        DeCount = p.DeCount,
                        Ot = p.Ot,
                        RetType = p.RetType,
                        SmsContent = p.SmsContent,
                        Status = p.Status,
                        TenantName = tenant?.Name,
                        TenantPhone = tenant?.Phone,
                        TypeDesc = typedesc
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysSmsLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> SysTenantOperationLogPaging(SysTenantOperationLogPagingRequest request)
        {
            var output = new List<SysTenantOperationLogPagingOutput>();
            var pagingData = await _sysTenantOperationLogDAL.GetPaging(request);
            //var tempBoxAgent = new AgentDataTempBox<SysAgent>();
            //var tempBoxTenant = new AgentDataTempBox<SysTenant>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    //var agent = await AgentComBusiness.GetAgent(tempBoxAgent, _sysAgentDAL, p.AgentId);
                    //var tenant = await AgentComBusiness.GetTenant(tempBoxTenant, _sysTenantDAL, p.TenantId);
                    output.Add(new SysTenantOperationLogPagingOutput()
                    {
                        //AgentId = p.AgentId,
                        //AgentName = agent?.Name,
                        //AgentPhone = agent?.Phone,
                        //TenantName = tenant?.Name,
                        //TenantPhone = tenant?.Phone,
                        //TenantId = p.TenantId,
                        ClientType = p.ClientType,
                        IpAddress = p.IpAddress,
                        OpContent = p.OpContent,
                        Ot = p.Ot,
                        Type = p.Type,
                        UserId = p.UserId,
                        ClientTypeDesc = EmUserOperationLogClientType.GetClientTypeDesc(p.ClientType),
                        TypeDesc = EnumDataLib.GetUserOperationTypeDesc.FirstOrDefault(j => j.Value == p.Type)?.Label
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysTenantOperationLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> SysTenantExDateLogPaging(SysTenantExDateLogPagingRequest request)
        {
            var output = new List<SysTenantExDateLogPagingOutput>();
            var pagingData = await _sysTenantLogDAL.GetSysTenantExDateLogPaging(request);
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new SysTenantExDateLogPagingOutput()
                    {
                        AfterDateDesc = p.AfterDate.EtmsToDateString(),
                        AgentId = p.AgentId,
                        BeforeDateDesc = p.BeforeDate.EtmsToDateString(),
                        ChangeDesc = p.ChangeDesc,
                        ChangeType = p.ChangeType,
                        Ot = p.Ot,
                        TenantId = p.TenantId,
                        Remark = p.Remark
                    }); ;
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysTenantExDateLogPagingOutput>(pagingData.Item2, output));
        }
    }
}
