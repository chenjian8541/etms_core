using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Open2.Output;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.ExternalService.Contract;
using ETMS.IBusiness;
using ETMS.IBusiness.SysOp;
using ETMS.IDataAccess;
using ETMS.IDataAccess.ElectronicAlbum;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.ShareTemplate;
using ETMS.IEventProvider;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
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

        private readonly ISysTenantUserDAL _sysTenantUserDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        protected readonly IHttpContextAccessor _httpContextAccessor;
        public Open2BLL(IStudentDAL studentDAL, IUserDAL userDAL, ICourseDAL courseDAL, IClassDAL classDAL,
            IClassRecordDAL classRecordDAL, IClassRoomDAL classRoomDAL, IClassRecordEvaluateDAL classRecordEvaluateDAL,
            ISysTryApplyLogDAL sysTryApplyLogDAL, ISysPhoneSmsCodeDAL sysPhoneSmsCodeDAL, ISmsService smsService,
            IEventPublisher eventPublisher, IShareTemplateUseTypeDAL shareTemplateUseTypeDAL, ISysTenantDAL sysTenantDAL,
            IElectronicAlbumDetailDAL electronicAlbumDetailDAL, IElectronicAlbumDAL electronicAlbumDAL,
            ISysTenantUserDAL sysTenantUserDAL, ITempDataCacheDAL tempDataCacheDAL, IUserOperationLogDAL userOperationLogDAL,
            IHttpContextAccessor httpContextAccessor)
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
            this._sysTenantUserDAL = sysTenantUserDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._httpContextAccessor = httpContextAccessor;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, this._studentDAL, this._userDAL, this._courseDAL, this._classDAL,
                this._classRecordDAL, _classRoomDAL, _classRecordEvaluateDAL, _shareTemplateUseTypeDAL,
                _electronicAlbumDetailDAL, _electronicAlbumDAL, _userOperationLogDAL);
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
                //var isNeedUpdateEvaluateIsRead = false;
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
                    //if (!classRecordEvaluateStudent.IsRead)
                    //{
                    //    isNeedUpdateEvaluateIsRead = true;
                    //}
                }
                //if (isNeedUpdateEvaluateIsRead)
                //{
                //    await _classRecordEvaluateDAL.ClassRecordEvaluateStudentSetRead(request.Id, classRecordEvaluateStudents.Count);
                //}
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
            var isExistTenant = await _sysTenantDAL.TenantGetByPhone(request.Phone);
            if (isExistTenant != null)
            {
                return ResponseBase.CommonError("此手机号码已注册");
            }
            var phoneLog = await _sysTryApplyLogDAL.SysTryApplyLogGet(request.Phone);
            if (phoneLog != null)
            {
                if (phoneLog.Status == EmSysTryApplyLogStatus.Processed)
                {
                    phoneLog.Status = EmSysTryApplyLogStatus.Untreated;
                    phoneLog.HandleRemark = $"{ phoneLog.HandleRemark}_重复提交";
                    await _sysTryApplyLogDAL.EditSysTryApplyLog(phoneLog);
                }
                return ResponseBase.CommonError("已存在此手机号码的注册信息");
            }

            var isHasUser = false;
            var isExistUsers = await _sysTenantUserDAL.GetTenantUser(request.Phone);
            if (isExistUsers != null && isExistUsers.Any())
            {
                isHasUser = true;
            }

            var remark = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("SceneTypeDesc", out var apiKeyHeaderValues))
            {
                var temp = apiKeyHeaderValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(temp))
                {
                    remark = temp;
                }
            }
            if (string.IsNullOrEmpty(remark))
            {
                remark = isHasUser ? "机构员工" : null;
            }
            var log = new ETMS.Entity.Database.Manage.SysTryApplyLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                LinkPhone = request.Phone,
                Name = request.Name,
                Ot = DateTime.Now,
                ClientType = request.ClientType,
                Remark = remark
            };
            await _sysTryApplyLogDAL.AddSysTryApplyLog(log);

            if (!isHasUser)
            {
                _eventPublisher.Publish(new NoticeManageEvent()
                {
                    Type = NoticeManageType.TryApply,
                    TryApplyLog = log
                });
            }
            await SyncTestAccount(request.Name, request.Phone);
            return ResponseBase.Success();
        }

        private async Task SyncTestAccount(string name, string phone)
        {
            this.InitTenantId(SystemConfig.ComConfig.DemoAccountTenantId);
            var logUser = await _userDAL.GetUser(phone);
            if (logUser == null)
            {
                var user = new EtUser()
                {
                    Address = string.Empty,
                    IsTeacher = true,
                    Name = name,
                    Phone = phone,
                    Remark = string.Empty,
                    RoleId = SystemConfig.ComConfig.DemoAccountRouleId,
                    TenantId = SystemConfig.ComConfig.DemoAccountTenantId,
                    JobType = EmUserJobType.FullTime,
                    Password = CryptogramHelper.Encrypt3DES("88888888", SystemConfig.CryptogramConfig.Key)
                };
                await _userDAL.AddUser(user);
                CoreBusiness.ProcessUserPhoneAboutAdd(user, _eventPublisher);
                _eventPublisher.Publish(new SysTenantStatistics2Event(SystemConfig.ComConfig.DemoAccountTenantId));
            }
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
            //设置已读
            if (!p.IsRead)
            {
                await _classRecordEvaluateDAL.ClassRecordEvaluateStudentSetRead(p.ClassRecordStudentId, 1);
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

        public ResponseBase PhoneVerificationCodeGet(PhoneVerificationCodeGetRequest request)
        {
            var myVerificationCode = PhoneVerificationHelper.GetPhoneVerificationCode();
            _tempDataCacheDAL.SetPhoneVerificationCodeBucket(request.Phone, myVerificationCode);
            var imgBase64 = PhoneVerificationHelper.GetCaptcha(myVerificationCode, 80, 34);
            return ResponseBase.Success(new PhoneVerificationCodeGetOutput()
            {
                PhoneVerificationCodeImg = imgBase64
            });
        }

        private async Task<ResponseBase> SendSmsCode(string phone)
        {
            var smsCode = RandomHelper.GetSmsCode();
            var sendSmsRes = await _smsService.ComSendSmscode(new ComSendSmscodeRequest()
            {
                Phone = phone,
                ValidCode = smsCode
            });
            if (!sendSmsRes.IsSuccess)
            {
                return ResponseBase.CommonError("发送短信失败,请稍后再试");
            }
            _sysPhoneSmsCodeDAL.AddSysPhoneSmsCode(phone, smsCode);
            _tempDataCacheDAL.RemovePhoneVerificationCodeBucket(phone);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> CheckPhoneSmsSafe(CheckPhoneSmsSafeRequest request)
        {
            var myVerificationCodeBucket = _tempDataCacheDAL.GetPhoneVerificationCodeBucket(request.Phone);
            if (myVerificationCodeBucket == null || myVerificationCodeBucket.VerificationCode != request.VerificationCode)
            {
                return ResponseBase.CommonError("校验码错误");
            }
            return await SendSmsCode(request.Phone);
        }

        public async Task<ResponseBase> SendSmsCodeAboutRegister(SendSmsCodeAboutRegisterRequest request)
        {
            var myVerificationCodeBucket = _tempDataCacheDAL.GetPhoneVerificationCodeBucket(request.Phone);
            if (myVerificationCodeBucket == null || myVerificationCodeBucket.VerificationCode != request.VerificationCode)
            {
                return ResponseBase.CommonError("校验码错误");
            }
            var isExistTenant = await _sysTenantDAL.TenantGetByPhone(request.Phone);
            if (isExistTenant != null)
            {
                return ResponseBase.CommonError("此手机号码已注册");
            }
            var phoneLog = await _sysTryApplyLogDAL.SysTryApplyLogGet(request.Phone);
            if (phoneLog != null)
            {
                if (phoneLog.Status == EmSysTryApplyLogStatus.Processed)
                {
                    phoneLog.Status = EmSysTryApplyLogStatus.Untreated;
                    phoneLog.HandleRemark = $"{ phoneLog.HandleRemark}_重复提交";
                    await _sysTryApplyLogDAL.EditSysTryApplyLog(phoneLog);
                }
                return ResponseBase.CommonError("已存在此手机号码的注册信息");
            }

            return await SendSmsCode(request.Phone);
        }

        public async Task<ResponseBase> CheckTenantAccount(CheckTenantAccountRequest request)
        {
            var safeSms = _sysPhoneSmsCodeDAL.GetSysPhoneSmsCode(request.Phone);
            if (safeSms == null || safeSms.ExpireAtTime < DateTime.Now || safeSms.SmsCode != request.SmsCode)
            {
                return ResponseBase.CommonError("验证码错误");
            }
            _sysPhoneSmsCodeDAL.RemoveSysPhoneSmsCode(request.Phone);
            var response = new ResponseBase().GetResponseBadRequest("账号信息错误");
            var sysTenantInfo = await _sysTenantDAL.GetTenant(request.TenantCode);
            if (sysTenantInfo == null)
            {
                return response;
            }
            if (!ComBusiness2.CheckTenantCanLogin(sysTenantInfo, out var myMsg))
            {
                return response.GetResponseError(myMsg);
            }
            _userDAL.InitTenantId(sysTenantInfo.Id);
            var userInfo = await _userDAL.GetUser(request.Phone);
            if (userInfo == null)
            {
                return response;
            }
            if (!ComBusiness2.CheckUserCanLogin(userInfo, out var msg))
            {
                return response.GetResponseError(msg);
            }
            var exTime = DateTime.Now.AddMinutes(10);
            return response.GetResponseSuccess(new CheckTenantAccountOutput()
            {
                TNo = TenantLib.GetTenantEncrypt(sysTenantInfo.Id),
                UNo = EtmsHelper2.GetIdEncrypt(userInfo.Id),
                CheckNo = EtmsHelper2.GetIdEncrypt(exTime.EtmsGetTimestamp())
            });
        }

        public async Task<ResponseBase> ChangeTenantUserPwd(ChangeTenantUserPwdRequest request)
        {
            var exTimestamp = EtmsHelper2.GetIdDecrypt2(request.CheckNo);
            var exTime = DataTimeExtensions.StampToDateTime(exTimestamp.ToString());
            if (exTime <= DateTime.Now)
            {
                return ResponseBase.CommonError("此请求已超时，请重新尝试");
            }
            var tenantId = TenantLib.GetTenantDecrypt(request.TNo);
            var userId = EtmsHelper2.GetIdDecrypt2(request.UNo);
            _userDAL.InitTenantId(tenantId);
            _userOperationLogDAL.InitTenantId(tenantId);
            var userInfo = await _userDAL.GetUser(userId);
            userInfo.Password = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _userDAL.EditUser(userInfo);
            await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
            {
                ClientType = request.LoginClientType,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"用户:{userInfo.Name},手机号:{userInfo.Phone}修改密码",
                Ot = DateTime.Now,
                Remark = null,
                TenantId = tenantId,
                UserId = userId,
                Type = (int)EmUserOperationType.UserChangePwd
            });
            return ResponseBase.Success();
        }
    }
}
