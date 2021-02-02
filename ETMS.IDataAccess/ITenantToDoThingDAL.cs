using ETMS.Entity.CacheBucket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITenantToDoThingDAL: IBaseDAL
    {
        Task<bool> ResetTenantToDoThing();

        Task<TenantToDoThingBucket> GetTenantToDoThing();
    }
}
