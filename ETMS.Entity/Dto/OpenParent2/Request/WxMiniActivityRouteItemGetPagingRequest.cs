using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Request
{
    public class WxMiniActivityRouteItemGetPagingRequest : OpenParent2RequestPagingBase
    {
        public override string ToString()
        {
            return $"IsDeleted = {EmIsDeleted.Normal} AND MiniPgmUserId = {this.MiniPgmUserId} ";
        }
    }
}
