using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class AudioGetPagingRequest : RequestPagingBase
    {
        public int? Type { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (Type != null)
            {
                condition.Append($" AND [Type] = {Type.Value}");
            }
            return condition.ToString();
        }
    }
}
