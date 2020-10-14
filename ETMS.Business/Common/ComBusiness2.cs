using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.IDataAccess;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    internal static class ComBusiness2
    {
        internal static string GetStudentRelationshipDesc(List<EtStudentRelationship> etStudentRelationships, long? id, string defaultDesc)
        {
            if (id == null || id == 0)
            {
                return defaultDesc;
            }
            var relationships = etStudentRelationships.FirstOrDefault(p => p.Id == id.Value);
            if (relationships == null)
            {
                return defaultDesc;
            }
            return relationships.Name;
        }

        internal static string GetParentTeacherName(EtUser user)
        {
            if (user == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(user.NickName))
            {
                return user.Name;
            }
            return user.NickName;
        }

        internal static async Task<string> GetParentTeacherName(IUserDAL userDAL, long? userId)
        {
            if (userId == null)
            {
                return string.Empty;
            }
            var user = await userDAL.GetUser(userId.Value);
            if (user == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(user.NickName))
            {
                return user.Name;
            }
            return user.NickName;
        }
    }
}
