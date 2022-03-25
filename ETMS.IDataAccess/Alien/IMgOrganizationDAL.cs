using ETMS.Entity.CacheBucket.Alien;
using ETMS.Entity.Database.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IMgOrganizationDAL : IBaseAlienDAL
    {
        Task<MgOrganizationBucket> GetOrganizationBucket();

        Task<MgOrganization> GetOrganization(long id);

        Task AddOrganization(MgOrganization entity);

        Task EditOrganization(MgOrganization entity);

        Task DelOrganization(long id);
    }
}
