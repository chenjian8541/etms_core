using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IBaseAlienDAL
    {
        /// <summary>
        /// 初始化企业
        /// </summary>
        /// <param name="HeadId"></param>
        void InitHeadId(int HeadId);

        /// <summary>
        /// 重置企业
        /// </summary>
        /// <param name="HeadId"></param>
        void ResetHeadId(int HeadId);
    }
}
