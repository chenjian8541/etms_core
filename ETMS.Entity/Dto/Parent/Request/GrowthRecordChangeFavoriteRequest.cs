using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class GrowthRecordChangeFavoriteRequest: ParentRequestBase
    {
        public long GrowthRecordDetailId { get; set; }

        /// <summary>
        /// <see cref="EmActiveGrowthRecordDetailFavoriteStatus"/>
        /// </summary>
        public byte NewFavoriteStatus { get; set; }

        public override string Validate()
        {
            if (GrowthRecordDetailId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
