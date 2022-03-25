using ETMS.IBusiness.Alien;
using ETMS.IDataAccess.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public class AlienUserBLL : IAlienUserBLL
    {
        private readonly IMgHeadDAL _mgHeadDAL;

        private readonly IMgUserDAL _mgUserDAL;

        private readonly IMgUserOpLogDAL _mgUserOpLogDAL;

        public void InitHeadId(int headId)
        {
            this.InitDataAccess(headId, _mgUserDAL, _mgUserOpLogDAL);
        }
    }
}
