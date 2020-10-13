using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassRecordGetPagingH5Request : RequestPagingBase
    {
        /// <summary>
        /// 查询时间
        /// </summary>
        public List<string> Ot { get; set; }
    }
}
