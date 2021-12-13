using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IBusiness.SysOp;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class Open2BLL : IOpen2BLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly IUserDAL _userDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IClassRecordEvaluateDAL _classRecordEvaluateDAL;

        private readonly ISysTryApplyLogDAL _sysTryApplyLogDAL;

        private readonly ISysPhoneSmsCodeDAL _sysPhoneSmsCodeDAL;

        private readonly ISmsService _smsService;

        public Open2BLL(IStudentDAL studentDAL, IUserDAL userDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            IClassRecordDAL classRecordDAL, IClassRoomDAL classRoomDAL, IClassRecordEvaluateDAL classRecordEvaluateDAL,
            ISysTryApplyLogDAL sysTryApplyLogDAL, ISysPhoneSmsCodeDAL sysPhoneSmsCodeDAL, ISmsService smsService)
        {
            this._studentDAL = studentDAL;
            this._userDAL = userDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._classRecordDAL = classRecordDAL;
            this._classRoomDAL = classRoomDAL;
            this._classRecordEvaluateDAL = classRecordEvaluateDAL;
            this._sysTryApplyLogDAL = sysTryApplyLogDAL;
            this._sysPhoneSmsCodeDAL = sysPhoneSmsCodeDAL;
            this._smsService = smsService;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, this._studentDAL, this._userDAL, this._courseDAL, this._classDAL,
                this._classRecordDAL, _classRoomDAL, _classRecordEvaluateDAL);
        }

        public async Task<ResponseBase> ClassRecordDetailGet(ClassRecordDetailGetOpenRequest request)
        {
            var p = await _classRecordDAL.GetEtClassRecordStudentById(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("上课记录不存在");
            }
            var courseTempBox = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var classBucket = await _classDAL.GetClassBucket(p.ClassId);
            if (classBucket == null || classBucket.EtClass == null)
            {
                return ResponseBase.CommonError("班级不存在");
            }
            var teachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, p.Teachers);
            var classRoomIdsDesc = string.Empty;
            if (!string.IsNullOrEmpty(p.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, p.ClassRoomIds);
            }
            var studentBucket = await _studentDAL.GetStudent(p.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            var output = new ClassRecordDetailGetOutput()
            {
                ClassRecordBascInfo = new ClassRecordBascInfo()
                {
                    ClassContent = p.ClassContent,
                    ClassId = p.ClassId,
                    ClassName = classBucket.EtClass.Name,
                    ClassOtDesc = p.ClassOt.EtmsToDateString(),
                    CourseDesc = await ComBusiness.GetCourseName(courseTempBox, _courseDAL, p.CourseId),
                    CourseId = p.CourseId,
                    EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                    RewardPoints = p.RewardPoints,
                    StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                    StudentCheckStatus = p.StudentCheckStatus,
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    StudentId = p.StudentId,
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(p.StudentType),
                    TeacherNum = p.TeacherNum,
                    TeachersDesc = teachersDesc,
                    Week = p.Week,
                    ClassRoomIdsDesc = classRoomIdsDesc,
                    StudentName = student.Name,
                    StudentAvatarUrl = AliyunOssUtil.GetAccessUrlHttps(student.Avatar),
                    WeekDesc = $"星期{EtmsHelper.GetWeekDesc(p.Week)}",
                    DeClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes),
                },
                EvaluateStudentInfos = new List<ClassRecordEvaluateStudentInfo>()
            };
            var classRecordEvaluateStudents = await _classRecordEvaluateDAL.GetClassRecordEvaluateStudent(request.Id);
            if (classRecordEvaluateStudents.Count > 0)
            {
                var isNeedUpdateEvaluateIsRead = false;
                foreach (var classRecordEvaluateStudent in classRecordEvaluateStudents)
                {
                    var teacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, classRecordEvaluateStudent.TeacherId);
                    if (teacher == null)
                    {
                        continue;
                    }
                    output.EvaluateStudentInfos.Add(new ClassRecordEvaluateStudentInfo()
                    {
                        EvaluateContent = classRecordEvaluateStudent.EvaluateContent,
                        EvaluateStudentId = classRecordEvaluateStudent.Id,
                        EvaluateOtDesc = EtmsHelper.GetOtFriendlyDesc(classRecordEvaluateStudent.Ot),
                        TeacherId = classRecordEvaluateStudent.TeacherId,
                        TeacherAvatar = AliyunOssUtil.GetAccessUrlHttps(teacher.Avatar),
                        TeacherName = ComBusiness2.GetParentTeacherName(teacher),
                        EvaluateMedias = ComBusiness3.GetMediasUrl(classRecordEvaluateStudent.EvaluateImg)
                    });
                    if (!classRecordEvaluateStudent.IsRead)
                    {
                        isNeedUpdateEvaluateIsRead = true;
                    }
                }
                if (isNeedUpdateEvaluateIsRead)
                {
                    await _classRecordEvaluateDAL.ClassRecordEvaluateStudentSetRead(request.Id, classRecordEvaluateStudents.Count);
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> CheckPhoneSmsSend(CheckPhoneSmsSendRequest request)
        {
            var smsCode = RandomHelper.GetSmsCode();
            var sendSmsRes = await _smsService.ComSendSmscode(new ComSendSmscodeRequest()
            {
                Phone = request.Phone,
                ValidCode = smsCode
            });
            if (!sendSmsRes.IsSuccess)
            {
                return ResponseBase.CommonError("发送短信失败,请稍后再试");
            }
            _sysPhoneSmsCodeDAL.AddSysPhoneSmsCode(request.Phone, smsCode);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TryApplyLogAdd(TryApplyLogAddRequest request)
        {
            var safeSms = _sysPhoneSmsCodeDAL.GetSysPhoneSmsCode(request.Phone);
            if (safeSms == null || safeSms.ExpireAtTime < DateTime.Now || safeSms.SmsCode != request.SmsCode)
            {
                return ResponseBase.CommonError("验证码错误");
            }
            _sysPhoneSmsCodeDAL.RemoveSysPhoneSmsCode(request.Phone);
            await _sysTryApplyLogDAL.AddSysTryApplyLog(new ETMS.Entity.Database.Manage.SysTryApplyLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                LinkPhone = request.Phone,
                Name = request.Name,
                Ot = DateTime.Now,
                Remark = null
            });
            return ResponseBase.Success();
        }
    }
}
