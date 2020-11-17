using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysWechartAuthTemplateMsgDAL : BaseCacheDAL<SysWechartAuthTemplateMsgBucket>, ISysWechartAuthTemplateMsgDAL, IEtmsManage
    {
        public SysWechartAuthTemplateMsgDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysWechartAuthTemplateMsgBucket> GetDb(params object[] keys)
        {
            var authorizerAppid = keys[0].ToString();
            var authorizerAppidTemplateMsg = await this.FindList<SysWechartAuthTemplateMsg>(p => p.AuthorizerAppid == authorizerAppid && p.IsDeleted == EmIsDeleted.Normal);
            return new SysWechartAuthTemplateMsgBucket()
            {
                SysWechartAuthTemplateMsgs = authorizerAppidTemplateMsg
            };
        }

        public async Task<List<SysWechartAuthTemplateMsg>> GetSysWechartAuthTemplateMsg(string authorizerAppid)
        {
            var bucket = await GetCache(authorizerAppid);
            return bucket?.SysWechartAuthTemplateMsgs;
        }

        public async Task<SysWechartAuthTemplateMsg> GetSysWechartAuthTemplateMsg(string authorizerAppid, string templateIdShort)
        {
            var bucket = await GetCache(authorizerAppid);
            if (bucket == null || bucket.SysWechartAuthTemplateMsgs == null || bucket.SysWechartAuthTemplateMsgs.Count == 0)
            {
                return null;
            }
            return bucket.SysWechartAuthTemplateMsgs.FirstOrDefault(p => p.TemplateIdShort == templateIdShort);
        }

        public async Task<bool> SaveSysWechartAuthTemplateMsg(string authorizerAppid, string templateIdShort, string templateId)
        {
            var hisData = await this.Find<SysWechartAuthTemplateMsg>(p => p.AuthorizerAppid == authorizerAppid && p.TemplateIdShort == templateIdShort && p.IsDeleted == EmIsDeleted.Normal);
            if (hisData != null)
            {
                hisData.UpOt = DateTime.Now;
                hisData.TemplateId = templateId;
                await this.Update(hisData);
            }
            else
            {
                await this.Insert(new SysWechartAuthTemplateMsg()
                {
                    AuthorizerAppid = authorizerAppid,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = string.Empty,
                    TemplateId = templateId,
                    TemplateIdShort = templateIdShort,
                    UpOt = DateTime.Now
                });
            }
            await UpdateCache(authorizerAppid);
            return true;
        }
    }
}
