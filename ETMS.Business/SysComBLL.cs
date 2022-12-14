using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.SysCom.Output;
using ETMS.Entity.Dto.SysCom.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.Business
{
    public class SysComBLL : ISysComBLL
    {
        private readonly ISysUpgradeMsgDAL _sysUpgradeMsgDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysExplainDAL _sysExplainDAL;

        private readonly ISysAgentDAL _sysAgentDAL;

        private readonly ISysAppsettingsBLL _sysAppsettingsBLL;

        private readonly ISysClientUpgradeDAL _sysClientUpgradeDAL;

        private readonly ISysNoticeBulletinDAL _sysNoticeBulletinDAL;

        public SysComBLL(ISysUpgradeMsgDAL sysUpgradeMsgDAL, ISysTenantDAL sysTenantDAL, ISysExplainDAL sysExplainDAL,
            ISysAgentDAL sysAgentDAL, ISysAppsettingsBLL sysAppsettingsBLL, ISysClientUpgradeDAL sysClientUpgradeDAL,
            ISysNoticeBulletinDAL sysNoticeBulletinDAL)
        {
            this._sysUpgradeMsgDAL = sysUpgradeMsgDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysExplainDAL = sysExplainDAL;
            this._sysAgentDAL = sysAgentDAL;
            this._sysAppsettingsBLL = sysAppsettingsBLL;
            this._sysClientUpgradeDAL = sysClientUpgradeDAL;
            this._sysNoticeBulletinDAL = sysNoticeBulletinDAL;
        }

        public void InitTenantId(int tenantId)
        {
        }

        [Obsolete("已弃用，请使用SysNotifyGet")]
        public async Task<ResponseBase> SysUpgradeGet(SysUpgradeGetRequest request)
        {
            var output = new SysUpgradeGetOutput()
            {
                IsHaveUpgrade = false
            };
            var upgradeGetMsg = await _sysUpgradeMsgDAL.GetLastSysUpgradeMsg();
            if (upgradeGetMsg == null || upgradeGetMsg.StartTime <= DateTime.Now)
            {
                return ResponseBase.Success(output);
            }
            if (await _sysUpgradeMsgDAL.GetUserIsRead(upgradeGetMsg.Id, request.LoginTenantId, request.LoginUserId))
            {
                return ResponseBase.Success(output);
            }
            output.IsHaveUpgrade = true;
            output.UpgradeInfo = new UpgradeInfo()
            {
                EndTime = upgradeGetMsg.EndTime,
                StartTime = upgradeGetMsg.StartTime,
                Title = upgradeGetMsg.Title,
                UpContent = upgradeGetMsg.UpContent,
                UpgradeId = upgradeGetMsg.Id,
                VersionNo = upgradeGetMsg.VersionNo,
                UpTimeDesc = $"{ upgradeGetMsg.StartTime.ToString("yyyy年MM月dd日HH:mm")} - {upgradeGetMsg.EndTime.ToString("MM月dd日HH:mm")}"
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> SysNotifyGet(RequestBase request)
        {
            var output = new SysNotifyGetOutput()
            {
                SysUpgradeInfo = new SysUpgradeGetOutput()
                {
                    IsHaveUpgrade = false
                }
            };
            var liveTeachingConfig = await _sysAppsettingsBLL.GetLiveTeachingConfig();
            output.LiveTeachingConfig = new LiveTeachingConfigOutput()
            {
                Config = liveTeachingConfig,
                IsLiving = false,
                IsOpen = liveTeachingConfig.IsOpen
            };
            if (liveTeachingConfig.IsOpen && liveTeachingConfig.Rules != null && liveTeachingConfig.Rules.Count > 0)
            {
                var nowTime = EtmsHelper.GetTimeHourAndMinuteDesc(DateTime.Now);
                var nowWeek = (int)DateTime.Now.DayOfWeek;
                var nowLog = liveTeachingConfig.Rules.FirstOrDefault(j => j.Week == nowWeek && nowTime >= j.StartTime && nowTime < j.EndTime);
                if (nowLog != null)
                {
                    output.LiveTeachingConfig.IsLiving = true;
                }
            }

            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var minExDate = DateTime.Now.AddDays(SystemConfig.ComConfig.SystemExpireDayLimit);
            output.SystemExpiredInfo = new SystemExpiredInfo()
            {
                ExpireDateDesc = myTenant.ExDate.EtmsToDateString(),
                IsRemind = myTenant.ExDate < minExDate
            };
            var upgradeGetMsg = await _sysUpgradeMsgDAL.GetLastSysUpgradeMsg();
            if (upgradeGetMsg == null || upgradeGetMsg.StartTime <= DateTime.Now)
            {
                return ResponseBase.Success(output);
            }
            if (await _sysUpgradeMsgDAL.GetUserIsRead(upgradeGetMsg.Id, request.LoginTenantId, request.LoginUserId))
            {
                return ResponseBase.Success(output);
            }
            output.SysUpgradeInfo.IsHaveUpgrade = true;
            output.SysUpgradeInfo.UpgradeInfo = new UpgradeInfo()
            {
                EndTime = upgradeGetMsg.EndTime,
                StartTime = upgradeGetMsg.StartTime,
                Title = upgradeGetMsg.Title,
                UpContent = upgradeGetMsg.UpContent,
                UpgradeId = upgradeGetMsg.Id,
                VersionNo = upgradeGetMsg.VersionNo,
                UpTimeDesc = $"{ upgradeGetMsg.StartTime.ToString("yyyy年MM月dd日HH:mm")} - {upgradeGetMsg.EndTime.ToString("MM月dd日HH:mm")}"
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> SysUpgradeSetRead(SysUpgradeSetReadRequest request)
        {
            await _sysUpgradeMsgDAL.SetRead(request.UpgradeId, request.LoginTenantId, request.LoginUserId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SysKefu(SysKefuRequest request)
        {
            var output = new SysKefuOutput()
            {
                HelpCenterInfos = new List<KefuHelpCenter>(),
                KefuInfo = new KefuInfo()
                {
                    Phone = new List<string>(),
                    qq = new List<string>()
                },
                UpgradeIngos = new List<UpgradeIngo>()
            };
            var helpCenterInfos = await _sysExplainDAL.GetSysExplainByType(EmSysExplainType.HelpCenter);
            if (helpCenterInfos != null && helpCenterInfos.Count > 0)
            {
                foreach (var p in helpCenterInfos)
                {
                    output.HelpCenterInfos.Add(new KefuHelpCenter()
                    {
                        RelationUrl = p.RelationUrl,
                        Title = p.Title,
                        Id = p.Id
                    });
                }
            }

            var upgradeIngos = await _sysExplainDAL.GetSysExplainByType(EmSysExplainType.UpgradeMsg);
            if (upgradeIngos != null && upgradeIngos.Count > 0)
            {
                foreach (var p in upgradeIngos)
                {
                    output.UpgradeIngos.Add(new UpgradeIngo()
                    {
                        Title = p.Title,
                        RelationUrl = p.RelationUrl,
                        Id = p.Id
                    });
                }
            }

            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var agent = await _sysAgentDAL.GetAgent(myTenant.AgentId);
            var globalConfig = await _sysAppsettingsBLL.GetEtmsGlobalConfig();
            if (string.IsNullOrEmpty(agent.SysAgent.KefuQQ) && string.IsNullOrEmpty(agent.SysAgent.KefuPhone))
            {
                output.KefuInfo.qq = EtmsHelper.GetStrList(globalConfig.KefuQQ);
                output.KefuInfo.Phone = EtmsHelper.GetStrList(globalConfig.KefuPhone);
            }
            else
            {
                output.KefuInfo.qq = EtmsHelper.GetStrList(agent.SysAgent.KefuQQ);
                output.KefuInfo.Phone = EtmsHelper.GetStrList(agent.SysAgent.KefuPhone);
            }
            output.KefuInfo.Phone404 = globalConfig.Phone404;
            output.KefuInfo.Kefu53 = globalConfig.Kefu53;
            return ResponseBase.Success(output);
        }

        private async Task<UploadConfigGetOutput> UploadConfigGet(int tenantId)
        {
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            var aliyunOssSTS = AliyunOssSTSUtil.GetSTSAccessToken(tenantId);
            return new UploadConfigGetOutput()
            {
                AccessKeyId = aliyunOssSTS.Credentials.AccessKeyId,
                AccessKeySecret = aliyunOssSTS.Credentials.AccessKeySecret,
                Bucket = AliyunOssUtil.BucketName,
                Region = AliyunOssSTSUtil.STSRegion,
                Basckey = AliyunOssUtil.GetBascKeyPrefix(tenantId, AliyunOssFileTypeEnum.STS),
                ExTime = aliyunOssSTS.Credentials.Expiration.AddMinutes(-5),
                BascAccessUrlHttps = AliyunOssUtil.OssAccessUrlHttps,
                SecurityToken = aliyunOssSTS.Credentials.SecurityToken,
                FileLimitMB = myTenant.FileLimitMB
            };
        }

        public async Task<ResponseBase> UploadConfigGet(RequestBase request)
        {
            return ResponseBase.Success(await UploadConfigGet(request.LoginTenantId));
        }

        public async Task<ResponseBase> UploadConfigGetOpenLink(UploadConfigGetOpenLinkRequest request)
        {
            return ResponseBase.Success(await UploadConfigGet(request.LoginTenantId));
        }

        public async Task<ResponseBase> ClientUpgradeGet(ClientUpgradeGetRequest request)
        {
            var clientType = EmUserOperationLogClientType.GetClientUpgradeClientType(request.LoginClientType);
            var log = await _sysClientUpgradeDAL.SysClientUpgradeLatestGet(clientType);
            if (log == null)
            {
                return ResponseBase.Success();
            }
            return ResponseBase.Success(new ClientUpgradeGetOutput()
            {
                FileUrl = log.FileUrl,
                UpgradeContent = log.UpgradeContent,
                UpgradeType = log.UpgradeType,
                VersionNo = log.VersionNo
            });
        }

        public async Task<ResponseBase> SysBulletinGet(RequestBase request)
        {
            var output = new SysBulletinGetOutput();
            var log = await _sysNoticeBulletinDAL.GetUsableLog();
            var myDate = DateTime.Now.Date;
            if (log == null)
            {
                return ResponseBase.Success(log);
            }
            if (log.EndTime != null && log.EndTime < myDate)
            {
                return ResponseBase.Success(log);
            }
            if (await _sysNoticeBulletinDAL.GetUserIsRead(log.Id, request.LoginTenantId, request.LoginUserId))
            {
                return ResponseBase.Success(log);
            }
            if (log.IsAdvertise == EmBool.True)
            {
                var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
                if (myTenant.Ot > myDate.AddMonths(-1))  //注册时间大于一个月的才会展示广告
                {
                    return ResponseBase.Success(log);
                }
            }
            output.IsHaveData = true;
            output.BulletinInfo = new SysBulletinGetInfo()
            {
                Title = log.Title,
                UrlLink = log.LinkUrl,
                Id = log.Id
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> SysBulletinSetRead(SysBulletinSetReadRequest request)
        {
            await _sysNoticeBulletinDAL.SetUserRead(request.BulletinId, request.LoginTenantId, request.LoginUserId);
            return ResponseBase.Success();
        }
    }
}
