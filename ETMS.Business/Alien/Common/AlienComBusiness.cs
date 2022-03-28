using ETMS.Entity.Database.Alien;
using ETMS.IDataAccess.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien.Common
{
    internal static class AlienComBusiness
    {
        internal static async Task<MgUser> GetUser(AlienDataTempBox<MgUser> tempBox, IMgUserDAL sysUserDAL, long userId)
        {
            if (userId == 0)
            {
                return null;
            }
            var user = await tempBox.GetData(userId, async () =>
            {
                return await sysUserDAL.GetUser(userId);
            });
            return user;
        }

    }
}
