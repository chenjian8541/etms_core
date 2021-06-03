using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebColumnSinglePageSaveRequest : RequestBase
    {
        public long ColumnId { get; set; }

        public string ArTitile { get; set; }

        public string ArCoverImg { get; set; }

        public string ArSummary { get; set; }

        public string ArContent { get; set; }

        public override string Validate()
        {
            if (ColumnId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(ArTitile))
            {
                return "请输入标题";
            }
            if (string.IsNullOrEmpty(ArCoverImg))
            {
                return "请上传封面图片";
            }
            if (string.IsNullOrEmpty(ArSummary))
            {
                return "请输入描述";
            }
            if (string.IsNullOrEmpty(ArContent))
            {
                return "请输入内容";
            }
            return string.Empty;
        }
    }
}
