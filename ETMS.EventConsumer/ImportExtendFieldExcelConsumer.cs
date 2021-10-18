using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.EventConsumer
{
    [QueueConsumerAttribution("ImportExtendFieldExcelQueue")]
    public class ImportExtendFieldExcelConsumer : ConsumerBase<ImportExtendFieldExcelEvent>
    {
        protected override async Task Receive(ImportExtendFieldExcelEvent eEvent)
        {
            var evStudentBLL = CustomServiceLocator.GetInstance<IEvStudentBLL>();
            evStudentBLL.InitTenantId(eEvent.TenantId);
            await evStudentBLL.ImportExtendFieldExcelConsumerEvent(eEvent);
        }
    }
}
