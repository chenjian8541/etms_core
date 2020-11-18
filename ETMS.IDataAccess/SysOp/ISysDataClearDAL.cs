using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.SysOp
{
    public interface ISysDataClearDAL : IBaseDAL
    {
        #region 驾校互动

        Task<bool> ClearActiveHomework();

        Task<bool> ClearGrowthRecord();

        Task<bool> ClearWxMessage();

        Task<bool> ClearStudentSmsLog();

        Task<bool> ClearClassRecordEvaluate();

        #endregion

        #region 营销中心

        Task<bool> ClearGiftExchange();

        Task<bool> ClearGift();

        Task<bool> ClearCoupons();

        #endregion

        #region 机构设置

        Task<bool> ClearIncomeProjectType();

        Task<bool> ClearStudentRelationship();

        Task<bool> ClearStudentTag();

        Task<bool> ClearClassRoom();

        Task<bool> ClearStudentGrowingTag();

        Task<bool> ClearClassSet();

        Task<bool> ClearGrade();

        Task<bool> ClearStudentSource();

        Task<bool> ClearSubject();

        Task<bool> ClearStudentExtendField();

        Task<bool> ClearHolidaySetting();

        Task<bool> ClearStudentTrackLog();

        #endregion

        #region 基础数据

        Task<bool> ClearStudentCourse();

        Task<bool> ClearOrder();

        /// <summary>
        /// 清除排课信息
        /// </summary>
        /// <returns></returns>
        Task<bool> ClearClassTimes();

        Task<bool> ClearClassRecord();

        Task<bool> ClearStudentLeaveApplyLog();

        /// <summary>
        /// 清除课程信息，会将订单信息也一起清除
        /// </summary>
        /// <returns></returns>
        Task<bool> ClearCourse();

        Task<bool> ClearGoodsAndCost();

        Task<bool> ClearClass();

        Task<bool> ClearStudent();

        Task<bool> ClearUser();

        #endregion

        #region 统计数据

        Task<bool> ClearUserOperationLog();

        Task<bool> ClearStatisticsStudent();

        Task<bool> ClearStatisticsSales();

        Task<bool> ClearStatisticsClass();

        #endregion 
    }
}
