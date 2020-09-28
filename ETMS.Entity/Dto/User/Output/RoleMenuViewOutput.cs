using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class RoleMenuViewOutput
    {
        public int Index { get; set; }

        public List<RoleMenuItem> PageItems { get; set; }

        public List<RoleMenuItem> ActionItems { get; set; }

        public List<int> PageCheck { get; set; }

        public List<int> ActionCheck { get; set; }
    }

    public class RoleMenuItem
    {
        public int Id { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Config.Menu.MenuType"/>
        /// </summary>
        public byte Type { get; set; }

        public List<RoleMenuItem> Children { get; set; }
    }
}
