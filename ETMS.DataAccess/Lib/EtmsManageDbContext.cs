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
    }
}
