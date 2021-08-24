using ETMS.Event.DataContract;
using ETMS.IBusiness.SendNotice;
using ETMS.IOC;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("NoticeTeacherSalaryQueue")]
    public class NoticeTeacherSalaryConsumer : ConsumerBase<NoticeTeacherSalaryEvent>
    {
        protected override async Task Receive(NoticeTeacherSalaryEvent eEvent)
        {
            var userSendNotice2BLL = CustomServiceLocator.GetInstance<IUserSendNotice2BLL>();
            userSendNotice2BLL.InitTenantId(eEvent.TenantId);
            await userSendNotice2BLL.NoticeTeacherSalaryConsumerEvent(eEvent);
        }
    }
}
