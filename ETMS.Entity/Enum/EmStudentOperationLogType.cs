using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ETMS.Entity.Enum
{
    public enum EmStudentOperationLogType
    {
        [Description("登陆")]
        Login = 1,

        [Description("礼品兑换")]
        GiftExchange = 2,

        [Description("领取优惠券")]
        CouponsReceive =3,

        [Description("请假申请")]
        StudentLeaveApply =4
    }
}
