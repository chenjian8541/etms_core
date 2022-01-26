using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ElectronicAlbumDetailGetPagingRequest : RequestPagingBase
    {
        public long ElectronicAlbumId { get; set; }
        public byte ReadType { get; set; }

        public override string Validate()
        {
            if (ElectronicAlbumId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND ElectronicAlbumId = {ElectronicAlbumId}");
            if (ReadType == 0)
            {
                condition.Append(" AND ReadCount = 0");
            }
            else
            {
                condition.Append(" AND ReadCount > 0");
            }
            return condition.ToString();
        }
    }
}
