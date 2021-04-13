using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Output;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysTenantBLL : ISysTenantBLL
    {
        private readonly IEtmsSourceDAL _etmsSourceDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysTenantLogDAL _sysTenantLogDAL;

        private readonly ISysVersionDAL _sysVersionDAL;

        private readonly ISysAgentDAL _sysAgentDAL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        private readonly ISysConnectionStringDAL _sysConnectionStringDAL;

        private readonly ISysAIFaceBiduAccountDAL _sysAIFaceBiduAccountDAL;

        private readonly ISysAITenantAccountDAL _sysAITenantAccountDAL;

        public SysTenantBLL(IEtmsSourceDAL etmsSourceDAL, ISysTenantDAL sysTenantDAL,
            ISysTenantLogDAL sysTenantLogDAL, ISysVersionDAL sysVersionDAL,
            ISysAgentDAL sysAgentDAL, ISysAgentLogDAL sysAgentLogDAL,
            ISysConnectionStringDAL sysConnectionStringDAL, ISysAIFaceBiduAccountDAL sysAIFaceBiduAccountDAL,
            ISysAITenantAccountDAL sysAITenantAccountDAL)
        {
            this._etmsSourceDAL = etmsSourceDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysTenantLogDAL = sysTenantLogDAL;
            this._sysVersionDAL = sysVersionDAL;
            this._sysAgentDAL = sysAgentDAL;
            this._sysAgentLogDAL = sysAgentLogDAL;
            this._sysConnectionStringDAL = sysConnectionStringDAL;
            this._sysAIFaceBiduAccountDAL = sysAIFaceBiduAccountDAL;
            this._sysAITenantAccountDAL = sysAITenantAccountDAL;
        }

        public ResponseBase TenantNewCodeGet(TenantNewCodeGetRequest request)
        {
            var code = new Random().Next(1000, 10000);
            return ResponseBase.Success($"{code}{DateTime.Now.ToString("MMddHHm")}");
        }

        public async Task<ResponseBase> TenantGetPaging(TenantGetPagingRequest request)
        {
            var tenantView = await _sysTenantDAL.GetPaging(request);
            var outList = new List<TenantGetPagingOutput>();
            var versions = await _sysVersionDAL.GetVersions();
            foreach (var p in tenantView.Item1)
            {
                var version = versions.FirstOrDefault(j => j.Id == p.VersionId);
                outList.Add(new TenantGetPagingOutput()
                {
                    VersionId = p.VersionId,
                    Id = p.Id,
                    AgentId = p.AgentId,
                    AgentName = p.AgentName,
                    AgentPhone = p.AgentPhone,
                    ExDateDesc = p.ExDate.EtmsToDateString(),
                    Name = p.Name,
                    Ot = p.Ot,
                    Phone = p.Phone,
                    Remark = p.Remark,
                    SmsCount = p.SmsCount,
                    Status = EmSysTenantStatus.GetSysTenantStatus(p.Status, p.ExDate),
                    StatusDesc = EmSysTenantStatus.GetSysTenantStatusDesc(p.Status, p.ExDate),
                    TenantCode = p.TenantCode,
                    VersionDesc = version?.Name,
                    Address = p.Address,
                    IdCard = p.IdCard,
                    LinkMan = p.LinkMan,
                    BuyStatus = p.BuyStatus,
                    BuyStatusDesc = EmSysTenantBuyStatus.GetSysTenantBuyStatusDesc(p.BuyStatus),
                    AICloudType = p.AICloudType,
                    BaiduCloudId = p.BaiduCloudId,
                    MaxUserCount = p.MaxUserCount,
                    TencentCloudId = p.TencentCloudId
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantGetPagingOutput>(tenantView.Item2, outList));
        }

        public async Task<ResponseBase> TenantAdd(TenantAddRequest request)
        {
            if (await this._sysTenantDAL.ExistTenantCode(request.TenantCode))
            {
                return ResponseBase.CommonError("机构编码已存在");
            }
            var agentBucket = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            var myAgent = agentBucket.SysAgent;
            var myAgentAccount = agentBucket.SysAgentEtmsAccounts;
            if (request.SmsCount > 0 && myAgent.EtmsSmsCount < request.SmsCount)
            {
                return ResponseBase.CommonError("代理商短信剩余条数不足");
            }
            var buyEtmsVersion = myAgentAccount.FirstOrDefault(p => p.VersionId == request.VersionId);
            if (buyEtmsVersion == null || (request.EtmsCount > 0 && buyEtmsVersion.EtmsCount < request.EtmsCount))
            {
                return ResponseBase.CommonError("代理商剩余授权点数不足");
            }
            var now = DateTime.Now;
            var remark = $"添加机构-名称:{request.Name};机构编码:{request.TenantCode};手机号码:{request.Phone}";
            //机构账户
            if (request.SmsCount > 0)
            {
                await _sysAgentDAL.SmsCountDeduction(request.LoginAgentId, request.SmsCount);
                await _sysAgentLogDAL.AddSysAgentSmsLog(new SysAgentSmsLog()
                {
                    AgentId = request.LoginAgentId,
                    ChangeCount = request.SmsCount,
                    ChangeType = EmSysAgentSmsLogChangeType.Deduction,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Remark = remark,
                    Sum = request.Sum
                });
            }

            if (request.EtmsCount > 0)
            {
                await _sysAgentDAL.EtmsAccountDeduction(request.LoginAgentId, request.VersionId, request.EtmsCount);
                await _sysAgentLogDAL.AddSysAgentEtmsAccountLog(new SysAgentEtmsAccountLog()
                {
                    VersionId = request.VersionId,
                    AgentId = request.LoginAgentId,
                    ChangeCount = request.EtmsCount,
                    ChangeType = EmSysAgentEtmsAccountLogChangeType.Deduction,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Sum = request.Sum,
                    Remark = remark
                });
            }

            //初始化账户
            var allDb = await _sysConnectionStringDAL.GetSysConnectionString();
            allDb = allDb.Where(p => p.Status == EmSysConnectionStringStatus.Open).ToList();
            var indexDb = 0;
            if (allDb.Count > 0)
            {
                var myRandom = new Random().Next(1000, 10000);
                indexDb = myRandom % allDb.Count;
            }
            DateTime exDate;
            if (request.EtmsCount > 0)
            {
                exDate = now.AddYears(request.EtmsCount).Date;
            }
            else
            {
                //15天试用
                exDate = now.AddDays(SystemConfig.TenantDefaultConfig.TenantTestDay);
            }
            var buyStatus = request.EtmsCount > 0 ? EmSysTenantBuyStatus.Official : EmSysTenantBuyStatus.Test;

            //初始化云账户(人脸识别)
            var biduCloudAccount = await _sysAIFaceBiduAccountDAL.GetSysAIFaceBiduAccountSystem();
            var index = new Random().Next(0, biduCloudAccount.Count);
            var biduCloudAccountId = biduCloudAccount[index].Id;

            var tenantId = await _sysTenantDAL.AddTenant(new SysTenant
            {
                AgentId = request.LoginAgentId,
                ConnectionId = allDb[indexDb].Id,
                ExDate = exDate,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                Ot = now,
                Phone = request.Phone,
                Remark = request.Remark,
                SmsCount = request.SmsCount,
                Status = EmSysTenantStatus.Normal,
                TenantCode = request.TenantCode,
                VersionId = request.VersionId,
                LinkMan = request.LinkMan,
                IdCard = request.IdCard,
                Address = request.Address,
                SmsSignature = request.SmsSignature,
                BuyStatus = buyStatus,
                MaxUserCount = SystemConfig.TenantDefaultConfig.MaxUserCountDefault,
                AICloudType = EmSysTenantAICloudType.BaiduCloud,
                TencentCloudId = 0,
                BaiduCloudId = biduCloudAccountId
            });
            _etmsSourceDAL.InitTenantId(tenantId);
            _etmsSourceDAL.InitEtmsSourceData(tenantId, request.Name, request.LinkMan, request.Phone);

            //机构变动记录
            if (request.SmsCount > 0)
            {
                await _sysTenantLogDAL.AddSysTenantSmsLog(new SysTenantSmsLog()
                {
                    AgentId = request.LoginAgentId,
                    ChangeCount = request.SmsCount,
                    ChangeType = EmSysTenantSmsLogChangeType.Add,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Remark = remark,
                    Sum = request.Sum,
                    TenantId = tenantId
                });
            }
            if (request.EtmsCount > 0)
            {
                await _sysTenantLogDAL.AddSysTenantEtmsAccountLog(new SysTenantEtmsAccountLog()
                {
                    TenantId = tenantId,
                    Sum = request.Sum,
                    Remark = remark,
                    AgentId = request.LoginAgentId,
                    ChangeCount = request.EtmsCount,
                    ChangeType = EmTenantEtmsAccountLogChangeType.Add,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    VersionId = request.VersionId
                });
            }

            await _sysAgentLogDAL.AddSysAgentOpLog(request, remark, EmSysAgentOpLogType.TenantMange);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantGet(TenantGetRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var versionAlls = await _sysVersionDAL.GetVersions();
            var version = versionAlls.FirstOrDefault(p => p.Id == tenant.VersionId);
            var output = new TenantGetOutput()
            {
                VersionId = tenant.VersionId,
                Id = tenant.Id,
                Address = tenant.Address,
                ExDateDesc = tenant.ExDate.EtmsToDateString(),
                IdCard = tenant.IdCard,
                LinkMan = tenant.LinkMan,
                Name = tenant.Name,
                Phone = tenant.Phone,
                Remark = tenant.Remark,
                SmsCount = tenant.SmsCount,
                Status = EmSysTenantStatus.GetSysTenantStatus(tenant.Status, tenant.ExDate),
                TenantCode = tenant.TenantCode,
                VersionName = version?.Name,
                SmsSignature = tenant.SmsSignature,
                BuyStatus = tenant.BuyStatus
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TenantEdit(TenantEditRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            tenant.Name = request.Name;
            tenant.Phone = request.Phone;
            tenant.Status = request.Status;
            tenant.Remark = request.Remark;
            tenant.LinkMan = request.LinkMan;
            tenant.IdCard = request.IdCard;
            tenant.Address = request.Address;
            tenant.SmsSignature = request.SmsSignature;
            tenant.BuyStatus = request.BuyStatus;
            await _sysTenantDAL.EditTenant(tenant);
            await _sysAgentLogDAL.AddSysAgentOpLog(request,
                $"编辑机构:名称:{request.Name};机构编码:{tenant.TenantCode};手机号码:{request.Phone}", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantSetExDate(TenantSetExDateRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var oldDate = tenant.ExDate;
            tenant.ExDate = request.NewExDate.Value;
            await _sysTenantDAL.EditTenant(tenant);
            await _sysAgentLogDAL.AddSysAgentOpLog(request,
                $"设置机构到期时间:名称:{tenant.Name};机构编码:{tenant.TenantCode};手机号码:{tenant.Phone},原到期时间:{oldDate.EtmsToDateString()},新到期时间:{tenant.ExDate.EtmsToDateString()}", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantDel(TenantDelRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var exDate = tenant.ExDate.AddDays(-SystemConfig.TenantDefaultConfig.TenantTestDay); //允许删除15天以内过期的机构

            var isNewTenant = false;
            var addDateLimit = tenant.Ot.AddDays(SystemConfig.TenantDefaultConfig.TenantTestDay);
            if (addDateLimit.Date >= DateTime.Now.Date)
            {
                isNewTenant = true;
            }
            if (exDate >= DateTime.Now.Date && !isNewTenant)
            {
                return ResponseBase.CommonError($"只允许删除有效期或者添加日期在{SystemConfig.TenantDefaultConfig.TenantTestDay}天内的机构");
            }


            var oldSmsCount = tenant.SmsCount;
            await _sysTenantDAL.DelTenant(tenant);

            if (oldSmsCount > 0) //短信数量返还给代理商
            {
                await _sysAgentDAL.SmsCountAdd(tenant.AgentId, oldSmsCount);
                await _sysAgentLogDAL.AddSysAgentSmsLog(new SysAgentSmsLog()
                {
                    AgentId = tenant.AgentId,
                    ChangeCount = oldSmsCount,
                    ChangeType = EmSysAgentSmsLogChangeType.Add,
                    Ot = DateTime.Now,
                    Sum = 0,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = $"删除机构-名称:[{tenant.Name}],机构编码:[{tenant.TenantCode}],返还短信数量"
                });
            }
            if (isNewTenant) //如果为近期添加的机构，则将授权点数返还给代理商
            {
                var etmslog = await _sysTenantLogDAL.GetTenantEtmsAccountLog(tenant.Id, tenant.AgentId, tenant.VersionId);
                if (etmslog.Count > 0)
                {
                    var tempAddCount = etmslog.Where(p => p.ChangeType == EmTenantEtmsAccountLogChangeType.Add).Sum(p => p.ChangeCount);
                    var tempDeductionCount = etmslog.Where(p => p.ChangeType == EmTenantEtmsAccountLogChangeType.Deduction).Sum(p => p.ChangeCount);
                    var surplusCount = tempAddCount - tempDeductionCount;
                    if (surplusCount > 0)
                    {
                        await _sysAgentDAL.EtmsAccountAdd(tenant.AgentId, tenant.VersionId, surplusCount);
                        await _sysAgentLogDAL.AddSysAgentEtmsAccountLog(new SysAgentEtmsAccountLog()
                        {
                            ChangeCount = surplusCount,
                            AgentId = tenant.AgentId,
                            ChangeType = EmSysAgentEtmsAccountLogChangeType.Add,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = DateTime.Now,
                            Sum = 0,
                            VersionId = tenant.VersionId,
                            Remark = $"删除机构-名称:[{tenant.Name}],机构编码:[{tenant.TenantCode}],返还授权点数"
                        });
                    }
                }
            }

            await _sysAgentLogDAL.AddSysAgentOpLog(request,
                $"删除机构:名称:{tenant.Name};机构编码:{tenant.TenantCode};手机号码:{tenant.Phone}", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantEtmsAccountLogPaging(TenantEtmsAccountLogPagingRequest request)
        {

            var pagingData = await _sysTenantLogDAL.GetTenantEtmsAccountLogPaging(request);
            var output = new List<TenantEtmsAccountLogPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var versions = await _sysVersionDAL.GetVersions();
                foreach (var p in pagingData.Item1)
                {
                    var myVersion = versions.FirstOrDefault(j => j.Id == p.VersionId);
                    output.Add(new TenantEtmsAccountLogPagingOutput()
                    {
                        AgentId = p.AgentId,
                        ChangeCountDesc = EmTenantEtmsAccountLogChangeType.GetChangeCountDesc(p.ChangeCount, p.ChangeType),
                        ChangeType = p.ChangeType,
                        ChangeTypeDesc = EmTenantEtmsAccountLogChangeType.GetChangeTypeDesc(p.ChangeType),
                        Id = p.Id,
                        IsDeleted = p.IsDeleted,
                        Ot = p.Ot,
                        Remark = p.Remark,
                        Sum = p.Sum,
                        TenantId = p.TenantId,
                        TenantName = p.TenantName,
                        TenantPhone = p.TenantPhone,
                        VersionId = p.VersionId,
                        VersionDesc = myVersion?.Name
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantEtmsAccountLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TenantSmsLogPaging(TenantSmsLogPagingRequest request)
        {
            var pagingData = await _sysTenantLogDAL.GetTenantSmsLogPaging(request);
            var output = new List<TenantSmsLogPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new TenantSmsLogPagingOutput()
                {
                    AgentId = p.AgentId,
                    ChangeTypeDesc = EmSysTenantSmsLogChangeType.GetSysTenantSmsLogChangeTypeDesc(p.ChangeType),
                    ChangeType = p.ChangeType,
                    ChangeCountDesc = EmSysTenantSmsLogChangeType.GetChangeCountDesc(p.ChangeCount, p.ChangeType),
                    Id = p.Id,
                    IsDeleted = p.IsDeleted,
                    Ot = p.Ot,
                    Remark = p.Remark,
                    Sum = p.Sum,
                    TenantId = p.TenantId,
                    TenantName = p.TenantName,
                    TenantPhone = p.TenantPhone
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantSmsLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TenantChangeSms(TenantChangeSmsRequest request)
        {
            if (request.ChangeType == TenantChangeEnum.Add)
            {
                return await TenantChangeSmsAdd(request);
            }
            else
            {
                return await TenantChangeSmsDeduction(request);
            }
        }

        private async Task<ResponseBase> TenantChangeSmsAdd(TenantChangeSmsRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var agent = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            if (agent.SysAgent.EtmsSmsCount < request.ChangeCount)
            {
                return ResponseBase.CommonError("代理商短信剩余不足");
            }
            //扣除代理商短信
            var now = DateTime.Now;
            var remark = $"机构短信充值:{request.Remark}";
            await _sysAgentDAL.SmsCountDeduction(request.LoginAgentId, request.ChangeCount);
            await _sysAgentLogDAL.AddSysAgentSmsLog(new SysAgentSmsLog()
            {
                AgentId = request.LoginAgentId,
                ChangeCount = request.ChangeCount,
                ChangeType = EmSysAgentSmsLogChangeType.Deduction,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remark,
                Sum = request.Sum
            });

            //添加机构短信
            tenant.SmsCount += request.ChangeCount;
            await _sysTenantDAL.EditTenant(tenant);
            await _sysTenantLogDAL.AddSysTenantSmsLog(new SysTenantSmsLog()
            {
                AgentId = request.LoginAgentId,
                ChangeCount = request.ChangeCount,
                TenantId = request.Id,
                ChangeType = EmSysTenantSmsLogChangeType.Add,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remark,
                Sum = request.Sum
            });

            await this._sysAgentLogDAL.AddSysAgentOpLog(request, remark, EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> TenantChangeSmsDeduction(TenantChangeSmsRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            if (tenant.SmsCount < request.ChangeCount)
            {
                return ResponseBase.CommonError("机构短信剩余不足");
            }

            //扣除机构短信数量
            var now = DateTime.Now;
            var remark = $"机构短信扣减:{request.Remark}";
            tenant.SmsCount -= request.ChangeCount;
            await _sysTenantDAL.EditTenant(tenant);
            await _sysTenantLogDAL.AddSysTenantSmsLog(new SysTenantSmsLog()
            {
                AgentId = request.LoginAgentId,
                ChangeCount = request.ChangeCount,
                TenantId = request.Id,
                ChangeType = EmSysTenantSmsLogChangeType.Deduction,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remark,
                Sum = request.Sum
            });

            //扣除的短信返回给代理商
            await _sysAgentDAL.SmsCountAdd(request.LoginAgentId, request.ChangeCount);
            await _sysAgentLogDAL.AddSysAgentSmsLog(new SysAgentSmsLog()
            {
                AgentId = request.LoginAgentId,
                ChangeCount = request.ChangeCount,
                ChangeType = EmSysAgentSmsLogChangeType.Add,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remark,
                Sum = request.Sum
            });

            await this._sysAgentLogDAL.AddSysAgentOpLog(request, remark, EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantChangeEtms(TenantChangeEtmsRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var agent = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            var buyEtmsVersion = agent.SysAgentEtmsAccounts.FirstOrDefault(p => p.VersionId == tenant.VersionId);
            if (buyEtmsVersion == null || buyEtmsVersion.EtmsCount < request.AddChangeCount)
            {
                return ResponseBase.CommonError("代理商剩余授权点数不足");
            }

            var now = DateTime.Now;
            var remeak = $"给机构[{tenant.Name}]充值授权点数";
            //扣除代理商授权点数
            await _sysAgentDAL.EtmsAccountDeduction(request.LoginAgentId, tenant.VersionId, request.AddChangeCount);
            await _sysAgentLogDAL.AddSysAgentEtmsAccountLog(new SysAgentEtmsAccountLog()
            {
                AgentId = request.LoginAgentId,
                ChangeCount = request.AddChangeCount,
                ChangeType = EmSysAgentEtmsAccountLogChangeType.Deduction,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remeak,
                Sum = request.Sum,
                VersionId = tenant.VersionId
            });

            //增加机构授权点数
            tenant.ExDate = tenant.ExDate.AddYears(request.AddChangeCount);
            tenant.BuyStatus = EmSysTenantBuyStatus.Official;
            await _sysTenantDAL.EditTenant(tenant);
            await _sysTenantLogDAL.AddSysTenantEtmsAccountLog(new SysTenantEtmsAccountLog()
            {
                AgentId = request.LoginAgentId,
                ChangeCount = request.AddChangeCount,
                ChangeType = EmTenantEtmsAccountLogChangeType.Add,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remeak,
                Sum = request.Sum,
                TenantId = request.Id,
                VersionId = tenant.VersionId
            });

            await _sysAgentLogDAL.AddSysAgentOpLog(request, remeak, EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantBindCloudSave(TenantBindCloudSaveRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            tenant.AICloudType = request.AICloudType;
            tenant.TencentCloudId = request.TencentCloudId;
            tenant.BaiduCloudId = request.BaiduCloudId;
            await _sysTenantDAL.EditTenant(tenant);

            await _sysAgentLogDAL.AddSysAgentOpLog(request,
                $"绑定机构云账户:名称:{tenant.Name};机构编码:{tenant.TenantCode};", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AIFaceBiduAccountGet(AIFaceBiduAccountGetRequest request)
        {
            var output = new List<AIFaceBiduAccountGetOutput>();
            var dataLog = await _sysAIFaceBiduAccountDAL.GetSysAIFaceBiduAccount();
            if (dataLog.Any())
            {
                foreach (var p in dataLog)
                {
                    output.Add(new AIFaceBiduAccountGetOutput()
                    {
                        Remark = p.Remark,
                        Id = p.Id,
                        Label = p.Remark,
                        Value = p.Id
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> AIFaceBiduAccountGetPaging(AIFaceBiduAccountGetPagingRequest request)
        {
            var pagingData = await _sysAIFaceBiduAccountDAL.GetPaging(request);
            var output = new List<AIFaceBiduAccountGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new AIFaceBiduAccountGetPagingOutput()
                {
                    Id = p.Id,
                    Type = p.Type,
                    Remark = p.Remark,
                    ApiKey = p.ApiKey,
                    Appid = p.Appid,
                    SecretKey = p.SecretKey
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AIFaceBiduAccountGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AIFaceBiduAccountAdd(AIFaceBiduAccountAddRequest request)
        {
            if (await _sysAIFaceBiduAccountDAL.ExistAIFaceBiduAccount(request.Appid))
            {
                return ResponseBase.CommonError("Appid已存在");
            }
            await _sysAIFaceBiduAccountDAL.AddSysAIFaceBiduAccount(new SysAIFaceBiduAccount()
            {
                ApiKey = request.ApiKey,
                Appid = request.Appid,
                IsDeleted = EmIsDeleted.Normal,
                Remark = request.Remark,
                SecretKey = request.SecretKey,
                Type = EmSysAIInterfaceType.Customize
            });

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"添加百度云账户:{request.Appid}", EmSysAgentOpLogType.AICloudMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AIFaceBiduAccountEdit(AIFaceBiduAccountEditRequest request)
        {
            var aIFaceBiduAccount = await _sysAIFaceBiduAccountDAL.GetSysAIFaceBiduAccount(request.Id);
            if (aIFaceBiduAccount == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            if (await _sysAIFaceBiduAccountDAL.ExistAIFaceBiduAccount(request.Appid, request.Id))
            {
                return ResponseBase.CommonError("Appid已存在");
            }

            aIFaceBiduAccount.Appid = request.Appid;
            aIFaceBiduAccount.ApiKey = request.ApiKey;
            aIFaceBiduAccount.SecretKey = request.SecretKey;
            aIFaceBiduAccount.Remark = request.Remark;
            await _sysAIFaceBiduAccountDAL.EditSysAIFaceBiduAccount(aIFaceBiduAccount);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"编辑百度云账户:{request.Appid}", EmSysAgentOpLogType.AICloudMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AIFaceBiduAccountDel(AIFaceBiduAccountDelRequest request)
        {
            var aIFaceBiduAccount = await _sysAIFaceBiduAccountDAL.GetSysAIFaceBiduAccount(request.Id);
            if (aIFaceBiduAccount == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            if (aIFaceBiduAccount.Type == EmSysAIInterfaceType.System)
            {
                return ResponseBase.CommonError("系统保留账户，无法删除");
            }
            if (await _sysAIFaceBiduAccountDAL.IsCanNotDel(request.Id))
            {
                return ResponseBase.CommonError("账户已使用，无法删除");
            }

            aIFaceBiduAccount.IsDeleted = EmIsDeleted.Normal;
            await _sysAIFaceBiduAccountDAL.EditSysAIFaceBiduAccount(aIFaceBiduAccount);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"删除百度云账户:{aIFaceBiduAccount.Appid}", EmSysAgentOpLogType.AICloudMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AIFaceTenantAccountGet(AIFaceTenantAccountGetRequest request)
        {
            var output = new List<AIFaceTenantAccountGetOutput>();
            var dataLog = await _sysAITenantAccountDAL.GetSysAITenantAccount();
            if (dataLog.Any())
            {
                foreach (var p in dataLog)
                {
                    output.Add(new AIFaceTenantAccountGetOutput()
                    {
                        Id = p.Id,
                        Label = p.Remark,
                        Remark = p.Remark,
                        Value = p.Id
                    });
                }
            }
            return ResponseBase.Success(dataLog);
        }

        public async Task<ResponseBase> AIFaceTenantAccountGetGetPaging(AIFaceTenantAccountGetGetPagingRequest request)
        {
            var pagingData = await _sysAITenantAccountDAL.GetPaging(request);
            var output = new List<AIFaceTenantAccountGetGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new AIFaceTenantAccountGetGetPagingOutput()
                {
                    Id = p.Id,
                    Type = p.Type,
                    Remark = p.Remark,
                    SecretKey = p.SecretKey,
                    SecretId = p.SecretId,
                    Endpoint = p.Endpoint,
                    Region = p.Region
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AIFaceTenantAccountGetGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AIFaceTenantAccountAdd(AIFaceTenantAccountAddRequest request)
        {
            if (await _sysAITenantAccountDAL.ExistAITenantAccount(request.SecretId))
            {
                return ResponseBase.CommonError("SecretId已存在");
            }
            await _sysAITenantAccountDAL.AddSysAITenantAccount(new SysAITenantAccount()
            {
                SecretId = request.SecretId,
                IsDeleted = EmIsDeleted.Normal,
                Region = request.Region,
                Endpoint = request.Endpoint,
                Remark = request.Remark,
                SecretKey = request.SecretKey,
                Type = EmSysAIInterfaceType.Customize
            });

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"添加腾讯云账户:{request.SecretId}", EmSysAgentOpLogType.AICloudMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AIFaceTenantAccountEdit(AIFaceTenantAccountEditRequest request)
        {
            var sysAITenantAccount = await _sysAITenantAccountDAL.GetSysAITenantAccount(request.Id);
            if (sysAITenantAccount == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            if (await _sysAITenantAccountDAL.ExistAITenantAccount(request.SecretId, request.Id))
            {
                return ResponseBase.CommonError("SecretId已存在");
            }
            sysAITenantAccount.SecretId = request.SecretId;
            sysAITenantAccount.SecretKey = request.SecretKey;
            sysAITenantAccount.Endpoint = request.Endpoint;
            sysAITenantAccount.Region = request.Region;
            await _sysAITenantAccountDAL.EditSysAITenantAccount(sysAITenantAccount);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"编辑腾讯云账户:{request.SecretId}", EmSysAgentOpLogType.AICloudMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AIFaceTenantAccountDel(AIFaceTenantAccountDelRequest request)
        {
            var sysAITenantAccount = await _sysAITenantAccountDAL.GetSysAITenantAccount(request.Id);
            if (sysAITenantAccount == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            if (sysAITenantAccount.Type == EmSysAIInterfaceType.System)
            {
                return ResponseBase.CommonError("系统保留账户，无法删除");
            }
            if (await _sysAITenantAccountDAL.IsCanNotDel(request.Id))
            {
                return ResponseBase.CommonError("账户已使用，无法删除");
            }

            sysAITenantAccount.IsDeleted = EmIsDeleted.Normal;
            await _sysAITenantAccountDAL.EditSysAITenantAccount(sysAITenantAccount);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"删除腾讯云账户:{sysAITenantAccount.SecretId}", EmSysAgentOpLogType.AICloudMgr);
            return ResponseBase.Success();
        }
    }
}
