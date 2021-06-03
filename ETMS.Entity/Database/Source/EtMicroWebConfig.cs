using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtMicroWebConfig")]
    public class EtMicroWebConfig : Entity<long>
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMicroWebConfigType"/>
        /// </summary>
        public int Type { get; set; }

        public string ConfigValue { get; set; }
    }
}
