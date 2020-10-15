using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Common;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysAgentDAL : BaseCacheDAL<SysAgentBucket>, ISysAgentDAL, IEtmsManage
    {
        public SysAgentDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysAgentBucket> GetDb(params object[] keys)
        {
            var agentId = keys[0].ToInt();
            var agent = await this.Find<SysAgent>(p => p.Id == agentId && p.IsDeleted == EmIsDeleted.Normal);
            var accounts = await this.FindList<SysAgentEtmsAccount>(p => p.AgentId == agentId && p.IsDeleted == EmIsDeleted.Normal);
            return new SysAgentBucket()
            {
                SysAgent = agent,
                SysAgentEtmsAccounts = accounts
            };
        }

        public async Task<SysAgent> ExistSysAgentByPhone(string phone, int notId = 0)
        {
            if (notId > 0)
            {
                return await this.Find<SysAgent>(p => p.Phone == phone && p.IsDeleted == EmIsDeleted.Normal && p.Id != notId);
            }
            else
            {
                return await this.Find<SysAgent>(p => p.Phone == phone && p.IsDeleted == EmIsDeleted.Normal);
            }
        }

        public async Task<SysAgentBucket> GetAgent(int id)
        {
            return await GetCache(id);
        }

        public async Task<bool> AddAgent(SysAgent entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.Id);
            return true;
        }

        public async Task<bool> IsCanNotDelete(int agentId)
        {
            var hisData1 = await this.Find<SysAgentEtmsAccount>(p => p.AgentId == agentId && p.IsDeleted == EmIsDeleted.Normal);
            if (hisData1 != null)
            {
                return true;
            }
            var hisData2 = await this.Find<SysAgentSmsLog>(p => p.AgentId == agentId && p.IsDeleted == EmIsDeleted.Normal);
            if (hisData2 != null)
            {
                return true;
            }
            return false;
        }

        public async Task UpdateAgentLastLoginTime(long agentId, DateTime lastLoginTime)
        {
            await this.Execute($"UPDATE SysAgent SET LastLoginOt = '{lastLoginTime.EtmsToString()}' WHERE Id = {agentId}");
            await UpdateCache(agentId);
        }

        public async Task<bool> EditAgent(SysAgent entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.Id);
            return true;
        }

        public async Task<bool> DelAgent(int id)
        {
            var sql = new StringBuilder();
            sql.Append($"update SysAgent set IsDeleted = {EmIsDeleted.Deleted} where Id = {id};");
            sql.Append($"update SysAgentEtmsAccount set IsDeleted = {EmIsDeleted.Deleted} where AgentId = {id};");
            await this.Execute(sql.ToString());
            this.RemoveCache(id);
            return true;
        }

        public async Task<bool> SmsCountAdd(int agentId, int count)
        {
            var agent = (await GetAgent(agentId)).SysAgent;
            agent.EtmsSmsCount += count;
            await this.Update(agent);
            await UpdateCache(agentId);
            return true;
        }

        public async Task<bool> SmsCountDeduction(int agentId, int count)
        {
            var agent = (await GetAgent(agentId)).SysAgent;
            agent.EtmsSmsCount -= count;
            await this.Update(agent);
            await UpdateCache(agentId);
            return true;
        }

        public async Task<bool> EtmsAccountAdd(int agentId, int versionId, int count)
        {
            var hisData = await this.Find<SysAgentEtmsAccount>(p => p.AgentId == agentId && p.VersionId == versionId && p.IsDeleted == EmIsDeleted.Normal);
            if (hisData == null)
            {
                hisData = new SysAgentEtmsAccount()
                {
                    AgentId = agentId,
                    IsDeleted = EmIsDeleted.Normal,
                    EtmsCount = count,
                    Remark = string.Empty,
                    VersionId = versionId
                };
                await this.Insert(hisData);
                await UpdateCache(agentId);
            }
            else
            {
                hisData.EtmsCount += count;
                await this.Update(hisData);
                await UpdateCache(agentId);
            }
            return true;
        }

        public async Task<bool> EtmsAccountDeduction(int agentId, int versionId, int count)
        {
            var hisData = await this.Find<SysAgentEtmsAccount>(p => p.AgentId == agentId && p.VersionId == versionId && p.IsDeleted == EmIsDeleted.Normal);
            hisData.EtmsCount -= count;
            await this.Update(hisData);
            await UpdateCache(agentId);
            return true;
        }

        public async Task<Tuple<IEnumerable<SysAgent>, int>> GetPaging(AgentPagingBase request)
        {
            return await this.ExecutePage<SysAgent>("SysAgent", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
