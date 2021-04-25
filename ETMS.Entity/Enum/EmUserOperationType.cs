using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 用户操作类型
    /// </summary>
    public enum EmUserOperationType
    {
        [Description("登录")]
        Login = 1,

        [Description("修改密码")]
        UserChangePwd = 2,

        [Description("修改用户信息")]
        UserUpdateInfo = 3,

        [Description("设置节假日")]
        HolidaySetting = 4,

        [Description("设置学员自定义属性")]
        StudentExtendFieldSetting = 5,

        [Description("科目设置")]
        SubjectSetting = 6,

        [Description("学员来源设置")]
        StudentSourceSetting = 7,

        [Description("年级设置")]
        GradeSetting = 8,

        [Description("上课时间段设置")]
        ClassSetSetting = 9,

        [Description("成长档案类型设置")]
        StudentGrowingTagSetting = 10,

        [Description("教室设置")]
        ClassRoomSetting = 11,

        [Description("学员标签设置")]
        StudentTagSetting = 12,

        [Description("家长关系设置")]
        StudentRelationshipSetting = 13,

        [Description("班级分类设置")]
        ClassCategorySetting = 14,

        [Description("礼品分类设置")]
        GiftCategorySetting = 15,

        [Description("角色设置")]
        RoleSetting = 46,

        [Description("用户设置")]
        UserSetting = 47,

        [Description("学员管理")]
        StudentManage = 48,

        [Description("学员跟进")]
        StudentTrackLog = 49,

        [Description("礼品管理")]
        GiftManage = 50,

        [Description("礼品兑换")]
        GiftExchange = 51,

        [Description("优惠券管理")]
        CouponsManage = 52,

        [Description("物品管理")]
        GoodsManage = 53,

        [Description("费用管理")]
        CostManage = 54,

        [Description("课程管理")]
        CourseManage = 55,

        [Description("学员报名/续费")]
        StudentEnrolment = 56,

        [Description("班级管理")]
        ClassManage = 57,

        [Description("请假申请")]
        StudentLeaveApplyManage = 58,

        [Description("上课记录")]
        ClassRecordManage = 59,

        [Description("学员课程管理")]
        StudentCourseManage = 60,

        [Description("班级点名")]
        ClassCheckSign = 61,

        [Description("报名补缴")]
        StudentEnrolmentAddPay = 62,

        [Description("新增收支")]
        IncomeLogAdd = 63,

        [Description("取消试听")]
        CancelTryClassStudent = 64,

        [Description("发放优惠券")]
        CouponsStudentSend = 65,

        [Description("修改系统设置")]
        SystemConfigModify = 66,

        [Description("收支项目设置")]
        IncomeProjectTypeSetting = 67,

        [Description("试听申请")]
        TryCalssApplyLogManage = 68,

        [Description("订单管理")]
        OrderMgr = 69,

        [Description("课后作业")]
        ActiveHomeworkMgr = 70,

        [Description("成长档案")]
        ActiveGrowthRecord = 71,

        [Description("微信通知")]
        WxMessage = 72,

        [Description("课后点评")]
        ClassEvaluate = 73,

        [Description("清理数据")]
        ClearData = 74,

        [Description("作废收支")]
        IncomeLogRevoke = 75,

        [Description("学员考勤")]
        StudentCheckOn = 76,

        [Description("充值管理")]
        StudentAccountRechargeManage = 77,

        [Description("课次管理")]
        ClassTimesMgr = 78,

        [Description("套餐管理")]
        SuitMgr = 79
    }
}
