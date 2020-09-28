using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class MenuItem
    {
        public int CId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Config.Menu.MenuType"/>
        /// </summary>
        public byte Type { get; set; }
    }
}
