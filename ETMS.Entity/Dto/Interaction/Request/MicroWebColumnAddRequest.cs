using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebColumnAddRequest : RequestBase
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

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowYuYue { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "名称不能为空";
            }
            if (IsShowInMenu == EmBool.True && string.IsNullOrEmpty(ShowInMenuIcon))
            {
                return "图标不能为空";
            }
            return base.Validate();
        }
    }
}
