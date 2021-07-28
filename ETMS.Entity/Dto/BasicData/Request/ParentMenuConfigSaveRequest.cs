using ETMS.Entity.Common;
using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class ParentMenuConfigSaveRequest : RequestBase
    {
        public string SmsCode { get; set; }

        public List<ParentMenuConfigItemRequest> Menus { get; set; }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (string.IsNullOrEmpty(SmsCode))
            {
                return "请先验证管理员身份";
            }
            if (Menus == null || Menus.Count == 0)
            {
                return "请求数据格式错误";
            }
            var showMenu = Menus.FirstOrDefault(p => p.IsShow);
            if (showMenu == null)
            {
                return "请勾选展示的菜单";
            }
            return string.Empty;
        }
    }

    public class ParentMenuConfigItemRequest
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string IcoKey { get; set; }

        public bool IsShow { get; set; }
    }
}