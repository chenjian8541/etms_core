using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    /// <summary>
    /// 分页数据
    /// </summary>
    public class ResponsePagingDataBase<T>
    {
        public ResponsePagingDataBase(int recordCount, IEnumerable<T> dataItems)
        {
            this.RecordCount = recordCount;
            this.DataItems = dataItems;
        }

        /// <summary>
        /// 记录数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T> DataItems { get; set; }
    }
}
