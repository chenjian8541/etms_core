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
        public ResponsePagingDataBase2(int recordCount, IEnumerable<T> dataItems, U otherInfo)
        {
            this.RecordCount = recordCount;
            this.DataItems = dataItems;
            this.OtherInfo = otherInfo;
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
        /// 其它信息
        /// </summary>
        public U OtherInfo { get; set; }
    }
}
