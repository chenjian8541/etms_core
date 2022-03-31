using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Common;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Alien.Common
{
    public class AlienRequestBase : IValidate
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        public int LoginHeadId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long LoginUserId { get; set; }

        /// <summary>
        /// 登录时间戳
        /// </summary>
        public string LoginTimestamp { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 客户端类型  <see cref="EmUserOperationLogClientType"/>
        /// </summary>
        public int LoginClientType { get; set; }

        /// <summary>
        /// 机构ID集合
        /// </summary>
        public List<int> AllTenants { get; set; }

        protected string DataFilterWhere
        {
            get
            {
                return $"HeadId = {LoginHeadId} AND IsDeleted = {EmIsDeleted.Normal}";
            }
        }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public virtual string Validate()
        {
            return string.Empty;
        }
    }
}
