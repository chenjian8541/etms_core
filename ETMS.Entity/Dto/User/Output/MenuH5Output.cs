using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Output
{
    public class MenuH5Output
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
        public string IconUrl { get; set; }


        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
