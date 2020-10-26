using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using PDEX.Core.Models;
using PDEX.Core;
using System.Data.Common;

namespace PDEX.DAL
{
    public class PDEXServerDBContext : DbContextBase
    {
        public PDEXServerDBContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PDEXServerDBContext, ServerConfiguration>());
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

    public class ServerDbContextFactory : IDbContextFactory<PDEXServerDBContext>
    {
        public PDEXServerDBContext Create()
        {
            #region For Debug
            const string serverName = ".";
            const string initialCatalog = "PDEXDbWeb1";
            var sQlServConString = "data source=" + serverName + ";initial catalog=" + initialCatalog + ";" +
                                    "user id=sa;password=amihan";
            Singleton.ConnectionStringName = sQlServConString;
            Singleton.ProviderName = "System.Data.SqlClient";
            var sql = new SqlConnectionFactory(sQlServConString);
            return new PDEXServerDBContext(sql.CreateConnection(sQlServConString), true); 
            #endregion

            #region For Release
            //const string serverIp = "198.38.83.33";
            //const string serverInitialCatalog = "ibrahim11_amstock1";
            //var sQlServerConString = "Data Source=" + serverIp + ";Initial Catalog=" + serverInitialCatalog + ";"+
            //                          "User ID=ibrahim11_armsdev;Password=@rmsd3v;"+
            //                          "encrypt=true;trustServerCertificate=true";

            //var sql = new SqlConnectionFactory(sQlServerConString);
            //return new PDEXServerDBContext(sql.CreateConnection(sQlServerConString), true); 
            #endregion
        }
    }

    public class ServerConfiguration : DbMigrationsConfiguration<PDEXServerDBContext>
    {
        public ServerConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(PDEXServerDBContext context)
        {
            if (Singleton.SeedDefaults)
            {
                //var client = context.Set<SettingDTO>().Find(1);// context.Set<ClientDTO>().Find(1);
                //if (client == null)
                //{
                    context = (PDEXServerDBContext)DbContextUtil.Seed(context);
                //}
            }
            base.Seed(context);
        }
    }
}
