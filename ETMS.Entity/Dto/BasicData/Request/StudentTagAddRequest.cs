using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentTagAddRequest : RequestBase
    {
        public string Name { get; set; }

        //public string DisplayStyle { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            //if (string.IsNullOrEmpty(DisplayStyle))
            //{
            //    return "请选择显示样式";
            //}
            return string.Empty;
        }
    }
}

