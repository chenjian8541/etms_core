using ETMS.Entity.Database.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien.Common
{
    internal class AlienDataTempBox<T> where T : EAlienEntityBase<long>
    {
        internal AlienDataTempBox()
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
