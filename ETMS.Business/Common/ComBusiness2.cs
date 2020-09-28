using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
    }
}
