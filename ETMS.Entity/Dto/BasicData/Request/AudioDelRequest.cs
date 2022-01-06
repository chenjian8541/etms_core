using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class AudioDelRequest : RequestBase
    {
        public long CId { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请选择数据";
            }
            return string.Empty;
        }
    }
}