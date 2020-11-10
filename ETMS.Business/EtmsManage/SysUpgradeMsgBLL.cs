using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.Agent.Output;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysUpgradeMsgBLL : ISysUpgradeMsgBLL
    {
        private readonly ISysUpgradeMsgDAL _sysUpgradeMsgDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        public SysUpgradeMsgBLL(ISysUpgradeMsgDAL sysUpgradeMsgDAL, ISysAgentLogDAL sysAgentLogDAL)
        {
            this._sysUpgradeMsgDAL = sysUpgradeMsgDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
        }

        public async Task<ResponseBase> SysUpgradeMsgAdd(SysUpgradeMsgAddRequest request)
        {
            var startTime = Convert.ToDateTime(request.Ot[0]);
            var endTime = Convert.ToDateTime(request.Ot[1]);
            var entity = new SysUpgradeMsg()
            {
                AgentId = request.LoginAgentId,
                EndTime = endTime,
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                StartTime = startTime,
                Title = request.Title,
                UpContent = request.UpContent,
                VersionNo = request.VersionNo,
                Status = EmSysUpgradeMsgStatus.Normal
            };
            await _sysUpgradeMsgDAL.AddSysUpgradeMsg(entity);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加升级公告：版本号:{request.VersionNo},标题:{request.Title}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.VersionUpgrade
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysUpgradeMsgDel(SysUpgradeMsgDelRequest request)
        {
            await _sysUpgradeMsgDAL.DelSysUpgradeMsg(request.CId);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = "删除升级公告",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.VersionUpgrade
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysUpgradeMsgPaging(SysUpgradeMsgPagingRequest request)
        {
            var pagingData = await _sysUpgradeMsgDAL.GetPaging(request);
            var output = new List<SysUpgradeMsgPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new SysUpgradeMsgPagingOutput()
                {
                    AgentId = p.AgentId,
                    EndTime = p.EndTime,
                    StartTime = p.StartTime,
                    Title = p.Title,
                    UpContent = p.UpContent,
                    VersionNo = p.VersionNo,
                    Status = p.Status,
                    StatusDesc = EmSysUpgradeMsgStatus.GetSysUpgradeMsgStatusDesc(p.Status),
                    CId = p.Id
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysUpgradeMsgPagingOutput>(pagingData.Item2, output));
        }
    }
}
