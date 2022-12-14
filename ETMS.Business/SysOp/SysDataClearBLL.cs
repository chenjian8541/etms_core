using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness.IncrementLib;
using ETMS.IBusiness.SysOp;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.SysOp;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.SysOp
{
    public class SysDataClearBLL : ISysDataClearBLL
    {
        private readonly ISysDataClearDAL _sysDataClearDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysSafeSmsCodeCheckBLL _sysSafeSmsCodeCheckBLL;

        private readonly ISysActivityRouteItemDAL _sysActivityRouteItemDAL;

        private readonly IAiface _aiface;
        public SysDataClearBLL(ISysDataClearDAL sysDataClearDAL, IUserOperationLogDAL userOperationLogDAL, ITempDataCacheDAL tempDataCacheDAL
            , ISysTenantDAL sysTenantDAL, ISysSafeSmsCodeCheckBLL sysSafeSmsCodeCheckBLL,
            ISysActivityRouteItemDAL sysActivityRouteItemDAL, IAiface aiface)
        {
            this._sysDataClearDAL = sysDataClearDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysSafeSmsCodeCheckBLL = sysSafeSmsCodeCheckBLL;
            this._sysActivityRouteItemDAL = sysActivityRouteItemDAL;
            this._aiface = aiface;
        }

        public void InitTenantId(int tenantId)
        {
            this._aiface.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _sysDataClearDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> ClearDataSendSms(ClearDataSendSmsRequest request)
        {
            var now = DateTime.Now;
            var limitBucket = _tempDataCacheDAL.GetClearDataBucket(request.LoginTenantId, now);
            if (limitBucket != null && limitBucket.TotalCount >= 3)
            {
                return ResponseBase.CommonError("已超过本月可执行的次数");
            }
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return await _sysSafeSmsCodeCheckBLL.SysSafeSmsCodeSend(request.LoginTenantId, sysTenantInfo.Phone);
        }

        public async Task<ResponseBase> ClearData(ClearDataRequest request)
        {
            var now = DateTime.Now;
            var limitBucket = _tempDataCacheDAL.GetClearDataBucket(request.LoginTenantId, now);
            if (limitBucket != null && limitBucket.TotalCount >= 3)
            {
                return ResponseBase.CommonError("已超过本月可执行的次数");
            }

            var chekSmsResult = _sysSafeSmsCodeCheckBLL.SysSafeSmsCodeCheck(request.LoginTenantId, request.SmsCode);
            if (!chekSmsResult.IsResponseSuccess())
            {
                return chekSmsResult;
            }

            if (request.IsClearCourse)
            {
                await this.ClearCourse();
            }
            if (request.IsClearGoodsAndCost)
            {
                await this.ClearGoodsAndCost();
            }
            if (request.IsClearClass)
            {
                await this.ClearClass();
            }
            if (request.IsClearStudent)
            {
                await this.ClearStudent();
                await _aiface.Gr0oupDelete(request.LoginTenantId);
            }
            if (request.IsClearUser)
            {
                await this.ClearUser();
            }
            if (request.IsClearStudentCourse)
            {
                await this.ClearStudentCourse();
            }
            if (request.IsClearOrder)
            {
                await this.ClearOrder();
            }
            if (request.IsClearClassTimes)
            {
                await this.ClearClassTimes();
            }
            if (request.IsClearClassRecord)
            {
                await this.ClearClassRecord();
            }
            if (request.IsClearStudentTrackLog)
            {
                await this.ClearStudentTrackLog();
            }
            if (request.IsClearStudentLeaveApplyLog)
            {
                await this.ClearStudentLeaveApplyLog();
            }
            if (request.IsClearIncomeProjectType)
            {
                await this.ClearIncomeProjectType();
            }
            if (request.IsClearStudentRelationship)
            {
                await this.ClearStudentRelationship();
            }
            if (request.IsClearStudentTag)
            {
                await this.ClearStudentTag();
            }
            if (request.IsClearClassRoom)
            {
                await this.ClearClassRoom();
            }
            if (request.IsClearStudentGrowingTag)
            {
                await this.ClearStudentGrowingTag();
            }
            if (request.IsClearClassSet)
            {
                await this.ClearClassSet();
            }
            if (request.IsClearGrade)
            {
                await this.ClearGrade();
            }
            if (request.IsClearStudentSource)
            {
                await this.ClearStudentSource();
            }
            if (request.IsClearSubject)
            {
                await this.ClearSubject();
            }
            if (request.IsClearStudentExtendField)
            {
                await this.ClearStudentExtendField();
            }
            if (request.IsClearHolidaySetting)
            {
                await this.ClearHolidaySetting();
            }
            if (request.IsClearOtherSetting)
            {
                await this.ClearOtherSetting();
            }
            if (request.IsClearGiftExchange)
            {
                await this.ClearGiftExchange();
            }
            if (request.IsClearGift)
            {
                await this.ClearGift();
            }
            if (request.IsClearCoupons)
            {
                await this.ClearCoupons();
            }
            if (request.IsClearActiveHomework)
            {
                await this.ClearActiveHomework();
            }
            if (request.IsClearGrowthRecord)
            {
                await this.ClearGrowthRecord();
            }
            if (request.IsClearWxMessage)
            {
                await this.ClearWxMessage();
            }
            if (request.IsClearElectronicAlbum)
            {
                await this.ClearElectronicAlbum();
            }
            if (request.IsClearStudentSmsLog)
            {
                await this.ClearStudentSmsLog();
            }
            if (request.IsClearClassRecordEvaluate)
            {
                await this.ClearClassRecordEvaluate();
            }
            if (request.IsClearActivity)
            {
                await this.ClearActivity(request.LoginTenantId);
            }
            if (request.IsClearAchievement)
            {
                await this.ClearAchievement();
            }

            var totalCount = limitBucket == null ? 0 : limitBucket.TotalCount;
            _tempDataCacheDAL.SetClearDataBucket(request.LoginTenantId, now, ++totalCount);

            await _userOperationLogDAL.AddUserLog(request, "清理数据", Entity.Enum.EmUserOperationType.ClearData);
            return ResponseBase.Success();
        }

        #region  基础信息

        /// <summary>
        /// 课程
        /// </summary>
        public async Task<bool> ClearCourse()
        {
            await _sysDataClearDAL.ClearCourse();
            await _sysDataClearDAL.ClearStudentCourse();
            await _sysDataClearDAL.ClearClassRecord();
            await this.ClearOrder();
            await _sysDataClearDAL.ClearClass();
            await _sysDataClearDAL.ClearClassTimes();
            await _sysDataClearDAL.ClearStatisticsClass();
            await _sysDataClearDAL.ClearStatisticsSales();
            await _sysDataClearDAL.ClearStudentLeaveApplyLog();
            await _sysDataClearDAL.ClearTeacherSalary();
            await _sysDataClearDAL.ClearTeacherSchooltimeConfigExclude();
            return true;
        }

        /// <summary>
        /// 物品/费用
        /// </summary>
        public async Task<bool> ClearGoodsAndCost()
        {
            await _sysDataClearDAL.ClearGoodsAndCost();
            return true;
        }

        /// <summary>
        /// 班级
        /// </summary>
        public async Task<bool> ClearClass()
        {
            await _sysDataClearDAL.ClearClass();
            await _sysDataClearDAL.ClearClassRecord();
            await _sysDataClearDAL.ClearClassTimes();
            await _sysDataClearDAL.ClearStatisticsClass();
            await _sysDataClearDAL.ClearStatisticsSales();
            await _sysDataClearDAL.ClearTeacherSalary();
            return true;
        }

        /// <summary>
        /// 学员
        /// </summary>
        public async Task<bool> ClearStudent()
        {
            await _sysDataClearDAL.ClearStudent();
            await _sysDataClearDAL.ClearStudentCourse();
            await _sysDataClearDAL.ClearClassRecord();
            await this.ClearOrder();
            await _sysDataClearDAL.ClearClass();
            await _sysDataClearDAL.ClearClassTimes();
            await _sysDataClearDAL.ClearStatisticsClass();
            await _sysDataClearDAL.ClearStatisticsSales();
            await _sysDataClearDAL.ClearStudentLeaveApplyLog();
            await _sysDataClearDAL.ClearStudentTrackLog();
            await _sysDataClearDAL.ClearGiftExchange();
            await _sysDataClearDAL.ClearCoupons();
            await _sysDataClearDAL.ClearGift();
            await _sysDataClearDAL.ClearTeacherSalary();

            await _sysDataClearDAL.ClearStatisticsStudent();
            await _sysDataClearDAL.ClearStatisticsSales();
            await _sysDataClearDAL.ClearStatisticsClass();

            await this.ClearActiveHomework();
            await this.ClearGrowthRecord();
            await this.ClearWxMessage();
            await this.ClearStudentSmsLog();
            await this.ClearClassRecordEvaluate();
            await this.ClearElectronicAlbum();

            await _sysDataClearDAL.ClearStudentCheckLog();
            await _sysDataClearDAL.ClearStudentAccountRecharge();
            await _sysDataClearDAL.ClearAchievement();
            return true;
        }

        /// <summary>
        /// 用户
        /// </summary>
        public async Task<bool> ClearUser()
        {
            await this.ClearStudent();
            await this.ClearGoodsAndCost();
            await this.ClearCourse();
            await _sysDataClearDAL.ClearUser();
            await _sysDataClearDAL.ClearUserOperationLog();
            await _sysDataClearDAL.ClearUserLog();
            await _sysDataClearDAL.ClearTeacherSalary();
            await _sysDataClearDAL.ClearTeacherSchooltimeConfigExclude();
            return true;
        }

        /// <summary>
        /// 学员课程
        /// </summary>
        public async Task<bool> ClearStudentCourse()
        {
            await this.ClearOrder();
            return true;
        }

        #endregion

        #region  记录

        /// <summary>
        /// 订单
        /// </summary>
        public async Task<bool> ClearOrder()
        {
            await _sysDataClearDAL.ClearClassRecordEvaluate();
            await _sysDataClearDAL.ClearCoupons();
            await _sysDataClearDAL.ClearStudentCourse();
            await _sysDataClearDAL.ClearOrder();
            await _sysDataClearDAL.ClearClassTimes();
            await _sysDataClearDAL.ClearClassRecord();
            await _sysDataClearDAL.ClearStudentLeaveApplyLog();
            await _sysDataClearDAL.ClearStatisticsSales();
            await _sysDataClearDAL.ClearStatisticsClass();
            await _sysDataClearDAL.ClearTeacherSalary();
            return true;
        }

        /// <summary>
        /// 排课
        /// </summary>
        public async Task<bool> ClearClassTimes()
        {
            await _sysDataClearDAL.ClearClassTimes();
            return true;
        }

        /// <summary>
        /// 上课记录
        /// </summary>
        public async Task<bool> ClearClassRecord()
        {
            await _sysDataClearDAL.ClearClassRecord();
            await _sysDataClearDAL.ClearStudentLeaveApplyLog();
            await _sysDataClearDAL.ClearStatisticsClass();
            await _sysDataClearDAL.ClearClassRecordEvaluate();
            await _sysDataClearDAL.ClearTeacherSalary();
            return true;
        }

        /// <summary>
        /// 学员跟进信息
        /// </summary>
        public async Task<bool> ClearStudentTrackLog()
        {
            await _sysDataClearDAL.ClearStudentTrackLog();
            return true;
        }

        /// <summary>
        /// 请假记录
        /// </summary>
        public async Task<bool> ClearStudentLeaveApplyLog()
        {
            await _sysDataClearDAL.ClearStudentLeaveApplyLog();
            return true;
        }

        #endregion

        #region 机构设置

        /// <summary>
        /// 收支项目类型
        /// </summary>
        public async Task<bool> ClearIncomeProjectType()
        {
            await _sysDataClearDAL.ClearIncomeProjectType();
            return true;
        }

        /// <summary>
        /// 亲属关系类型
        /// </summary>
        public async Task<bool> ClearStudentRelationship()
        {
            await _sysDataClearDAL.ClearStudentRelationship();
            return true;
        }

        /// <summary>
        /// 学员标签
        /// </summary>
        public async Task<bool> ClearStudentTag()
        {
            await _sysDataClearDAL.ClearStudentTag();
            return true;
        }

        /// <summary>
        /// 教室
        /// </summary>
        public async Task<bool> ClearClassRoom()
        {
            await _sysDataClearDAL.ClearClassRoom();
            return true;
        }

        /// <summary>
        /// 成长档案类型
        /// </summary>
        public async Task<bool> ClearStudentGrowingTag()
        {
            await _sysDataClearDAL.ClearStudentGrowingTag();
            return true;
        }

        /// <summary>
        /// 上课时间段
        /// </summary>
        public async Task<bool> ClearClassSet()
        {
            await _sysDataClearDAL.ClearClassSet();
            return true;
        }

        /// <summary>
        /// 年级
        /// </summary>
        public async Task<bool> ClearGrade()
        {
            await _sysDataClearDAL.ClearGrade();
            return true;
        }

        /// <summary>
        /// 学员来源
        /// </summary>
        public async Task<bool> ClearStudentSource()
        {
            await _sysDataClearDAL.ClearStudentSource();
            return true;
        }

        /// <summary>
        /// 科目
        /// </summary>
        public async Task<bool> ClearSubject()
        {
            await _sysDataClearDAL.ClearSubject();
            return true;
        }

        /// <summary>
        /// 学员自定义属性
        /// </summary>
        public async Task<bool> ClearStudentExtendField()
        {
            await _sysDataClearDAL.ClearStudentExtendField();
            return true;
        }

        /// <summary>
        /// 节假日设置
        /// </summary>
        public async Task<bool> ClearHolidaySetting()
        {
            await _sysDataClearDAL.ClearHolidaySetting();
            return true;
        }

        public async Task<bool> ClearOtherSetting()
        {
            await _sysDataClearDAL.ClearOtherSetting();
            return true;
        }

        #endregion

        #region 营销中心

        /// <summary>
        /// 礼品兑换记录
        /// </summary>
        public async Task<bool> ClearGiftExchange()
        {
            await _sysDataClearDAL.ClearGiftExchange();
            return true;
        }

        /// <summary>
        /// 礼品信息
        /// </summary>
        public async Task<bool> ClearGift()
        {
            await _sysDataClearDAL.ClearGift();
            return true;
        }

        /// <summary>
        /// 优惠券
        /// </summary>
        public async Task<bool> ClearCoupons()
        {
            await _sysDataClearDAL.ClearCoupons();
            return true;
        }

        /// <summary>
        /// 小禾招生
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearActivity(int tenantId)
        {
            await _sysDataClearDAL.ClearActivity();
            await _sysActivityRouteItemDAL.DelSysActivityRouteItemByTenantId(tenantId);
            return true;
        }

        #endregion


        #region 家校互动

        /// <summary>
        /// 作业
        /// </summary>
        public async Task<bool> ClearActiveHomework()
        {
            await _sysDataClearDAL.ClearActiveHomework();
            return true;
        }

        /// <summary>
        /// 成长档案
        /// </summary>
        public async Task<bool> ClearGrowthRecord()
        {
            await _sysDataClearDAL.ClearGrowthRecord();
            return true;
        }

        /// <summary>
        /// 微信通知
        /// </summary>
        public async Task<bool> ClearWxMessage()
        {
            await _sysDataClearDAL.ClearWxMessage();
            return true;
        }

        /// <summary>
        /// 短信记录
        /// </summary>
        public async Task<bool> ClearStudentSmsLog()
        {
            await _sysDataClearDAL.ClearStudentSmsLog();
            return true;
        }

        /// <summary>
        /// 课后点评
        /// </summary>
        public async Task<bool> ClearClassRecordEvaluate()
        {
            await _sysDataClearDAL.ClearClassRecordEvaluate();
            return true;
        }

        /// <summary>
        /// 电子相册
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearElectronicAlbum()
        {
            await _sysDataClearDAL.ClearElectronicAlbum();
            return true;
        }

        /// <summary>
        /// 清除成绩单
        /// </summary>
        /// <returns></returns>
        public async Task ClearAchievement()
        {
            await _sysDataClearDAL.ClearAchievement();
        }
        #endregion
    }
}
