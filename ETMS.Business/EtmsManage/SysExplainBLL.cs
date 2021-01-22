using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.Explain.Output;
using ETMS.Entity.EtmsManage.Dto.Explain.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysExplainBLL : ISysExplainBLL
    {
        private readonly ISysExplainDAL _sysExplainDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        public SysExplainBLL(ISysExplainDAL sysExplainDAL, ISysAgentLogDAL sysAgentLogDAL)
        {
            this._sysExplainDAL = sysExplainDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
        }

        public async Task<ResponseBase> SysExplainAdd(SysExplainAddRequest request)
        {
            await _sysExplainDAL.AddSysExplain(new SysExplain()
            {
                IsDeleted = EmIsDeleted.Normal,
                Ot = DateTime.Now,
                RelationUrl = request.RelationUrl,
                Remark = request.Remark,
                Title = request.Title,
                Type = request.Type,
                AgentId = request.LoginAgentId
            });

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加系统内容维护：{request.Title}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysExplainMgr
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysExplainEdit(SysExplainEditRequest request)
        {
            var data = await _sysExplainDAL.GetSysExplain(request.Id);
            if (data == null)
            {
                return ResponseBase.CommonError("系统内容维护不存在");
            }
            data.RelationUrl = request.RelationUrl;
            data.Remark = request.Remark;
            data.Title = request.Title;
            await _sysExplainDAL.EditSysExplain(data);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"编辑系统内容维护：{request.Title}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysExplainMgr
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysExplainDel(SysExplainDelRequest request)
        {
            var data = await _sysExplainDAL.GetSysExplain(request.Id);
            if (data == null)
            {
                return ResponseBase.CommonError("系统内容维护不存在");
            }
            await _sysExplainDAL.DelSysExplain(data);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"删除系统内容维护：{data.Title}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysExplainMgr
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysExplainPaging(SysExplainPagingRequest request)
        {
            var pagingData = await _sysExplainDAL.GetPaging(request);
            var output = new List<SysExplainPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new SysExplainPagingOutput()
                {
                    Id = p.Id,
                    RelationUrl = p.RelationUrl,
                    Title = p.Title,
                    Type = p.Type,
                    TypeDesc = EmSysExplainType.GetSysExplainTypeDesc(p.Type),
                    Remark = p.Remark
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysExplainPagingOutput>(pagingData.Item2, output));
        }
    }
}
