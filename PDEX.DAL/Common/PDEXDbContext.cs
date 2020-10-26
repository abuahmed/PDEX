using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using PDEX.Core;
using PDEX.Core.Models;

namespace PDEX.DAL
{
    public class PDEXDbContext : DbContextBase
    {
        public PDEXDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PDEXDbContext, Configuration>());
            Configuration.ProxyCreationEnabled = false;
        }
        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DbContextUtil.OnModelCreating(modelBuilder);
        }
    }

    public class DbContextFactory : IDbContextFactory<PDEXDbContext>
    {
        public PDEXDbContext Create()
        {
            //switch (Singleton.Edition)
            //{
            //    case PDEXEdition.CompactEdition:
            //var sqlCeConString = "Data Source=" + Singleton.SqlceFileName + ";" +
            //                     "Max Database Size=4091;Password=amSt0ckP@ssw0rd";
            //Singleton.ConnectionStringName = sqlCeConString;
            //Singleton.ProviderName = "System.Data.SqlServerCe.4.0";
            //var sqlce = new SqlCeConnectionFactory(Singleton.ProviderName);
            //return new PDEXDbContext(sqlce.CreateConnection(sqlCeConString), true);

            //    case PDEXEdition.ServerEdition:
            const string serverName = "."; //pdexserver "server-01";
            var sQlServConString = "data source=" + serverName + ";initial catalog=" + Singleton.SqlceFileName +
                                      ";user id=sa;password=amihan";
            Singleton.ConnectionStringName = sQlServConString;
            Singleton.ProviderName = "System.Data.SqlClient";
            var sql = new SqlConnectionFactory(sQlServConString);
            return new PDEXDbContext(sql.CreateConnection(sQlServConString), true);
            //}
            //return null;
        }
    }

    public class Configuration : DbMigrationsConfiguration<PDEXDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PDEXDbContext context)
        {
            if (Singleton.SeedDefaults)
            {
                var setting = context.Set<UserDTO>().Find(1);
                if (setting == null)
                {
                    context = (PDEXDbContext)DbContextUtil.Seed(context);
                }
            }
            base.Seed(context);
        }
    }
}
