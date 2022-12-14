using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class MicroWebColumnGetListOutput
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

        public string IsShowInMenuDesc { get; set; }

        public string ShowInMenuIcon { get; set; }

        public string ShowInMenuIconUrl { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowInHome { get; set; }

        public string IsShowInHomeDesc { get; set; }

        public int ShowInHomeTopIndex { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowYuYue { get; set; }

        public string IsShowYuYueDesc { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMicroWebStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }
    }
}
