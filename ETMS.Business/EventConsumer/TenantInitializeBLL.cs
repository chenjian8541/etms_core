using ETMS.Business.Common;
using ETMS.DataAccess.Lib;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess.ShareTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    /// <summary>
    /// 初始化机构数据，可以重复执行
    /// </summary>
    public class TenantInitializeBLL : ITenantInitializeBLL
    {
        private readonly IShareTemplateIdDAL _shareTemplateIdDAL;

        private int _tenantId;

        public TenantInitializeBLL(IShareTemplateIdDAL shareTemplateIdDAL)
        {
            this._shareTemplateIdDAL = shareTemplateIdDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantId = tenantId;
            this.InitDataAccess(tenantId, _shareTemplateIdDAL);
        }

        public async Task TenantInitializeConsumerEvent(TenantInitializeEvent request)
        {
            await this.InitShareTemplateDefault();
        }

        /// <summary>
        /// 初始化分享展示模板
        /// </summary>
        /// <returns></returns>
        private async Task InitShareTemplateDefault()
        {
            var isInit = await _shareTemplateIdDAL.IsInitializeSystemData();
            if (isInit)
            {
                return;
            }
            _shareTemplateIdDAL.AddShareTemplate(ShareTemplateLib.GetNewSysInitShareTemplates(_tenantId));
        }
    }
}
