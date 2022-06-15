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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //无主键的实体
            modelBuilder.Entity<SysLcsBank>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<SysLcsBankMCC1>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<SysLcsBankMCC2>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<SysLcsBankMCC3>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<SysLcswArea>(entity =>
            {
                entity.HasNoKey();
            });
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
        public DbSet<SysSmsLog> SysSmsLogs { get; set; }
        public DbSet<SysTenantOperationLog> SysTenantOperationLogs { get; set; }
        public DbSet<SysTenantStatistics> SysTenantStatisticss { get; set; }
        public DbSet<SysTenantExDateLog> SysTenantExDateLogs { get; set; }
        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<SysUserRole> SysUserRoles { get; set; }
        public DbSet<SysSmsTemplate> SysSmsTemplates { get; set; }
        public DbSet<SysTenantOtherInfo> SysTenantOtherInfos { get; set; }
        public DbSet<SysTenantUserFeedback> SysTenantUserFeedbacks { get; set; }
        public DbSet<SysTenantLcsAccount> SysTenantLcsAccounts { get; set; }
        public DbSet<SysTenantFubeiAccount> SysTenantFubeiAccounts { get; set; }
        public DbSet<SysTryApplyLog> SysTryApplyLogs { get; set; }
        public DbSet<SysDangerousIp> SysDangerousIps { get; set; }
        public DbSet<SysElectronicAlbum> SysElectronicAlbums { get; set; }
        public DbSet<SysExternalConfig> SysExternalConfigs { get; set; }
        public DbSet<SysNoticeBulletin> SysNoticeBulletins { get; set; }
        public DbSet<SysNoticeBulletinRead> SysNoticeBulletinReads { get; set; }
        public DbSet<SysTenantCloudStorage> SysTenantCloudStorages { get; set; }
        public DbSet<SysTenantMqSchedule> SysTenantMqSchedules { get; set; }
        public DbSet<SysTenantStatistics2> SysTenantStatistics2s { get; set; }
        public DbSet<SysTenantStatisticsWeek> SysTenantStatisticsWeeks { get; set; }
        public DbSet<SysTenantStatisticsMonth> SysTenantStatisticsMonths { get; set; }
        public DbSet<SysTenantSuixingAccount> SysTenantSuixingAccounts { get; set; }
        public DbSet<SysWechatMiniPgmUser> SysWechatMiniPgmUsers { get; set; }
        public DbSet<SysActivity> SysActivitys { get; set; }
    }
}
