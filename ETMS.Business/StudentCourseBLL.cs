using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
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
    public class StudentCourseBLL : IStudentCourseBLL
    {
        private readonly ICourseDAL _courseDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassDAL _classDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IStudentCourseStopLogDAL _studentCourseStopLogDAL;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IOrderDAL _orderDAL;

        private readonly IStudentCourseOpLogDAL _studentCourseOpLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentSourceDAL _studentSourceDAL;

        private readonly IGradeDAL _gradeDAL;

        public StudentCourseBLL(ICourseDAL courseDAL, IStudentCourseDAL studentCourseDAL, IClassDAL classDAL, IStudentDAL studentDAL,
            IUserOperationLogDAL userOperationLogDAL, IEventPublisher eventPublisher, IClassRecordDAL classRecordDAL,
            IStudentCourseStopLogDAL studentCourseStopLogDAL, IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL,
            ITenantConfigDAL tenantConfigDAL, IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IOrderDAL orderDAL, IStudentCourseOpLogDAL studentCourseOpLogDAL, IUserDAL userDAL,
            IStudentSourceDAL studentSourceDAL, IGradeDAL gradeDAL)
        {
            this._courseDAL = courseDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._classDAL = classDAL;
            this._studentDAL = studentDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._eventPublisher = eventPublisher;
            this._classRecordDAL = classRecordDAL;
            this._studentCourseStopLogDAL = studentCourseStopLogDAL;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._orderDAL = orderDAL;
            this._studentCourseOpLogDAL = studentCourseOpLogDAL;
            this._userDAL = userDAL;
            this._studentSourceDAL = studentSourceDAL;
            this._gradeDAL = gradeDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _courseDAL, _studentCourseDAL, _classDAL, _studentDAL, _userOperationLogDAL, _classRecordDAL,
                _studentCourseStopLogDAL, _studentCourseConsumeLogDAL, _tenantConfigDAL, _orderDAL, _studentCourseOpLogDAL,
                _userDAL, _gradeDAL, _studentSourceDAL);
        }

        public async Task<ResponseBase> StudentCourseGetPaging(StudentCourseGetPagingRequest request)
        {
            if (request.IsQueryShort != null && request.IsQueryShort.Value)
            {
                var config = await _tenantConfigDAL.GetTenantConfig();
                request.LimitClassTimes = config.StudentCourseRenewalConfig.LimitClassTimes;
                request.LimitDay = config.StudentCourseRenewalConfig.LimitDay;
            }
            var pagingData = await _studentCourseDAL.GetStudentCoursePaging(request);
            var studentCourses = new List<StudentCourseGetPagingOutput>();
            var tempBoxCourse = new DataTempBox<EtCourse>();

            List<EtStudentSource> sources = null;
            List<EtGrade> grades = null;
            DataTempBox<EtUser> tempBoxUser = null;
            if (request.IsLoadRich)
            {
                sources = await _studentSourceDAL.GetAllStudentSource();
                grades = await _gradeDAL.GetAllGrade();
                tempBoxUser = new DataTempBox<EtUser>();
            }
            foreach (var p in pagingData.Item1)
            {
                var myCourse = new StudentCourseGetPagingOutput()
                {
                    CId = p.Id,
                    CourseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId),
                    CourseId = p.CourseId,
                    DeTypeDesc = p.DeType == EmDeClassTimesType.ClassTimes ? "按课时" : "按天",
                    ExceedTotalClassTimes = p.ExceedTotalClassTimes,
                    Status = p.Status,
                    StatusDesc = EmStudentCourseStatus.GetStudentCourseStatusDesc(p.Status),
                    StudentName = p.StudentName,
                    StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType),
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(p.BuyQuantity, p.BuySmallQuantity, p.BugUnit, EmProductType.Course),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(p.GiveQuantity, p.GiveSmallQuantity, p.DeType),
                    SurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(p.SurplusQuantity, p.SurplusSmallQuantity, p.DeType),
                    UseQuantityDesc = ComBusiness.GetUseQuantityDesc(p.UseQuantity, p.UseUnit),
                    NotEnoughRemindCount = p.NotEnoughRemindCount,
                    StudentId = p.StudentId,
                    Value = p.StudentId,
                    Label = p.StudentName
                };
                if (request.IsLoadRich)
                {
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    myCourse.Age = student.Age;
                    myCourse.TrackUser = student.TrackUser;
                    myCourse.GradeId = student.GradeId;
                    myCourse.SourceId = student.SourceId;
                    myCourse.SchoolName = student.SchoolName;
                    myCourse.GradeIdDesc = ComBusiness3.GetName(grades, student.GradeId);
                    myCourse.BirthdayDesc = student.Birthday.EtmsToDateString();
                    myCourse.HomeAddress = student.HomeAddress;
                    myCourse.LearningManager = student.LearningManager;
                    myCourse.LearningManagerDesc = ComBusiness.GetUserName(tempBoxUser, _userDAL, student.LearningManager).Result;
                    myCourse.Points = student.Points;
                    myCourse.SourceIdDesc = ComBusiness3.GetName(sources, student.SourceId);
                    myCourse.TrackUserDesc = ComBusiness.GetUserName(tempBoxUser, _userDAL, student.TrackUser).Result;
                }
                studentCourses.Add(myCourse);
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCourseGetPagingOutput>(pagingData.Item2, studentCourses));
        }

        public async Task<ResponseBase> StudentCourseOwnerGetPaging(StudentCourseOwnerGetPagingRequest request)
        {
            var pagingData = await _studentCourseDAL.GetStudentCoursePaging(request);
            var output = new List<StudentCourseOwnerGetPagingOutput>();
            var studentIds = new List<long>();
            foreach (var p in pagingData.Item1)
            {
                if (studentIds.Exists(j => j == p.StudentId))
                {
                    continue;
                }
                studentIds.Add(p.StudentId);
                output.Add(new StudentCourseOwnerGetPagingOutput()
                {
                    CId = p.StudentId,
                    Name = p.StudentName,
                    Phone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.SecrecyType),
                    Value = p.StudentId,
                    Label = p.StudentName,
                    AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p.Avatar)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCourseOwnerGetPagingOutput>(pagingData.Item2, output));
        }

        private async Task<Tuple<List<StudentCourseDetailGetOutput>, bool>> GetStudentCourseDetail(EtStudent myStudent, int secrecyType)
        {
            var phone = ComBusiness3.PhoneSecrecy(myStudent.Phone, secrecyType);
            var studentCourse = await _studentCourseDAL.GetStudentCourse(myStudent.Id);
            var studentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(myStudent.Id);
            var studentClass = await _classDAL.GetStudentClass(myStudent.Id);
            var opLogs = await _studentCourseOpLogDAL.GetStudentCourseOpLogs(myStudent.Id);
            var outputNormal = new List<StudentCourseDetailGetOutput>();
            var outputOver = new List<StudentCourseDetailGetOutput>();
            if (studentCourse != null && studentCourse.Any())
            {
                var courseIds = studentCourse.OrderByDescending(j => j.Id).Select(p => p.CourseId).Distinct();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                var tempBoxUser = new DataTempBox<EtUser>();
                foreach (var courseId in courseIds)
                {
                    var course = await ComBusiness.GetCourse(tempBoxCourse, _courseDAL, courseId);
                    if (course == null)
                    {
                        continue;
                    }
                    var myStudentCourseDetail = new StudentCourseDetailGetOutput()
                    {
                        CourseName = course.Name,
                        CourseColor = course.StyleColor,
                        CourseId = courseId,
                        Type = course.Type,
                        StudentName = myStudent.Name,
                        StudentPhone = phone
                    };
                    var myCourse = studentCourse.Where(p => p.CourseId == courseId).ToList();
                    myStudentCourseDetail.SurplusQuantityDesc = ComBusiness.GetStudentCourseDesc(myCourse);

                    var myFirstCourseLog = myCourse[0];
                    myStudentCourseDetail.Status = myFirstCourseLog.Status;
                    myStudentCourseDetail.StopTimeDesc = myFirstCourseLog.StopTime.EtmsToDateString();
                    myStudentCourseDetail.RestoreTimeDesc = myFirstCourseLog.RestoreTime.EtmsToDateString();
                    var myClassTimesCourseLog = myCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
                    if (myClassTimesCourseLog != null)
                    {
                        myStudentCourseDetail.ExceedTotalClassTimes = myClassTimesCourseLog.ExceedTotalClassTimes.EtmsToString();
                    }

                    //foreach (var theCourse in myCourse)
                    //{
                    //    myStudentCourseDetail.Status = theCourse.Status;
                    //    myStudentCourseDetail.StopTimeDesc = theCourse.StopTime.EtmsToDateString();
                    //    myStudentCourseDetail.RestoreTimeDesc = theCourse.RestoreTime.EtmsToDateString();
                    //    if (theCourse.DeType == EmDeClassTimesType.ClassTimes)
                    //    {
                    //        myStudentCourseDetail.ExceedTotalClassTimes = theCourse.ExceedTotalClassTimes.EtmsToString();
                    //    }
                    //}
                    var myClass = studentClass.Where(p => p.CourseList.IndexOf($",{courseId},") != -1);
                    if (myClass.Any())
                    {
                        myStudentCourseDetail.StudentClass = new List<StudentClass>();
                        foreach (var c in myClass)
                        {
                            myStudentCourseDetail.StudentClass.Add(new StudentClass()
                            {
                                Id = c.Id,
                                Name = c.Name
                            });
                        }
                    }
                    var myDetail = studentCourseDetail.Where(p => p.CourseId == courseId);
                    myStudentCourseDetail.StudentCourseDetail = new List<StudentCourseDetail>();
                    foreach (var p in myDetail)
                    {
                        int giveQuantity = 0, giveSmallQuantity = 0;
                        if (p.GiveUnit == EmCourseUnit.Day)
                        {
                            giveSmallQuantity = p.GiveQuantity;
                        }
                        else
                        {
                            giveQuantity = p.GiveQuantity;
                        }
                        myStudentCourseDetail.StudentCourseDetail.Add(new StudentCourseDetail()
                        {
                            DeType = p.DeType,
                            OrderId = p.OrderId,
                            OrderNo = p.OrderNo,
                            Status = p.Status,
                            ExpirationDate = ComBusiness.GetExpirationDate(p),
                            BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(p.BuyQuantity, 0, p.BugUnit, EmProductType.Course),
                            GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(giveQuantity, giveSmallQuantity, p.DeType),
                            CId = p.Id,
                            SurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(p.SurplusQuantity, p.SurplusSmallQuantity, p.DeType),
                            UseQuantityDesc = ComBusiness.GetUseQuantityDesc(p.UseQuantity, p.DeType),
                            StatusDesc = EmStudentCourseStatus.GetStudentCourseStatusDesc(p.Status),
                            EndCourseRemark = p.EndCourseRemark,
                            SurplusQuantity = p.SurplusQuantity,
                            SurplusSmallQuantity = p.SurplusSmallQuantity,
                            EndTime = p.EndTime.EtmsToDateString()
                        });
                    }
                    if (opLogs != null && opLogs.Any())
                    {
                        var myOpLog = opLogs.Where(p => p.CourseId == courseId).OrderByDescending(p => p.Id);
                        if (myOpLog.Any())
                        {
                            myStudentCourseDetail.OpLogs = new List<OpLog>();
                            foreach (var log in myOpLog)
                            {
                                myStudentCourseDetail.OpLogs.Add(new OpLog()
                                {
                                    CourseId = courseId,
                                    CourseName = course.Name,
                                    OpContent = log.OpContent,
                                    OpTime = log.OpTime,
                                    OpType = log.OpType,
                                    OpTypeDesc = EmStudentCourseOpLogType.GetStudentCourseOpLogTypeDesc(log.OpType),
                                    OpUser = log.OpUser,
                                    OpUserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, log.OpUser),
                                    Remark = log.Remark,
                                    StudentId = log.StudentId
                                });
                            }
                        }
                    }
                    if (myStudentCourseDetail.Status == EmStudentCourseStatus.EndOfClass)
                    {
                        outputOver.Add(myStudentCourseDetail);
                    }
                    else
                    {
                        myStudentCourseDetail.StudentCheckDefault = myFirstCourseLog.StudentCheckDefault;
                        outputNormal.Add(myStudentCourseDetail);
                    }
                }
            }
            var isShowSetStudentCheckDefault = outputNormal.Count > 1;
            if (outputOver.Count > 0)
            {
                outputNormal.AddRange(outputOver);
            }
            return Tuple.Create(outputNormal, isShowSetStudentCheckDefault);
        }

        public async Task<ResponseBase> StudentCourseDetailGet(StudentCourseDetailGetRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.SId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var myStudent = studentBucket.Student;
            var result = await GetStudentCourseDetail(myStudent, request.SecrecyType);
            return ResponseBase.Success(result.Item1);
        }

        /// <summary>
        /// 与StudentCourseDetailGet相比 返回值多了IsShowSetStudentCheckDefault
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase> StudentCourseDetailGet2(StudentCourseDetailGetRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.SId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var myStudent = studentBucket.Student;
            var result = await GetStudentCourseDetail(myStudent, request.SecrecyType);
            var isShowSetStudentCheckDefault = result.Item2;
            if (isShowSetStudentCheckDefault)
            {
                var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
                var configResult = ComBusiness3.GetTenantConfigGetSimple(tenantConfig);
                isShowSetStudentCheckDefault = configResult.IsEnableStudentCheckDeClassTimes
                    && tenantConfig.StudentCheckInConfig.RelationClassTimesGoDeStudentCourseMulCourseType == EmRelationClassTimesGoDeStudentCourseMulCourseType.NeedSetStudentCoueseCheckDefault;
            }
            var output = new StudentCourseDetailGet2Output()
            {
                CourseItems = result.Item1,
                IsShowSetStudentCheckDefault = isShowSetStudentCheckDefault
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentCourseStop(StudentCourseStopRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            var stopTime = DateTime.Now.Date;
            await _studentCourseDAL.StudentCourseStop(request.StudentId, request.CourseId, stopTime, request.RestoreTime);
            //await _studentCourseStopLogDAL.AddStudentCourseStopLog(new EtStudentCourseStopLog()
            //{
            //    CourseId = request.CourseId,
            //    IsDeleted = EmIsDeleted.Normal,
            //    Ot = stopTime,
            //    Remark = request.Remark,
            //    RestoreTime = null,
            //    StopDay = 0,
            //    StopTime = stopTime,
            //    StudentId = request.StudentId,
            //    TenantId = request.LoginTenantId,
            //    UserId = request.LoginUserId

            //});
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId
            });

            var str = new StringBuilder();
            str.Append($"课程停课，停课日期({stopTime.EtmsToDateString()})");
            if (request.RestoreTime != null)
            {
                str.Append($"，复课日期({stopTime.EtmsToDateString()})");
            }
            await _studentCourseOpLogDAL.AddStudentCourseOpLog(new EtStudentCourseOpLog()
            {
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                OpTime = DateTime.Now,
                OpType = EmStudentCourseOpLogType.CourseStop,
                OpUser = request.LoginUserId,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                OpContent = str.ToString(),
                Remark = request.Remark
            });

            await _userOperationLogDAL.AddUserLog(request, $"学员课程停课-学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},停课课程:{request.CourseName},{request.Remark}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseRestoreTime(StudentCourseRestoreTimeRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }

            await ComBusiness3.RestoreStudentCourse(_studentCourseDAL, request.LoginTenantId, request.StudentId, request.CourseId,
                DateTime.Now.Date);

            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                IsSendNoticeStudent = true
            });

            await _studentCourseOpLogDAL.AddStudentCourseOpLog(new EtStudentCourseOpLog()
            {
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                OpTime = DateTime.Now,
                OpType = EmStudentCourseOpLogType.CourseRestore,
                OpUser = request.LoginUserId,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                OpContent = "课程复课",
                Remark = string.Empty
            });

            await _userOperationLogDAL.AddUserLog(request, $"学员课程复课-学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},复课课程:{request.CourseName}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseMarkExceedClassTimes(StudentCourseMarkExceedClassTimesRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            var deTimesDesc = string.Empty;
            if (request.IsDeMyCourse)
            {
                var myCourse = await _studentCourseDAL.GetStudentCourseDb(request.StudentId, request.CourseId);
                var myDeClassTimeCourse = myCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
                if (myDeClassTimeCourse == null || myDeClassTimeCourse.ExceedTotalClassTimes <= 0)
                {
                    return ResponseBase.CommonError("未查询到学员在此课程有超上课时");
                }
                if (myDeClassTimeCourse.Status == EmStudentCourseStatus.EndOfClass)
                {
                    return ResponseBase.CommonError("课程已结课，无法扣除相应的课时");
                }
                if (myDeClassTimeCourse.Status == EmStudentCourseStatus.StopOfClass)
                {
                    return ResponseBase.CommonError("课程已停课，无法扣除相应的课时");
                }
                request.ExceedTotalClassTimes = myDeClassTimeCourse.ExceedTotalClassTimes;
                if (myDeClassTimeCourse.SurplusQuantity < request.ExceedTotalClassTimes)
                {
                    return ResponseBase.CommonError("学员剩余课时不足，无法扣除相应的课时");
                }
                //扣减课时
                var deOt = DateTime.Now.Date;
                var deResult = await CoreBusiness.DeStudentClassTimes(_studentCourseDAL, new DeStudentClassTimesTempRequest()
                {
                    ClassOt = deOt,
                    CourseId = myDeClassTimeCourse.CourseId,
                    StudentId = myDeClassTimeCourse.StudentId,
                    TenantId = myDeClassTimeCourse.TenantId,
                    DeClassTimes = request.ExceedTotalClassTimes
                });
                if (deResult.DeType != EmDeClassTimesType.NotDe)
                {
                    await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
                    {
                        CourseId = myDeClassTimeCourse.CourseId,
                        DeClassTimes = deResult.DeClassTimes,
                        DeType = deResult.DeType,
                        IsDeleted = EmIsDeleted.Normal,
                        OrderId = deResult.OrderId,
                        OrderNo = deResult.OrderNo,
                        Ot = deOt,
                        SourceType = EmStudentCourseConsumeSourceType.MarkExceedClassTimes,
                        StudentId = myDeClassTimeCourse.StudentId,
                        TenantId = myDeClassTimeCourse.TenantId,
                        DeClassTimesSmall = 0
                    });
                    deTimesDesc = $"扣减{deResult.DeClassTimes.EtmsToString()}课时";
                }
            }

            await _studentCourseDAL.StudentCourseMarkExceedClassTimes(request.StudentId, request.CourseId);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                IsSendNoticeStudent = true
            });

            await _studentCourseOpLogDAL.AddStudentCourseOpLog(new EtStudentCourseOpLog()
            {
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                OpTime = DateTime.Now,
                OpType = EmStudentCourseOpLogType.MarkExceedClassTimes,
                OpUser = request.LoginUserId,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                OpContent = $"超上{request.ExceedTotalClassTimes.EtmsToString()}课时，标记已处理，{deTimesDesc}",
                Remark = string.Empty
            });

            await _userOperationLogDAL.AddUserLog(request, $"超上课时标记处理-学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},超上课程:{request.CourseName}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseSetExpirationDate(StudentCourseSetExpirationDateRequest request)
        {
            var studentCourseDetail = await _studentCourseDAL.GetEtStudentCourseDetailById(request.CId);
            if (studentCourseDetail == null)
            {
                return ResponseBase.CommonError("不存在此订单");
            }
            if (studentCourseDetail.Status == EmStudentCourseStatus.EndOfClass)
            {
                return ResponseBase.CommonError("已结课，无法设置有效期");
            }
            var studentBucket = await _studentDAL.GetStudent(studentCourseDetail.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            var desc = string.Empty;
            if (studentCourseDetail.DeType == EmDeClassTimesType.ClassTimes)
            {
                studentCourseDetail.StartTime = null;
                studentCourseDetail.EndTime = request.EndTime;
                desc = $"设置课程有效期:{request.EndTime.EtmsToDateString()}";
            }
            else
            {
                if (request.Ot == null || request.Ot.Count != 2)
                {
                    return ResponseBase.CommonError("课程起止日期格式不正确");
                }
                studentCourseDetail.StartTime = Convert.ToDateTime(request.Ot[0]);
                studentCourseDetail.EndTime = Convert.ToDateTime(request.Ot[1]);
                var startTime = studentCourseDetail.StartTime.Value;
                if (studentCourseDetail.StartTime.Value < DateTime.Now.Date)
                {
                    startTime = DateTime.Now.Date;
                }
                var beforeSurplusQuantity = studentCourseDetail.SurplusQuantity;
                var beforeSurplusSmallQuantity = studentCourseDetail.SurplusSmallQuantity;
                var dffTime = EtmsHelper.GetDffTime(startTime, studentCourseDetail.EndTime.Value.AddDays(1));
                studentCourseDetail.SurplusQuantity = dffTime.Item1;
                studentCourseDetail.SurplusSmallQuantity = dffTime.Item2;

                //重新计算单价
                studentCourseDetail.Price = ComBusiness2.GetOneClassDeSumByDay(studentCourseDetail.TotalMoney, studentCourseDetail.StartTime.Value,
                    studentCourseDetail.EndTime.Value);

                await AddStudentCourseConsumeLog(studentCourseDetail, (beforeSurplusQuantity - studentCourseDetail.SurplusQuantity),
                    (beforeSurplusSmallQuantity - studentCourseDetail.SurplusSmallQuantity), EmStudentCourseConsumeSourceType.SetExpirationDate, DateTime.Now);

                desc = $"设置课程起止日期:{studentCourseDetail.StartTime.EtmsToDateString()}~{studentCourseDetail.EndTime.EtmsToDateString()}";
            }
            await _studentCourseDAL.UpdateStudentCourseDetail(studentCourseDetail);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = studentCourseDetail.CourseId,
                StudentId = studentCourseDetail.StudentId,
                IsSendNoticeStudent = true
            });

            await _studentCourseOpLogDAL.AddStudentCourseOpLog(new EtStudentCourseOpLog()
            {
                CourseId = studentCourseDetail.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                OpTime = DateTime.Now,
                OpType = EmStudentCourseOpLogType.SetExpirationDate,
                OpUser = request.LoginUserId,
                StudentId = studentCourseDetail.StudentId,
                TenantId = request.LoginTenantId,
                OpContent = $"订单号:{studentCourseDetail.OrderNo},{desc}",
                Remark = string.Empty
            });
            await _userOperationLogDAL.AddUserLog(request, $"设置学生课程有效期-学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},课程订单号:{studentCourseDetail.OrderNo}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentClearance(StudentCourseClearRequest request)
        {
            var studentCourseDetail = await _studentCourseDAL.GetEtStudentCourseDetailById(request.CId);
            if (studentCourseDetail == null)
            {
                return ResponseBase.CommonError("不存在此订单");
            }
            if (studentCourseDetail.Status == EmStudentCourseStatus.EndOfClass)
            {
                return ResponseBase.CommonError("已剩余0课时，请勿重复操作");
            }
            var studentBucket = await _studentDAL.GetStudent(studentCourseDetail.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            var surplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(studentCourseDetail.SurplusQuantity, studentCourseDetail.SurplusSmallQuantity, studentCourseDetail.DeType);

            if (studentCourseDetail.SurplusQuantity > 0 || studentCourseDetail.SurplusSmallQuantity > 0)
            {
                await AddStudentCourseConsumeLog(studentCourseDetail, studentCourseDetail.SurplusQuantity, studentCourseDetail.SurplusSmallQuantity, EmStudentCourseConsumeSourceType.CourseClearance,
                    DateTime.Now);
            }
            studentCourseDetail.Status = EmStudentCourseStatus.EndOfClass;
            studentCourseDetail.SurplusQuantity = 0;
            studentCourseDetail.SurplusSmallQuantity = 0;
            studentCourseDetail.EndCourseRemark = request.Remark;
            studentCourseDetail.EndCourseTime = DateTime.Now;
            studentCourseDetail.EndCourseUser = request.LoginUserId;
            await _studentCourseDAL.UpdateStudentCourseDetail(studentCourseDetail);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = studentCourseDetail.CourseId,
                StudentId = studentCourseDetail.StudentId,
                IsSendNoticeStudent = true
            });

            await _studentCourseOpLogDAL.AddStudentCourseOpLog(new EtStudentCourseOpLog()
            {
                CourseId = studentCourseDetail.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                OpTime = DateTime.Now,
                OpType = EmStudentCourseOpLogType.CourseClearance,
                OpUser = request.LoginUserId,
                StudentId = studentCourseDetail.StudentId,
                TenantId = request.LoginTenantId,
                OpContent = $"订单号:{studentCourseDetail.OrderNo},原剩余课时{surplusQuantityDesc},已清零",
                Remark = request.Remark
            });
            await _userOperationLogDAL.AddUserLog(request, $"学员课时清零-学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},课程订单号:{studentCourseDetail.OrderNo},备注：{request.Remark}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseClassOver(StudentCourseClassOverRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            var studentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId, request.CourseId);
            if (studentCourseDetail.Count == 0)
            {
                return ResponseBase.CommonError("未查询到学员的课程信息");
            }
            var studentCourse = await _studentCourseDAL.GetStudentCourseDb(request.StudentId, request.CourseId);
            if (studentCourse.First().Status == EmStudentCourseStatus.EndOfClass)
            {
                return ResponseBase.CommonError("课程已结课");
            }

            var now = DateTime.Now;
            var myUsefulCourseDetail = studentCourseDetail.Where(p => p.Status != EmStudentCourseStatus.EndOfClass);
            if (myUsefulCourseDetail.Any())
            {
                foreach (var myStudentCourseDetail in myUsefulCourseDetail)
                {
                    myStudentCourseDetail.Status = EmStudentCourseStatus.EndOfClass;
                    myStudentCourseDetail.SurplusQuantity = 0;
                    myStudentCourseDetail.SurplusSmallQuantity = 0;
                    myStudentCourseDetail.EndCourseRemark = request.Remark;
                    myStudentCourseDetail.EndCourseTime = now;
                    myStudentCourseDetail.EndCourseUser = request.LoginUserId;
                    await _studentCourseDAL.UpdateStudentCourseDetail(myStudentCourseDetail);
                }
            }
            await _studentCourseDAL.SetStudentCourseOver(request.StudentId, request.CourseId);

            var beforeDesc = ComBusiness.GetStudentCourseDesc(studentCourse);
            var surplusQuantityClassTiems = studentCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            var surplusQuantityDay = studentCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
            if (surplusQuantityClassTiems != null
                && (surplusQuantityClassTiems.SurplusQuantity > 0 || surplusQuantityClassTiems.SurplusSmallQuantity > 0))
            {
                await AddStudentCourseConsumeLog(surplusQuantityClassTiems, EmStudentCourseConsumeSourceType.StudentCourseOver, now);
            }
            if (surplusQuantityDay != null
                && (surplusQuantityDay.SurplusQuantity > 0 || surplusQuantityDay.SurplusSmallQuantity > 0))
            {
                await AddStudentCourseConsumeLog(surplusQuantityDay, EmStudentCourseConsumeSourceType.StudentCourseOver, now);
            }

            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                IsClassOfOneAutoOver = true,
                IsSendNoticeStudent = true
            });
            _eventPublisher.Publish(new ClassRemoveStudentEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId
            });

            await _studentCourseOpLogDAL.AddStudentCourseOpLog(new EtStudentCourseOpLog()
            {
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                OpTime = DateTime.Now,
                OpType = EmStudentCourseOpLogType.CourseOver,
                OpUser = request.LoginUserId,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                OpContent = $"课程结课,原剩余课时{beforeDesc},剩余课时清零",
                Remark = request.Remark
            });
            await _userOperationLogDAL.AddUserLog(request, $"学员课程结课-学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},结课课程:{request.CourseName},备注：{request.Remark}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseChangeTimes(StudentCourseChangeTimesRequest request)
        {
            var studentCourseDetail = await _studentCourseDAL.GetEtStudentCourseDetailById(request.CId);
            if (studentCourseDetail == null)
            {
                return ResponseBase.CommonError("不存在此订单");
            }
            var studentBucket = await _studentDAL.GetStudent(studentCourseDetail.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            var beforSurplusDesc = ComBusiness.GetSurplusQuantityDesc(studentCourseDetail.SurplusQuantity, studentCourseDetail.SurplusSmallQuantity, studentCourseDetail.DeType);
            if (studentCourseDetail.DeType == EmDeClassTimesType.ClassTimes)
            {
                var beforSurplusQuantity = studentCourseDetail.SurplusQuantity;
                studentCourseDetail.StartTime = null;
                studentCourseDetail.EndTime = request.EndTime;
                studentCourseDetail.SurplusQuantity = Convert.ToDecimal(request.NewSurplusQuantity);

                await AddStudentCourseConsumeLog(studentCourseDetail, beforSurplusQuantity - studentCourseDetail.SurplusQuantity,
                    0, EmStudentCourseConsumeSourceType.CorrectStudentCourse, DateTime.Now);
            }
            else
            {
                studentCourseDetail.StartTime = Convert.ToDateTime(request.Ot[0]);
                studentCourseDetail.EndTime = Convert.ToDateTime(request.Ot[1]);
                var startTime = studentCourseDetail.StartTime.Value;
                if (studentCourseDetail.StartTime.Value < DateTime.Now.Date)
                {
                    startTime = DateTime.Now.Date;
                }
                var beforeSurplusQuantity = studentCourseDetail.SurplusQuantity;
                var beforeSurplusSmallQuantity = studentCourseDetail.SurplusSmallQuantity;

                var dffTime = EtmsHelper.GetDffTime(startTime, studentCourseDetail.EndTime.Value.AddDays(1));
                studentCourseDetail.SurplusQuantity = dffTime.Item1;
                studentCourseDetail.SurplusSmallQuantity = dffTime.Item2;

                await AddStudentCourseConsumeLog(studentCourseDetail, (beforeSurplusQuantity - studentCourseDetail.SurplusQuantity),
                    (beforeSurplusSmallQuantity - studentCourseDetail.SurplusSmallQuantity), EmStudentCourseConsumeSourceType.CorrectStudentCourse, DateTime.Now);
            }
            if (studentCourseDetail.Status == EmStudentCourseStatus.EndOfClass)
            {
                studentCourseDetail.Status = EmStudentCourseStatus.Normal;
            }
            studentCourseDetail.EndCourseRemark = string.Empty;
            studentCourseDetail.EndCourseTime = null;
            studentCourseDetail.EndCourseUser = null;
            await _studentCourseDAL.UpdateStudentCourseDetail(studentCourseDetail);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = studentCourseDetail.CourseId,
                StudentId = studentCourseDetail.StudentId,
                IsSendNoticeStudent = true
            });

            var endSurplusDesc = ComBusiness.GetSurplusQuantityDesc(studentCourseDetail.SurplusQuantity, studentCourseDetail.SurplusSmallQuantity, studentCourseDetail.DeType);
            await _studentCourseOpLogDAL.AddStudentCourseOpLog(new EtStudentCourseOpLog()
            {
                CourseId = studentCourseDetail.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                OpTime = DateTime.Now,
                OpType = EmStudentCourseOpLogType.CourseChangeTimes,
                OpUser = request.LoginUserId,
                StudentId = studentCourseDetail.StudentId,
                TenantId = request.LoginTenantId,
                OpContent = $"修正课时,原剩余课时:{beforSurplusDesc},修正后剩余课时:{endSurplusDesc}",
                Remark = request.Remark
            });
            await _userOperationLogDAL.AddUserLog(request, $"修正课时-学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},课程订单号:{studentCourseDetail.OrderNo}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        private async Task AddStudentCourseConsumeLog(EtStudentCourseDetail log, decimal deClassTimes, decimal deClassTimesSmall, byte sourceType, DateTime ot)
        {
            await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                TenantId = log.TenantId,
                Ot = ot,
                CourseId = log.CourseId,
                DeType = log.DeType,
                DeClassTimes = deClassTimes,
                DeClassTimesSmall = deClassTimesSmall,
                OrderId = log.OrderId,
                OrderNo = log.OrderNo,
                StudentId = log.StudentId,
                SourceType = sourceType
            });
        }

        private async Task AddStudentCourseConsumeLog(EtStudentCourse log, byte sourceType, DateTime ot)
        {
            await _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(new EtStudentCourseConsumeLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                TenantId = log.TenantId,
                Ot = ot,
                CourseId = log.CourseId,
                DeType = log.DeType,
                DeClassTimes = log.SurplusQuantity,
                DeClassTimesSmall = log.SurplusSmallQuantity,
                OrderId = null,
                OrderNo = string.Empty,
                StudentId = log.StudentId,
                SourceType = sourceType
            });
        }

        public async Task<ResponseBase> StudentCourseSurplusGet(StudentCourseSurplusGetRequest request)
        {
            var output = new List<StudentCourseSurplusGetOutput>();
            var course = await _courseDAL.GetCourse(request.CourseId);
            foreach (var student in request.StudentIds)
            {
                var studentCourse = await _studentCourseDAL.GetStudentCourse(student.Value, request.CourseId);
                var studentInfo = await _studentDAL.GetStudent(student.Value);
                output.Add(new StudentCourseSurplusGetOutput()
                {
                    CourseId = request.CourseId,
                    CourseName = course.Item1.Name,
                    CourseSurplusDesc = ComBusiness.GetStudentCourseDesc(studentCourse),
                    StudentId = student.Value,
                    StudentName = studentInfo.Student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(studentInfo.Student.Phone, request.SecrecyType),
                    StudentType = request.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(request.StudentType),
                    Points = course.Item1.CheckPoints,
                    StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, studentInfo.Student.Avatar),
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentCourseConsumeLogGetPaging(StudentCourseConsumeLogGetPagingRequest request)
        {
            var pagingData = await _studentCourseConsumeLogDAL.GetPaging(request);
            var output = new List<StudentCourseConsumeLogGetPagingOutput>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                output.Add(new StudentCourseConsumeLogGetPagingOutput()
                {
                    CId = p.Id,
                    CourseId = p.CourseId,
                    DeClassTimes = p.DeClassTimes.EtmsToString(),
                    DeClassTimesSmall = p.DeClassTimesSmall.EtmsToString(),
                    DeType = p.DeType,
                    DeTypeDesc = EmDeClassTimesType.GetDeClassTimesTypeDesc(p.DeType),
                    Ot = p.Ot,
                    SourceType = p.SourceType,
                    SourceTypeDesc = EmStudentCourseConsumeSourceType.GetStudentCourseConsumeSourceType(p.SourceType),
                    StudentId = p.StudentId,
                    DeClassTimesDesc = GetDeClassTimesDesc(p.SourceType, p.DeType, p.DeClassTimes, p.DeClassTimesSmall),
                    CourseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId),
                    StudentName = student?.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(student?.Phone, request.SecrecyType)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCourseConsumeLogGetPagingOutput>(pagingData.Item2, output));
        }

        private string GetDeClassTimesDesc(int sourceType, byte deType, decimal deClassTimes, decimal deClassTimesSmall)
        {
            var tag = EmStudentCourseConsumeSourceType.GetStudentCourseConsumeSourceTypeTag(sourceType);
            if (deClassTimes == 0 && deClassTimesSmall == 0)
            {
                return string.Empty;
            }
            if (deType == EmDeClassTimesType.ClassTimes)
            {
                if (deClassTimes == 0)
                {
                    return string.Empty;
                }
                return $"{tag}{deClassTimes.EtmsToString()}课时";
            }
            if (deClassTimes != 0 && deClassTimesSmall != 0)
            {
                var tatalDay = deClassTimes * 30 + deClassTimesSmall;
                return $"{tatalDay.EtmsToString()}天";
            }
            var strDesc = new StringBuilder();
            if (deClassTimes != 0)
            {
                strDesc.Append($"{deClassTimes.EtmsToString()}月");
            }
            if (deClassTimesSmall != 0)
            {
                strDesc.Append($"{deClassTimesSmall.EtmsToString()}天");
            }
            return $"{tag}{strDesc}";
        }

        public async Task<ResponseBase> StudentCourseHasGet(StudentCourseHasGetRequest request)
        {
            var output = new List<StudentCourseHasGetOutput>();
            var myStudentCourse = await _studentCourseDAL.GetStudentCourse(request.StudentId);
            if (myStudentCourse.Any())
            {
                foreach (var p in myStudentCourse)
                {
                    if (output.Exists(j => j.CourseId == p.CourseId))
                    {
                        break;
                    }
                    if (p.Status != EmStudentCourseStatus.EndOfClass && (p.SurplusQuantity > 0 || p.SurplusSmallQuantity > 0))
                    {
                        var myCourse = await _courseDAL.GetCourse(p.CourseId);
                        output.Add(new StudentCourseHasGetOutput()
                        {
                            CourseId = p.CourseId,
                            CourseName = myCourse.Item1.Name
                        });
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentCourseHasDetailGet(StudentCourseHasDetailGetRequest request)
        {
            var myCourse = await _courseDAL.GetCourse(request.CourseId);
            if (myCourse == null || myCourse.Item1 == null)
            {
                return ResponseBase.CommonError("课程不存在");
            }
            var myStudentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId, request.CourseId);
            var output = new List<StudentCourseHasDetailGetOutput>();
            if (myStudentCourseDetail.Count > 0)
            {
                foreach (var courseDetail in myStudentCourseDetail)
                {
                    if (courseDetail.Status == EmStudentCourseStatus.EndOfClass || (courseDetail.SurplusQuantity == 0 && courseDetail.SurplusSmallQuantity == 0))
                    {
                        continue;
                    }
                    var p = await _orderDAL.GetOrderDetail(courseDetail.OrderId, courseDetail.CourseId, EmProductType.Course);
                    if (p.OutOrderId != null)
                    {
                        continue;
                    }
                    if (p.InOutType == EmOrderInOutType.Out)
                    {
                        continue;
                    }
                    if (p == null)
                    {
                        LOG.Log.Error("[StudentCourseHasDetailGet]学员课程详情未找到对应的订单详情", request, this.GetType());
                        continue;
                    }
                    var courseCanReturnInfo = ComBusiness2.GetCourseCanReturnInfo(p, courseDetail);
                    if (!courseCanReturnInfo.IsHas)
                    {
                        continue;
                    }
                    output.Add(new StudentCourseHasDetailGetOutput()
                    {
                        OrderDetailId = p.Id,
                        PriceRule = p.PriceRule,
                        DiscountDesc = ComBusiness.GetDiscountDesc(p.DiscountValue, p.DiscountType),
                        BugUnit = p.BugUnit,
                        BuyQuantity = p.BuyQuantity,
                        BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(p.BuyQuantity, 0, p.BugUnit, p.ProductType),
                        GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(p.GiveQuantity, p.GiveUnit),
                        GiveQuantity = p.GiveQuantity,
                        ItemAptSum = p.ItemAptSum,
                        Price = courseCanReturnInfo.Price,
                        PriceDesc = p.PriceRule,
                        SurplusQuantity = courseCanReturnInfo.SurplusQuantity.EtmsToString(),
                        SurplusQuantityDesc = courseCanReturnInfo.SurplusQuantityDesc,
                        ItemSum = p.ItemSum,
                        ProductTypeDesc = EmProductType.GetProductType(p.ProductType),
                        ProductId = p.ProductId,
                        BuyValidSmallQuantity = courseCanReturnInfo.BuyValidSmallQuantity,
                        OrderNo = p.OrderNo,
                        ProductName = myCourse.Item1.Name
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        /// <summary>
        /// 学员课时不足提醒
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseBase StudentCourseNotEnoughRemind(StudentCourseNotEnoughRemindRequest request)
        {
            _eventPublisher.Publish(new NoticeStudentCourseNotEnoughEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId,
                IsOwnTrigger = true
            }); ;
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseNotEnoughRemindCancel(StudentCourseNotEnoughRemindCancelRequest request)
        {
            await _studentCourseDAL.CancelStudentCourseNotEnoughRemind(request.StudentId, request.CourseId);

            await _userOperationLogDAL.AddUserLog(request, "取消学员课程不足续费预警", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseSetCheckDefault(StudentCourseSetCheckDefaultRequest request)
        {
            await _studentCourseDAL.StudentCourseSetCheckDefault(request.StudentId, request.CourseId);

            await _userOperationLogDAL.AddUserLog(request, "设置学员考勤记上课课程", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }
    }
}
