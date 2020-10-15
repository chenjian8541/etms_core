﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Common
{
    public class AgentPagingBase : AgentRequestBase
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
                return string.Empty;
            }
            else
            {
                return "分页参数错误";
            }
        }
    }
}
