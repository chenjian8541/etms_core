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
        CouponsReceive = 3,

        [Description("请假申请")]
        StudentLeaveApply = 4,

        [Description("课后作业")]
        Homework = 5,

        [Description("成长档案")]
        GrowthRecord = 6,

        [Description("评价老师")]
        EvaluateTeacher = 7,

        [Description("在线约课")]
        StudentReservation = 8,

        [Description("退出登陆")]
        Loginout = 9
    }
}
