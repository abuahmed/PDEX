using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Extensions;
using PDEX.Core.Models;

namespace PDEX.Core
{
    public static class CommonUtility
    {
        public static string Encrypt(string stringToEncrypt)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var data = System.Text.Encoding.ASCII.GetBytes(stringToEncrypt);
            data = x.ComputeHash(data);
            var md5Hash = System.Text.Encoding.ASCII.GetString(data);

            return md5Hash;
        }

        public static string GetSecretCode()
        {
            return new Random().Next(1001, 9990).ToString(CultureInfo.InvariantCulture);
        }

        public static IList<RoleDTO> GetRolesList()
        {
            return Enum.GetNames(typeof(RoleTypes))
                .Select(name => (RoleTypes)Enum.Parse(typeof(RoleTypes), name))
                .Select(GetRoleDTO).ToList();
        }

        public static RoleDTO GetRoleDTO(RoleTypes roleType)
        {
            var role = new RoleDTO
             {
                 RoleName = roleType.ToString(),
                 RoleDescription = EnumUtil.GetEnumDesc(roleType),
                 RoleDescriptionShort = roleType.ToString()
             };
            return role;
        }

        public static bool UserHasRole(RoleTypes role)
        {
            return Singleton.User.Roles.Any(u => u.Role.RoleName == role.ToString());
        }

        public static IList<ListDataItem> GetList(Type enumType)
        {
            var enumList = new List<ListDataItem>();

            var enumTypes = Enum.GetNames(enumType);
            foreach (var staffType in enumTypes)
            {
                var staffTy = new ListDataItem
                {
                    Display = EnumUtil.GetEnumDesc(Enum.Parse(enumType, staffType)),
                    Value = Convert.ToInt32(Enum.Parse(enumType, staffType))
                };
                enumList.Add(staffTy);
            }

            return enumList;
        } 
    }
}
