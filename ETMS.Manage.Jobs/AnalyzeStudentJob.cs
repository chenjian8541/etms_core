using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    public class AnalyzeStudentJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private readonly IJobAnalyzeBLL _jobAnalyzeBLL;

        private const int _pageSize = 100;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private bool _isOpentGradeAutoUpgrade;

        private List<EtGrade> _allGrade;

        private readonly IGradeDAL _gradeDAL;

        public AnalyzeStudentJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher,
            IJobAnalyzeBLL jobAnalyzeBLL, ITenantConfigDAL tenantConfigDAL, IGradeDAL gradeDAL) : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            this._jobAnalyzeBLL = jobAnalyzeBLL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._gradeDAL = gradeDAL;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            _tenantConfigDAL.InitTenantId(tenant.Id);
            _gradeDAL.InitTenantId(tenant.Id);
            var myTenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var otherConfig = myTenantConfig.TenantOtherConfig;
            if (otherConfig.IsOpentGradeAutoUpgrade)
            {
                if (otherConfig.GradeAutoUpgradeMonth != null && otherConfig.GradeAutoUpgradeDay != null)
                {
                    var month = DateTime.Now.Month;
                    var day = DateTime.Now.Day;
                    if (otherConfig.GradeAutoUpgradeMonth == month && otherConfig.GradeAutoUpgradeDay == day)
                    {
                        this._isOpentGradeAutoUpgrade = true;
                        _allGrade = await _gradeDAL.GetAllGrade();
                    }
                }
 
            }
            var pageCurrent = 1;
            this._jobAnalyzeBLL.ResetTenantId(tenant.Id);
            var myStudentResult = await _jobAnalyzeBLL.GetStudent(_pageSize, pageCurrent);
            if (myStudentResult.Item2 == 0)
            {
                return;
            }
            HandleStudent(tenant.Id, myStudentResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(myStudentResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                var studentResult = await _jobAnalyzeBLL.GetStudent(_pageSize, pageCurrent);
                HandleStudent(tenant.Id, studentResult.Item1);
                pageCurrent++;
            }
        }

        private void HandleStudent(int tenantId, IEnumerable<EtStudent> students)
        {
            foreach (var p in students)
            {
                //_eventPublisher.Publish(new SyncStudentClassInfoEvent(tenantId)
                //{
                //    StudentId = p.Id
                //});
                _eventPublisher.Publish(new UpdateStudentInfoEvent(tenantId)
                {
                    MyStudent = p,
                    IsAnalyzeStudentClass = true,
                    IsOpentGradeAutoUpgrade = _isOpentGradeAutoUpgrade,
                    AllGrade = _allGrade
                });
            }
        }
    }
}
