using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtStatisticsSalesProduct")]
    public class EtStatisticsSalesProduct : Entity<long>
    {
        public DateTime Ot { get; set; }

        public decimal SalesTotalSum { get; set; }

        public decimal CourseSum { get; set; }

        public decimal GoodsSum { get; set; }

        public decimal CostSum { get; set; }
    }
}
