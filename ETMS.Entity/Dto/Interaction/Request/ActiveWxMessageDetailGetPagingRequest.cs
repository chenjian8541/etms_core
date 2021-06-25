using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveWxMessageDetailGetPagingRequest : RequestPagingBase
    {
        public long WxMessageId { get; set; }

        /// <summary>
        /// 是否已读 <see cref="EmBool"/>
        /// </summary>
        public byte? IsRead { get; set; }

        /// <summary>
        /// 是否确认   <see cref="EmBool"/>
        /// </summary>
        public byte? IsConfirm { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND WxMessageId = {WxMessageId}");
            if (IsRead != null)
            {
                condition.Append($" AND IsRead = {IsRead}");
            }
            if (IsConfirm != null)
            {
                condition.Append($" AND IsConfirm = {IsConfirm}");
            }
            return condition.ToString();
        }

        public override string Validate()
        {
            if (WxMessageId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}

