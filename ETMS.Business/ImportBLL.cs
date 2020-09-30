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


        public ImportBLL(IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IStudentSourceDAL studentSourceDAL, IStudentRelationshipDAL studentRelationshipDAL, IGradeDAL gradeDAL, ISysTenantDAL sysTenantDAL,
            IStudentDAL studentDAL)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._studentSourceDAL = studentSourceDAL;
            this._studentRelationshipDAL = studentRelationshipDAL;
            this._gradeDAL = gradeDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._studentDAL = studentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSourceDAL, _studentRelationshipDAL, _gradeDAL);
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
                //byte? gender = null;
                //if (!string.IsNullOrEmpty(p.GenderDesc))
                //{ 
                
                //}
                //var etStudent = new EtStudent()
                //{
                //    Age = p.Birthday.EtmsGetAge(),
                //    Name = p.StudentName,
                //    Avatar = string.Empty,
                //    Birthday = p.Birthday,
                //    CardNo = string.Empty,
                //    CreateBy = request.LoginUserId,
                //    EndClassOt = null,
                //    Gender = p.,
                //    GradeId = request.GradeId,
                //    HomeAddress = request.HomeAddress,
                //    IntentionLevel = request.IntentionLevel,
                //    IsBindingWechat = EmStudentIsBindingWechat.No,
                //    IsDeleted = EmIsDeleted.Normal,
                //    LastJobProcessTime = now,
                //    LastTrackTime = null,
                //    LearningManager = null,
                //    NextTrackTime = null,
                //    Ot = now.Date,
                //    Phone = request.Phone,
                //    PhoneBak = request.PhoneBak,
                //    PhoneBakRelationship = request.PhoneBakRelationship,
                //    PhoneRelationship = request.PhoneRelationship ?? 0,
                //    Points = 0,
                //    Remark = request.Remark,
                //    SchoolName = request.SchoolName,
                //    SourceId = request.SourceId,
                //    StudentType = EmStudentType.HiddenStudent,
                //    Tags = tags,
                //    TenantId = request.LoginTenantId,
                //    TrackStatus = EmStudentTrackStatus.NotTrack,
                //    TrackUser = request.TrackUser
                //};
            }
            return null;
            //var studentExtendInfos = new List<EtStudentExtendInfo>();
            //if (request.StudentExtendItems != null && request.StudentExtendItems.Any())
            //{
            //    foreach (var s in request.StudentExtendItems)
            //    {
            //        studentExtendInfos.Add(new EtStudentExtendInfo()
            //        {
            //            ExtendFieldId = s.CId,
            //            IsDeleted = EmIsDeleted.Normal,
            //            Remark = string.Empty,
            //            StudentId = 0,
            //            TenantId = request.LoginTenantId,
            //            Value1 = s.Value
            //        });
            //    }
            //}
            //var studentId = await _studentDAL.AddStudent(etStudent, studentExtendInfos);
            //SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
            //{
            //    ChangeCount = 1,
            //    OpType = StatisticsStudentOpType.Add,
            //    Time = now
            //}, request, etStudent.Ot, true);
            //await _userOperationLogDAL.AddUserLog(request, $"添加学员：姓名:{request.Name},手机号码:{request.Phone}", EmUserOperationType.StudentManage);
            //return ResponseBase.Success(studentId);
        }
    }
}
