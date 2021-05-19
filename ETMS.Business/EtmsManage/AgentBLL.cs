using ETMS.Authority;
using ETMS.Business.EtmsManage.Common;
using ETMS.DataAccess.EtmsManage.Lib;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Config.Menu;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.Agent.Output;
using ETMS.Entity.EtmsManage.Dto.Agent.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class AgentBLL : IAgentBLL
    {
        private readonly ISysAgentDAL _sysAgentDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        private readonly ISysRoleDAL _sysRoleDAL;

        private readonly ISysVersionDAL _sysVersionDAL;

        private readonly ISysUserDAL _sysUserDAL;

        private readonly ISysUserRoleDAL _sysUserRoleDAL;

        public AgentBLL(ISysAgentDAL sysAgentDAL, ISysAgentLogDAL sysAgentLogDAL, ISysRoleDAL sysRoleDAL, ISysVersionDAL sysVersionDAL,
            ISysUserDAL sysUserDAL, ISysUserRoleDAL sysUserRoleDAL)
        {
            this._sysAgentDAL = sysAgentDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
            this._sysRoleDAL = sysRoleDAL;
            this._sysVersionDAL = sysVersionDAL;
            this._sysUserDAL = sysUserDAL;
            this._sysUserRoleDAL = sysUserRoleDAL;
        }

        public async Task<ResponseBase> AgentLogin(AgentLoginRequest request)
        {
            var response = new ResponseBase().GetResponseBadRequest("账号信息错误");
            var agentInfo = await _sysAgentDAL.ExistSysAgentByCode(request.AgentCode);
            if (agentInfo == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            if (agentInfo.IsLock == EmSysAgentIsLock.IsLock)
            {
                return ResponseBase.CommonError("账号已锁定");
            }

            var userInfo = await _sysUserDAL.ExistSysUserByPhone(agentInfo.Id, request.Phone);
            if (userInfo == null)
            {
                return response;
            }
            var pwd = CryptogramHelper.Encrypt3DES(request.Pwd, SystemConfig.CryptogramConfig.Key);
            if (!userInfo.Password.Equals(pwd))
            {
                return response;
            }
            if (userInfo.IsLock == EmSysAgentIsLock.IsLock)
            {
                return ResponseBase.CommonError("员工账号已锁定");
            }

            var token = AgentJwtHelper.GenerateToken(userInfo.AgentId, userInfo.Id, out var exTime);
            var time = DateTime.Now;
            await _sysAgentDAL.UpdateAgentLastLoginTime(userInfo.AgentId, time);
            await _sysUserDAL.UpdateUserLastLoginTime(userInfo.Id, time);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                IpAddress = string.Empty,
                Ot = time,
                AgentId = agentInfo.Id,
                OpContent = $"代理商:{agentInfo.Name},手机号:{agentInfo.Phone}在{time.EtmsToString()}登录",
                Type = (int)EmSysAgentOpLogType.Login,
                IsDeleted = EmIsDeleted.Normal
            }, userInfo.Id);

            var userAuthorityValueMenu = string.Empty;
            if (userInfo.IsAdmin == EmBool.False && userInfo.UserRoleId > 0)
            {
                var myUserRole = await _sysUserRoleDAL.GetRole(userInfo.UserRoleId);
                userAuthorityValueMenu = myUserRole.AuthorityValueMenu;
            }

            var role = await _sysRoleDAL.GetRole(agentInfo.RoleId);
            var output = new AgentLoginOutput()
            {
                Token = token,
                ExpiresTime = exTime,
                Permission = AgentComBusiness.GetPermissionOutput(role.AuthorityValueMenu, userAuthorityValueMenu)
            };
            return response.GetResponseSuccess(output);
        }

        public async Task<ResponseBase> CheckAgentLogin(AgentRequestBase request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            if (agentBucket.SysAgent.IsLock == EmSysAgentIsLock.IsLock)
            {
                return ResponseBase.CommonError("账号已锁定");
            }
            var userInfo = await _sysUserDAL.GetUser(request.LoginUserId);
            if (userInfo == null)
            {
                return ResponseBase.CommonError("员工不存在");
            }
            if (userInfo.IsLock == EmSysAgentIsLock.IsLock)
            {
                return ResponseBase.CommonError("员工账号已锁定");
            }
            var isUserLimitData = false;
            if (userInfo.IsAdmin == EmBool.False && userInfo.UserRoleId > 0)
            {
                var myUserRole = await _sysUserRoleDAL.GetRole(userInfo.UserRoleId);
                isUserLimitData = EmDataLimitType.GetIsDataLimit(myUserRole.AuthorityValueData);
            }
            var role = await _sysRoleDAL.GetRole(agentBucket.SysAgent.RoleId);
            var output = new CheckAgentLoginOutput()
            {
                IsRoleLimitData = EmDataLimitType.GetIsDataLimit(role.AuthorityValueData),
                IsUserLimitData = isUserLimitData
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> AgentLoginInfoGet(AgentLoginInfoGetRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            var agent = agentBucket.SysAgent;
            var output = new AgentLoginInfoGetOuptut()
            {
                AgentId = agent.Id,
                Name = agent.Name,
                Phone = agent.Phone
            };
            var userAuthorityValueMenu = string.Empty;
            var userInfo = await _sysUserDAL.GetUser(request.LoginUserId);
            if (userInfo.IsAdmin == EmBool.False && userInfo.UserRoleId > 0)
            {
                var myUserRole = await _sysUserRoleDAL.GetRole(userInfo.UserRoleId);
                userAuthorityValueMenu = myUserRole.AuthorityValueMenu;
            }

            var role = await _sysRoleDAL.GetRole(agent.RoleId);
            output.RouteConfigs = AgentComBusiness.GetRouteConfigs(role.AuthorityValueMenu, userAuthorityValueMenu);
            return ResponseBase.Success(output);
        }

        private async Task<AgentLoginInfoGetBascOutput> AgentGetViewInfo(SysAgentBucket agentBucket)
        {
            var agent = agentBucket.SysAgent;
            var output = new AgentLoginInfoGetBascOutput()
            {
                Address = agent.Address,
                AgentId = agent.Id,
                EtmsSmsCount = agent.EtmsSmsCount,
                IdCard = agent.IdCard,
                Name = agent.Name,
                Phone = agent.Phone,
                TagKey = agent.TagKey,
                Code = agent.Code,
                AgentEtmsAccounts = new List<AgentEtmsAccountOutput>()
            };
            if (agentBucket.SysAgentEtmsAccounts != null && agentBucket.SysAgentEtmsAccounts.Count > 0)
            {
                var versions = await _sysVersionDAL.GetVersions();
                foreach (var p in agentBucket.SysAgentEtmsAccounts)
                {
                    var version = versions.FirstOrDefault(j => j.Id == p.VersionId);
                    output.AgentEtmsAccounts.Add(new AgentEtmsAccountOutput()
                    {
                        EtmsCount = p.EtmsCount,
                        VersionId = p.VersionId,
                        VersionName = version?.Name
                    });
                }
            }
            return output;
        }

        public async Task<ResponseBase> AgentLoginInfoGetBasc(AgentLoginInfoGetBascRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            return ResponseBase.Success(await AgentGetViewInfo(agentBucket));
        }

        public async Task<ResponseBase> AgentLoginPermissionGet(AgentLoginPermissionGetRequest request)
        {
            var agent = (await _sysAgentDAL.GetAgent(request.LoginAgentId)).SysAgent;
            var role = await _sysRoleDAL.GetRole(agent.RoleId);

            var userInfo = await _sysUserDAL.GetUser(request.LoginUserId);
            var userAuthorityValueMenu = string.Empty;
            if (userInfo.IsAdmin == EmBool.False && userInfo.UserRoleId > 0)
            {
                var myUserRole = await _sysUserRoleDAL.GetRole(userInfo.UserRoleId);
                userAuthorityValueMenu = myUserRole.AuthorityValueMenu;
            }

            return ResponseBase.Success(AgentComBusiness.GetPermissionOutput(role.AuthorityValueMenu, userAuthorityValueMenu));
        }

        public async Task<ResponseBase> AgentChangPwd(AgentChangPwdRequest request)
        {
            var userInfo = await _sysUserDAL.GetUser(request.LoginUserId);
            if (userInfo == null)
            {
                return ResponseBase.CommonError("用户不存在");
            }

            userInfo.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _sysUserDAL.EditUser(userInfo);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"修改密码:名称:{userInfo.Name},手机号码,{userInfo.Phone}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AgentAdd(AgentAddRequest request)
        {
            var existAgent = await _sysAgentDAL.ExistSysAgentByPhone(request.Phone);
            if (existAgent != null)
            {
                return ResponseBase.CommonError("手机号码已存在");
            }
            var existCode = await _sysAgentDAL.ExistSysAgentByCode(request.Code);
            if (existCode != null)
            {
                return ResponseBase.CommonError("编码已存在");
            }
            var newAgent = new SysAgent()
            {
                LastLoginOt = null,
                IsLock = request.IsLock ? EmSysAgentIsLock.IsLock : EmSysAgentIsLock.Normal,
                Address = request.Address,
                CreatedAgentId = request.LoginAgentId,
                EtmsSmsCount = 0,
                IdCard = request.IdCard,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Ot = DateTime.Now,
                Phone = request.Phone,
                Remark = request.Remark,
                RoleId = request.RoleId,
                TagKey = string.Empty,
                KefuPhone = request.KefuPhone,
                KefuQQ = request.KefuQQ,
                Code = request.Code
            };
            await _sysAgentDAL.AddAgent(newAgent, request.LoginUserId);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"新增代理商:名称:{newAgent.Name},手机号码,{newAgent.Phone}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AgentGet(AgentGetRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            var p = agentBucket.SysAgent;
            return ResponseBase.Success(new AgentGetOutput()
            {
                Address = p.Address,
                Id = p.Id,
                IdCard = p.IdCard,
                IsLock = p.IsLock == EmSysAgentIsLock.IsLock,
                Name = p.Name,
                Phone = p.Phone,
                Remark = p.Remark,
                RoleId = p.RoleId,
                TagKey = p.TagKey,
                KefuPhone = p.KefuPhone,
                KefuQQ = p.KefuQQ,
                Code = p.Code
            });
        }
        public async Task<ResponseBase> AgentGetView(AgentGetViewRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            return ResponseBase.Success(await AgentGetViewInfo(agentBucket));
        }

        public async Task<ResponseBase> AgentEdit(AgentEditRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            var p = agentBucket.SysAgent;
            var existAgent = await _sysAgentDAL.ExistSysAgentByPhone(request.Phone, p.Id);
            if (existAgent != null)
            {
                return ResponseBase.CommonError("手机号码已存在");
            }
            var existCode = await _sysAgentDAL.ExistSysAgentByCode(request.Code, p.Id);
            if (existCode != null)
            {
                return ResponseBase.CommonError("编码已存在");
            }

            p.RoleId = request.RoleId;
            p.Name = request.Name;
            p.Phone = request.Phone;
            p.IdCard = request.IdCard;
            p.Address = request.Address;
            p.IsLock = request.IsLock ? EmSysAgentIsLock.IsLock : EmSysAgentIsLock.Normal;
            p.Remark = request.Remark;
            p.KefuQQ = request.KefuQQ;
            p.KefuPhone = request.KefuPhone;
            p.Code = request.Code;
            await _sysAgentDAL.EditAgent(p);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"编辑代理商:名称:{request.Name},手机号码,{request.Phone}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AgentDel(AgentDelRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            if (await _sysAgentDAL.IsCanNotDelete(request.Id))
            {
                return ResponseBase.CommonError("代理商存在交易记录，无法删除");
            }
            await _sysAgentDAL.DelAgent(request.Id);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"删除代理商:名称:{agentBucket.SysAgent.Name},手机号码,{agentBucket.SysAgent.Phone}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AgentPaging(AgentPagingRequest request)
        {
            var pagingData = await _sysAgentDAL.GetPaging(request);
            var output = new List<AgentPagingOutput>();
            if (pagingData.Item1.Count() > 0)
            {
                var allVersion = await _sysVersionDAL.GetVersions();
                var allRole = await _sysRoleDAL.GetRoles();
                foreach (var p in pagingData.Item1)
                {
                    var agent = await _sysAgentDAL.GetAgent(p.Id);
                    var myAccounts = new List<MyEtmsAccountOutput>();
                    foreach (var j in agent.SysAgentEtmsAccounts)
                    {
                        myAccounts.Add(new MyEtmsAccountOutput()
                        {
                            EtmsCount = j.EtmsCount,
                            VersionId = j.VersionId,
                            VersionName = GetVersionName(allVersion, j.VersionId)
                        });
                    }
                    output.Add(new AgentPagingOutput()
                    {
                        Id = p.Id,
                        Address = p.Address,
                        EtmsSmsCount = p.EtmsSmsCount,
                        IdCard = p.IdCard,
                        IsLock = p.IsLock,
                        LastLoginOtDesc = p.LastLoginOt.EtmsToString(),
                        Name = p.Name,
                        Ot = p.Ot,
                        Phone = p.Phone,
                        Remark = p.Remark,
                        RoleId = p.RoleId,
                        RoleName = GetRoleName(allRole, p.RoleId),
                        MyAccounts = myAccounts,
                        IsLockDesc = EmSysAgentIsLock.GetSysAgentIsLockDesc(p.IsLock),
                        KefuPhone = p.KefuPhone,
                        KefuQQ = p.KefuQQ,
                        Label = p.Name,
                        Value = p.Id,
                        Code = p.Code
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AgentPagingOutput>(pagingData.Item2, output));
        }

        private string GetVersionName(List<SysVersion> versions, int versionId)
        {
            var version = versions.FirstOrDefault(p => p.Id == versionId);
            return version?.Name;
        }

        private string GetRoleName(List<SysRole> sysRoles, int roleId)
        {
            var myRole = sysRoles.FirstOrDefault(p => p.Id == roleId);
            return myRole?.Name;
        }

        public async Task<ResponseBase> AgentChangeSmsCount(AgentChangeSmsCountRequest request)
        {
            if (request.ChangeType == AgentChangeEnum.Add)
            {
                return await AgentAddSmsCount(request);
            }
            else
            {
                return await AgentDeductionSmsCount(request);
            }
        }

        private async Task<ResponseBase> AgentAddSmsCount(AgentChangeSmsCountRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            var agent = agentBucket.SysAgent;
            var now = DateTime.Now;
            await _sysAgentDAL.SmsCountAdd(request.Id, request.ChangeCount);
            await _sysAgentLogDAL.AddSysAgentSmsLog(new SysAgentSmsLog()
            {
                AgentId = request.Id,
                ChangeCount = request.ChangeCount,
                ChangeType = EmSysAgentSmsLogChangeType.Add,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = request.Remark,
                Sum = request.Sum
            }, request.LoginUserId);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"增加代理商短信条数:名称:{agent.Name},手机号码,{agent.Phone},增加条数:{request.ChangeCount}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> AgentDeductionSmsCount(AgentChangeSmsCountRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            var agent = agentBucket.SysAgent;
            var now = DateTime.Now;
            await _sysAgentDAL.SmsCountDeduction(request.Id, request.ChangeCount);
            await _sysAgentLogDAL.AddSysAgentSmsLog(new SysAgentSmsLog()
            {
                AgentId = request.Id,
                ChangeCount = request.ChangeCount,
                ChangeType = EmSysAgentSmsLogChangeType.Deduction,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = request.Remark,
                Sum = request.Sum
            }, request.LoginUserId);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"扣减代理商短信条数:名称:{agent.Name},手机号码,{agent.Phone},扣减条数:{request.ChangeCount}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AgentChangeEtmsCount(AgentChangeEtmsCountRequest request)
        {
            if (request.ChangeType == AgentChangeEnum.Add)
            {
                return await AgentAddEtmsCount(request);
            }
            else
            {
                return await AgentDeductionEtmsCount(request);
            }
        }

        private async Task<ResponseBase> AgentAddEtmsCount(AgentChangeEtmsCountRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            await _sysAgentDAL.EtmsAccountAdd(request.Id, request.VersionId, request.ChangeCount);
            var agent = agentBucket.SysAgent;
            var now = DateTime.Now;
            await _sysAgentLogDAL.AddSysAgentEtmsAccountLog(new SysAgentEtmsAccountLog()
            {
                AgentId = request.Id,
                ChangeCount = request.ChangeCount,
                ChangeType = EmSysAgentEtmsAccountLogChangeType.Add,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = request.Remark,
                Sum = request.Sum,
                VersionId = request.VersionId
            }, request.LoginUserId);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"增加代理商授权点数:名称:{agent.Name},手机号码,{agent.Phone},增加授权点数:{request.ChangeCount}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> AgentDeductionEtmsCount(AgentChangeEtmsCountRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            var myVersion = agentBucket.SysAgentEtmsAccounts.FirstOrDefault(p => p.VersionId == request.VersionId);
            if (myVersion == null)
            {
                return ResponseBase.CommonError("此代理商未购买过此版本系统");
            }
            await _sysAgentDAL.EtmsAccountDeduction(request.Id, request.VersionId, request.ChangeCount);
            var agent = agentBucket.SysAgent;
            var now = DateTime.Now;
            await _sysAgentLogDAL.AddSysAgentEtmsAccountLog(new SysAgentEtmsAccountLog()
            {
                AgentId = request.Id,
                ChangeCount = request.ChangeCount,
                ChangeType = EmSysAgentEtmsAccountLogChangeType.Deduction,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = request.Remark,
                Sum = request.Sum,
                VersionId = request.VersionId
            }, request.LoginUserId);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"扣减代理商授权点数:名称:{agent.Name},手机号码,{agent.Phone},扣减授权点数:{request.ChangeCount}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AgentOpLogPaging(AgentOpLogPagingRequest request)
        {
            var pagingData = await _sysAgentLogDAL.GetPagingOpLog(request);
            var output = new List<AgentOpLogPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new AgentOpLogPagingOutput()
                {
                    AgentId = p.AgentId,
                    AgentName = p.AgentName,
                    AgentPhone = p.AgentPhone,
                    CId = p.Id,
                    IpAddress = p.IpAddress,
                    OpContent = p.OpContent,
                    Ot = p.Ot,
                    Type = p.Type,
                    TypeDesc = EmSysAgentOpLogType.GetSysAgentOpLogTypeDesc(p.Type)
                }); ;
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AgentOpLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AgentEtmsAccountLogPaging(AgentEtmsAccountLogPagingRequest request)
        {
            var pagingData = await _sysAgentLogDAL.GetPagingEtmsAccountLog(request);
            var output = new List<AgentEtmsAccountLogPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var allVersion = await _sysVersionDAL.GetVersions();
                foreach (var p in pagingData.Item1)
                {
                    var version = allVersion.FirstOrDefault(j => j.Id == p.VersionId);
                    output.Add(new AgentEtmsAccountLogPagingOutput()
                    {
                        AgentId = p.AgentId,
                        AgentName = p.AgentName,
                        AgentPhone = p.AgentPhone,
                        ChangeCount = p.ChangeCount,
                        ChangeType = p.ChangeType,
                        ChangeTypeDesc = EmSysAgentEtmsAccountLogChangeType.GetSysAgentEtmsAccountLogChangeTypeDesc(p.ChangeType),
                        CId = p.Id,
                        IsDeleted = p.IsDeleted,
                        Ot = p.Ot,
                        Remark = p.Remark,
                        Sum = p.Sum,
                        VersionId = p.VersionId,
                        VersionDesc = version?.Name,
                        ChangeCountDesc = EmSysAgentEtmsAccountLogChangeType.GetChangeCountDesc(p.ChangeCount, p.ChangeType)
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AgentEtmsAccountLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AgentSmsLogPaging(AgentSmsLogPagingRequest request)
        {
            var pagingData = await _sysAgentLogDAL.GetPagingSmsLog(request);
            var output = new List<AgentSmsLogPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new AgentSmsLogPagingOutput()
                    {
                        AgentId = p.AgentId,
                        AgentName = p.AgentName,
                        AgentPhone = p.AgentPhone,
                        ChangeCount = p.ChangeCount,
                        ChangeType = p.ChangeType,
                        ChangeTypeDesc = EmSysAgentSmsLogChangeType.GetSysAgentSmsLogChangeTypeDesc(p.ChangeType),
                        Id = p.Id,
                        IsDeleted = p.IsDeleted,
                        Ot = p.Ot,
                        Remark = p.Remark,
                        Sum = p.Sum,
                        ChangeCountDesc = EmSysAgentSmsLogChangeType.GetChangeCountDesc(p.ChangeCount, p.ChangeType)
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AgentSmsLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> VersionAdd(VersionAddRequest request)
        {
            await _sysVersionDAL.AddVersion(new SysVersion()
            {
                EtmsAuthorityValue = AgentComBusiness.GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds),
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Remark = request.Remark,
                DetailInfo = request.DetailInfo,
                Img = string.Empty
            });
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"添加系统版本:{request.Name}", EmSysAgentOpLogType.VersionMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> VersionEdit(VersionEditRequest request)
        {
            var version = await _sysVersionDAL.GetVersion(request.CId);
            if (version == null)
            {
                return ResponseBase.CommonError("系统版本不存在");
            }
            version.Name = request.Name;
            version.Remark = request.Remark;
            version.DetailInfo = request.DetailInfo;
            version.EtmsAuthorityValue = AgentComBusiness.GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds);
            await _sysVersionDAL.EditVersion(version);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"编辑系统版本:{request.Name}", EmSysAgentOpLogType.VersionMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> VersionDel(VersionDelRequest request)
        {
            var version = await _sysVersionDAL.GetVersion(request.CId);
            if (version == null)
            {
                return ResponseBase.CommonError("系统版本不存在");
            }
            if (await _sysVersionDAL.IsCanNotDelete(request.CId))
            {
                return ResponseBase.CommonError("此版本已使用无法删除");
            }
            await _sysVersionDAL.DelVersion(request.CId);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"删除系统版本:{version.Name}", EmSysAgentOpLogType.VersionMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> VersionGet(VersionGetRequest request)
        {
            var version = await _sysVersionDAL.GetVersion(request.CId);
            if (version == null)
            {
                return ResponseBase.CommonError("系统版本不存在");
            }
            var myAuthorityValueMenu = version.EtmsAuthorityValue.Split('|');
            var pageWeight = myAuthorityValueMenu[0].ToBigInteger();
            var actionWeight = myAuthorityValueMenu[1].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var authorityCoreAction = new AuthorityCore(actionWeight);
            var myMenuConfigs = EtmsHelper.DeepCopy(PermissionData.MenuConfigs);
            AgentComBusiness.MenuConfigsHandle(myMenuConfigs, authorityCorePage, authorityCoreAction);
            return ResponseBase.Success(new VersionGetOutput()
            {
                Name = version.Name,
                Remark = version.Remark,
                DetailInfo = version.DetailInfo,
                Menus = AgentComBusiness.GetSysMenuViewOutputs(myMenuConfigs),
            });
        }

        public ResponseBase VersionDefaultGet(VersionDefaultGetRequest request)
        {
            return ResponseBase.Success(AgentComBusiness.GetSysMenuViewOutputs(PermissionData.MenuConfigs));
        }

        public async Task<ResponseBase> VersionGetAll(VersionGetAllRequest request)
        {
            var versions = await _sysVersionDAL.GetVersions();
            var output = new List<VersionGetAllOutput>();
            foreach (var p in versions)
            {
                output.Add(new VersionGetAllOutput()
                {
                    CId = p.Id,
                    Label = p.Name,
                    Name = p.Name,
                    Remark = p.Remark,
                    DetailInfo = p.DetailInfo,
                    Value = p.Id
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> SysRoleListGet(SysRoleListGetRequest request)
        {
            var roles = await _sysRoleDAL.GetRoles();
            var output = new List<SysRoleListGetOutput>();
            if (roles.Any())
            {
                foreach (var p in roles)
                {
                    output.Add(new SysRoleListGetOutput()
                    {
                        CId = p.Id,
                        Label = p.Name,
                        Name = p.Name,
                        Remark = p.Remark,
                        Value = p.Id,
                        DataLimitDesc = EmDataLimitType.GetIsDataLimit(p.AuthorityValueData) ? "是" : "否"
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> SysRoleAdd(SysRoleAddRequest request)
        {
            await _sysRoleDAL.AddRole(new SysRole()
            {
                AuthorityValueData = EmDataLimitType.GetAuthorityValueData(request.IsMyDataLimit),
                AuthorityValueMenu = AgentComBusiness.GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds),
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Remark = request.Remark
            });
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"添加角色:{request.Name}", EmSysAgentOpLogType.RoleMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysRoleEdit(SysRoleEditRequest request)
        {
            var role = await _sysRoleDAL.GetRole(request.CId);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            role.Name = request.Name;
            role.Remark = request.Remark;
            role.AuthorityValueMenu = AgentComBusiness.GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds);
            role.AuthorityValueData = EmDataLimitType.GetAuthorityValueData(request.IsMyDataLimit);
            await _sysRoleDAL.EditRole(role);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"编辑角色:{request.Name}", EmSysAgentOpLogType.RoleMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysRoleGet(SysRoleGetRequest request)
        {
            var role = await _sysRoleDAL.GetRole(request.CId);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            var myAuthorityValueMenu = role.AuthorityValueMenu.Split('|');
            var pageWeight = myAuthorityValueMenu[0].ToBigInteger();
            var actionWeight = myAuthorityValueMenu[1].ToBigInteger();
            var authorityCorePage = new AuthorityCore(pageWeight);
            var authorityCoreAction = new AuthorityCore(actionWeight);
            var myMenuConfigs = EtmsHelper.DeepCopy(AgentPermissionData.MenuConfigs);
            AgentComBusiness.MenuConfigsHandle(myMenuConfigs, authorityCorePage, authorityCoreAction);
            return ResponseBase.Success(new SysRoleGetOutput()
            {
                Name = role.Name,
                Remark = role.Remark,
                Menus = AgentComBusiness.GetSysMenuViewOutputs(myMenuConfigs),
                IsDataLimit = EmDataLimitType.GetIsDataLimit(role.AuthorityValueData)
            });
        }

        public ResponseBase SysRoleDefaultGet(SysRoleDefaultGetRequest request)
        {
            return ResponseBase.Success(AgentComBusiness.GetSysMenuViewOutputs(AgentPermissionData.MenuConfigs));
        }

        public async Task<ResponseBase> SysRoleDel(SysRoleDelRequest request)
        {
            var role = await _sysRoleDAL.GetRole(request.CId);
            if (role == null)
            {
                return ResponseBase.CommonError("角色不存在");
            }
            if (await _sysRoleDAL.IsCanNotDel(request.CId))
            {
                return ResponseBase.CommonError("此角色有对应的代理商，无法删除");
            }
            await _sysRoleDAL.DelRole(request.CId);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"删除角色:{role.Name}", EmSysAgentOpLogType.RoleMange);
            return ResponseBase.Success();
        }
    }
}
