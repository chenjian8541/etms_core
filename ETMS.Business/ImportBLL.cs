using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.External.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Event.DataContract;
using ETMS.IEventProvider;
using ETMS.Entity.Dto.External.Output;

namespace ETMS.Business
{
    public class ImportBLL : IImportBLL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IStudentSourceDAL _studentSourceDAL;

        private readonly IStudentRelationshipDAL _studentRelationshipDAL;

        private readonly IGradeDAL _gradeDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IUserOperationLogDAL _userOperationLogDAL;


        public ImportBLL(IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IStudentSourceDAL studentSourceDAL, IStudentRelationshipDAL studentRelationshipDAL, IGradeDAL gradeDAL, ISysTenantDAL sysTenantDAL,
            IStudentDAL studentDAL, IEventPublisher eventPublisher, IUserOperationLogDAL userOperationLogDAL)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._studentSourceDAL = studentSourceDAL;
            this._studentRelationshipDAL = studentRelationshipDAL;
            this._gradeDAL = gradeDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._studentDAL = studentDAL;
            this._eventPublisher = eventPublisher;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSourceDAL, _studentRelationshipDAL, _gradeDAL, _studentDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> GetImportStudentExcelTemplate(GetImportStudentExcelTemplateRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var checkImportStudentTemplateFileResult = ExcelLib.CheckImportStudentExcelTemplate(tenant.TenantCode, _appConfigurtaionServices.AppSettings.StaticFilesConfig.ServerPath);
            if (!checkImportStudentTemplateFileResult.IsExist)
            {
                ExcelLib.GenerateImportStudentExcelTemplate(new ImportStudentExcelTemplateRequest()
                {
                    CheckResult = checkImportStudentTemplateFileResult,
                    GradeAll = await _gradeDAL.GetAllGrade(),
                    StudentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship(),
                    StudentSourceAll = await _studentSourceDAL.GetAllStudentSource(),
                });
            }
            return ResponseBase.Success(UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, checkImportStudentTemplateFileResult.UrlKey));
        }

        public async Task<ResponseBase> ImportStudent(ImportStudentRequest request)
        {
            var msg = request.Validate();
            if (!string.IsNullOrEmpty(msg))
            {
                return ResponseBase.CommonError(msg);
            }
            var studentList = new List<EtStudent>();
            var now = DateTime.Now;
            var gradeAll = await _gradeDAL.GetAllGrade();
            var studentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship();
            var studentSourceAll = await _studentSourceDAL.GetAllStudentSource();
            foreach (var p in request.ImportStudents)
            {
                if (await _studentDAL.ExistStudent(p.StudentName, p.Phone))
                {
                    continue;
                }

                var phoneRelationship = 0L;
                if (!string.IsNullOrEmpty(p.PhoneRelationshipDesc))
                {
                    var myPhoneRelationship = studentRelationshipAll.FirstOrDefault(j => j.Name == p.PhoneRelationshipDesc);
                    if (myPhoneRelationship != null)
                    {
                        phoneRelationship = myPhoneRelationship.Id;
                    }
                }

                byte? gender = null;
                if (!string.IsNullOrEmpty(p.GenderDesc))
                {
                    gender = p.GenderDesc.Trim() == "男" ? EmGender.Man : EmGender.Woman;
                }

                long? gradeId = null;
                if (!string.IsNullOrEmpty(p.GradeDesc))
                {
                    var myGenderDesc = gradeAll.FirstOrDefault(j => j.Name == p.GradeDesc);
                    gradeId = myGenderDesc?.Id;
                }

                long? sourceId = null;
                if (!string.IsNullOrEmpty(p.SourceDesc))
                {
                    var mySourceDesc = studentSourceAll.FirstOrDefault(j => j.Name == p.SourceDesc);
                    sourceId = mySourceDesc?.Id;
                }

                studentList.Add(new EtStudent()
                {
                    Age = p.Birthday.EtmsGetAge(),
                    Name = p.StudentName,
                    Avatar = string.Empty,
                    Birthday = p.Birthday,
                    CardNo = string.Empty,
                    CreateBy = request.LoginUserId,
                    EndClassOt = null,
                    Gender = gender,
                    GradeId = gradeId,
                    HomeAddress = p.HomeAddress,
                    IntentionLevel = EmStudentIntentionLevel.Middle,
                    IsBindingWechat = EmStudentIsBindingWechat.No,
                    IsDeleted = EmIsDeleted.Normal,
                    LastJobProcessTime = now,
                    LastTrackTime = null,
                    LearningManager = null,
                    NextTrackTime = null,
                    Ot = now.Date,
                    Phone = p.Phone,
                    PhoneBak = p.PhoneBak,
                    PhoneBakRelationship = null,
                    PhoneRelationship = phoneRelationship,
                    Points = 0,
                    Remark = p.Remark,
                    SchoolName = p.SchoolName,
                    SourceId = sourceId,
                    StudentType = EmStudentType.HiddenStudent,
                    Tags = string.Empty,
                    TenantId = request.LoginTenantId,
                    TrackStatus = EmStudentTrackStatus.NotTrack,
                    TrackUser = null
                });
            }
            if (studentList.Count > 0)
            {
                _studentDAL.AddStudent(studentList);
                SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
                {
                    ChangeCount = studentList.Count,
                    OpType = StatisticsStudentOpType.Add,
                    Time = now
                }, request, now.Date, true);
                await _userOperationLogDAL.AddUserLog(request, $"批量导入潜在学员，成功导入了{studentList.Count}位学员", EmUserOperationType.StudentManage);
            }
            return ResponseBase.Success(new ImportStudentOutput()
            {
                SuccessCount = studentList.Count
            });
        }

        private void SyncStatisticsStudentInfo(StatisticsStudentCountEvent studentCountEvent, RequestBase request, DateTime ot, bool isChangeStudentSource)
        {
            if (studentCountEvent != null)
            {
                _eventPublisher.Publish(studentCountEvent);
            }
            if (isChangeStudentSource)
            {
                _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentSource, StatisticsDate = ot });
            }
            _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentType, StatisticsDate = ot });
        }
    }
}
