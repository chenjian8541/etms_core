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
using ETMS.LOG;
using ETMS.Entity.CacheBucket.RedisLock;

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

        private readonly ITempStudentNeedCheckDAL _tempStudentNeedCheckDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly IStudentCourseAnalyzeBLL _studentCourseAnalyzeBLL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private IDistributedLockDAL _distributedLockDAL;

        private ICommonHandlerBLL _commonHandlerBLL;

        private readonly IStudentTrackLogDAL _studentTrackLogDAL;

        public Student2BLL(IStudentDAL studentDAL, IAiface aiface, IUserOperationLogDAL userOperationLogDAL,
            IStudentCheckOnLogDAL studentCheckOnLogDAL, IClassDAL classDAL, ICourseDAL courseDAL, ITenantConfigDAL tenantConfigDAL,
            IEventPublisher eventPublisher, IClassTimesDAL classTimesDAL, IUserDAL userDAL, IStudentCourseDAL studentCourseDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL,
            ITempStudentNeedCheckDAL tempStudentNeedCheckDAL, ITempDataCacheDAL tempDataCacheDAL, IStudentCourseAnalyzeBLL studentCourseAnalyzeBLL,
            IStudentPointsLogDAL studentPointsLogDAL, IDistributedLockDAL distributedLockDAL, ICommonHandlerBLL commonHandlerBLL,
            IStudentTrackLogDAL studentTrackLogDAL)
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
            this._tempStudentNeedCheckDAL = tempStudentNeedCheckDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._studentCourseAnalyzeBLL = studentCourseAnalyzeBLL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._distributedLockDAL = distributedLockDAL;
            this._commonHandlerBLL = commonHandlerBLL;
            this._studentTrackLogDAL = studentTrackLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._aiface.InitTenantId(tenantId);
            this._studentCourseAnalyzeBLL.InitTenantId(tenantId);
            this._commonHandlerBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _userOperationLogDAL, _studentCheckOnLogDAL, _classDAL,
                _courseDAL, _tenantConfigDAL, _classTimesDAL, _userDAL, _studentCourseDAL, _studentCourseConsumeLogDAL,
                _tempStudentNeedCheckDAL, _studentPointsLogDAL, _studentTrackLogDAL);
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

            await _userOperationLogDAL.AddUserLog(request, $"清除人脸信息-学员姓名:{student.Name},手机号码:{student.Phone}", Entity.Enum.EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        [Obsolete("合并到StudentBindingFace接口")]
        public async Task<ResponseBase> StudentBindingFaceCheck(StudentBindingFaceCheckRequest request)
        {
            request.FaceImageBase64 = request.FaceImageBase64.Substring(request.FaceImageBase64.IndexOf(",") + 1);
            var faceResult = await _aiface.SearchPerson(request.FaceImageBase64);
            var output = new StudentBindingFaceCheckOutput();
            if (faceResult.Item1 == 0 && !string.IsNullOrEmpty(faceResult.Item2)) //人脸图像模糊
            {
                output.IsCanBinding = false;
                output.ErrMsg = faceResult.Item2;
                return ResponseBase.Success(output);
            }
            if (faceResult.Item1 > 0 && faceResult.Item1 != request.CId) //检测到其他学员
            {
                var studentBucket = await _studentDAL.GetStudent(faceResult.Item1);
                if (studentBucket != null && studentBucket.Student != null)
                {
                    output.IsCanBinding = false;
                    output.IsSameStudent = true;
                    output.SameStudentName = studentBucket.Student.Name;
                    output.SameStudentPhone = ComBusiness3.PhoneSecrecy(studentBucket.Student.Phone, request.SecrecyType);
                    return ResponseBase.Success(output);
                }
                else
                {
                    //检测到的学员不存在
                    await _aiface.StudentClearFace(faceResult.Item1);
                }
            }
            output.IsCanBinding = true;
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentBindingFace(StudentBindingFaceKeyRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            request.FaceImageBase64 = request.FaceImageBase64.Substring(request.FaceImageBase64.IndexOf(",") + 1);
            if (!request.IsIgnoreSameStudent) //检验是否有其他学员
            {
                var faceResult = await _aiface.SearchPerson(request.FaceImageBase64);
                if (faceResult.Item1 == 0 && !string.IsNullOrEmpty(faceResult.Item2)) //人脸图像模糊
                {
                    return ResponseBase.Success(new StudentBindingFaceOutput()
                    {
                        BindingState = StudentBindingFaceOutputState.Fail,
                        ErrMsg = faceResult.Item2
                    });
                }
                if (faceResult.Item1 > 0 && faceResult.Item1 != request.CId) //检测到其他学员
                {
                    var otherStudentBucket = await _studentDAL.GetStudent(faceResult.Item1);
                    if (otherStudentBucket != null && otherStudentBucket.Student != null)
                    {
                        return ResponseBase.Success(new StudentBindingFaceOutput()
                        {
                            BindingState = StudentBindingFaceOutputState.Fail,
                            ErrMsg = "人脸信息重复",
                            IsSameStudent = true,
                            SameStudentName = otherStudentBucket.Student.Name,
                            SameStudentPhone = ComBusiness3.PhoneSecrecy(otherStudentBucket.Student.Phone, request.SecrecyType)
                        });
                    }
                    else
                    {
                        //检测到的学员不存在
                        await _aiface.StudentClearFace(faceResult.Item1);
                    }
                }
            }
            var student = studentBucket.Student;
            var imgOssKey = ImageLib.SaveStudentFace(request.LoginTenantId, request.FaceImageBase64);
            var initFaceResult = await _aiface.StudentInitFace(request.CId, request.FaceImageBase64);
            if (!initFaceResult.Item1)
            {
                Log.Fatal($"[人脸识别]人脸采集，{imgOssKey}", this.GetType());
                AliyunOssUtil.DeleteObject(imgOssKey);
                return ResponseBase.Success(new StudentBindingFaceOutput(StudentBindingFaceOutputState.Fail, initFaceResult.Item2));
            }
            await _studentDAL.StudentBindingFaceKey(request.CId, imgOssKey, imgOssKey);

            if (!string.IsNullOrEmpty(student.FaceKey) || !string.IsNullOrEmpty(student.FaceGreyKey))
            {
                AliyunOssUtil.DeleteObject(student.FaceKey, student.FaceGreyKey);
            }

            await _userOperationLogDAL.AddUserLog(request, $"学员人脸采集-姓名:{student.Name},手机号码:{student.Phone}", Entity.Enum.EmUserOperationType.StudentManage);
            return ResponseBase.Success(new StudentBindingFaceOutput(StudentBindingFaceOutputState.Success, initFaceResult.Item2));
        }

        public async Task<ResponseBase> StudentCheckOnLogGetPaging(StudentCheckOnLogGetPagingRequest request)
        {
            if (request.TrackUser == null) //客户要求  考勤记录增加跟进人查询
            {
                return await StudentCheckOnLogGetPaging1(request);
            }
            else
            {
                return await StudentCheckOnLogGetPaging2(request);
            }
        }

        private async Task<ResponseBase> StudentCheckOnLogGetPaging1(StudentCheckOnLogGetPagingRequest request)
        {
            var pagingData = await _studentCheckOnLogDAL.GetPaging(request);
            var output = new List<StudentCheckOnLogGetPagingOutput>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            string className;
            string courseName;
            string deClassTimesDesc;
            string checkMedium;
            string studentTrackUserName;
            var maxDate = DateTime.Now.AddDays(-7);
            foreach (var p in pagingData.Item1)
            {
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                var explain = string.Empty;
                className = string.Empty;
                courseName = string.Empty;
                deClassTimesDesc = string.Empty;
                checkMedium = string.Empty;
                if (p.CheckType == EmStudentCheckOnLogCheckType.CheckIn)
                {
                    switch (p.Status)
                    {
                        case EmStudentCheckOnLogStatus.NormalNotClass:
                            explain = "签到成功";
                            break;
                        case EmStudentCheckOnLogStatus.NormalAttendClass:
                        case EmStudentCheckOnLogStatus.BeRollcall:
                            if (p.ClassTimesId == null)
                            {
                                //直接扣减课时
                                var ckCourse = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId.Value);
                                courseName = ckCourse;
                                deClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes);
                                explain = $"签到成功 - 记上课：消耗课程({ckCourse})，扣课时({deClassTimesDesc})";
                            }
                            else
                            {
                                var ckClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId.Value);
                                var ckCourse = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId.Value);
                                className = ckClass?.Name;
                                courseName = ckCourse;
                                deClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes);
                                explain = $"签到成功 - 记上课：班级({className})，上课时间({p.ClassOtDesc})，消耗课程({ckCourse})，扣课时({deClassTimesDesc})";
                            }
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
                if (p.CheckForm == EmStudentCheckOnLogCheckForm.Face
                    && !string.IsNullOrEmpty(p.CheckMedium) && p.CheckOt > maxDate)
                {
                    checkMedium = AliyunOssUtil.GetAccessUrlHttps(p.CheckMedium);
                }
                if (p.CheckForm == EmStudentCheckOnLogCheckForm.Card)
                {
                    checkMedium = p.CheckMedium;
                }
                studentTrackUserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, student.TrackUser);
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
                    StudentPhone = ComBusiness3.PhoneSecrecy(student.Phone, request.SecrecyType),
                    Explain = explain,
                    Remark = p.Remark,
                    CheckMedium = checkMedium,
                    ClassName = className,
                    CourseName = courseName,
                    DeClassTimesDesc = deClassTimesDesc,
                    StatusDesc = EmStudentCheckOnLogStatus.GetStatusDesc(p.Status),
                    StudentTrackUserName = studentTrackUserName
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCheckOnLogGetPagingOutput>(pagingData.Item2, output));
        }

        private async Task<ResponseBase> StudentCheckOnLogGetPaging2(StudentCheckOnLogGetPagingRequest request)
        {
            var pagingData = await _studentCheckOnLogDAL.GetViewPaging(request);
            var output = new List<StudentCheckOnLogGetPagingOutput>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            string className;
            string courseName;
            string deClassTimesDesc;
            string checkMedium;
            string studentTrackUserName;
            var maxDate = DateTime.Now.AddDays(-7);
            foreach (var p in pagingData.Item1)
            {
                var explain = string.Empty;
                className = string.Empty;
                courseName = string.Empty;
                deClassTimesDesc = string.Empty;
                checkMedium = string.Empty;
                if (p.CheckType == EmStudentCheckOnLogCheckType.CheckIn)
                {
                    switch (p.Status)
                    {
                        case EmStudentCheckOnLogStatus.NormalNotClass:
                            explain = "签到成功";
                            break;
                        case EmStudentCheckOnLogStatus.NormalAttendClass:
                        case EmStudentCheckOnLogStatus.BeRollcall:
                            if (p.ClassTimesId == null)
                            {
                                //直接扣减课时
                                var ckCourse = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId.Value);
                                courseName = ckCourse;
                                deClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes);
                                explain = $"签到成功 - 记上课：消耗课程({ckCourse})，扣课时({deClassTimesDesc})";
                            }
                            else
                            {
                                var ckClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId.Value);
                                var ckCourse = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId.Value);
                                className = ckClass?.Name;
                                courseName = ckCourse;
                                deClassTimesDesc = ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes);
                                explain = $"签到成功 - 记上课：班级({className})，上课时间({p.ClassOtDesc})，消耗课程({ckCourse})，扣课时({deClassTimesDesc})";
                            }
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
                if (p.CheckForm == EmStudentCheckOnLogCheckForm.Face
                    && !string.IsNullOrEmpty(p.CheckMedium) && p.CheckOt > maxDate)
                {
                    checkMedium = AliyunOssUtil.GetAccessUrlHttps(p.CheckMedium);
                }
                if (p.CheckForm == EmStudentCheckOnLogCheckForm.Card)
                {
                    checkMedium = p.CheckMedium;
                }
                studentTrackUserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.TrackUser);
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
                    StudentName = p.StudentName,
                    StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType),
                    Explain = explain,
                    Remark = p.Remark,
                    CheckMedium = checkMedium,
                    ClassName = className,
                    CourseName = courseName,
                    DeClassTimesDesc = deClassTimesDesc,
                    StatusDesc = EmStudentCheckOnLogStatus.GetStatusDesc(p.Status),
                    StudentTrackUserName = studentTrackUserName
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCheckOnLogGetPagingOutput>(pagingData.Item2, output));
        }

        //private string GetCheckMedium(byte checkForm, string checkMedium)
        //{
        //    if (checkForm == EmStudentCheckOnLogCheckForm.Face)
        //    {
        //        return AliyunOssUtil.GetAccessUrlHttps(checkMedium);
        //    }
        //    return checkMedium;
        //}

        public async Task<ResponseBase> StudentCheckByCard(StudentCheckByCardRequest request)
        {
            var student = await _studentDAL.GetStudent(request.CardNo);
            if (student == null)
            {
                return ResponseBase.CommonError("未找到此卡号对应的学员信息");
            }
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var studentUseCardCheckInConfig = tenantConfig.StudentCheckInConfig.StudentUseCardCheckIn;
            var studentCheckProcess = new StudentCheckProcess(new StudentCheckProcessRequest(tenantConfig)
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
                FaceAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, ComBusiness2.GetStudentImage(student.Avatar, student.FaceKey))
            }, _classTimesDAL, _classDAL, _courseDAL, _eventPublisher, _studentCheckOnLogDAL, _userDAL, _studentCourseDAL, _studentCourseConsumeLogDAL,
            _userOperationLogDAL, _tempStudentNeedCheckDAL, _tempDataCacheDAL, _studentCourseAnalyzeBLL, _studentDAL, _studentPointsLogDAL,
            _distributedLockDAL, _studentTrackLogDAL);
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
            var studentCheckProcess = new StudentCheckProcess(new StudentCheckProcessRequest(tenantConfig)
            {
                CheckForm = EmStudentCheckOnLogCheckForm.ManualCheck,
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
                FaceAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, ComBusiness2.GetStudentImage(studentBucket.Student.Avatar, studentBucket.Student.FaceKey))
            }, _classTimesDAL, _classDAL, _courseDAL, _eventPublisher, _studentCheckOnLogDAL, _userDAL, _studentCourseDAL, _studentCourseConsumeLogDAL,
            _userOperationLogDAL, _tempStudentNeedCheckDAL, _tempDataCacheDAL, _studentCourseAnalyzeBLL, _studentDAL, _studentPointsLogDAL,
            _distributedLockDAL, _studentTrackLogDAL);
            return await studentCheckProcess.Process();
        }

        public async Task<ResponseBase> StudentCheckByFace(StudentCheckByFaceRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                await _aiface.StudentDelete(request.StudentId);
                return ResponseBase.CommonError("未找到此学员");
            }
            request.FaceImageBase64 = request.FaceImageBase64.Substring(request.FaceImageBase64.IndexOf(",") + 1);
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
            var studentCheckProcess = new StudentCheckProcess(new StudentCheckProcessRequest(tenantConfig)
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
                FaceAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, ComBusiness2.GetStudentImage(studentBucket.Student.Avatar, studentBucket.Student.FaceKey))
            }, _classTimesDAL, _classDAL, _courseDAL, _eventPublisher, _studentCheckOnLogDAL, _userDAL, _studentCourseDAL, _studentCourseConsumeLogDAL,
            _userOperationLogDAL, _tempStudentNeedCheckDAL, _tempDataCacheDAL, _studentCourseAnalyzeBLL, _studentDAL, _studentPointsLogDAL,
            _distributedLockDAL, _studentTrackLogDAL);
            return await studentCheckProcess.Process();
        }

        public async Task<ResponseBase> StudentCheckByFace2(StudentCheckByFace2Request request)
        {
            request.FaceImageBase64 = request.FaceImageBase64.Substring(request.FaceImageBase64.IndexOf(",") + 1);
            var faceResult = await _aiface.SearchPerson(request.FaceImageBase64);
            if (faceResult.Item1 == 0 && !string.IsNullOrEmpty(faceResult.Item2))
            {
                return ResponseBase.Success(StudentCheckOutput.CheckFail(faceResult.Item2, null, true));
            }
            if (faceResult.Item1 == 0)
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
                StudentId = faceResult.Item1,
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
            var myStudentDeLog = await _studentCheckOnLogDAL.GetStudentDeLog(myClassTimes.Id, studentCheckOnLog.StudentId);
            if (myStudentDeLog != null) //已存在扣课时记录,繁殖重复扣
            {
                return ResponseBase.CommonError("已记上课，请勿重复操作");
            }
            else
            {
                var deStudentClassTimesResultTuple = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, _classTimesDAL, _classDAL,
                     tenantConfig.ClassCheckSignConfig.MakeupIsDeClassTimes, myClassTimes, studentCheckOnLog.StudentId, studentCheckOnLog.CheckOt);
                if (!string.IsNullOrEmpty(deStudentClassTimesResultTuple.Item1))
                {
                    return ResponseBase.CommonError(deStudentClassTimesResultTuple.Item1);
                }
                var deStudentClassTimesResult = deStudentClassTimesResultTuple.Item2;
                var myCourse = await _courseDAL.GetCourse(deStudentClassTimesResult.DeCourseId);
                if (myCourse.Item1.CheckPoints > 0)
                {
                    await _studentDAL.AddPoint(studentCheckOnLog.StudentId, myCourse.Item1.CheckPoints);
                    await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        No = string.Empty,
                        Ot = studentCheckOnLog.CheckOt,
                        Points = myCourse.Item1.CheckPoints,
                        Remark = string.Empty,
                        StudentId = studentCheckOnLog.StudentId,
                        TenantId = studentCheckOnLog.TenantId,
                        Type = EmStudentPointsLogType.StudentCheckOn
                    });
                }
                if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe)
                {
                    await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
                    {
                        CourseId = deStudentClassTimesResult.DeCourseId,
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
                    _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(studentCheckOnLog.TenantId)
                    {
                        StudentId = studentCheckOnLog.StudentId,
                        CourseId = deStudentClassTimesResult.DeCourseId
                    });
                }

                studentCheckOnLog.Points = myCourse.Item1.CheckPoints;
                studentCheckOnLog.ClassTimesId = myClassTimes.Id;
                studentCheckOnLog.ClassId = myClassTimes.ClassId;
                studentCheckOnLog.CourseId = deStudentClassTimesResult.DeCourseId;
                studentCheckOnLog.ClassOtDesc = $"{myClassTimes.ClassOt.EtmsToDateString()}（{EtmsHelper.GetTimeDesc(myClassTimes.StartTime)}~{EtmsHelper.GetTimeDesc(myClassTimes.EndTime)}）";
                studentCheckOnLog.DeType = deStudentClassTimesResult.DeType;
                studentCheckOnLog.DeClassTimes = deStudentClassTimesResult.DeClassTimes;
                studentCheckOnLog.DeSum = deStudentClassTimesResult.DeSum;
                studentCheckOnLog.ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes;
                studentCheckOnLog.DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId;
                studentCheckOnLog.Remark = deStudentClassTimesResult.Remrak;
                studentCheckOnLog.Status = EmStudentCheckOnLogStatus.NormalAttendClass;
                await _studentCheckOnLogDAL.EditStudentCheckOnLog(studentCheckOnLog);
                await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassSetIsAttendClass(myClassTimes.Id, studentCheckOnLog.StudentId);
                //发通知
                _eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.LoginTenantId)
                {
                    CourseId = deStudentClassTimesResult.DeCourseId,
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
            if (p.Status == EmStudentCheckOnLogStatus.NormalNotClass || p.Status == EmStudentCheckOnLogStatus.Revoke)
            {
                return ResponseBase.CommonError("该考勤未记上课");
            }
            if (p.Status == EmStudentCheckOnLogStatus.BeRollcall)
            {
                return ResponseBase.CommonError("课次已点名，无法撤销");
            }
            if (p.DeClassTimes > 0 || p.ExceedClassTimes > 0)
            {
                if (p.DeClassTimes > 0)
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

                        await _commonHandlerBLL.AnalyzeStudentCourseDetailRestoreNormalStatus(p.DeStudentCourseDetailId.Value);
                    }
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
            var oldCourseId = p.CourseId.Value;

            if (p.Points > 0)
            {
                await _studentDAL.DeductionPoint(p.StudentId, p.Points);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = DateTime.Now,
                    Points = p.Points,
                    No = string.Empty,
                    Remark = string.Empty,
                    StudentId = p.StudentId,
                    TenantId = p.TenantId,
                    Type = EmStudentPointsLogType.StudentCheckOnRevoke
                });
                p.Points = 0;
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
                CourseId = oldCourseId,
                StudentId = p.StudentId
            });

            await _userOperationLogDAL.AddUserLog(request, "撤销考勤记上课", EmUserOperationType.StudentCheckOn);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentNeedCheckStatistics(StudentNeedCheckStatisticsRequest request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var isShowCheckOut = tenantConfig.StudentCheckInConfig.StudentUseCardCheckIn.IsMustCheckOutCard == EmBool.True
                || tenantConfig.StudentCheckInConfig.StudentUseFaceCheckIn.IsMustCheckOutFace == EmBool.True;
            var isShowAttendClass = tenantConfig.StudentCheckInConfig.StudentUseCardCheckIn.IsRelationClassTimesCard == EmBool.True
                || tenantConfig.StudentCheckInConfig.StudentUseFaceCheckIn.IsRelationClassTimesFace == EmBool.True;

            var output = new StudentNeedCheckStatisticsOutput()
            {
                IsShowCheckOut = isShowCheckOut,
                IsShowAttendClass = isShowAttendClass
            };
            var tempStudentNeedCheckCountBucket = await _tempStudentNeedCheckDAL.GetTempStudentNeedCheckCount(DateTime.Now);
            if (tempStudentNeedCheckCountBucket == null)
            {
                return ResponseBase.Success(output);
            }
            output.NeedAttendClassCount = tempStudentNeedCheckCountBucket.NeedAttendClassCount;
            output.NeedCheckInCount = tempStudentNeedCheckCountBucket.NeedCheckInCount;
            output.NeedCheckOutCount = tempStudentNeedCheckCountBucket.NeedCheckOutCount;
            output.NeedCheckCount = tempStudentNeedCheckCountBucket.NeedCheckCount;
            output.FinishCheckInCount = tempStudentNeedCheckCountBucket.FinishCheckInCount;
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentNeedCheckInGetPaging(StudentNeedCheckInGetPagingRequest request)
        {
            var pagingData = await _tempStudentNeedCheckDAL.TempStudentNeedCheckGetPaging(request);
            var output = new List<StudentNeedCheckInGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    LOG.Log.Error($"[StudentNeedCheckInGetPaging]学员不存在:{p.StudentId}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                output.Add(new StudentNeedCheckInGetPagingOutput()
                {
                    StudentId = p.StudentId,
                    IsBindingCard = !string.IsNullOrEmpty(student.CardNo),
                    IsBindingFaceKey = !string.IsNullOrEmpty(student.FaceKey),
                    FaceKeyUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.FaceKey),
                    CardNo = student.CardNo,
                    NeedCheckLogId = p.Id,
                    StartTimeDesc = EtmsHelper.GetTimeDesc(p.StartTime),
                    StudentName = student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(student.Phone, request.SecrecyType),
                    ClassDesc = p.ClassDesc
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentNeedCheckInGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentNeedCheckOutGetPaging(StudentNeedCheckOutGetPagingRequest request)
        {
            var pagingData = await _tempStudentNeedCheckDAL.TempStudentNeedCheckGetPaging(request);
            var output = new List<StudentNeedCheckOutGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    LOG.Log.Error($"[StudentNeedCheckOutGetPaging]学员不存在:{p.StudentId}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                output.Add(new StudentNeedCheckOutGetPagingOutput()
                {
                    StudentId = p.StudentId,
                    IsBindingCard = !string.IsNullOrEmpty(student.CardNo),
                    IsBindingFaceKey = !string.IsNullOrEmpty(student.FaceKey),
                    FaceKeyUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.FaceKey),
                    CardNo = student.CardNo,
                    NeedCheckLogId = p.Id,
                    StudentName = student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(student.Phone, request.SecrecyType),
                    ClassDesc = p.ClassDesc,
                    CheckInTimeDesc = p.CheckInOt.EtmsToOnlyMinuteString()
                }); ;
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentNeedCheckOutGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentNeedAttendClassGetPaging(StudentNeedAttendClassGetPagingRequest request)
        {
            var pagingData = await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassGetPaging(request);
            var output = new List<StudentNeedAttendClassGetPagingOutput>();
            var tempBoxClass = new DataTempBox<EtClass>();
            foreach (var p in pagingData.Item1)
            {
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                if (myClass == null)
                {
                    LOG.Log.Error($"[StudentNeedAttendClassGetPaging]班级不存在:{p.ClassId}", this.GetType());
                    continue;
                }
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    LOG.Log.Error($"[StudentNeedAttendClassGetPaging]学员不存在:{p.StudentId}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                output.Add(new StudentNeedAttendClassGetPagingOutput()
                {
                    StudentId = p.StudentId,
                    IsBindingCard = !string.IsNullOrEmpty(student.CardNo),
                    IsBindingFaceKey = !string.IsNullOrEmpty(student.FaceKey),
                    FaceKeyUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.FaceKey),
                    CardNo = student.CardNo,
                    StudentName = student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(student.Phone, request.SecrecyType),
                    ClassName = myClass.Name,
                    ClassOt = p.ClassOt.EtmsToDateString(),
                    TimeDesc = $"{EtmsHelper.GetTimeDesc(p.StartTime)}~{EtmsHelper.GetTimeDesc(p.EndTime)}",
                    Week = p.Week,
                    WeekDesc = $"周{EtmsHelper.GetWeekDesc(p.Week)}",
                    NeedCheckClassLogId = p.Id
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentNeedAttendClassGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentNeedLogCheckIn(StudentNeedLogCheckInRequest request)
        {
            var log = await _tempStudentNeedCheckDAL.TempStudentNeedCheckGet(request.NeedCheckLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("待考勤记录不存在");
            }
            if (log.IsCheckIn == EmBool.True)
            {
                return ResponseBase.CommonError("已签到");
            }
            var checkTimeDesc = request.CheckTime.Split(':');
            var now = DateTime.Now;
            var checkTime = new DateTime(now.Year, now.Month, now.Day, checkTimeDesc[0].ToInt(), checkTimeDesc[1].ToInt(), 0);
            await _tempStudentNeedCheckDAL.TempStudentNeedCheckSetIsCheckInById(request.NeedCheckLogId, checkTime);
            await _studentCheckOnLogDAL.AddStudentCheckOnLog(new EtStudentCheckOnLog()
            {
                StudentId = log.StudentId,
                CheckForm = EmStudentCheckOnLogCheckForm.TeacherManual,
                CheckOt = checkTime,
                CheckType = EmStudentCheckOnLogCheckType.CheckIn,
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                Status = EmStudentCheckOnLogStatus.NormalNotClass,
                TenantId = request.LoginTenantId,
                CheckMedium = string.Empty,
                DeType = EmDeClassTimesType.NotDe
            });

            await _userOperationLogDAL.AddUserLog(request, "签到补卡", EmUserOperationType.StudentCheckOn);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentNeedLogCheckOut(StudentNeedLogCheckOutRequest request)
        {
            var log = await _tempStudentNeedCheckDAL.TempStudentNeedCheckGet(request.NeedCheckLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("待考勤记录不存在");
            }
            if (log.IsCheckOut == EmBool.True)
            {
                return ResponseBase.CommonError("已签退");
            }
            var checkTimeDesc = request.CheckTime.Split(':');
            var now = DateTime.Now;
            var checkTime = new DateTime(now.Year, now.Month, now.Day, checkTimeDesc[0].ToInt(), checkTimeDesc[1].ToInt(), 0);
            await _tempStudentNeedCheckDAL.TempStudentNeedCheckSetIsCheckOutById(request.NeedCheckLogId, checkTime);
            await _studentCheckOnLogDAL.AddStudentCheckOnLog(new EtStudentCheckOnLog()
            {
                StudentId = log.StudentId,
                CheckForm = EmStudentCheckOnLogCheckForm.TeacherManual,
                CheckOt = checkTime,
                CheckType = EmStudentCheckOnLogCheckType.CheckOut,
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                Status = EmStudentCheckOnLogStatus.NormalNotClass,
                TenantId = request.LoginTenantId,
                CheckMedium = string.Empty,
                DeType = EmDeClassTimesType.NotDe
            });

            await _userOperationLogDAL.AddUserLog(request, "签退补卡", EmUserOperationType.StudentCheckOn);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentNeedLogAttendClass(StudentNeedLogAttendClassRequest request)
        {
            var log = await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassGet(request.NeedCheckClassLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("待记上课记录不存在");
            }
            if (log.Status == EmTempStudentNeedCheckClassStatus.IsAttendClass)
            {
                return ResponseBase.CommonError("已记上课");
            }
            var myClassTimes = await _classTimesDAL.GetClassTimes(log.ClassTimesId);
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
            var checkTimeDesc = request.CheckTime.Split(':');
            var now = DateTime.Now;
            var checkOt = new DateTime(now.Year, now.Month, now.Day, checkTimeDesc[0].ToInt(), checkTimeDesc[1].ToInt(), 0);

            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var myStudentDeLog = await _studentCheckOnLogDAL.GetStudentDeLog(myClassTimes.Id, log.StudentId);
            if (myStudentDeLog != null) //已存在扣课时记录,防止重复扣
            {
                return ResponseBase.CommonError("已存在扣课次记录");
            }
            else
            {
                var deStudentClassTimesResultTuple = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, _classTimesDAL, _classDAL,
                    tenantConfig.ClassCheckSignConfig.MakeupIsDeClassTimes, myClassTimes, log.StudentId, checkOt);
                if (!string.IsNullOrEmpty(deStudentClassTimesResultTuple.Item1))
                {
                    return ResponseBase.CommonError(deStudentClassTimesResultTuple.Item1);
                }
                var deStudentClassTimesResult = deStudentClassTimesResultTuple.Item2;
                if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe)
                {
                    await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
                    {
                        CourseId = deStudentClassTimesResult.DeCourseId,
                        DeClassTimes = deStudentClassTimesResult.DeClassTimes,
                        DeType = deStudentClassTimesResult.DeType,
                        IsDeleted = EmIsDeleted.Normal,
                        OrderId = deStudentClassTimesResult.OrderId,
                        OrderNo = deStudentClassTimesResult.OrderNo,
                        Ot = checkOt,
                        SourceType = EmStudentCourseConsumeSourceType.StudentCheckIn,
                        StudentId = log.StudentId,
                        TenantId = log.TenantId,
                        DeClassTimesSmall = 0
                    });
                    _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(log.TenantId)
                    {
                        StudentId = log.StudentId,
                        CourseId = deStudentClassTimesResult.DeCourseId
                    });
                }

                await _studentCheckOnLogDAL.AddStudentCheckOnLog(new EtStudentCheckOnLog()
                {
                    StudentId = log.StudentId,
                    CheckForm = EmStudentCheckOnLogCheckForm.TeacherManual,
                    CheckOt = checkOt,
                    CheckType = EmStudentCheckOnLogCheckType.CheckIn,
                    IsDeleted = EmIsDeleted.Normal,
                    Remark = string.Empty,
                    Status = EmStudentCheckOnLogStatus.NormalAttendClass,
                    TenantId = log.TenantId,
                    CheckMedium = string.Empty,
                    DeType = deStudentClassTimesResult.DeType,
                    ClassOtDesc = $"{myClassTimes.ClassOt.EtmsToDateString()}（{EtmsHelper.GetTimeDesc(myClassTimes.StartTime)}~{EtmsHelper.GetTimeDesc(myClassTimes.EndTime)}）",
                    ClassId = myClassTimes.ClassId,
                    ClassTimesId = myClassTimes.Id,
                    CourseId = deStudentClassTimesResult.DeCourseId,
                    DeClassTimes = deStudentClassTimesResult.DeClassTimes,
                    DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId,
                    ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes,
                    DeSum = deStudentClassTimesResult.DeSum
                });

                await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassSetIsAttendClassById(request.NeedCheckClassLogId);
                await _tempStudentNeedCheckDAL.TempStudentNeedCheckSetIsCheckIn(log.StudentId, now);
                //发通知
                _eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.LoginTenantId)
                {
                    CourseId = deStudentClassTimesResult.DeCourseId,
                    StudentId = log.StudentId
                });

                await _userOperationLogDAL.AddUserLog(request, "签到补卡记上课", EmUserOperationType.StudentCheckOn);
                return ResponseBase.Success();
            }
        }

        public async Task<ResponseBase> StudentCheckOnLogDel(StudentCheckOnLogDelRequest request)
        {
            var p = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (p == null)
            {
                return ResponseBase.CommonError("考勤记录不存在");
            }
            if (p.Status == EmStudentCheckOnLogStatus.NormalAttendClass || p.Status == EmStudentCheckOnLogStatus.BeRollcall)
            {
                return ResponseBase.CommonError("已记上课，无法删除");
            }

            p.IsDeleted = EmIsDeleted.Deleted;
            await _studentCheckOnLogDAL.EditStudentCheckOnLog(p);

            await _userOperationLogDAL.AddUserLog(request, "删除考勤记录", EmUserOperationType.StudentCheckOn);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseAndClassGet(StudentCourseAndClassGetRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var ouput = new StudentCourseAndClassGetOutput()
            {
                StudentCourses = new List<MyStudentCourseInfo>(),
                StudentId = request.StudentId,
                StudentName = studentBucket.Student.Name,
                StudentPhone = ComBusiness3.PhoneSecrecy(studentBucket.Student.Phone, request.SecrecyType)
            };
            var myCourse = await _studentCourseDAL.GetStudentCourse(request.StudentId);
            if (myCourse == null || myCourse.Count == 0)
            {
                return ResponseBase.Success(ouput);
            }
            var usableCourse = myCourse.Where(p => p.Status != EmStudentCourseStatus.EndOfClass);
            if (!usableCourse.Any())
            {
                return ResponseBase.Success(ouput);
            }

            var allCourseId = usableCourse.Select(p => p.CourseId).Distinct();
            var studentInClassInfo = await _classDAL.GetStudentClass2(request.StudentId);
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var courseId in allCourseId)
            {
                var myItemCourse = await _courseDAL.GetCourse(courseId);
                if (myItemCourse == null || myItemCourse.Item1 == null)
                {
                    LOG.Log.Error($"[StudentCourseAndClassGet]课程不存在,TenantId:{request.LoginTenantId},courseId:{courseId}", this.GetType());
                    continue;
                }
                var mySurplusCourse = myCourse.Where(p => p.CourseId == courseId).ToList();
                var myStudentCourseInfoItem = new MyStudentCourseInfo()
                {
                    CourseId = courseId,
                    CourseName = myItemCourse.Item1.Name,
                    Type = myItemCourse.Item1.Type,
                    SurplusQuantityDesc = ComBusiness.GetStudentCourseDesc(mySurplusCourse),
                    StudentCourseInClass = new List<MyStudentCourseInClass>()
                };
                if (studentInClassInfo != null && studentInClassInfo.Any())
                {
                    var myInClassStudet = studentInClassInfo.Where(p => p.CourseId == courseId);
                    if (myInClassStudet.Any())
                    {
                        var myInClassIds = myInClassStudet.Select(p => p.ClassId).Distinct();
                        foreach (var classId in myInClassIds)
                        {
                            var classBucket = await _classDAL.GetClassBucket(classId);
                            if (classBucket == null || classBucket.EtClass == null)
                            {
                                LOG.Log.Error($"[StudentCourseAndClassGet]班级不存在,TenantId:{request.LoginTenantId},classId:{classId}", this.GetType());
                                continue;
                            }
                            if (classBucket.EtClass.DataType == EmClassDataType.Temp)
                            {
                                continue;
                            }
                            myStudentCourseInfoItem.StudentCourseInClass.Add(new MyStudentCourseInClass()
                            {
                                ClassId = classId,
                                ClassName = classBucket.EtClass.Name,
                                TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, classBucket.EtClass.Teachers)
                            });
                        }
                    }
                }
                ouput.StudentCourses.Add(myStudentCourseInfoItem);
            }
            return ResponseBase.Success(ouput);
        }

        public async Task<ResponseBase> StudentCourseRelationClass(StudentCourseRelationClassRequest request)
        {
            var output = new List<StudentCourseRelationClassOutput>();
            var courseAllClassOneToMore = await _classDAL.GetClassOfCourseIdOneToMore(request.CourseId, request.ClassName);
            if (courseAllClassOneToMore == null || !courseAllClassOneToMore.Any())
            {
                return ResponseBase.Success(output);
            }
            var studentInClassIds = await _classDAL.GetStudentCourseInClass(request.StudentId, request.CourseId);
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            foreach (var myClass in courseAllClassOneToMore)
            {
                output.Add(new StudentCourseRelationClassOutput()
                {
                    ClassId = myClass.Id,
                    LimitStudentNums = myClass.LimitStudentNums,
                    StudentNums = myClass.StudentNums,
                    Name = myClass.Name,
                    TeachersDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, myClass.Teachers),
                    CourseListDesc = await ComBusiness.GetCourseNames(tempBoxCourse, _courseDAL, myClass.CourseList),
                    IsDefaultInClass = studentInClassIds.FirstOrDefault(p => p.ClassId == myClass.Id) != null
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> CheckOnLogGet(CheckOnLogGet2Request request)
        {
            var log = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (log == null)
            {
                return ResponseBase.CommonError("考勤记录不存在");
            }
            return ResponseBase.Success(new CheckOnLog2Get()
            {
                CheckOt = log.CheckOt,
                CheckMediumUrl = AliyunOssUtil.GetAccessUrlHttps(log.CheckMedium),
                CheckType = log.CheckType,
                CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(log.CheckType)
            });
        }

        /// <summary>
        /// 存在并发的bug
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase> StudentCheckChoiceStudentCourse(StudentCheckChoiceStudentCourseRequest request)
        {
            var lockKey = new StudentCheckChoiceStudentCourseToken(request.LoginTenantId, request.StudentCheckOnLogId);
            if (_distributedLockDAL.LockTake(lockKey))
            {
                try
                {
                    return await StudentCheckChoiceStudentCourseProcess(request);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"【StudentCheckChoiceStudentCourse】错误:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                    throw;
                }
                finally
                {
                    _distributedLockDAL.LockRelease(lockKey);
                }
            }
            return ResponseBase.CommonError("请勿重复操作");
        }

        private async Task<ResponseBase> StudentCheckChoiceStudentCourseProcess(StudentCheckChoiceStudentCourseRequest request)
        {
            var studentCheckOnLog = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (studentCheckOnLog == null)
            {
                return ResponseBase.CommonError("考勤记录不存在");
            }
            if (studentCheckOnLog.Status != EmStudentCheckOnLogStatus.NormalNotClass)
            {
                return ResponseBase.CommonError("考勤记录已记上课");
            }
            var myDeCourse = await _courseDAL.GetCourse(request.CourseId);
            if (myDeCourse == null || myDeCourse.Item1 == null)
            {
                return ResponseBase.CommonError("学员报读课程不存在");
            }
            var deStudentClassTimesResult = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, new DeStudentClassTimesTempRequest()
            {
                ClassOt = studentCheckOnLog.CheckOt,
                TenantId = request.LoginTenantId,
                StudentId = studentCheckOnLog.StudentId,
                DeClassTimes = myDeCourse.Item1.StudentCheckDeClassTimes,
                CourseId = request.CourseId
            });
            if (myDeCourse.Item1.CheckPoints > 0)
            {
                await _studentDAL.AddPoint(studentCheckOnLog.StudentId, myDeCourse.Item1.CheckPoints);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = string.Empty,
                    Ot = studentCheckOnLog.CheckOt,
                    Points = myDeCourse.Item1.CheckPoints,
                    Remark = string.Empty,
                    StudentId = studentCheckOnLog.StudentId,
                    TenantId = studentCheckOnLog.TenantId,
                    Type = EmStudentPointsLogType.StudentCheckOn
                });
            }
            await _tempStudentNeedCheckDAL.TempStudentNeedCheckClassSetIsAttendClassByStudentId(studentCheckOnLog.StudentId, studentCheckOnLog.CheckOt);
            if (deStudentClassTimesResult.DeType != EmDeClassTimesType.NotDe)
            {
                await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
                {
                    CourseId = deStudentClassTimesResult.DeCourseId,
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
                _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(studentCheckOnLog.TenantId)
                {
                    StudentId = studentCheckOnLog.StudentId,
                    CourseId = deStudentClassTimesResult.DeCourseId,
                    IsSendNoticeStudent = true
                });
            }

            studentCheckOnLog.Points = myDeCourse.Item1.CheckPoints;
            studentCheckOnLog.ClassTimesId = null;
            studentCheckOnLog.ClassId = null;
            studentCheckOnLog.CourseId = deStudentClassTimesResult.DeCourseId;
            studentCheckOnLog.ClassOtDesc = "";
            studentCheckOnLog.DeType = deStudentClassTimesResult.DeType;
            studentCheckOnLog.DeClassTimes = deStudentClassTimesResult.DeClassTimes;
            studentCheckOnLog.DeSum = deStudentClassTimesResult.DeSum;
            studentCheckOnLog.ExceedClassTimes = deStudentClassTimesResult.ExceedClassTimes;
            studentCheckOnLog.DeStudentCourseDetailId = deStudentClassTimesResult.DeStudentCourseDetailId;
            studentCheckOnLog.Remark = deStudentClassTimesResult.Remrak;
            studentCheckOnLog.Status = EmStudentCheckOnLogStatus.NormalAttendClass;
            await _studentCheckOnLogDAL.EditStudentCheckOnLog(studentCheckOnLog);

            return ResponseBase.Success();
        }
    }
}
