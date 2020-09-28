using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class GetExchangeLogDetailParentPagingOuput
    {
        /// <summary>
        /// 兑换单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 兑换时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 消耗积分
        /// </summary>
        public int ItemPoints { get; set; }

        public string GiftName { get; set; }

        public string Img { get; set; }

        public string StudentName { get; set; }
    }
}
