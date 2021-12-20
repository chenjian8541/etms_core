using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class NoticeManageBLL : INoticeManageBLL
    {
        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly ISmsService _smsService;

        private readonly ISysDangerousIpDAL _sysDangerousIpDAL;

        public NoticeManageBLL(IAppConfigurtaionServices appConfigurtaionServices, ISmsService smsService,
             ISysDangerousIpDAL sysDangerousIpDAL)
        {
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._smsService = smsService;
            this._sysDangerousIpDAL = sysDangerousIpDAL;
        }

        public async Task NoticeManageConsumerEvent(NoticeManageEvent request)
        {
            var otherConfig = _appConfigurtaionServices.AppSettings.OtherConfig;
            var items = new List<CommonSmsItem>();
            if (request.Type == NoticeManageType.TryApply)
            {
                var log = request.TryApplyLog;
                var smsContent = $"试用申请：机构名称{log.Name}，手机号码{log.LinkPhone}";
                foreach (var p in otherConfig.ManagerPhone)
                {
                    items.Add(new CommonSmsItem()
                    {
                        Phone = p,
                        SmsContent = smsContent
                    });
                }
            }

            if (request.Type == NoticeManageType.DangerousIp)
            {
                var log = request.MyDangerousVisitor;
                await _sysDangerousIpDAL.AddSysDangerousIp(new SysDangerousIp()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    LocalIpAddress = log.LocalIpAddress,
                    Ot = log.Time,
                    RemoteIpAddress = log.RemoteIpAddress,
                    Url = log.Url
                });
            }

            if (items.Any())
            {
                await _smsService.CommonSms(new CommonSmsRequest()
                {
                    Items = items
                });
            }
        }
    }
}
