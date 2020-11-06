using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveWxMessageEditRequest : RequestBase
    {
        public long CId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string MsgContent { get; set; }

        /// <summary>
        /// 是否需要家长确认  <see cref="EmBool"/>
        /// </summary>
        public byte IsNeedConfirm { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(Title))
            {
                return "请输入标题";
            }
            if (string.IsNullOrEmpty(MsgContent))
            {
                return "请输入内容";
            }
            return base.Validate();
        }
    }
}
