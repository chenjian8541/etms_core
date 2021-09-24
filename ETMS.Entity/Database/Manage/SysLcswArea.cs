using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysLcswArea")]
    public class SysLcswArea
    {
        public string ProvinceId { get; set; }

        public string CityId { get; set; }

        public string AreaId { get; set; }

        public string ProvinceName { get; set; }

        public string CityName { get; set; }

        public string AreaName { get; set; }
    }
}
