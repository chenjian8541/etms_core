using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    /// <summary>
    /// 请求基类
    /// </summary>
    public class RequestBase : IValidate
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long LoginUserId { set; get; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int LoginTenantId { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 是否启用数据限制
        /// </summary>
        public bool IsDataLimit { get; set; }


        protected string DataFilterWhere
        {
            get
            {
                return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal}";
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
