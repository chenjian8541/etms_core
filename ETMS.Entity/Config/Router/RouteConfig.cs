using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config.Router
{
    [Serializable]
    public class RouteConfig
    {
        /// <summary>
        /// Id为0 则代表隐藏项
        /// </summary>
        public int Id { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        /// <summary>
        ///  <see cref="RouteItemType"/>
        /// </summary>
        public byte Type { get; set; }

        public string Component { get; set; }

        public bool Hidden { get; set; }

        public RouterMeta Meta { get; set; }

        public List<RouteConfig> Children { get; set; }
    }
}
