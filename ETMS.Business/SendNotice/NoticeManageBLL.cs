using ETMS.Entity.Config;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
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

        public NoticeManageBLL(IAppConfigurtaionServices appConfigurtaionServices, ISmsService smsService)
        {
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._smsService = smsService;
        }

        public async Task NoticeManageConsumerEvent(NoticeManageEvent request)
        {
            var managerPhones = _appConfigurtaionServices.AppSettings.OtherConfig.ManagerPhone;
            if (request.Type == NoticeManageType.TryApply)
            {
                var log = request.TryApplyLog;
                var smsContent = $"试用申请：机构名称{log.Name}，手机号码{log.LinkPhone}";
                var items = new List<CommonSmsItem>();
                foreach (var p in managerPhones)
                {
                    items.Add(new CommonSmsItem()
                    {
                        Phone = p,
                        SmsContent = smsContent
                    });
                }
                await _smsService.CommonSms(new CommonSmsRequest()
                {
                    Items = items
                });
            }
        }
    }
}
