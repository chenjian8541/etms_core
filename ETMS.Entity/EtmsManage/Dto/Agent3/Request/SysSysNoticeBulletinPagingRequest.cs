using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Agent3.Request
{
    public class SysSysNoticeBulletinPagingRequest : AgentPagingBase
    {
        /// <summary>
        /// 代理商信息
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否需要限制用户数据
        /// </summary>
        /// <returns></returns>
        public override bool IsNeedLimitUserData()
        {
            return true;
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet());
            if (!string.IsNullOrEmpty(Title))
            {
                condition.Append($" AND Title LIKE '%{Title}%'");
            }
            return condition.ToString();
        }
    }
}
