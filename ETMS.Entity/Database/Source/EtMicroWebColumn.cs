using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace ETMS.Entity.Database.Source
{
    [Serializable]
    [Table("EtMicroWebColumn")]
    public class EtMicroWebColumn: Entity<long>
    {
        public string Name { get; set; }

        public int OrderIndex { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmMicroWebColumnType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 样式
        ///  <see cref="ETMS.Entity.Enum.EmMicroWebStyle"/>
        /// </summary>
        public int ShowStyle { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowInMenu { get; set; }

        public string ShowInMenuIcon { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowInHome { get; set; }

        public int ShowInHomeTopIndex { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowYuYue { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMicroWebStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
