using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config.Menu
{
    [Serializable]
    public class MenuConfig
    {
        /// <summary>
        /// 如果为action 则为0
        /// </summary>
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// <see cref="MenuType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 当为action 则为此ID
        /// </summary>
        public int ActionId { get; set; }

        public bool IsOwner { get; set; }

        public List<MenuConfig> ChildrenPage { get; set; }

        public List<MenuConfig> ChildrenAction { get; set; }
    }
}
