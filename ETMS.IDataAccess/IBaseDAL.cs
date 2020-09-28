using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.IDataAccess
{
    public interface IBaseDAL
    {
        /// <summary>
        /// 初始化机构
        /// </summary>
        /// <param name="tenantId"></param>
        void InitTenantId(int tenantId);

        /// <summary>
        /// 重置机构
        /// </summary>
        /// <param name="tenantId"></param>
        void ResetTenantId(int tenantId);
    }
}
