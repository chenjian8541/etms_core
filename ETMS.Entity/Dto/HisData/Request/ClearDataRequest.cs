using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class ClearDataRequest : RequestBase
    {
        public string SmsCode { get; set; }

        #region 基础信息

        /// <summary>
        /// 课程
        /// </summary>
        public bool IsClearCourse { get; set; }

        /// <summary>
        /// 物品/费用
        /// </summary>
        public bool IsClearGoodsAndCost { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public bool IsClearClass { get; set; }

        /// <summary>
        /// 学员
        /// </summary>
        public bool IsClearStudent { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public bool IsClearUser { get; set; }

        /// <summary>
        /// 学员课程
        /// </summary>
        public bool IsClearStudentCourse { get; set; }

        #endregion

        #region  记录

        /// <summary>
        /// 订单
        /// </summary>
        public bool IsClearOrder { get; set; }

        /// <summary>
        /// 排课
        /// </summary>
        public bool IsClearClassTimes { get; set; }

        /// <summary>
        /// 上课记录
        /// </summary>
        public bool IsClearClassRecord { get; set; }

        /// <summary>
        /// 学员跟进信息
        /// </summary>
        public bool IsClearStudentTrackLog { get; set; }

        /// <summary>
        /// 请假记录
        /// </summary>
        public bool IsClearStudentLeaveApplyLog { get; set; }

        #endregion

        #region 机构设置

        /// <summary>
        /// 收支项目类型
        /// </summary>
        public bool IsClearIncomeProjectType { get; set; }

        /// <summary>
        /// 亲属关系类型
        /// </summary>
        public bool IsClearStudentRelationship { get; set; }

        /// <summary>
        /// 学员标签
        /// </summary>
        public bool IsClearStudentTag { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public bool IsClearClassRoom { get; set; }

        /// <summary>
        /// 成长档案类型
        /// </summary>
        public bool IsClearStudentGrowingTag { get; set; }

        /// <summary>
        /// 上课时间段
        /// </summary>
        public bool IsClearClassSet { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public bool IsClearGrade { get; set; }

        /// <summary>
        /// 学员来源
        /// </summary>
        public bool IsClearStudentSource { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public bool IsClearSubject { get; set; }

        /// <summary>
        /// 学员自定义属性
        /// </summary>
        public bool IsClearStudentExtendField { get; set; }

        /// <summary>
        /// 节假日设置
        /// </summary>
        public bool IsClearHolidaySetting { get; set; }

        #endregion 

        #region 营销中心

        /// <summary>
        /// 礼品兑换记录
        /// </summary>
        public bool IsClearGiftExchange { get; set; }

        /// <summary>
        /// 礼品信息
        /// </summary>
        public bool IsClearGift { get; set; }

        /// <summary>
        /// 优惠券
        /// </summary>
        public bool IsClearCoupons { get; set; }

        #endregion


        #region 家校互动

        /// <summary>
        /// 作业
        /// </summary>
        public bool IsClearActiveHomework { get; set; }

        /// <summary>
        /// 成长档案
        /// </summary>
        public bool IsClearGrowthRecord { get; set; }

        /// <summary>
        /// 微信通知
        /// </summary>
        public bool IsClearWxMessage { get; set; }

        /// <summary>
        /// 短信记录
        /// </summary>
        public bool IsClearStudentSmsLog { get; set; }

        /// <summary>
        /// 课后点评
        /// </summary>
        public bool IsClearClassRecordEvaluate { get; set; }

        #endregion
    }
}
