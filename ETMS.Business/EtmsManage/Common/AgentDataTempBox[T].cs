using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage.Common
{
    public class AgentDataTempBox<T> where T : EManageEntity<int>
    {
        internal AgentDataTempBox()
        {
            ListTempData = new List<T>();
        }

        internal List<T> ListTempData { get; set; }

        internal async Task<T> GetData(long id, Func<Task<T>> dataAccessProcess)
        {
            var myData = this.ListTempData.FirstOrDefault(p => p.Id == id);
            if (myData != null)
            {
                return myData;
            }
            myData = await dataAccessProcess();
            if (myData != null)
            {
                this.ListTempData.Add(myData);
            }
            return myData;
        }
    }
}
