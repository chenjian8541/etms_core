using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
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

        private int _tenantId;

        public CommonHandlerBLL(IStudentCourseDAL studentCourseDAL)
        {
            this._studentCourseDAL = studentCourseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantId = tenantId;
            this.InitDataAccess(tenantId, this._studentCourseDAL);
        }

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
    }
}
