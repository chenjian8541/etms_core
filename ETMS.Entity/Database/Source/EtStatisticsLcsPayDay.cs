using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Source
{
    [Table("EtStatisticsLcsPayDay")]
    public class EtStatisticsLcsPayDay : Entity<long>
    {
        public DateTime Ot { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmLcsPayLogOrderType"/>
        /// </summary>
        [Obsolete("未用")]
        public int OrderType { get; set; }

        public decimal TotalMoney { get; set; }

        public decimal TotalMoneyRefund { get; set; }

        public decimal TotalMoneyValue { get; set; }
    }
}
