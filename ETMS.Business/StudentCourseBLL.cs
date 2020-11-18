using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
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

        public StudentCourseBLL(ICourseDAL courseDAL, IStudentCourseDAL studentCourseDAL, IClassDAL classDAL, IStudentDAL studentDAL,
            IUserOperationLogDAL userOperationLogDAL, IEventPublisher eventPublisher, IClassRecordDAL classRecordDAL,
            IStudentCourseStopLogDAL studentCourseStopLogDAL, IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL,
            ITenantConfigDAL tenantConfigDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _courseDAL, _studentCourseDAL, _classDAL, _studentDAL, _userOperationLogDAL, _classRecordDAL,
                _studentCourseStopLogDAL, _studentCourseConsumeLogDAL, _tenantConfigDAL);
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
            foreach (var p in pagingData.Item1)
            {
                studentCourses.Add(new StudentCourseGetPagingOutput()
                {
                    CId = p.Id,
                    CourseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId),
                    DeTypeDesc = p.DeType == EmDeClassTimesType.ClassTimes ? "按课时" : "按月",
                    ExceedTotalClassTimes = p.ExceedTotalClassTimes,
                    Status = p.Status,
                    StatusDesc = EmStudentCourseStatus.GetStudentCourseStatusDesc(p.Status),
                    StudentName = p.StudentName,
                    StudentPhone = p.StudentPhone,
                    BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(p.BuyQuantity, p.BugUnit),
                    GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(p.GiveQuantity, p.GiveSmallQuantity, p.DeType),
                    SurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(p.SurplusQuantity, p.SurplusSmallQuantity, p.DeType),
                    UseQuantityDesc = ComBusiness.GetUseQuantityDesc(p.UseQuantity, p.UseUnit),
                    StudentId = p.StudentId,
                    Value = p.StudentId,
                    Label = p.StudentName
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCourseGetPagingOutput>(pagingData.Item2, studentCourses));
        }

        public async Task<ResponseBase> StudentCourseDetailGet(StudentCourseDetailGetRequest request)
        {
            var studentCourse = await _studentCourseDAL.GetStudentCourse(request.SId);
            var studentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.SId);
            var studentClass = await _classDAL.GetStudentClass(request.SId);
            var stopLogs = await _studentCourseStopLogDAL.GetStudentCourseStopLog(request.SId);
            var output = new List<StudentCourseDetailGetOutput>();
            if (studentCourse != null && studentCourse.Any())
            {
                var courseIds = studentCourse.Select(p => p.CourseId).Distinct();
                var tempBoxCourse = new DataTempBox<EtCourse>();
                foreach (var courseId in courseIds)
                {
                    var course = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, courseId);
                    if (course == null)
                    {
                        continue;
                    }
                    var myStudentCourseDetail = new StudentCourseDetailGetOutput()
                    {
                        CourseName = course.Item1,
                        CourseColor = course.Item2,
                        CourseId = courseId
                    };
                    var myCourse = studentCourse.Where(p => p.CourseId == courseId);
                    foreach (var theCourse in myCourse)
                    {
                        myStudentCourseDetail.Status = theCourse.Status;
                        myStudentCourseDetail.StopTimeDesc = theCourse.StopTime.EtmsToDateString();
                        myStudentCourseDetail.RestoreTimeDesc = theCourse.RestoreTime.EtmsToDateString();
                        if (theCourse.DeType == EmDeClassTimesType.ClassTimes)
                        {
                            myStudentCourseDetail.ExceedTotalClassTimes = theCourse.ExceedTotalClassTimes;
                            myStudentCourseDetail.DeTypeClassTimes = new DeTypeClassTimes()
                            {
                                SurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(theCourse.SurplusQuantity, theCourse.SurplusSmallQuantity, theCourse.DeType)
                            };
                        }
                        if (theCourse.DeType == EmDeClassTimesType.Day)
                        {
                            myStudentCourseDetail.DeTypeDay = new DeTypeDay()
                            {
                                SurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(theCourse.SurplusQuantity, theCourse.SurplusSmallQuantity, theCourse.DeType)
                            };
                        }
                    }
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
                            BuyQuantityDesc = ComBusiness.GetBuyQuantityDesc(p.BuyQuantity, p.BugUnit),
                            GiveQuantityDesc = ComBusiness.GetGiveQuantityDesc(giveQuantity, giveSmallQuantity, p.DeType),
                            CId = p.Id,
                            SurplusQuantityDesc = ComBusiness.GetSurplusQuantityDesc(p.SurplusQuantity, p.SurplusSmallQuantity, p.DeType),
                            UseQuantityDesc = ComBusiness.GetUseQuantityDesc(p.UseQuantity, p.DeType),
                            StatusDesc = EmStudentCourseStatus.GetStudentCourseStatusDesc(p.Status),
                            EndCourseRemark = p.EndCourseRemark
                        });
                    }
                    if (stopLogs != null && stopLogs.Any())
                    {
                        var myStopLog = stopLogs.Where(p => p.CourseId == courseId).OrderByDescending(p => p.Id);
                        if (myStopLog.Any())
                        {
                            myStudentCourseDetail.StopLogs = new List<StopLog>();
                            foreach (var log in myStopLog)
                            {
                                myStudentCourseDetail.StopLogs.Add(new StopLog()
                                {
                                    Remark = log.Remark,
                                    StopDay = log.StopDay,
                                    StopTimeDesc = log.StopTime.EtmsToDateString(),
                                    RestoreTimeDesc = log.RestoreTime == null ? "未复课" : log.RestoreTime.EtmsToDateString()
                                });
                            }
                        }
                    }
                    output.Add(myStudentCourseDetail);
                }
            }
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
            await _studentCourseDAL.StudentCourseStop(request.StudentId, request.CourseId, stopTime);
            await _studentCourseStopLogDAL.AddStudentCourseStopLog(new EtStudentCourseStopLog()
            {
                CourseId = request.CourseId,
                IsDeleted = EmIsDeleted.Normal,
                Ot = stopTime,
                Remark = request.Remark,
                RestoreTime = null,
                StopDay = 0,
                StopTime = stopTime,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId

            });
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId
            });
            await _userOperationLogDAL.AddUserLog(request, $"学员课程停课：学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},停课课程:{request.CourseName},{request.Remark}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseRestoreTime(StudentCourseRestoreTimeRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            await _studentCourseDAL.StudentCourseRestoreTime(request.StudentId, request.CourseId);
            await _studentCourseStopLogDAL.StudentCourseRestore(request.StudentId, request.CourseId, DateTime.Now);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId
            });
            await _userOperationLogDAL.AddUserLog(request, $"学员课程复课：学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},复课课程:{request.CourseName}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseMarkExceedClassTimes(StudentCourseMarkExceedClassTimesRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            await _studentCourseDAL.StudentCourseMarkExceedClassTimes(request.StudentId, request.CourseId);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = request.CourseId,
                StudentId = request.StudentId
            });
            await _userOperationLogDAL.AddUserLog(request, $"超上课时标记处理：学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},超上课程:{request.CourseName}", EmUserOperationType.StudentCourseManage);
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
            if (studentCourseDetail.DeType == EmDeClassTimesType.ClassTimes)
            {
                studentCourseDetail.StartTime = null;
                studentCourseDetail.EndTime = request.EndTime;
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
                    (beforeSurplusSmallQuantity - studentCourseDetail.SurplusSmallQuantity), EmStudentCourseConsumeSourceType.SetExpirationDate, DateTime.Now);
            }
            await _studentCourseDAL.UpdateStudentCourseDetail(studentCourseDetail);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.LoginTenantId)
            {
                CourseId = studentCourseDetail.CourseId,
                StudentId = studentCourseDetail.StudentId
            });
            await _userOperationLogDAL.AddUserLog(request, $"设置学生课程有效期：学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},课程订单号:{studentCourseDetail.OrderNo}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCourseClassOver(StudentCourseClassOverRequest request)
        {
            var studentCourseDetail = await _studentCourseDAL.GetEtStudentCourseDetailById(request.CId);
            if (studentCourseDetail == null)
            {
                return ResponseBase.CommonError("不存在此订单");
            }
            if (studentCourseDetail.Status == EmStudentCourseStatus.EndOfClass)
            {
                return ResponseBase.CommonError("已结课，请勿重复操作");
            }
            var studentBucket = await _studentDAL.GetStudent(studentCourseDetail.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("不存在此学员");
            }
            if (studentCourseDetail.SurplusQuantity > 0 || studentCourseDetail.SurplusSmallQuantity > 0)
            {
                await AddStudentCourseConsumeLog(studentCourseDetail, studentCourseDetail.SurplusQuantity, studentCourseDetail.SurplusSmallQuantity, EmStudentCourseConsumeSourceType.StopStudentCourse,
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
                StudentId = studentCourseDetail.StudentId
            });
            await _userOperationLogDAL.AddUserLog(request, $"课程结课：学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},课程订单号:{studentCourseDetail.OrderNo},备注：{request.Remark}", EmUserOperationType.StudentCourseManage);
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
            if (studentCourseDetail.DeType == EmDeClassTimesType.ClassTimes)
            {
                var beforSurplusQuantity = studentCourseDetail.SurplusQuantity;
                studentCourseDetail.StartTime = null;
                studentCourseDetail.EndTime = request.EndTime;
                studentCourseDetail.SurplusQuantity = Convert.ToInt32(request.NewSurplusQuantity);

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
                StudentId = studentCourseDetail.StudentId
            });
            await _userOperationLogDAL.AddUserLog(request, $"修改课时：学员:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},课程订单号:{studentCourseDetail.OrderNo}", EmUserOperationType.StudentCourseManage);
            return ResponseBase.Success();
        }

        private async Task AddStudentCourseConsumeLog(EtStudentCourseDetail log, int deClassTimes, int deClassTimesSmall, byte sourceType, DateTime ot)
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
                    StudentPhone = studentInfo.Student.Phone,
                    StudentType = request.StudentType,
                    StudentTypeDesc = EmClassStudentType.GetClassStudentTypeDesc(request.StudentType)
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
                    DeClassTimes = p.DeClassTimes,
                    DeClassTimesSmall = p.DeClassTimesSmall,
                    DeType = p.DeType,
                    DeTypeDesc = EmDeClassTimesType.GetDeClassTimesTypeDesc(p.DeType),
                    Ot = p.Ot,
                    SourceType = p.SourceType,
                    SourceTypeDesc = EmStudentCourseConsumeSourceType.GetStudentCourseConsumeSourceType(p.SourceType),
                    StudentId = p.StudentId,
                    DeClassTimesDesc = GetDeClassTimesDesc(p.SourceType, p.DeType, p.DeClassTimes, p.DeClassTimesSmall),
                    CourseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId),
                    StudentName = student?.Name,
                    StudentPhone = student?.Phone
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCourseConsumeLogGetPagingOutput>(pagingData.Item2, output));
        }

        private string GetDeClassTimesDesc(int sourceType, byte deType, int deClassTimes, int deClassTimesSmall)
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
                return $"{tag}{deClassTimes}课时";
            }
            var strDesc = new StringBuilder();
            if (deClassTimes != 0)
            {
                strDesc.Append($"{deClassTimes}月");
            }
            if (deClassTimesSmall != 0)
            {
                strDesc.Append($"{deClassTimesSmall}天");
            }
            return $"{tag}{strDesc}";
        }
    }
}
