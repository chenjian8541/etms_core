using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    /// <summary>
    /// 处理一些公共的业务逻辑
    /// </summary>
    public class CommonHandlerBLL : ICommonHandlerBLL
    {
        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IIncomeLogDAL _incomeLogDAL;

        private int _tenantId;

        public CommonHandlerBLL(IStudentCourseDAL studentCourseDAL, IEventPublisher eventPublisher, IIncomeLogDAL incomeLogDAL)
        {
            this._studentCourseDAL = studentCourseDAL;
            this._eventPublisher = eventPublisher;
            this._incomeLogDAL = incomeLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantId = tenantId;
            this.InitDataAccess(tenantId, this._studentCourseDAL, _incomeLogDAL);
        }

        /// <summary>
        /// 撤销点名/修改点名时  
        /// 如果课程是"已结课"，判断学员的课程是否应该从"已结课"改成"正常"
        /// </summary>
        /// <param name="deStudentCourseDetailId"></param>
        /// <returns></returns>
        public async Task AnalyzeStudentCourseDetailRestoreNormalStatus(long deStudentCourseDetailId)
        {
            var studentCourseDetail = await _studentCourseDAL.GetEtStudentCourseDetailById(deStudentCourseDetailId);
            if (studentCourseDetail == null)
            {
                LOG.Log.Error($"[AnalyzeStudentCourseDetailRestoreNormalStatus]学员课程详情记录不存在,{_tenantId}-{deStudentCourseDetailId}", this.GetType());
                return;
            }
            if (studentCourseDetail.Status != EmStudentCourseStatus.EndOfClass)
            {
                return;
            }
            if (studentCourseDetail.DeType != EmDeClassTimesType.ClassTimes)
            {
                return;
            }
            if (studentCourseDetail.SurplusQuantity == 0 && studentCourseDetail.SurplusSmallQuantity == 0)
            {
                return;
            }
            if (studentCourseDetail.EndTime != null && studentCourseDetail.EndTime.Value < DateTime.Now.Date)
            {
                return;
            }
            await _studentCourseDAL.SetStudentCourseDetailNewStatus(deStudentCourseDetailId, studentCourseDetail.StudentId, EmStudentCourseStatus.Normal);
        }

        public async Task DelOrdersRefreshAboutStatus(IEnumerable<OrderStudentOt> orders)
        {
            var ot = new List<DateTime>();
            if (orders.Any())
            {
                var ids = new List<long>();
                foreach (var p in orders)
                {
                    ids.Add(p.Id);
                    ot.Add(p.Ot);
                }
                await _incomeLogDAL.DelIncomeLog(ids);
            }
            if (ot.Any())
            {
                var myot = ot.Distinct();
                foreach (var item in myot)
                {
                    _eventPublisher.Publish(new StatisticsSalesProductEvent(_tenantId)
                    {
                        StatisticsDate = item
                    });
                    _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(_tenantId)
                    {
                        StatisticsDate = item
                    });
                    _eventPublisher.Publish(new StatisticsSalesCourseEvent(_tenantId)
                    {
                        StatisticsDate = item
                    });
                }
            }
            _eventPublisher.Publish(new SysTenantStatisticsWeekAndMonthEvent(_tenantId));
        }
    }
}
