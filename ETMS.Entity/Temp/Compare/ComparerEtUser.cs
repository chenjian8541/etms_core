using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ETMS.Entity.Temp.Compare
{
    public class ComparerEtUser : IEqualityComparer<EtUser>
    {
        public bool Equals([AllowNull] EtUser x, [AllowNull] EtUser y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] EtUser obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
