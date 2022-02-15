﻿using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Open2.Output;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Event.DataContract;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IBusiness.SysOp;
using ETMS.IDataAccess;
using ETMS.IDataAccess.ElectronicAlbum;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.ShareTemplate;
using ETMS.IEventProvider;
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

        private readonly IEventPublisher _eventPublisher;

        private readonly IShareTemplateUseTypeDAL _shareTemplateUseTypeDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IElectronicAlbumDetailDAL _electronicAlbumDetailDAL;

        private readonly IElectronicAlbumDAL _electronicAlbumDAL;

        public Open2BLL(IStudentDAL studentDAL, IUserDAL userDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            IClassRecordDAL classRecordDAL, IClassRoomDAL classRoomDAL, IClassRecordEvaluateDAL classRecordEvaluateDAL,
            ISysTryApplyLogDAL sysTryApplyLogDAL, ISysPhoneSmsCodeDAL sysPhoneSmsCodeDAL, ISmsService smsService,
            IEventPublisher eventPublisher, IShareTemplateUseTypeDAL shareTemplateUseTypeDAL, ISysTenantDAL sysTenantDAL,
            IElectronicAlbumDetailDAL electronicAlbumDetailDAL, IElectronicAlbumDAL electronicAlbumDAL)
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
            this._eventPublisher = eventPublisher;
            this._shareTemplateUseTypeDAL = shareTemplateUseTypeDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._electronicAlbumDetailDAL = electronicAlbumDetailDAL;
            this._electronicAlbumDAL = electronicAlbumDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, this._studentDAL, this._userDAL, this._courseDAL, this._classDAL,
                this._classRecordDAL, _classRoomDAL, _classRecordEvaluateDAL, _shareTemplateUseTypeDAL,
                _electronicAlbumDetailDAL, _electronicAlbumDAL);
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
            var log = new ETMS.Entity.Database.Manage.SysTryApplyLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                LinkPhone = request.Phone,
                Name = request.Name,
                Ot = DateTime.Now,
                Remark = null
            };
            await _sysTryApplyLogDAL.AddSysTryApplyLog(log);

            _eventPublisher.Publish(new NoticeManageEvent()
            {
                Type = NoticeManageType.TryApply,
                TryApplyLog = log
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> EvaluateStudentDetail(EvaluateStudentDetailRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var p = await _classRecordEvaluateDAL.ClassRecordEvaluateStudentGet(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("点评记录不存在");
            }
            var studentBucket = await _studentDAL.GetStudent(p.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var className = string.Empty;
            var teacherName = string.Empty;
            var studentName = studentBucket.Student.Name;
            var courseName = string.Empty;
            var classBucket = await _classDAL.GetClassBucket(p.ClassId);
            if (classBucket != null && classBucket.EtClass != null)
            {
                className = classBucket.EtClass.Name;
            }
            var myUser = await _userDAL.GetUser(p.TeacherId);
            if (myUser != null)
            {
                teacherName = ComBusiness2.GetParentTeacherName(myUser);
            }
            var myCourse = await _courseDAL.GetCourse(p.CourseId);
            if (myCourse != null && myCourse.Item1 != null)
            {
                courseName = myCourse.Item1.Name;
            }
            var output = new EvaluateStudentDetailOutput()
            {
                TeacherId = p.TeacherId,
                ClassId = p.ClassId,
                ClassName = className,
                ClassOt = p.ClassOt.EtmsToDateString(),
                StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                EvaluateContent = p.EvaluateContent,
                Evaluates = EtmsHelper2.GetMediasUrl(p.EvaluateImg),
                Id = p.Id,
                Ot = p.Ot,
                StudentId = p.StudentId,
                StudentName = studentName,
                TeacherName = teacherName,
                StudentAvatar = AliyunOssUtil.GetAccessUrlHttps(studentBucket.Student.Avatar),
                Week = p.Week,
                TenantName = myTenant.Name
            };
            var shareTemplateBucket = await _shareTemplateUseTypeDAL.GetShareTemplate(EmShareTemplateUseType.ClassEvaluate);
            if (shareTemplateBucket != null)
            {
                output.ShareContent = ShareTemplateHandler.TemplateLinkClassEvaluate(shareTemplateBucket.MyShareTemplateLink,
                    output.StudentName, className, courseName, output.ClassOt, output.EvaluateContent, output.TeacherName);
                output.ShowContent = ShareTemplateHandler.TemplateShowClassEvaluate(shareTemplateBucket.MyShareTemplateShow,
                   output.StudentName, className, courseName, output.ClassOt, output.EvaluateContent, output.TeacherName);
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ShareContentGet(ShareContentGetRequest request)
        {
            var shareTemplateBucket = await _shareTemplateUseTypeDAL.GetShareTemplate(request.UseType);
            if (shareTemplateBucket == null)
            {
                return ResponseBase.Success();
            }
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            switch (request.UseType)
            {
                case EmShareTemplateUseType.MicWebsite:
                    return ResponseBase.Success(ShareTemplateHandler.TemplateLinkMicWebsite(shareTemplateBucket.MyShareTemplateLink, myTenant.Name));
                case EmShareTemplateUseType.OnlineMall:
                    return ResponseBase.Success(ShareTemplateHandler.TemplateLinkOnlineMall(shareTemplateBucket.MyShareTemplateLink, myTenant.Name));
            }
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AlbumDetailGet(AlbumDetailGetRequest request)
        {
            var p = await _electronicAlbumDetailDAL.GetElectronicAlbumDetail(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("相册不存在");
            }
            if (p.Status == EmElectronicAlbumStatus.Save)
            {
                return ResponseBase.CommonError("相册未开放");
            }
            var studentBucket = await _studentDAL.GetStudent(p.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            _eventPublisher.Publish(new ElectronicAlbumStatisticsEvent(request.LoginTenantId)
            {
                ElectronicAlbumDetailId = request.Id,
                OpType = ElectronicAlbumStatisticsOpType.Read,
                Ot = DateTime.Now
            });
            var output = new AlbumDetailGetOutput()
            {
                Id = p.Id,
                Name = p.Name,
                CoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.CoverKey),
                RenderUrl = AliyunOssUtil.GetAccessUrlHttps(p.RenderKey)
            };
            var shareTemplateBucket = await _shareTemplateUseTypeDAL.GetShareTemplate(EmShareTemplateUseType.StudentPhoto);
            if (shareTemplateBucket != null)
            {
                output.ShareContent = ShareTemplateHandler.TemplateLinkStudentPhoto(shareTemplateBucket.MyShareTemplateLink, studentBucket.Student.Name, p.Name);
            }
            return ResponseBase.Success(output);
        }

        public ResponseBase AlbumShare(AlbumShareRequest request)
        {
            _eventPublisher.Publish(new ElectronicAlbumStatisticsEvent(request.LoginTenantId)
            {
                ElectronicAlbumDetailId = request.Id,
                OpType = ElectronicAlbumStatisticsOpType.Share,
                Ot = DateTime.Now
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AlbumInfoGet(AlbumInfoGetRequest request)
        {
            var p = await _electronicAlbumDAL.GetElectronicAlbum(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("相册不存在");
            }
            if (p.Status == EmElectronicAlbumStatus.Save)
            {
                return ResponseBase.CommonError("相册未开放");
            }
            var output = new AlbumDetailGetOutput()
            {
                Id = p.Id,
                Name = p.Name,
                CoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.CoverKey),
                RenderUrl = AliyunOssUtil.GetAccessUrlHttps(p.RenderKey)
            };
            var shareTemplateBucket = await _shareTemplateUseTypeDAL.GetShareTemplate(EmShareTemplateUseType.StudentPhoto);
            if (shareTemplateBucket != null)
            {
                output.ShareContent = ShareTemplateHandler.TemplateLinkStudentPhoto(shareTemplateBucket.MyShareTemplateLink, string.Empty, p.Name);
            }
            return ResponseBase.Success(output);
        }
    }
}
