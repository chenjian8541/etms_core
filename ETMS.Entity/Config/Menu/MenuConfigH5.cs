using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Config.Menu
{
    public class MenuConfigH5
    {
        /// <summary>
        /// H5菜单显示的 权限值
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// 操作权限ID
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 类别ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
    }
}
