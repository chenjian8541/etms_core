using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.Agent3.Output;
using ETMS.Entity.EtmsManage.Dto.Agent3.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysNoticeBulletinBLL : ISysNoticeBulletinBLL
    {
        private readonly ISysNoticeBulletinDAL _sysNoticeBulletinDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        public SysNoticeBulletinBLL(ISysNoticeBulletinDAL sysNoticeBulletinDAL, ISysAgentLogDAL sysAgentLogDAL)
        {
            this._sysNoticeBulletinDAL = sysNoticeBulletinDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
        }

        public async Task<ResponseBase> SysSysNoticeBulletinAdd(SysSysNoticeBulletinAddRequest request)
        {
            var entity = new SysNoticeBulletin()
            {
                AgentId = request.LoginAgentId,
                UserId = request.LoginUserId,
                EndTime = request.EndTime,
                IsAdvertise = request.IsAdvertise,
                IsDeleted = EmIsDeleted.Normal,
                LinkUrl = request.LinkUrl,
                Remark = string.Empty,
                Status = EmNoticeBulletinStatus.Normal,
                Title = request.Title
            };
            await _sysNoticeBulletinDAL.AddSysNoticeBulletin(entity);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加系统公告：{request.Title}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysNoticeBulletin
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysSysNoticeBulletinDel(SysSysNoticeBulletinDelRequest request)
        {
            await _sysNoticeBulletinDAL.DelSysNoticeBulletin(request.Id);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = "删除系统公告",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysNoticeBulletin
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysSysNoticeBulletinPaging(SysSysNoticeBulletinPagingRequest request)
        {
            var pagingData = await _sysNoticeBulletinDAL.GetPaging(request);
            var output = new List<SysSysNoticeBulletinPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new SysSysNoticeBulletinPagingOutput()
                {
                    EndTime = p.EndTime.EtmsToDateString(),
                    Id = p.Id,
                    IsAdvertise = p.IsAdvertise,
                    LinkUrl = p.LinkUrl,
                    Status = p.Status,
                    Title = p.Title
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysSysNoticeBulletinPagingOutput>(pagingData.Item2, output));
        }
    }
}
