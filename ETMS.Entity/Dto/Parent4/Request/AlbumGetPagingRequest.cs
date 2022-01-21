using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent4.Request
{
    public class AlbumGetPagingRequest : ParentRequestPagingBase
    {
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            condition.Append($" AND [Status] = {EmElectronicAlbumStatus.Push}");
            return condition.ToString();
        }
    }
}
