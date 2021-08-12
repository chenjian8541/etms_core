using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    /// <summary>
    /// 分页数据
    /// </summary>
    public class ResponsePagingDataBase2<T, U>
    {
        public ResponsePagingDataBase2(int recordCount, IEnumerable<T> dataItems, IEnumerable<U> dataHeads)
        {
            this.RecordCount = recordCount;
            this.DataItems = dataItems;
            this.DataHeads = dataHeads;
        }

        /// <summary>
        /// 记录数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T> DataItems { get; set; }

        /// <summary>
        /// 表头
        /// </summary>
        public IEnumerable<U> DataHeads { get; set; }
    }
}
