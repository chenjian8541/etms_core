using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    /// <summary>
    /// 获取分页数据请求
    /// </summary>
    public abstract class RequestPagingBase : RequestBase, IPagingRequest
    {
        /// <summary>
        /// 每页数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageCurrent { get; set; }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (PageSize > 0 && PageCurrent > 0)
            {
                try
                {
                    var s = this.ToString();
                    if (!SQLHelper.SqlValidate(s))
                    {
                        return "请勿输入敏感字符";
                    }
                }
                catch (Exception ex)
                {
                    LOG.Log.Error("[验证敏感字符出错]", ex, this.GetType());
                }
                return string.Empty;
            }
            else
            {
                return "分页参数错误";
            }
        }
    }
}
