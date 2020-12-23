using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Dto.Student.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.IncrementLib;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using Microsoft.AspNetCore.Http;
using ETMS.Entity.Config;
using ETMS.Entity.Temp;
using Newtonsoft.Json;

namespace ETMS.Business
{
    public class Student2BLL : IStudent2BLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly IAiface _aiface;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IClassDAL _classDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        public Student2BLL(IStudentDAL studentDAL, IAiface aiface, IUserOperationLogDAL userOperationLogDAL,
            IStudentCheckOnLogDAL studentCheckOnLogDAL, IClassDAL classDAL, ICourseDAL courseDAL, ITenantConfigDAL tenantConfigDAL,
            IEventPublisher eventPublisher, IClassTimesDAL classTimesDAL, IUserDAL userDAL, IStudentCourseDAL studentCourseDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL)
        {
            this._studentDAL = studentDAL;
            this._aiface = aiface;
            this._userOperationLogDAL = userOperationLogDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._classDAL = classDAL;
            this._courseDAL = courseDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._eventPublisher = eventPublisher;
            this._classTimesDAL = classTimesDAL;
            this._userDAL = userDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._aiface.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _userOperationLogDAL, _studentCheckOnLogDAL, _classDAL,
                _courseDAL, _tenantConfigDAL, _classTimesDAL, _userDAL, _studentCourseDAL, _studentCourseConsumeLogDAL);
        }

        public async Task<ResponseBase> StudentFaceListGet(StudentFaceListGetRequest request)
        {
            var studentFaces = await _studentDAL.GetStudentFace();
            var output = new List<StudentFaceListGetOutput>();
            if (studentFaces.Any())
            {
                foreach (var p in studentFaces)
                {
                    output.Add(new StudentFaceListGetOutput()
                    {
                        StudentId = p.Id,
                        FaceUrl = AliyunOssUtil.GetAccessUrlHttps(p.FaceGreyKey)
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentRelieveFace(StudentRelieveFaceKeyRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.FaceKey) && string.IsNullOrEmpty(student.FaceGreyKey))
            {
                return ResponseBase.CommonError("人脸信息已清除");
            }
            await _aiface.StudentClearFace(student.Id);
            await _studentDAL.StudentRelieveFaceKey(student.Id);
            AliyunOssUtil.DeleteObject(student.FaceKey, student.FaceGreyKey);

            await _userOperationLogDAL.AddUserLog(request, $"清除人脸信息：姓名:{student.Name},手机号码:{student.Phone}", Entity.Enum.EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentBindingFace(StudentBindingFaceKeyRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            var imgOssKey = ImageLib.SaveStudentFace(request.LoginTenantId, request.FaceImageBase64);
            var initFaceResult = await _aiface.StudentInitFace(request.CId, AliyunOssUtil.GetAccessUrlHttps(imgOssKey));
            if (!initFaceResult)
            {
                return ResponseBase.CommonError("保存人脸信息失败");
            }
            await _studentDAL.StudentBindingFaceKey(request.CId, imgOssKey, imgOssKey);

            if (!string.IsNullOrEmpty(student.FaceKey) || !string.IsNullOrEmpty(student.FaceGreyKey))
            {
                AliyunOssUtil.DeleteObject(student.FaceKey, student.FaceGreyKey);
            }

            await _userOperationLogDAL.AddUserLog(request, $"采集学员人脸信息：姓名:{student.Name},手机号码:{student.Phone}", Entity.Enum.EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCheckOnLogGetPaging(StudentCheckOnLogGetPagingRequest request)
        {
            var pagingData = await _studentCheckOnLogDAL.GetPaging(request);
            var output = new List<StudentCheckOnLogGetPagingOutput>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            foreach (var p in pagingData.Item1)
            {
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                var explain = string.Empty;
                if (p.CheckType == EmStudentCheckOnLogCheckType.CheckIn)
                {
                    switch (p.Status)
                    {
                        case EmStudentCheckOnLogStatus.NormalNotClass:
                            explain = "签到成功";
                            break;
                        case EmStudentCheckOnLogStatus.NormalAttendClass:
                        case EmStudentCheckOnLogStatus.BeRollcall:
                            var ckClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId.Value);
                            var ckCourse = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId.Value);
                            explain = $"签到成功 - 记上课：班级:{ckClass?.Name},上课时间:{p.ClassOtDesc},消耗课程:{ckCourse},扣课时:{ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes)}";
                            break;
                        case EmStudentCheckOnLogStatus.Revoke:
                            explain = "签到成功 - 已撤销记上课";
                            break;
                    }
                }
                else
                {
                    explain = "签退成功";
                }
                output.Add(new StudentCheckOnLogGetPagingOutput()
                {
                    CheckForm = p.CheckForm,
                    CheckFormDesc = EmStudentCheckOnLogCheckForm.GetStudentCheckOnLogCheckFormDesc(p.CheckForm),
                    CheckOt = p.CheckOt,
                    CheckType = p.CheckType,
                    CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(p.CheckType),
                    Status = p.Status,
                    StudentCheckOnLogId = p.Id,
                    StudentId = p.StudentId,
                    StudentName = student.Name,
                    StudentPhone = student.Phone,
                    Explain = explain,
                    CheckMedium = GetCheckMedium(p.CheckForm, p.CheckMedium)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCheckOnLogGetPagingOutput>(pagingData.Item2, output));
        }

        private string GetCheckMedium(byte checkForm, string checkMedium)
        {
            if (string.IsNullOrEmpty(checkMedium))
            {
                return string.Empty;
            }
            switch (checkForm)
            {
                case EmStudentCheckOnLogCheckForm.Face:
                    return AliyunOssUtil.GetAccessUrlHttps(checkMedium);
                case EmStudentCheckOnLogCheckForm.Card:
                    return checkMedium;
                case EmStudentCheckOnLogCheckForm.TeacherManual:
                    return checkMedium;
            }
            return string.Empty;
        }

        public async Task<ResponseBase> StudentCheckByCard(StudentCheckByCardRequest request)
        {
            var student = await _studentDAL.GetStudent(request.CardNo);
            if (student == null)
            {
                return ResponseBase.CommonError("未找到此卡号对应的学员信息");
            }
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var studentUseCardCheckInConfig = tenantConfig.StudentCheckInConfig.StudentUseCardCheckIn;
            var studentCheckProcess = new StudentCheckProcess(new StudentCheckProcessRequest()
            {
                CheckForm = EmStudentCheckOnLogCheckForm.Card,
                CheckMedium = request.CardNo,
                CheckOt = DateTime.Now,
                IntervalTime = studentUseCardCheckInConfig.IntervalTimeCard,
                Student = student,
                IsRelationClassTimes = studentUseCardCheckInConfig.IsRelationClassTimesCard,
                RelationClassTimesLimitMinute = studentUseCardCheckInConfig.RelationClassTimesLimitMinuteCard,
                IsMustCheckOut = studentUseCardCheckInConfig.IsMustCheckOutCard,
                LoginTenantId = request.LoginTenantId,
                RequestBase = request,
                FaceWhite = null,
                MakeupIsDeClassTimes = tenantConfig.ClassCheckSignConfig.MakeupIsDeClassTimes,
                StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar)
            }, _classTimesDAL, _classDAL, _courseDAL, _eventPublisher, _studentCheckOnLogDAL, _userDAL, _studentCourseDAL, _studentCourseConsumeLogDAL,
            _userOperationLogDAL);
            return await studentCheckProcess.Process();
        }

        public async Task<ResponseBase> StudentCheckByTeacher(StudentCheckByTeacherRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("未找到此学员");
            }
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var studentUseCardCheckInConfig = tenantConfig.StudentCheckInConfig.StudentUseCardCheckIn;
            var studentCheckProcess = new StudentCheckProcess(new StudentCheckProcessRequest()
            {
                CheckForm = EmStudentCheckOnLogCheckForm.TeacherManual,
                CheckMedium = string.Empty,
                CheckOt = DateTime.Now,
                IntervalTime = studentUseCardCheckInConfig.IntervalTimeCard,
                Student = studentBucket.Student,
                IsRelationClassTimes = studentUseCardCheckInConfig.IsRelationClassTimesCard,
                RelationClassTimesLimitMinute = studentUseCardCheckInConfig.RelationClassTimesLimitMinuteCard,
                IsMustCheckOut = studentUseCardCheckInConfig.IsMustCheckOutCard,
                LoginTenantId = request.LoginTenantId,
                FaceWhite = null,
                RequestBase = request,
                MakeupIsDeClassTimes = tenantConfig.ClassCheckSignConfig.MakeupIsDeClassTimes,
                StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, studentBucket.Student.Avatar)
            }, _classTimesDAL, _classDAL, _courseDAL, _eventPublisher, _studentCheckOnLogDAL, _userDAL, _studentCourseDAL, _studentCourseConsumeLogDAL,
            _userOperationLogDAL);
            return await studentCheckProcess.Process();
        }

        public async Task<ResponseBase> StudentCheckByFace(StudentCheckByFaceRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("未找到此学员");
            }
            var checkMedium = ImageLib.SaveStudentSearchFace(request.LoginTenantId, request.FaceImageBase64, AliyunOssTempFileTypeEnum.FaceStudentCheckOn);
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var studentUseFaceCheckInConfig = tenantConfig.StudentCheckInConfig.StudentUseFaceCheckIn;
            FaceInfo FaceWhite = null;
            if (request.ImageIsFaceWhite)
            {
                FaceWhite = new FaceInfo()
                {
                    FaceUrl = AliyunOssUtil.GetAccessUrlHttps(checkMedium),
                    StudentId = request.StudentId
                };
                LOG.Log.Fatal($"[人脸考勤]前端未识别出的人脸:{FaceWhite.FaceUrl}", this.GetType());
            }
            var studentCheckProcess = new StudentCheckProcess(new StudentCheckProcessRequest()
            {
                CheckForm = EmStudentCheckOnLogCheckForm.Face,
                CheckMedium = checkMedium,
                CheckOt = DateTime.Now,
                IntervalTime = studentUseFaceCheckInConfig.IntervalTimeFace,
                Student = studentBucket.Student,
                IsRelationClassTimes = studentUseFaceCheckInConfig.IsRelationClassTimesFace,
                RelationClassTimesLimitMinute = studentUseFaceCheckInConfig.RelationClassTimesLimitMinuteFace,
                IsMustCheckOut = studentUseFaceCheckInConfig.IsMustCheckOutFace,
                LoginTenantId = request.LoginTenantId,
                MakeupIsDeClassTimes = tenantConfig.ClassCheckSignConfig.MakeupIsDeClassTimes,
                FaceWhite = FaceWhite,
                RequestBase = request,
                StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, studentBucket.Student.Avatar)
            }, _classTimesDAL, _classDAL, _courseDAL, _eventPublisher, _studentCheckOnLogDAL, _userDAL, _studentCourseDAL, _studentCourseConsumeLogDAL,
            _userOperationLogDAL);
            return await studentCheckProcess.Process();
        }

        public async Task<ResponseBase> StudentCheckByFace2(StudentCheckByFace2Request request)
        {
            var studentId = await _aiface.SearchPerson(request.FaceImageBase64);
            if (studentId == 0)
            {
                //未识别
                var unCheckMedium = ImageLib.SaveStudentSearchFace(request.LoginTenantId, request.FaceImageBase64, AliyunOssTempFileTypeEnum.FaceBlacklist);
                var faceBlackUrl = AliyunOssUtil.GetAccessUrlHttps(unCheckMedium);
                LOG.Log.Fatal($"[人脸考勤]后端未识别出的人脸:{faceBlackUrl}", this.GetType());
                return ResponseBase.Success(new StudentCheckOutput()
                {
                    CheckState = StudentCheckOutputCheckState.Fail,
                    FaceBlack = new FaceInfo()
                    {
                        StudentId = 0,
                        FaceUrl = faceBlackUrl
                    }
                });
            }
            return await StudentCheckByFace(new StudentCheckByFaceRequest()
            {
                FaceImageBase64 = request.FaceImageBase64,
                IpAddress = request.IpAddress,
                LoginTenantId = request.LoginTenantId,
                IsDataLimit = request.IsDataLimit,
                LoginClientType = request.LoginClientType,
                LoginTimestamp = request.LoginTimestamp,
                LoginUserId = request.LoginUserId,
                StudentId = studentId,
                ImageIsFaceWhite = true
            });
        }

        public async Task<ResponseBase> StudentCheckChoiceClass(StudentCheckChoiceClassRequest request)
        {
            var studentCheckOnLog = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (studentCheckOnLog == null)
            {
                return ResponseBase.CommonError("考勤记录不存在");
            }
            if (studentCheckOnLog.ClassTimesId != null && studentCheckOnLog.ClassTimesId > 0)
            {
                return ResponseBase.CommonError("该次考勤已记上课");
            }
            var myClassTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (myClassTimes == null)
            {
                return ResponseBase.CommonError("课次不存在");
            }
            if (myClassTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                return ResponseBase.CommonError("课次已点名");
            }
            //扣减课时
            //直接扣课时
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var deClassTimesDesc = string.Empty;
            var myStudentDeLog = await _studentCheckOnLogDAL.GetStudentDeLog(myClassTimes.Id, studentCheckOnLog.StudentId);
            if (myStudentDeLog != null) //已存在扣课时记录,繁殖重复扣
            {
                return ResponseBase.CommonError("已存在扣课次记录");
            }
            else
            {
                var classStudent = await _classTimesDAL.GetClassTimesStudent(myClassTimes.Id);
                var myClassStudent = classStudent.FirstOrDefault(p => p.StudentId == studentCheckOnLog.StudentId);
                var isProcess = false;
                var deCourseId = 0L;
                var deStudentClassTimesResult = DeStudentClassTimesResult.GetNotDeEntity();
                var remrak = string.Empty;
                if (myClassStudent != null)
                {
                    deCourseId = myClassStudent.CourseId;
                    if (myClassStudent.StudentType == EmClassStudentType.TryCalssStudent)
                    {
                        isProcess = true;
                        remrak = "试听学员不扣课时";
                        LOG.Log.Info($"[StudentCheckProcess]试听学员不扣课时,LoginTenantId:{studentCheckOnLog.TenantId},Student:{studentCheckOnLog.StudentId}", this.GetType());
                    }
                    if (!tenantConfig.ClassCheckSignConfig.MakeupIsDeClassTimes && myClassStudent.StudentType == EmClassStudentType.MakeUpStudent)
                    {
                        isProcess = true;
                        remrak = "补课学员不扣课时";
                        LOG.Log.Info($"[StudentCheckProcess]补课学员不扣课时,LoginTenantId:{studentCheckOnLog.TenantId},Student:{studentCheckOnLog.StudentId}", this.GetType());
                    }
                }
                if (!isProcess)
                {
                    //扣减课时
                    var myClass = await _classDAL.GetClassBucket(myClassTimes.ClassId);
                    if (myClass == null || myClass.EtClass == null)
                    {
                        return ResponseBase.CommonError("班级不存在");
                    }
                    else
                    {
                        if (deCourseId == 0 && myClass.EtClassStudents != null && myClass.EtClassStudents.Count > 0)
                        {
                            var thisStudent = myClass.EtClassStudents.FirstOrDefault(p => p.StudentId == studentCheckOnLog.StudentId);
                            if (thisStudent != null)
                            {
                                deCourseId = thisStudent.CourseId;
                            }
                        }
                        if (deCourseId == 0)
                        {
                            return ResponseBase.CommonError("未查询到消耗课程信息");
                        }
                        else
                        {
                            deStudentClassTimesResult = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, new DeStudentClassTimesTempRequest()
                            {
                                ClassOt = studentCheckOnLog.CheckOt,
                                TenantId = studentCheckOnLog.TenantId,
                                StudentId = studentCheckOnLog.StudentId,
                                DeClassTimes = myClass.EtClass.DefaultClassTimes,
                                CourseId = deCourseId
                            });
                            if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe)
                            {
                                await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
                                {
                                    CourseId = deCourseId,
                                    DeClassTimes = deStudentClassTimesResult.DeClassTimes,
                                    DeType = deStudentClassTimesResult.DeType,
                                    IsDeleted = EmIsDeleted.Normal,
                                    OrderId = deStudentClassTimesResult.OrderId,
                                    OrderNo = deStudentClassTimesResult.OrderNo,
                                    Ot = studentCheckOnLog.CheckOt,
                                    SourceType = EmStudentCourseConsumeSourceType.StudentCheckIn,
                                    StudentId = studentCheckOnLog.StudentId,
                                    TenantId = studentCheckOnLog.TenantId,
                                    DeClassTimesSmall = 0
                                });
                            }
                            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(studentCheckOnLog.TenantId)
                            {
                                StudentId = studentCheckOnLog.StudentId,
                                CourseId = deCourseId
                            });
                        }
                    }
                }

                studentCheckOnLog.ClassTimesId = myClassTimes.Id;
                studentCheckOnLog.ClassId = myClassTimes.ClassId;
                studentCheckOnLog.CourseId = deCourseId;
                studentCheckOnLog.ClassOtDesc = $"{myClassTimes.ClassOt.EtmsToDateString()}（{EtmsHelper.GetTimeDesc(myClassTimes.StartTime)}~{EtmsHelper.GetTimeDesc(myClassTimes.EndTime)}）";
                studentCheckOnLog.DeType = deStudentClassTimesResult.DeType;
                studentCheckOnLog.DeClassTimes = deStudentClassTimesResult.DeClassTimes;
                studentCheckOnLog.DeSum = deStudentClassTimesResult.DeSum;
                studentCheckOnLog.ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes;
                studentCheckOnLog.DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId;
                studentCheckOnLog.Remark = remrak;
                studentCheckOnLog.Status = EmStudentCheckOnLogStatus.NormalAttendClass;
                await _studentCheckOnLogDAL.EditStudentCheckOnLog(studentCheckOnLog);
                //发通知
                _eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.LoginTenantId)
                {
                    CourseId = deCourseId,
                    StudentId = studentCheckOnLog.StudentId
                });
                return ResponseBase.Success();
            }
        }

        public async Task<ResponseBase> StudentCheckOnLogRevoke(StudentCheckOnLogRevokeRequest request)
        {
            var p = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (p == null)
            {
                return ResponseBase.CommonError("考勤记录不存在");
            }
            if (p.ClassTimesId == null || p.ClassTimesId == 0)
            {
                return ResponseBase.CommonError("该考勤未记上课");
            }
            if (p.Status == EmStudentCheckOnLogStatus.BeRollcall)
            {
                return ResponseBase.CommonError("课次已点名，无法撤销");
            }
            if (p.DeClassTimes > 0 || p.ExceedClassTimes > 0)
            {
                if (p.DeStudentCourseDetailId == null)
                {
                    LOG.Log.Error($"[StudentCheckOnLogRevoke]扣减的课时未记录具体的扣减订单:{JsonConvert.SerializeObject(request)}", this.GetType());
                }
                else
                {
                    //原路返还所扣除的课时
                    await _studentCourseDAL.AddClassTimesOfStudentCourseDetail(p.DeStudentCourseDetailId.Value, p.DeClassTimes);

                    //课消记录
                    await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        DeClassTimes = p.DeClassTimes,
                        DeClassTimesSmall = 0,
                        CourseId = p.CourseId.Value,
                        DeType = EmDeClassTimesType.ClassTimes,
                        OrderId = 0,
                        OrderNo = string.Empty,
                        Ot = DateTime.Now,
                        SourceType = EmStudentCourseConsumeSourceType.StudentCheckInRevoke,
                        StudentId = p.StudentId,
                        TenantId = p.TenantId
                    });
                }
                if (p.ExceedClassTimes > 0)
                {
                    await _studentCourseDAL.DeExceedTotalClassTimes(p.StudentId, p.CourseId.Value, p.ExceedClassTimes);
                }

                _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(p.TenantId)
                {
                    StudentId = p.StudentId,
                    CourseId = p.CourseId.Value
                });
            }

            p.ClassTimesId = null;
            p.ClassId = null;
            p.CourseId = null;
            p.ClassOtDesc = string.Empty;
            p.DeType = EmDeClassTimesType.NotDe;
            p.DeClassTimes = 0;
            p.DeSum = 0;
            p.ExceedClassTimes = 0;
            p.DeStudentCourseDetailId = null;
            p.Remark = "撤销考勤记上课";
            p.Status = EmStudentCheckOnLogStatus.Revoke;
            await _studentCheckOnLogDAL.EditStudentCheckOnLog(p);

            //发通知
            _eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.LoginTenantId)
            {
                CourseId = p.CourseId.Value,
                StudentId = p.StudentId
            });

            await _userOperationLogDAL.AddUserLog(request, "撤销考勤记上课", EmUserOperationType.StudentCheckOn);
            return ResponseBase.Success();
        }
    }
}
