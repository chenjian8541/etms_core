using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Output;
using ETMS.Entity.EtmsManage.Dto.ClientUpgrade.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysClientUpgradeBLL : ISysClientUpgradeBLL
    {
        private readonly ISysClientUpgradeDAL _sysClientUpgradeDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        public SysClientUpgradeBLL(ISysClientUpgradeDAL sysClientUpgradeDAL, ISysAgentLogDAL sysAgentLogDAL)
        {
            this._sysClientUpgradeDAL = sysClientUpgradeDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
        }

        public async Task<ResponseBase> SysClientUpgradeAdd(SysClientUpgradeAddRequest request)
        {
            await _sysClientUpgradeDAL.SysClientUpgradeAdd(new SysClientUpgrade()
            {
                VersionNo = request.VersionNo,
                AgentId = request.LoginAgentId,
                ClientType = request.ClientType,
                FileUrl = request.FileUrl,
                IsDeleted = EmIsDeleted.Normal,
                Ot = DateTime.Now,
                Remark = string.Empty,
                UpgradeContent = request.UpgradeContent,
                UpgradeType = request.UpgradeType
            });

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加客户端升级公告：{request.VersionNo}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysClientUpgradeMgr
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysClientUpgradeEdit(SysClientUpgradeEditRequest request)
        {
            var data = await _sysClientUpgradeDAL.SysClientUpgradeGet(request.Id);
            if (data == null)
            {
                return ResponseBase.CommonError("升级公告不存在");
            }
            data.UpgradeType = request.UpgradeType;
            data.VersionNo = request.VersionNo;
            data.UpgradeContent = request.UpgradeContent;
            data.FileUrl = request.FileUrl;
            await _sysClientUpgradeDAL.SysClientUpgradeEdit(data);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"编辑客户端升级公告：{request.VersionNo}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysClientUpgradeMgr
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysClientUpgradeDel(SysClientUpgradeDelRequest request)
        {
            var data = await _sysClientUpgradeDAL.SysClientUpgradeGet(request.Id);
            if (data == null)
            {
                return ResponseBase.CommonError("升级公告不存在");
            }
            await _sysClientUpgradeDAL.SysClientUpgradeDel(data);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"删除客户端升级公告：{data.VersionNo}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysClientUpgradeMgr
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysClientUpgradePaging(SysClientUpgradePagingRequest request)
        {
            var pagingData = await _sysClientUpgradeDAL.GetPaging(request);
            var output = new List<SysClientUpgradePagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new SysClientUpgradePagingOutput()
                {
                    AgentId = p.AgentId,
                    ClientType = p.ClientType,
                    FileUrl = p.FileUrl,
                    Id = p.Id,
                    Ot = p.Ot,
                    Remark = p.Remark,
                    UpgradeContent = p.UpgradeContent,
                    UpgradeType = p.UpgradeType,
                    VersionNo = p.VersionNo,
                    ClientTypeDesc = EmSysClientUpgradeClientType.GetSysClientUpgradeClientTypeDesc(p.ClientType),
                    UpgradeTypeDesc = EmSysClientUpgradeUpgradeType.GetSysClientUpgradeUpgradeTypeDesc(p.UpgradeType)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysClientUpgradePagingOutput>(pagingData.Item2, output));
        }
    }
}
