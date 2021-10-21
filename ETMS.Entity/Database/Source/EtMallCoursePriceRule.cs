using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Source
{
    [Table("EtMallCoursePriceRule")]
    public class EtMallCoursePriceRule : BaseCoursePrice
    {
        public long MallGoodsId { get; set; }
    }
}
