using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.DAL.Interfaces;
using PDEX.DAL.Mappings;

namespace PDEX.DAL
{
    public static class DbContextUtil
    {
        public static DbModelBuilder OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CompanyMap());
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new CategoryMap());

            modelBuilder.Configurations.Add(new ClientMap());
            modelBuilder.Configurations.Add(new DocumentMap());
            modelBuilder.Configurations.Add(new ContactPersonMap());
            modelBuilder.Configurations.Add(new StaffMap());

            modelBuilder.Configurations.Add(new WarehouseMap());
            modelBuilder.Configurations.Add(new StorageBinMap());

            modelBuilder.Configurations.Add(new DeliveryHeaderMap());
            modelBuilder.Configurations.Add(new DeliveryLineMap());
            modelBuilder.Configurations.Add(new DeliveryRouteMap());
            modelBuilder.Configurations.Add(new MessageMap());
            modelBuilder.Configurations.Add(new VehicleMap());

            modelBuilder.Configurations.Add(new TenderMap());
            modelBuilder.Configurations.Add(new TenderItemMap());
            modelBuilder.Configurations.Add(new TaskProcessMap());

            modelBuilder.Configurations.Add(new PaymentMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new MembershipMap());
            modelBuilder.Configurations.Add(new RoleMap());

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            return modelBuilder;
        }

        public static IDbContext Seed(IDbContext context)
        {
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.ClientCategory, DisplayName = "VIP" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.ClientCategory, DisplayName = "Special" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.ClientCategory, DisplayName = "Normal" });

            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "No Category" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "Document" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "Food" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "Spare Part" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "ዶክመንት" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "ምግብ" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.Category, DisplayName = "መለዋወጫ" });

            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.UnitOfMeasure, DisplayName = "Pcs" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.UnitOfMeasure, DisplayName = "ፒስ" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.UnitOfMeasure, DisplayName = "Kilo" });

            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.CityPlace, DisplayName = "Mexico" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.CityPlace, DisplayName = "Piasa" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.CityBuilding, DisplayName = "Golagul Tower" });
            context.Set<CategoryDTO>().Add(new CategoryDTO { NameType = NameTypes.CityBuilding, DisplayName = "Kurtu Building" });

            return context;
        }

        public static IDbContext GetDbContextInstance()
        {
            //switch (Singleton.Edition)
            //{
            //    case PDEXEdition.CompactEdition:
            //        return new DbContextFactory().Create();
            //    case PDEXEdition.ServerEdition:
            //        return new DbContextFactory().Create();
            //    case PDEXEdition.OnlineEdition:
            //        return new ServerDbContextFactory().Create();
            //}
            return new DbContextFactory().Create();
        }
    }
}