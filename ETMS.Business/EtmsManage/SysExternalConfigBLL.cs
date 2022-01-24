using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent3.Output;
using ETMS.Entity.EtmsManage.Dto.Agent3.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysExternalConfigBLL : ISysExternalConfigBLL
    {
        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        private readonly ISysExternalConfigDAL _sysExternalConfigDAL;

        public SysExternalConfigBLL(ISysAgentLogDAL sysAgentLogDAL, ISysExternalConfigDAL sysExternalConfigDAL)
        {
            this._sysAgentLogDAL = sysAgentLogDAL;
            this._sysExternalConfigDAL = sysExternalConfigDAL;
        }

        public async Task<ResponseBase> SysExternalConfigAdd(SysExternalConfigAddRequest request)
        {
            var hisLog = await _sysExternalConfigDAL.GetSysExternalConfigByType(request.Type);
            if (hisLog != null)
            {
                return ResponseBase.CommonError("类型已存在");
            }
            await _sysExternalConfigDAL.AddSysExternalConfig(new SysExternalConfig()
            {
                Data1 = request.Data1,
                Data2 = request.Data2,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Remark = string.Empty,
                Type = request.Type
            });

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加外部配置信息：{request.Name}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.ExternalConfigMgr
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysExternalConfigEdit(SysExternalConfigEditRequest request)
        {
            var data = await _sysExternalConfigDAL.GetSysExternalConfigById(request.Id);
            if (data == null)
            {
                return ResponseBase.CommonError("数据不存在");
            }
            data.Name = request.Name;
            data.Data1 = request.Data1;
            data.Data2 = request.Data2;
            await _sysExternalConfigDAL.EditSysExternalConfig(data);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"编辑外部配置信息：{request.Name}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.ExternalConfigMgr
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysExternalConfigDel(SysExternalConfigDelRequest request)
        {
            var data = await _sysExternalConfigDAL.GetSysExternalConfigById(request.Id);
            if (data == null)
            {
                return ResponseBase.CommonError("数据不存在");
            }
            await _sysExternalConfigDAL.DelSysExternalConfig(data);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"删除外部配置信息：{data.Name}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.ExternalConfigMgr
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysExternalConfigPaging(SysExternalConfigPagingRequest request)
        {
            var pagingData = await _sysExternalConfigDAL.GetPaging(request);
            var output = new List<SysExternalConfigPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new SysExternalConfigPagingOutput()
                {
                    Id = p.Id,
                    Type = p.Type,
                    Name = p.Name,
                    Data1 = p.Data1,
                    Data2 = p.Data2
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysExternalConfigPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> SysExternalConfigGetList(AgentRequestBase request)
        {
            var myDataList = await _sysExternalConfigDAL.GetSysExternalConfigs();
            var output = new List<SysExternalConfigPagingOutput>();
            if (myDataList != null && myDataList.Any())
            {
                foreach (var p in myDataList)
                {
                    output.Add(new SysExternalConfigPagingOutput()
                    {
                        Id = p.Id,
                        Type = p.Type,
                        Name = p.Name,
                        Data1 = p.Data1,
                        Data2 = p.Data2
                    });
                }
            }
            return ResponseBase.Success(output);
        }
    }
}
