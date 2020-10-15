using ETMS.Authority;
using ETMS.Business.EtmsManage.Common;
using ETMS.DataAccess.EtmsManage.Lib;
using ETMS.DataAccess.Lib;
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

        public AgentBLL(ISysAgentDAL sysAgentDAL, ISysAgentLogDAL sysAgentLogDAL, ISysRoleDAL sysRoleDAL, ISysVersionDAL sysVersionDAL)
        {
            this._sysAgentDAL = sysAgentDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
            this._sysRoleDAL = sysRoleDAL;
            this._sysVersionDAL = sysVersionDAL;
        }

        public async Task<ResponseBase> AgentLogin(AgentLoginRequest request)
        {
            var response = new ResponseBase().GetResponseBadRequest("账号信息错误");
            var agentInfo = await _sysAgentDAL.ExistSysAgentByPhone(request.Phone);
            if (agentInfo == null)
            {
                return response;
            }
            var pwd = CryptogramHelper.Encrypt3DES(request.Pwd, SystemConfig.CryptogramConfig.Key);
            if (!agentInfo.Password.Equals(pwd))
            {
                return response;
            }
            if (agentInfo.IsLock == EmSysAgentIsLock.IsLock)
            {
                return ResponseBase.CommonError("账号已锁定");
            }

            var token = AgentJwtHelper.GenerateToken(agentInfo.Id, out var exTime);
            var time = DateTime.Now;
            await _sysAgentDAL.UpdateAgentLastLoginTime(agentInfo.Id, time);
            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                IpAddress = string.Empty,
                Ot = time,
                AgentId = agentInfo.Id,
                OpContent = $"代理商:{agentInfo.Name},手机号:{agentInfo.Phone}在{time.EtmsToString()}登录",
                Type = (int)EmSysAgentOpLogType.Login,
                IsDeleted = EmIsDeleted.Normal
            });
            var role = await _sysRoleDAL.GetRole(agentInfo.RoleId);
            var output = new AgentLoginOutput()
            {
                Token = token,
                ExpiresTime = exTime,
                Permission = AgentComBusiness.GetPermissionOutput(role.AuthorityValueMenu)
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
            var role = await _sysRoleDAL.GetRole(agentBucket.SysAgent.RoleId);
            var output = new CheckAgentLoginOutput()
            {
                IsLimitData = EmDataLimitType.GetIsDataLimit(role.AuthorityValueData)
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
            var role = await _sysRoleDAL.GetRole(agent.RoleId);
            output.RouteConfigs = AgentComBusiness.GetRouteConfigs(role.AuthorityValueMenu);
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> AgentLoginInfoGetBasc(AgentLoginInfoGetBascRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
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
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> AgentLoginPermissionGet(AgentLoginPermissionGetRequest request)
        {
            var agent = (await _sysAgentDAL.GetAgent(request.LoginAgentId)).SysAgent;
            var role = await _sysRoleDAL.GetRole(agent.RoleId);
            return ResponseBase.Success(AgentComBusiness.GetPermissionOutput(role.AuthorityValueMenu));
        }

        public async Task<ResponseBase> AgentChangPwd(AgentChangPwdRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            var agent = agentBucket.SysAgent;
            agent.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _sysAgentDAL.EditAgent(agent);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"修改密码:名称:{agent.Name},手机号码,{agent.Phone}", EmSysAgentOpLogType.AgentMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AgentAdd(AgentAddRequest request)
        {
            var existAgent = await _sysAgentDAL.ExistSysAgentByPhone(request.Phone);
            if (existAgent != null)
            {
                return ResponseBase.CommonError("手机号码已存在");
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
                Password = CryptogramHelper.Encrypt3DES("88888888", SystemConfig.CryptogramConfig.Key),
                Phone = request.Phone,
                Remark = request.Remark,
                RoleId = request.RoleId,
                TagKey = string.Empty
            };
            await _sysAgentDAL.AddAgent(newAgent);
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
                TagKey = p.TagKey
            });
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
            p.RoleId = request.RoleId;
            p.Name = request.Name;
            p.Phone = request.Phone;
            p.IdCard = request.IdCard;
            p.Address = request.Address;
            p.IsLock = request.IsLock ? EmSysAgentIsLock.IsLock : EmSysAgentIsLock.Normal;
            p.Remark = request.Remark;
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

        public async Task<ResponseBase> AgentSetPwd(AgentSetPwdRequest request)
        {
            var agentBucket = await _sysAgentDAL.GetAgent(request.Id);
            if (agentBucket == null)
            {
                return ResponseBase.CommonError("代理商不存在");
            }
            var agent = agentBucket.SysAgent;
            agent.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _sysAgentDAL.EditAgent(agent);
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"修改代理商密码:名称:{agent.Name},手机号码,{agent.Phone}", EmSysAgentOpLogType.AgentMange);
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
                        IsLockDesc = EmSysAgentIsLock.GetSysAgentIsLockDesc(p.IsLock)
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
            });
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
            });
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
            });
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
            });
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
                EtmsAuthorityValue = GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds),
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Remark = request.Remark,
                DetailInfo = request.DetailInfo,
                Img = string.Empty
            });
            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"添加系统版本:{request.Name}", EmSysAgentOpLogType.VersionMange);
            return ResponseBase.Success();
        }

        private string GetAuthorityValueMenu(List<int> pageMenus, List<int> actionMenus, List<int> pageRouteIds)
        {
            return $"{GetAuthorityValue(pageMenus.ToArray())}|{GetAuthorityValue(actionMenus.ToArray())}|{GetAuthorityValue(pageRouteIds.ToArray())}";
        }

        /// <summary>
        /// 通过选择的菜单ID，计算权值
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private string GetAuthorityValue(int[] ids)
        {
            if (ids == null || !ids.Any())
            {
                return string.Empty;
            }
            var authorityCore = new AuthorityCore();
            var weightSum = authorityCore.AuthoritySum(ids);
            return weightSum.ToString();
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
            version.EtmsAuthorityValue = GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds);
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
            MenuConfigsHandle(myMenuConfigs, authorityCorePage, authorityCoreAction);
            return ResponseBase.Success(new VersionGetOutput()
            {
                Name = version.Name,
                Remark = version.Remark,
                DetailInfo = version.DetailInfo,
                Menus = GetSysMenuViewOutputs(myMenuConfigs),
            });
        }

        private List<SysMenuViewOutput> GetSysMenuViewOutputs(List<MenuConfig> menuConfigs)
        {
            var output = new List<SysMenuViewOutput>();
            var index = 1;
            foreach (var p in menuConfigs)
            {
                var item = new SysMenuViewOutput()
                {
                    ActionCheck = new List<int>(),
                    ActionItems = new List<SysMenuItem>(),
                    PageCheck = new List<int>(),
                    PageItems = new List<SysMenuItem>(),
                    Index = index
                };
                index++;
                var thisPageItem = new SysMenuItem()
                {
                    Children = new List<SysMenuItem>(),
                    Id = p.Id,
                    Label = p.Name,
                    Type = p.Type
                };
                item.PageItems.Add(thisPageItem);
                if (p.IsOwner)
                {
                    item.PageCheck.Add(p.Id);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    AddChildrenPage(p.ChildrenPage, thisPageItem, item);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    AddChildrenCheck(p.ChildrenAction, item);
                }
                output.Add(item);
            }
            return output;
        }

        private void AddChildrenCheck(List<MenuConfig> ChildrenAction, SysMenuViewOutput itemOutput)
        {
            if (itemOutput.ActionItems.Count == 0)
            {
                itemOutput.ActionItems.Add(new SysMenuItem()
                {
                    Children = new List<SysMenuItem>(),
                    Id = 0,
                    Label = "全选",
                    Type = MenuType.Action
                });
            }
            foreach (var p in ChildrenAction)
            {
                itemOutput.ActionItems[0].Children.Add(new SysMenuItem()
                {
                    Id = p.ActionId,
                    Label = p.Name,
                    Type = p.Type
                });
                if (p.IsOwner)
                {
                    itemOutput.ActionCheck.Add(p.ActionId);
                }
            }
        }

        private void AddChildrenPage(List<MenuConfig> childrenPage, SysMenuItem item, SysMenuViewOutput itemOutput)
        {
            foreach (var p in childrenPage)
            {
                var thisRoleMenuItem = new SysMenuItem()
                {
                    Children = new List<SysMenuItem>(),
                    Id = p.Id,
                    Label = p.Name,
                    Type = p.Type
                };
                item.Children.Add(thisRoleMenuItem);
                if (p.IsOwner)
                {
                    itemOutput.PageCheck.Add(p.Id);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    AddChildrenPage(p.ChildrenPage, thisRoleMenuItem, itemOutput);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    AddChildrenCheck(p.ChildrenAction, itemOutput);
                }
            }
        }

        private void MenuConfigsHandle(List<MenuConfig> myMenuConfigs, AuthorityCore authorityCorePage, AuthorityCore authorityCoreAction)
        {
            foreach (var p in myMenuConfigs)
            {
                if (p.Type == MenuType.Page)
                {
                    p.IsOwner = authorityCorePage.Validation(p.Id);
                }
                else
                {
                    p.IsOwner = authorityCoreAction.Validation(p.ActionId);
                }
                if (p.ChildrenPage != null && p.ChildrenPage.Any())
                {
                    MenuConfigsHandle(p.ChildrenPage, authorityCorePage, authorityCoreAction);
                }
                if (p.ChildrenAction != null && p.ChildrenAction.Any())
                {
                    MenuConfigsHandle(p.ChildrenAction, authorityCorePage, authorityCoreAction);
                }
            }
        }

        public ResponseBase VersionDefaultGet(VersionDefaultGetRequest request)
        {
            return ResponseBase.Success(GetSysMenuViewOutputs(PermissionData.MenuConfigs));
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
                AuthorityValueMenu = GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds),
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
            role.AuthorityValueMenu = GetAuthorityValueMenu(request.PageIds, request.ActionIds, request.PageRouteIds);
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
            MenuConfigsHandle(myMenuConfigs, authorityCorePage, authorityCoreAction);
            return ResponseBase.Success(new SysRoleGetOutput()
            {
                Name = role.Name,
                Remark = role.Remark,
                Menus = GetSysMenuViewOutputs(myMenuConfigs),
                IsDataLimit = EmDataLimitType.GetIsDataLimit(role.AuthorityValueData)
            });
        }

        public ResponseBase SysRoleDefaultGet(SysRoleDefaultGetRequest request)
        {
            return ResponseBase.Success(GetSysMenuViewOutputs(AgentPermissionData.MenuConfigs));
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
