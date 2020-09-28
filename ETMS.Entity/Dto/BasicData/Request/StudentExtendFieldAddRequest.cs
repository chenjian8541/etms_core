using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentExtendFieldAddRequest : RequestBase
    {
        public string Name { get; set; }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            return string.Empty;
        }
    }
}
