using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.IOC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess.Lib
{
    /// <summary>
    /// EtmsManage 数据访问
    /// </summary>
    public class EtmsManageDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(CustomServiceLocator.GetInstance<IAppConfigurtaionServices>().AppSettings.DatabseConfig.EtmsManageConnectionString);
        }

        public DbSet<SysConnectionString> SysConnectionStrings { get; set; }
        public DbSet<SysStudentWechart> SysStudentWecharts { get; set; }
        public DbSet<SysTeacherWechart> SysTeacherWecharts { get; set; }
        public DbSet<SysTenant> SysTenants { get; set; }
        public DbSet<SysVersion> SysVersions { get; set; }
        public DbSet<SysRole> SysRoles { get; set; }
        public DbSet<SysAgent> SysAgents { get; set; }
        public DbSet<SysAgentEtmsAccount> SysAgentEtmsAccounts { get; set; }
        public DbSet<SysAgentOpLog> SysAgentOpLogs { get; set; }
        public DbSet<SysAgentEtmsAccountLog> SysAgentEtmsAccountLogs { get; set; }
        public DbSet<SysAgentSmsLog> SysAgentSmsLogs { get; set; }
        public DbSet<SysTenantEtmsAccountLog> SysTenantEtmsAccountLogs { get; set; }
        public DbSet<SysTenantSmsLog> SysTenantSmsLogs { get; set; }
        public DbSet<SysTenantWechartAuth> SysTenantWechartAuths { get; set; }
        public DbSet<SysWechartVerifyTicket> SysWechartVerifyTickets { get; set; }
        public DbSet<SysWechartAuthorizerToken> SysWechartAuthorizerTokens { get; set; }
        public DbSet<SysAppsettings> SysAppsettingss { get; set; }
        public DbSet<SysUpgradeMsg> SysUpgradeMsgs { get; set; }
        public DbSet<SysUpgradeMsgRead> SysUpgradeMsgReads { get; set; }
        public DbSet<SysWechartAuthTemplateMsg> SysWechartAuthTemplateMsgs { get; set; }
        public DbSet<SysTenantTxCloudUCount> SysTenantTxCloudUCounts { get; set; }
        public DbSet<SysExplain> SysExplains { get; set; }
        public DbSet<SysTenantUser> SysTenantUsers { get; set; }
        public DbSet<SysTenantStudent> SysTenantStudents { get; set; }
        public DbSet<SysClientUpgrade> SysClientUpgrades { get; set; }
        public DbSet<SysTenantWechartError> SysTenantWechartErrors { get; set; }
        public DbSet<SysAITenantAccount> SysAITenantAccounts { get; set; }
        public DbSet<SysAIFaceBiduAccount> SysAIFaceBiduAccounts { get; set; }
    }
}
