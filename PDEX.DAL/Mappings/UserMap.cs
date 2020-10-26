using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class UserMap : EntityTypeConfiguration<UserDTO>
    {
        public UserMap()
        {
            // Primary Key
            HasKey(t => t.UserId);

            // Properties
            Property(t => t.UserName)
               .IsRequired()
               .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Users");

            //HasMany(u => u.Roles)
            //    .WithMany()
            //    .Map(a => a.ToTable("UserRoles")
            //                .MapLeftKey("UserId")
            //                .MapRightKey("RoleId"));
        }
    }

    public class MembershipMap : EntityTypeConfiguration<MembershipDTO>
    {
        public MembershipMap()
        {
            // Primary Key
            HasKey(t => t.UserId);

            // Properties
            Property(t => t.Password)
                .IsRequired();

            // Table & Column Mappings
            ToTable("webpages_Membership");

            //Relationships
            //HasMany(u => u.Roles)
            //    .WithMany()
            //    .Map(a => a.ToTable("UserRoles")
            //                .MapLeftKey("UserId")
            //                .MapRightKey("RoleId"));
        }
    }

    public class RoleMap : EntityTypeConfiguration<RoleDTO>
    {
        public RoleMap()
        {
            // Primary Key
            HasKey(t => t.RoleId);

            // Properties
            Property(t => t.RoleName)
               .IsRequired();

            // Table & Column Mappings
            ToTable("webpages_Roles");

            //HasMany(u => u.Users)
            //    .WithMany(u => u.Roles)
            //    .Map(a => a.ToTable("UserRoles")
            //                .MapLeftKey("RoleId")
            //                .MapRightKey("UserId"));

        }
    }
    public class UsersInRolesMap : EntityTypeConfiguration<UsersInRoles>
    {
        public UsersInRolesMap()
        {
            // Primary Key
            //this.HasKey(t => {t.RoleId,t.UserId});

            // Properties
            //Property(t => t.RoleDescription)
            // .IsRequired();

            // Table & Column Mappings
            ToTable("webpages_UsersInRoles");

            //Relationships
            HasRequired(t => t.User)
             .WithMany(e => e.Roles)
             .HasForeignKey(t => t.UserId);

            HasRequired(t => t.Role)
             .WithMany(e => e.Users)
             .HasForeignKey(t => t.RoleId);

            //HasMany(u => u.Users)
            //    .WithMany(u => u.Roles)
            //    .Map(a => a.ToTable("UserRoles")
            //                .MapLeftKey("RoleId")
            //                .MapRightKey("UserId"));

        }
    }
}
