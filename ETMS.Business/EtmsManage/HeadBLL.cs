using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.Alien;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.Head.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.Alien;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using ETMS.Business.Alien;
using ETMS.Entity.EtmsManage.Dto.Head.Output;
using ETMS.Business.EtmsManage.Common;

namespace ETMS.Business.EtmsManage
{
    public class HeadBLL : IHeadBLL
    {
        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        private readonly IMgHeadDAL _mgHeadDAL;

        private readonly IMgTenantsDAL _mgTenantsDAL;

        private readonly IMgUserDAL _mgUserDAL;

        private readonly IMgRoleDAL _mgRoleDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysAgentDAL _sysAgentDAL;

        private readonly IMgUserOpLogDAL _mgUserOpLogDAL;

        public HeadBLL(ISysAgentLogDAL sysAgentLogDAL, IMgHeadDAL mgHeadDAL, IMgTenantsDAL mgTenantsDAL,
            IMgUserDAL mgUserDAL, IMgRoleDAL mgRoleDAL, ISysTenantDAL sysTenantDAL, ISysAgentDAL sysAgentDAL,
            IMgUserOpLogDAL mgUserOpLogDAL)
        {
            this._sysAgentLogDAL = sysAgentLogDAL;
            this._mgHeadDAL = mgHeadDAL;
            this._mgTenantsDAL = mgTenantsDAL;
            this._mgUserDAL = mgUserDAL;
            this._mgRoleDAL = mgRoleDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysAgentDAL = sysAgentDAL;
            this._mgUserOpLogDAL = mgUserOpLogDAL;
        }

        public void InitHeadId(int headId)
        {
            this.InitDataAccess(headId, _mgTenantsDAL, _mgUserDAL, _mgRoleDAL);
        }

        public async Task<ResponseBase> HeadGetSimple(HeadGetRequest request)
        {
            var myHead = await _mgHeadDAL.GetMgHead(request.CId);
            if (myHead == null)
            {
                return ResponseBase.CommonError("企业不存在");
            }
            return ResponseBase.Success(new HeadGetSimpleOutput()
            {
                Address = myHead.Address,
                CId = myHead.Id,
                HeadCode = myHead.HeadCode,
                LinkMan = myHead.LinkMan,
                Name = myHead.Name,
                Ot = myHead.Ot,
                Phone = myHead.Phone,
                Status = myHead.Status,
                TenantCount = myHead.TenantCount
            });
        }

        public async Task<ResponseBase> HeadGet(HeadGetRequest request)
        {
            var myHead = await _mgHeadDAL.GetMgHead(request.CId);
            if (myHead == null)
            {
                return ResponseBase.CommonError("企业不存在");
            }
            this.InitHeadId(request.CId);
            var output = new HeadGetOutput();
            output.HeadBascInfo = new HeadBascInfoOutput()
            {
                Address = myHead.Address,
                Name = myHead.Name,
                CId = myHead.Id,
                HeadCode = myHead.HeadCode,
                LinkMan = myHead.LinkMan,
                Ot = myHead.Ot,
                Phone = myHead.Phone,
                Status = myHead.Status,
                TenantCount = myHead.TenantCount,
                StatusDesc = EmMgHeadStatus.GetHeadStatusDesc(myHead.Status)
            };
            output.HeadTenants = new List<HeadTenantOutput>();
            var myAllTenants = await _mgTenantsDAL.GetMgTenants();
            if (myAllTenants != null && myAllTenants.Any())
            {
                foreach (var p in myAllTenants)
                {
                    var myTenant = await _sysTenantDAL.GetTenant(p.TenantId);
                    if (myTenant == null)
                    {
                        continue;
                    }
                    output.HeadTenants.Add(new HeadTenantOutput()
                    {
                        Address = myTenant.Address,
                        BuyStatus = myTenant.BuyStatus,
                        BuyStatusDesc = EmSysTenantBuyStatus.GetSysTenantBuyStatusDesc(myTenant.BuyStatus),
                        Status = EmSysTenantStatus.GetSysTenantStatus(myTenant.Status, myTenant.ExDate),
                        StatusDesc = EmSysTenantStatus.GetSysTenantStatusDesc(myTenant.Status, myTenant.ExDate),
                        ExDateDesc = myTenant.ExDate.EtmsToDateString(),
                        LinkMan = myTenant.LinkMan,
                        Name = myTenant.Name,
                        Phone = myTenant.Phone,
                        Remark = myTenant.Remark,
                        SmsCount = myTenant.SmsCount,
                        SmsSignature = myTenant.SmsSignature,
                        TenantCode = myTenant.TenantCode,
                        Id = p.Id
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> HeadGetPaging(HeadGetPagingRequest request)
        {
            var pagingData = await _mgHeadDAL.GetPaging(request);
            var output = new List<HeadGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxAgent = new AgentDataTempBox<SysAgent>();
                foreach (var p in pagingData.Item1)
                {
                    var agent = await AgentComBusiness.GetAgent(tempBoxAgent, _sysAgentDAL, p.AgentId);
                    output.Add(new HeadGetPagingOutput()
                    {
                        Address = p.Address,
                        AgentName = agent?.Name,
                        Name = p.Name,
                        CId = p.Id,
                        HeadCode = p.HeadCode,
                        LinkMan = p.LinkMan,
                        Ot = p.Ot,
                        Phone = p.Phone,
                        Status = p.Status,
                        TenantCount = p.TenantCount,
                        StatusDesc = EmMgHeadStatus.GetHeadStatusDesc(p.Status)
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<HeadGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> HeadAdd(HeadAddRequest request)
        {
            if (await _mgHeadDAL.ExistHeadCode(request.HeadCode))
            {
                return ResponseBase.CommonError("企业编码已存在");
            }
            var now = DateTime.Now;
            var myHead = new MgHead()
            {
                Address = request.Address,
                AgentId = request.LoginAgentId,
                HeadCode = request.HeadCode,
                IsDeleted = EmIsDeleted.Normal,
                LinkMan = request.LinkMan,
                Name = request.Name,
                Ot = now,
                Phone = request.Phone,
                Status = EmMgHeadStatus.Normal,
                TenantCount = 0,
                TenantId = 0,
            };
            await _mgHeadDAL.AddMgHead(myHead);
            this.InitHeadId(myHead.Id);

            var myRole = new MgRole()
            {
                IsDeleted = EmIsDeleted.Normal,
                Name = "超级管理员",
                HeadId = myHead.Id,
                Remark = string.Empty,
                AuthorityValueData = string.Empty,
                AuthorityValueMenu = "262142||262142"
            };
            await _mgRoleDAL.AddRole(myRole);

            var myUser = new MgUser()
            {
                Address = request.Address,
                Gender = null,
                HeadId = myHead.Id,
                IsAdmin = EmBool.True,
                IsDeleted = EmIsDeleted.Normal,
                IsLock = EmSysAgentIsLock.Normal,
                JobAtTenants = string.Empty,
                LastLoginOt = null,
                MgRoleId = myRole.Id,
                Name = request.LinkMan,
                OrganizationId = null,
                OrgParentsAll = string.Empty,
                Ot = now,
                Phone = request.Phone,
                Password = CryptogramHelper.Encrypt3DES(SystemConfig.ComConfig.DefaultPwd, SystemConfig.CryptogramConfig.Key),
            };
            await _mgUserDAL.AddUser(myUser);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加企业：{request.Name}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.HeadMgr
            }, request.LoginUserId);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HeadEdit(HeadEditRequest request)
        {
            var myHead = await _mgHeadDAL.GetMgHead(request.CId);
            if (myHead == null)
            {
                return ResponseBase.CommonError("企业不存在");
            }
            this.InitHeadId(myHead.Id);
            if (await _mgHeadDAL.ExistHeadCode(request.HeadCode, request.CId))
            {
                return ResponseBase.CommonError("企业编码已存在");
            }
            myHead.HeadCode = request.HeadCode;
            myHead.Name = request.Name;
            myHead.Phone = request.Phone;
            myHead.Address = request.Address;
            myHead.LinkMan = request.LinkMan;
            myHead.Status = request.NewStatus;
            await _mgHeadDAL.EditMgHead(myHead);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"编辑企业：{request.Name}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.HeadMgr
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HeadDel(HeadDelRequest request)
        {
            var myHead = await _mgHeadDAL.GetMgHead(request.CId);
            if (myHead == null)
            {
                return ResponseBase.CommonError("企业不存在");
            }
            await _mgHeadDAL.DelMgHead(request.CId);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"删除企业：{myHead.Name}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.HeadMgr
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HeadAddTenant(HeadAddTenantRequest request)
        {
            var myHead = await _mgHeadDAL.GetMgHead(request.HeadId);
            if (myHead == null)
            {
                return ResponseBase.CommonError("企业不存在");
            }
            this.InitHeadId(request.HeadId);
            if (await _mgTenantsDAL.ExistTenant(request.TenantId))
            {
                return ResponseBase.CommonError("校区已存在");
            }
            var myTenant = await _sysTenantDAL.GetTenant(request.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("校区不存在");
            }
            await _mgTenantsDAL.AddMgTenant(new MgTenants()
            {
                CreateTime = DateTime.Now,
                HeadId = request.HeadId,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = request.TenantId
            });
            await _mgHeadDAL.UpdateTenantCount(request.HeadId);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加校区：{myHead.Name}->{myTenant.Name}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.HeadMgr
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HeadRemoveTenant(HeadRemoveTenantRequest request)
        {
            var log = await _mgTenantsDAL.GetMgTenant(request.CId);
            if (log == null)
            {
                return ResponseBase.CommonError("记录不存在");
            }
            this.InitHeadId(log.HeadId);
            await _mgTenantsDAL.DelMgLog(request.CId);
            await _mgHeadDAL.UpdateTenantCount(log.HeadId);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"移除校区：{request.HeadName}->{request.TenantName}",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.HeadMgr
            }, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HeadUserOpLogGetPaging(HeadUserOpLogGetPagingRequest request)
        {
            var pagingData = await _mgUserOpLogDAL.GetViewPaging(request);
            var output = new List<HeadUserOpLogGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new HeadUserOpLogGetPagingOutput()
                    {
                        ClientType = p.ClientType,
                        Id = p.Id,
                        IpAddress = p.IpAddress,
                        MgUserId = p.MgUserId,
                        Name = p.Name,
                        OpContent = p.OpContent,
                        Ot = DateTime.Now,
                        Phone = p.Phone,
                        Remark = p.Remark,
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<HeadUserOpLogGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
