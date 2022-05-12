using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ETMS.Entity.Temp.Compare
{
    public class ComparerNoticeUserView : IEqualityComparer<NoticeUserView>
    {
        public bool Equals([AllowNull] NoticeUserView x, [AllowNull] NoticeUserView y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] NoticeUserView obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
