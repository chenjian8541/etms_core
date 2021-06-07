using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class MicroWebColumnGetOutput
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int OrderIndex { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmMicroWebColumnType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

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

        public string ShowInMenuIconUrl { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowInHome { get; set; }

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
