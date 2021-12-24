using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.ShareTemplate;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.ShareTemplate;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.ShareTemplate
{
    public class ShareTemplateUseTypeDAL : DataAccessBase<ShareTemplateUseTypeBucket>, IShareTemplateUseTypeDAL
    {
        public ShareTemplateUseTypeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ShareTemplateUseTypeBucket> GetDb(params object[] keys)
        {
            var useType = keys[1].ToInt();
            var myEnabledLogs = await this._dbWrapper.FindList<EtShareTemplate>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.UseType == useType && p.Status == EmShareTemplateStatus.Enabled);
            var myShareTemplateLink = myEnabledLogs.FirstOrDefault(p => p.Type == EmShareTemplateType.Link);
            if (myShareTemplateLink == null)
            {
                myShareTemplateLink = await this._dbWrapper.Find<EtShareTemplate>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.UseType == useType && p.Type == EmShareTemplateType.Link);
            }
            var myShareTemplatePoster = myEnabledLogs.FirstOrDefault(p => p.Type == EmShareTemplateType.ShowTemplate);
            if (myShareTemplatePoster == null)
            {
                myShareTemplateLink = await this._dbWrapper.Find<EtShareTemplate>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.UseType == useType && p.Type == EmShareTemplateType.ShowTemplate);
            }
            return new ShareTemplateUseTypeBucket()
            {
                MyShareTemplateLink = myShareTemplateLink,
                MyShareTemplatePoster = myShareTemplatePoster
            };
        }

        public async Task<ShareTemplateUseTypeBucket> GetShareTemplate(int useType)
        {
            return await GetCache(_tenantId, useType);
        }

        public async Task UpdateShareTemplate(int useType)
        {
            await UpdateCache(_tenantId, useType);
        }
    }
}
