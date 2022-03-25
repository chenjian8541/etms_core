using ETMS.Entity.Config;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Database.Manage;
using ETMS.IOC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.DataAccess.Lib
{
    /// <summary>
    /// EtmsAlien数据库访问
    /// </summary>
    public class EtmsAlienDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(CustomServiceLocator.GetInstance<IAppConfigurtaionServices>().AppSettings.DatabseConfig.EtmsAlienConnectionString);
        }

        public DbSet<MgHead> MgHeads { get; set; }

        public DbSet<MgUser> MgUsers { get; set; }

        public DbSet<MgRole> MgRoles { get; set; }

        public DbSet<MgTenants> MgTenantss { get; set; }

        public DbSet<MgUserOpLog> MgUserOpLogs { get; set; }

        public DbSet<MgOrganization> MgOrganizations { get; set; }
    }
}
