using ETMS.Business.Common;
using ETMS.Business.EtmsManage.Common;
using ETMS.Entity.CacheBucket.EtmsManage.RedisLock;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Output;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
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

        private readonly IUserDAL _userDAL;

        private readonly ISmsService _smsService;

        private readonly ISysTenantStatisticsDAL _sysTenantStatisticsDAL;

        private readonly ISysUserDAL _sysUserDAL;

        private readonly ISysTenantOtherInfoDAL _sysTenantOtherInfoDAL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private IDistributedLockDAL _distributedLockDAL;

        private readonly IEventPublisher _eventPublisher;

        public SysTenantBLL(IEtmsSourceDAL etmsSourceDAL, ISysTenantDAL sysTenantDAL,
            ISysTenantLogDAL sysTenantLogDAL, ISysVersionDAL sysVersionDAL,
            ISysAgentDAL sysAgentDAL, ISysAgentLogDAL sysAgentLogDAL,
            ISysConnectionStringDAL sysConnectionStringDAL, ISysAIFaceBiduAccountDAL sysAIFaceBiduAccountDAL,
            ISysAITenantAccountDAL sysAITenantAccountDAL, IUserDAL userDAL, ISmsService smsService, ISysTenantStatisticsDAL sysTenantStatisticsDAL,
            ISysUserDAL sysUserDAL, ISysTenantOtherInfoDAL sysTenantOtherInfoDAL, IAppConfigurtaionServices appConfigurtaionServices,
            IDistributedLockDAL distributedLockDAL, IEventPublisher eventPublisher)
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
            this._userDAL = userDAL;
            this._smsService = smsService;
            this._sysTenantStatisticsDAL = sysTenantStatisticsDAL;
            this._sysUserDAL = sysUserDAL;
            this._sysTenantOtherInfoDAL = sysTenantOtherInfoDAL;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._distributedLockDAL = distributedLockDAL;
            this._eventPublisher = eventPublisher;
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
            var tempBoxUser = new AgentDataTempBox2<SysUser>();
            foreach (var p in tenantView.Item1)
            {
                var version = versions.FirstOrDefault(j => j.Id == p.VersionId);
                var myUser = await AgentComBusiness.GetUser(tempBoxUser, _sysUserDAL, p.UserId);
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
                    TencentCloudId = p.TencentCloudId,
                    Value = p.Id,
                    Label = p.Name,
                    UserName = myUser?.Name,
                    LastOpTimeDesc = p.LastOpTime.EtmsToString(),
                    CloudStorageLimitGB = p.CloudStorageLimitGB,
                    CloudStorageValueGB = p.CloudStorageValueGB,
                    CloudStorageValueMB = p.CloudStorageValueMB,
                    LastRenewalTime = p.LastRenewalTime,
                    AgtPayType = p.AgtPayType,
                    AgtPayTypeDesc = EmAgtPayType.GetAgtPayTypeDesc(p.AgtPayType)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantGetPagingOutput>(tenantView.Item2, outList));
        }

        private async Task AddTenantExDateLog(int tenantId, int agentId, DateTime? myBeforeDate, DateTime afterDate, string remark,
            string changeDesc = "")
        {
            var now = DateTime.Now;
            DateTime beforeDate;
            if (myBeforeDate == null)
            {
                beforeDate = now.Date;
            }
            else
            {
                beforeDate = myBeforeDate.Value;
            }
            var changeType = EmTenantExDateChangeType.Add;
            if (afterDate < beforeDate)
            {
                changeType = EmTenantExDateChangeType.Deduction;
            }
            if (string.IsNullOrEmpty(changeDesc))
            {
                Tuple<int, int> diffTime;
                if (beforeDate > afterDate)
                {
                    diffTime = EtmsHelper.GetDffTime(afterDate, beforeDate);
                }
                else
                {
                    diffTime = EtmsHelper.GetDffTime(beforeDate, afterDate);
                }
                if (diffTime.Item1 > 0)
                {
                    changeDesc = $"{diffTime.Item1}个月";
                }
                if (diffTime.Item2 > 0)
                {
                    if (changeDesc.Length > 0)
                    {
                        changeDesc = $"{changeDesc}&{diffTime.Item2 }天";
                    }
                    else
                    {
                        changeDesc = $"{diffTime.Item2 }天";
                    }
                }
            }
            await _sysTenantLogDAL.AddSysTenantExDateLog(new SysTenantExDateLog()
            {
                AfterDate = afterDate,
                AgentId = agentId,
                BeforeDate = myBeforeDate,
                ChangeDesc = changeDesc,
                ChangeType = changeType,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remark,
                TenantId = tenantId
            });
        }

        public async Task<ResponseBase> TenantAdd(TenantAddRequest request)
        {
            var lockKey = new TenantAddToken(request.LoginAgentId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await TenantAddProcess(request);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"【紧急处理】初始化机构发生异常:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("系统正在初始化机构，请稍后再试...");
        }

        private async Task<ResponseBase> TenantAddProcess(TenantAddRequest request)
        {
            if (await this._sysTenantDAL.ExistTenantCode(request.TenantCode))
            {
                return ResponseBase.CommonError("机构编码已存在");
            }
            var user = await _sysUserDAL.GetUser(request.LoginUserId);
            if (user.IsAdmin == EmBool.False) //非主账号 只能添加测试账号
            {
                if (request.SmsCount > 0 || request.EtmsCount > 0)
                {
                    return ResponseBase.CommonError("您无权添加正式账号和分配短信");
                }
            }
            var agentBucket = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            var myAgent = agentBucket.SysAgent;
            var myAgentAccount = agentBucket.SysAgentEtmsAccounts;
            if (request.SmsCount > 0 && myAgent.EtmsSmsCount < request.SmsCount)
            {
                return ResponseBase.CommonError("代理商短信剩余条数不足");
            }
            if (request.EtmsCount > 0)
            {
                var buyEtmsVersion = myAgentAccount.FirstOrDefault(p => p.VersionId == request.VersionId);
                if (buyEtmsVersion == null || (request.EtmsCount > 0 && buyEtmsVersion.EtmsCount < request.EtmsCount))
                {
                    return ResponseBase.CommonError("代理商剩余授权点数不足");
                }
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
                }, request.LoginUserId);
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
                }, request.LoginUserId);
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
            var changeDesc = string.Empty;
            if (request.EtmsCount > 0)
            {
                exDate = now.AddYears(request.EtmsCount).Date;
                changeDesc = $"{request.EtmsCount}年";
            }
            else
            {
                //15天试用
                exDate = now.AddDays(SystemConfig.TenantDefaultConfig.TenantTestDay);
                changeDesc = $"{SystemConfig.TenantDefaultConfig.TenantTestDay}天";
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
                BaiduCloudId = biduCloudAccountId,
                CloudStorageLimitGB = 20
            }, request.LoginUserId);
            _etmsSourceDAL.InitTenantId(tenantId);
            _etmsSourceDAL.InitEtmsSourceData(tenantId, request.Name, request.LinkMan, request.Phone);

            _eventPublisher.Publish(new TenantInitializeEvent(tenantId));

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
                }, request.LoginUserId);
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
                    VersionId = request.VersionId,
                    SceneType = EmSysTenantEtmsAccountLogSceneType.RechargeAdd
                }, request.LoginUserId);
            }

            if (!string.IsNullOrEmpty(request.SmsSignature))
            {
                await _smsService.AddSmsSign(new AddSmsSignRequest()
                {
                    SmsSignature = request.SmsSignature
                });
            }

            await AddTenantExDateLog(tenantId, request.LoginAgentId, null, exDate, remark, changeDesc);

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
                BuyStatus = tenant.BuyStatus,
                MaxUserCount = tenant.MaxUserCount,
                CloudStorageLimitGB = tenant.CloudStorageLimitGB,
                CloudStorageValueMB = tenant.CloudStorageValueMB,
                CloudStorageValueGB = tenant.CloudStorageValueGB,
                LastRenewalTime = tenant.LastRenewalTime
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TenantUseStatisticsGet(TenantUseStatisticsGetRequest request)
        {
            var log = await _sysTenantStatisticsDAL.GetSysTenantStatistics(request.Id);
            TenantUseStatisticsGetOutput output;
            if (log != null)
            {
                output = new TenantUseStatisticsGetOutput()
                {
                    ClassCount1 = log.ClassCount1.ToString(),
                    ClassCount2 = log.ClassCount2.ToString(),
                    ClassRecordCount = log.ClassRecordCount.ToString(),
                    OrderCount = log.OrderCount.ToString(),
                    StudentCount1 = log.StudentCount1.ToString(),
                    StudentCount2 = log.StudentCount2.ToString(),
                    StudentCount3 = log.StudentCount3.ToString(),
                    UserCount = log.UserCount.ToString()
                };
            }
            else
            {
                output = new TenantUseStatisticsGetOutput();
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TenantGetView(TenantGetViewRequest request)
        {
            var p = await _sysTenantDAL.GetTenant(request.Id);
            var version = await _sysVersionDAL.GetVersion(p.VersionId);
            var agentBucket = await _sysAgentDAL.GetAgent(p.AgentId);
            var agent = agentBucket?.SysAgent;
            var output = new TenantGetPagingOutput()
            {
                VersionId = p.VersionId,
                Id = p.Id,
                AgentId = p.AgentId,
                AgentName = agent?.Name,
                AgentPhone = agent?.Phone,
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
                TencentCloudId = p.TencentCloudId,
                LastOpTimeDesc = p.LastOpTime.EtmsToString(),
                CloudStorageLimitGB = p.CloudStorageLimitGB,
                CloudStorageValueGB = p.CloudStorageValueGB,
                CloudStorageValueMB = p.CloudStorageValueMB,
                LastRenewalTime = p.LastRenewalTime,
                AgtPayType = p.AgtPayType,
                AgtPayTypeDesc = EmAgtPayType.GetAgtPayTypeDesc(p.AgtPayType),
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TenantEdit(TenantEditRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var oldSmsSignature = tenant.SmsSignature;

            tenant.Name = request.Name;
            tenant.Phone = request.Phone;
            tenant.Status = request.Status;
            tenant.Remark = request.Remark;
            tenant.LinkMan = request.LinkMan;
            tenant.IdCard = request.IdCard;
            tenant.Address = request.Address;
            tenant.SmsSignature = request.SmsSignature;
            //tenant.BuyStatus = request.BuyStatus;
            await _sysTenantDAL.EditTenant(tenant);
            await _sysAgentLogDAL.AddSysAgentOpLog(request,
                $"编辑机构:名称:{request.Name};机构编码:{tenant.TenantCode};手机号码:{request.Phone}", EmSysAgentOpLogType.TenantMange);

            if (!string.IsNullOrEmpty(request.SmsSignature) && request.SmsSignature != oldSmsSignature)
            {
                await _smsService.AddSmsSign(new AddSmsSignRequest()
                {
                    SmsSignature = request.SmsSignature
                });
            }

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantEditImportant(TenantEditImportantRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            if (tenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            if (tenant.TenantCode == request.NewTenantCode)
            {
                return ResponseBase.CommonError("新编码与原编码相同");
            }
            if (await this._sysTenantDAL.ExistTenantCode(request.NewTenantCode))
            {
                return ResponseBase.CommonError("机构编码已存在");
            }
            await _sysTenantDAL.EditTenantCode(tenant.Id, request.NewTenantCode);

            await _sysAgentLogDAL.AddSysAgentOpLog(request,
              $"编辑机构编码:原编码:{tenant.TenantCode};新编码:{request.NewTenantCode}", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantSetUser(TenantSetUserRequest request)
        {
            await _sysTenantDAL.EditTenantUserId(request.Ids, request.UserId);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, "绑定机构所属业务员", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantSetExDate(TenantSetExDateRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var oldDate = tenant.ExDate;
            tenant.ExDate = request.NewExDate.Value;
            await _sysTenantDAL.EditTenant(tenant);

            await AddTenantExDateLog(tenant.Id, tenant.AgentId, oldDate, tenant.ExDate, $"设置过期时间：{request.Remark}");

            await _sysAgentLogDAL.AddSysAgentOpLog(request,
                $"设置机构到期时间:名称:{tenant.Name};机构编码:{tenant.TenantCode};手机号码:{tenant.Phone},原到期时间:{oldDate.EtmsToDateString()},新到期时间:{tenant.ExDate.EtmsToDateString()}", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantDel(TenantDelRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var exDate = tenant.ExDate.AddDays(-SystemConfig.TenantDefaultConfig.TenantTestDay); //允许删除30天以内过期的机构

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

            var oldTenantCode = tenant.TenantCode;
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
                }, request.LoginUserId);
            }
            if (isNewTenant) //如果为近期添加的机构，则将授权点数返还给代理商
            {
                var etmslog = await _sysTenantLogDAL.GetTenantEtmsAccountLogNormal(tenant.Id, tenant.AgentId, tenant.VersionId);
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
                            SceneType = EmSysAgentEtmsAccountLogSceneType.TenantDel,
                            Remark = $"删除机构-名称:[{tenant.Name}],机构编码:[{tenant.TenantCode}],返还授权点数"
                        }, request.LoginUserId);
                    }
                }
            }
            AliyunOssUtil.DelTenant(tenant.Id); //删除OSS文件

            await _sysAgentLogDAL.AddSysAgentOpLog(request,
                $"删除机构:名称:{tenant.Name};机构编码:{oldTenantCode};手机号码:{tenant.Phone}", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantEtmsAccountLogPaging(TenantEtmsAccountLogPagingRequest request)
        {
            var pagingData = await _sysTenantLogDAL.GetTenantEtmsAccountLogPaging(request);
            var output = new List<TenantEtmsAccountLogPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var versions = await _sysVersionDAL.GetVersions();
                var tempBoxAgent = new AgentDataTempBox<SysAgent>();
                var tempBoxUser = new AgentDataTempBox2<SysUser>();
                var repealDateLimit = DateTime.Now.AddDays(-SystemConfig.TenantDefaultConfig.TenantEtmsAddLogRepealLimitDay);
                foreach (var p in pagingData.Item1)
                {
                    var myVersion = versions.FirstOrDefault(j => j.Id == p.VersionId);
                    var agent = await AgentComBusiness.GetAgent(tempBoxAgent, _sysAgentDAL, p.AgentId);
                    var myUser = await AgentComBusiness.GetUser(tempBoxUser, _sysUserDAL, p.UserId);
                    var isCanRepeal = false;
                    if (p.Status == EmSysTenantEtmsAccountLogStatus.Normal && p.ChangeType == EmTenantEtmsAccountLogChangeType.Add
                        && p.SceneType == EmSysTenantEtmsAccountLogSceneType.RechargeAdd && p.Ot >= repealDateLimit)
                    {
                        isCanRepeal = true;
                    }
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
                        VersionDesc = myVersion?.Name,
                        AgentName = agent?.Name,
                        UserName = myUser?.Name,
                        Status = p.Status,
                        StatusDesc = EmSysTenantEtmsAccountLogStatus.GetEtmsAccountLogStatusDesc(p.Status),
                        SceneType = p.SceneType,
                        IsCanRepeal = isCanRepeal
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantEtmsAccountLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TenantSmsLogPaging(TenantSmsLogPagingRequest request)
        {
            var pagingData = await _sysTenantLogDAL.GetTenantSmsLogPaging(request);
            var output = new List<TenantSmsLogPagingOutput>();
            var tempBoxAgent = new AgentDataTempBox<SysAgent>();
            var tempBoxUser = new AgentDataTempBox2<SysUser>();
            foreach (var p in pagingData.Item1)
            {
                var agent = await AgentComBusiness.GetAgent(tempBoxAgent, _sysAgentDAL, p.AgentId);
                var myUser = await AgentComBusiness.GetUser(tempBoxUser, _sysUserDAL, p.UserId);
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
                    TenantPhone = p.TenantPhone,
                    AgentName = agent?.Name,
                    UserName = myUser?.Name
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
            }, request.LoginUserId);

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
            }, request.LoginUserId);

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
            }, request.LoginUserId);

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
            }, request.LoginUserId);

            await this._sysAgentLogDAL.AddSysAgentOpLog(request, remark, EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantChangeEtms(TenantChangeEtmsRequest request)
        {
            var lockKey = new TenantChangeEtmsToken(request.LoginAgentId, request.Id);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await TenantChangeEtmsProcess(request);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"【紧急处理】给机构充值授权点数发生异常:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("操作过于频繁，请稍后再试...");
        }

        private async Task<ResponseBase> TenantChangeEtmsProcess(TenantChangeEtmsRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            var oldDate = tenant.ExDate;
            var agent = await _sysAgentDAL.GetAgent(request.LoginAgentId);
            var buyEtmsVersion = agent.SysAgentEtmsAccounts.FirstOrDefault(p => p.VersionId == tenant.VersionId);
            if (buyEtmsVersion == null || buyEtmsVersion.EtmsCount < request.AddChangeCount)
            {
                return ResponseBase.CommonError("代理商剩余授权点数不足");
            }

            var now = DateTime.Now;
            var remeak = $"给机构[{tenant.Name}]充值授权点数：{request.Remark}";
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
            }, request.LoginUserId);

            //增加机构授权点数
            var startDate = tenant.ExDate;
            if (tenant.ExDate < DateTime.Now.Date)
            {
                startDate = DateTime.Now.Date;
            }
            if (tenant.BuyStatus == EmSysTenantBuyStatus.Official
                && (now - tenant.Ot).TotalDays > 30)
            {
                tenant.LastRenewalTime = now;
            }
            tenant.ExDate = startDate.AddYears(request.AddChangeCount);
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
                VersionId = tenant.VersionId,
                SceneType = EmSysTenantEtmsAccountLogSceneType.RechargeAdd
            }, request.LoginUserId);

            await AddTenantExDateLog(tenant.Id, tenant.AgentId, oldDate, tenant.ExDate, remeak, $"{request.AddChangeCount}年");

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

            aIFaceBiduAccount.IsDeleted = EmIsDeleted.Deleted;
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
            sysAITenantAccount.Remark = request.Remark;
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

            sysAITenantAccount.IsDeleted = EmIsDeleted.Deleted;
            await _sysAITenantAccountDAL.EditSysAITenantAccount(sysAITenantAccount);

            await _sysAgentLogDAL.AddSysAgentOpLog(request, $"删除腾讯云账户:{sysAITenantAccount.SecretId}", EmSysAgentOpLogType.AICloudMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AIFaceAllAccountGet(AgentRequestBase request)
        {
            var dataTenantLog = await _sysAITenantAccountDAL.GetSysAITenantAccount();
            var dataBiduLog = await _sysAIFaceBiduAccountDAL.GetSysAIFaceBiduAccount();
            var output = new AIFaceAllAccountGetOutput()
            {
                BiduAccounts = new List<SelectItem<int>>(),
                TenantAccounts = new List<SelectItem<int>>()
            };
            if (dataTenantLog.Any())
            {
                foreach (var p in dataTenantLog)
                {
                    output.TenantAccounts.Add(new SelectItem<int>()
                    {
                        Label = p.Remark,
                        Value = p.Id
                    });
                }
            }
            if (dataBiduLog.Any())
            {
                foreach (var p in dataBiduLog)
                {
                    output.BiduAccounts.Add(new SelectItem<int>()
                    {
                        Value = p.Id,
                        Label = p.Remark
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ResetTenantAdminUserPwd(ResetTenantAdminUserPwdRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.TenantId);
            _userDAL.InitTenantId(tenant.Id);
            var userInfo = await _userDAL.GetAdminUser();
            userInfo.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _userDAL.EditUser(userInfo);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantOtherInfoGet(TenantOtherInfoGetRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.TenantId);
            var output = new TenantOtherInfoGetOutput()
            {
                TenantId = tenant.Id,
                AgentId = request.LoginAgentId,
                TenantName = tenant.Name,
                TenantCode = tenant.TenantCode,
                TenantPhone = tenant.Phone,
                TenantMyLink = TenantLib.GetTenantLoginUrl(tenant.Id, _appConfigurtaionServices.AppSettings.SysAddressConfig.MainLoginParms)
            };
            var tenantOtherInfo = await _sysTenantOtherInfoDAL.GetSysTenantOtherInfo(request.TenantId);
            if (tenantOtherInfo == null)
            {
                return ResponseBase.Success(output);
            }
            output.HomeLogo1 = tenantOtherInfo.HomeLogo1;
            output.HomeLogo2 = tenantOtherInfo.HomeLogo2;
            output.LoginBg = tenantOtherInfo.LoginBg;
            output.LoginLogo1 = tenantOtherInfo.LoginLogo1;
            output.HomeLogo1Url = AliyunOssUtil.GetAccessUrlHttps(tenantOtherInfo.HomeLogo1);
            output.HomeLogo2Url = AliyunOssUtil.GetAccessUrlHttps(tenantOtherInfo.HomeLogo2);
            output.LoginBgUrl = AliyunOssUtil.GetAccessUrlHttps(tenantOtherInfo.LoginBg);
            output.LoginLogo1Url = AliyunOssUtil.GetAccessUrlHttps(tenantOtherInfo.LoginLogo1);
            output.IsHideKeFu = tenantOtherInfo.IsHideKeFu == EmBool.True;
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TenantOtherInfoSave(TenantOtherInfoSaveRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.TenantId);
            var entity = new SysTenantOtherInfo()
            {
                LoginLogo1 = request.LoginLogo1,
                LoginBg = request.LoginBg,
                HomeLogo2 = request.HomeLogo2,
                HomeLogo1 = request.HomeLogo1,
                AgentId = tenant.AgentId,
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                TenantId = request.TenantId,
                IsHideKeFu = request.IsHideKeFu ? EmBool.True : EmBool.False
            };
            await _sysTenantOtherInfoDAL.SaveTenantOtherInfo(entity);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantChangeMaxUserCount(TenantChangeMaxUserCountRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.Id);
            if (tenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var oldMaxCount = tenant.MaxUserCount;
            tenant.MaxUserCount = request.NewMaxUserCount;
            await _sysTenantDAL.EditTenant(tenant);

            await this._sysAgentLogDAL.AddSysAgentOpLog(request,
                $"修改机构最大员工数:{tenant.Name};机构编码:{tenant.TenantCode};原数量:{oldMaxCount},新数量:{request.NewMaxUserCount}", EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantEtmsLogRepeal(TenantEtmsLogRepealRequest request)
        {
            var lockKey = new TenantEtmsLogRepealToken(request.Id);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await ProcessTenantEtmsLogRepeal(request);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"授权记录撤销错误:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("操作过于频繁，请稍后再试...");
        }

        private async Task<ResponseBase> ProcessTenantEtmsLogRepeal(TenantEtmsLogRepealRequest request)
        {
            var log = await _sysTenantLogDAL.GetTenantEtmsAccountLog(request.Id);
            if (log == null)
            {
                return ResponseBase.CommonError("充值授权记录不存在");
            }
            if (log.Status == EmSysTenantEtmsAccountLogStatus.Revoked)
            {
                return ResponseBase.CommonError("充值授权记录已撤销");
            }
            if (log.ChangeType != EmTenantEtmsAccountLogChangeType.Add)
            {
                return ResponseBase.CommonError("此记录不支持撤销");
            }
            if (log.SceneType != EmSysTenantEtmsAccountLogSceneType.RechargeAdd)
            {
                return ResponseBase.CommonError("此记录不支持撤销");
            }
            var repealDateLimit = DateTime.Now.AddDays(-SystemConfig.TenantDefaultConfig.TenantEtmsAddLogRepealLimitDay);
            if (log.Ot < repealDateLimit)
            {
                return ResponseBase.CommonError($"只允许撤销{SystemConfig.TenantDefaultConfig.TenantEtmsAddLogRepealLimitDay}天内的充值授权记录");
            }
            //处理机构
            var myTenant = await _sysTenantDAL.GetTenant(log.TenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var oldExDate = myTenant.ExDate;
            var remeak = $"撤销给机构[{myTenant.Name}]的充值授权：{request.Remark}";
            myTenant.ExDate = myTenant.ExDate.AddYears(-log.ChangeCount);
            if (myTenant.ExDate <= DateTime.Now.AddDays(20) && myTenant.Ot > DateTime.Now.AddDays(-30))
            {
                myTenant.BuyStatus = EmSysTenantBuyStatus.Test;
            }
            await _sysTenantDAL.EditTenant(myTenant);
            var now = DateTime.Now;
            await _sysTenantLogDAL.AddSysTenantEtmsAccountLog(new SysTenantEtmsAccountLog()
            {
                AgentId = log.AgentId,
                ChangeCount = log.ChangeCount,
                ChangeType = EmTenantEtmsAccountLogChangeType.Deduction,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remeak,
                Sum = 0,
                TenantId = log.TenantId,
                VersionId = log.VersionId,
                SceneType = EmSysTenantEtmsAccountLogSceneType.RechargeRepeal,
                RelatedId = log.Id,
                Status = EmSysTenantEtmsAccountLogStatus.Normal
            }, request.LoginUserId);
            log.Status = EmSysTenantEtmsAccountLogStatus.Revoked;
            await _sysTenantLogDAL.EditTenantEtmsAccountLog(log);

            //返还给代理商
            await _sysAgentDAL.EtmsAccountAdd(log.AgentId, log.VersionId, log.ChangeCount);
            await _sysAgentLogDAL.AddSysAgentEtmsAccountLog(new SysAgentEtmsAccountLog()
            {
                AgentId = log.AgentId,
                ChangeCount = log.ChangeCount,
                ChangeType = EmSysAgentEtmsAccountLogChangeType.Add,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                Remark = remeak,
                Sum = 0,
                VersionId = log.VersionId,
                SceneType = EmSysAgentEtmsAccountLogSceneType.TenantEtmsLogRepeal
            }, request.LoginUserId);

            await AddTenantExDateLog(log.TenantId, log.AgentId, oldExDate, myTenant.ExDate, remeak, $"撤销充值{log.ChangeCount}年");

            await _sysAgentLogDAL.AddSysAgentOpLog(request, remeak, EmSysAgentOpLogType.TenantMange);
            return ResponseBase.Success();
        }
    }
}
