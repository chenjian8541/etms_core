using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class SysMenuViewOutput
    {
        public int Index { get; set; }

        public List<SysMenuItem> PageItems { get; set; }

        public List<SysMenuItem> ActionItems { get; set; }

        public List<int> PageCheck { get; set; }

        public List<int> ActionCheck { get; set; }
    }

    public class SysMenuItem
    {
        public int Id { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Config.Menu.MenuType"/>
        /// </summary>
        public byte Type { get; set; }

        public List<SysMenuItem> Children { get; set; }
    }
}

