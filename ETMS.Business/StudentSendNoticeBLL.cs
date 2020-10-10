using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.LOG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IEventProvider;
using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.ExternalService.Contract;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Utility;
using ETMS.Entity.Enum;

namespace ETMS.Business
{
    public class StudentSendNoticeBLL : IStudentSendNoticeBLL
    {
        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly ITempDataCacheDAL _tempDataCacheDAL;

        private readonly IJobAnalyzeDAL _jobAnalyzeDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly ISmsService _smsService;

        private readonly IClassDAL _classDAL;

        private readonly ITempStudentClassNoticeDAL _tempStudentClassNoticeDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        public StudentSendNoticeBLL(ITenantConfigDAL tenantConfigDAL, ITempDataCacheDAL tempDataCacheDAL, IJobAnalyzeDAL jobAnalyzeDAL,
            IEventPublisher eventPublisher, IStudentDAL studentDAL, ICourseDAL courseDAL, IClassRoomDAL classRoomDAL, ISmsService smsService,
            IClassDAL classDAL, ITempStudentClassNoticeDAL tempStudentClassNoticeDAL, IStudentCourseDAL studentCourseDAL)
        {
            this._tenantConfigDAL = tenantConfigDAL;
            this._tempDataCacheDAL = tempDataCacheDAL;
            this._jobAnalyzeDAL = jobAnalyzeDAL;
            this._eventPublisher = eventPublisher;
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._classRoomDAL = classRoomDAL;
            this._smsService = smsService;
            this._classDAL = classDAL;
            this._tempStudentClassNoticeDAL = tempStudentClassNoticeDAL;
            this._studentCourseDAL = studentCourseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _tenantConfigDAL, _jobAnalyzeDAL, _studentDAL, _courseDAL, _classRoomDAL, _classDAL,
                _tempStudentClassNoticeDAL, _studentCourseDAL);
        }

        public async Task NoticeStudentsOfClassBeforeDayTenant(NoticeStudentsOfClassBeforeDayTenantEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StartClassDayBeforeIsOpen) //是否开启了提前一天提醒
            {
                return;
            }
            var noticeTime = tenantConfig.StudentNoticeConfig.StartClassDayBeforeTimeValue;
            if (request.NowTime < noticeTime)  //提醒时间限制
            {
                return;
            }
            var hisBucket = _tempDataCacheDAL.GetNoticeStudentsOfClassDayBucket(request.TenantId, request.ClassOt);
            if (hisBucket != null)
            {
                Log.Debug($"[NoticeStudentsOfClassBeforeDayTenant]重复收到处理请求:TenantId:{request.TenantId},ClassOt:{request.ClassOt}", this.GetType());
                return;
            }
            Log.Info($"[NoticeStudentsOfClassBeforeDayTenant]准备处理上课前一天上课提醒：TenantId:{request.TenantId}", this.GetType());
            var classTimes = await _jobAnalyzeDAL.GetClassTimesUnRollcall(request.ClassOt);
            if (classTimes != null && classTimes.Any())
            {
                foreach (var p in classTimes)
                {
                    _eventPublisher.Publish(new NoticeStudentsOfClassBeforeDayTimesEvent(request.TenantId)
                    {
                        ClassTimes = p,
                        IsSendSms = tenantConfig.StudentNoticeConfig.StartClassSms,
                        IsSendWeChat = tenantConfig.StudentNoticeConfig.StartClassWeChat
                    });
                }
            }
            _tempDataCacheDAL.SetNoticeStudentsOfClassDayBucket(request.TenantId, request.ClassOt);
        }

        public async Task NoticeStudentsOfClassBeforeDayClassTimes(NoticeStudentsOfClassBeforeDayTimesEvent request)
        {
            if (!request.IsSendSms && !request.IsSendWeChat)
            {
                Log.Debug($"[NoticeStudentsOfClassBeforeDayClassTimes]未开启发送上课提醒服务:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimes.Id}", this.GetType());
                return;
            }
            var classTimesStudents = await _jobAnalyzeDAL.GetClassTimesStudent(request.ClassTimes.Id);
            var classBucket = await _classDAL.GetClassBucket(request.ClassTimes.ClassId);
            if (classBucket == null)
            {
                Log.Debug($"[NoticeStudentsOfClassBeforeDayClassTimes]未找到对应的班级信息:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimes.Id}", this.GetType());
                return;
            }
            var classStudent = classBucket.EtClassStudents;
            if (!classTimesStudents.Any() && !classStudent.Any())
            {
                Log.Debug($"[NoticeStudentsOfClassBeforeDayClassTimes]未查询到要提醒的学生:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimes.Id}", this.GetType());
                return;
            }
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var stringClassRoom = string.Empty;
            if (!string.IsNullOrEmpty(request.ClassTimes.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                stringClassRoom = ComBusiness.GetClassRoomDesc(allClassRoom, request.ClassTimes.ClassRoomIds);
            }
            var smsReq = new NoticeStudentsOfClassBeforeDayRequest()
            {
                ClassRoom = stringClassRoom,
                ClassTimeDesc = EtmsHelper.GetTimeDesc(request.ClassTimes.StartTime, request.ClassTimes.EndTime, "-"),
                Students = new List<NoticeStudentsOfClassBeforeDayStudent>()
            };
            if (classTimesStudents != null && classTimesStudents.Any())
            {
                foreach (var p in classTimesStudents)
                {
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    if (string.IsNullOrEmpty(student.Phone))
                    {
                        continue;
                    }
                    var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                    smsReq.Students.Add(new NoticeStudentsOfClassBeforeDayStudent()
                    {
                        CourseName = courseName,
                        Phone = student.Phone,
                        StudentName = student.Name
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        smsReq.Students.Add(new NoticeStudentsOfClassBeforeDayStudent()
                        {
                            CourseName = courseName,
                            Phone = student.PhoneBak,
                            StudentName = student.Name
                        });
                    }
                }
            }
            if (classStudent != null && classStudent.Any())
            {
                foreach (var p in classStudent)
                {
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    if (string.IsNullOrEmpty(student.Phone))
                    {
                        continue;
                    }
                    var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                    smsReq.Students.Add(new NoticeStudentsOfClassBeforeDayStudent()
                    {
                        CourseName = courseName,
                        Phone = student.Phone,
                        StudentName = student.Name
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        smsReq.Students.Add(new NoticeStudentsOfClassBeforeDayStudent()
                        {
                            CourseName = courseName,
                            Phone = student.PhoneBak,
                            StudentName = student.Name
                        });
                    }
                }
            }
            if (smsReq.Students.Any())
            {
                if (request.IsSendSms)
                {
                    await _smsService.NoticeStudentsOfClassBeforeDay(smsReq);
                }
                if (request.IsSendWeChat)
                {
                    // TODA
                }
            }
        }

        public async Task NoticeStudentsOfClassTodayGenerate(NoticeStudentsOfClassTodayGenerateEvent request)
        {
            Log.Info($"[NoticeStudentsOfClassTodayGenerate]准备提前生成学员上课通知数据：TenantId:{request.TenantId}", this.GetType());
            await _tempStudentClassNoticeDAL.GenerateTempStudentClassNotice(request.ClassOt);
        }

        public async Task NoticeStudentsOfClassTodayTenant(NoticeStudentsOfClassTodayTenantEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StartClassBeforeMinuteIsOpen) //是否开启当天提醒
            {
                return;
            }
            var beforeMinute = tenantConfig.StudentNoticeConfig.StartClassBeforeMinuteValue;
            var classNoticeData = await _tempStudentClassNoticeDAL.GetTempStudentClassNotice(request.ClassOt);
            if (classNoticeData != null && classNoticeData.Count > 0)
            {
                var ids = new List<long>();
                foreach (var p in classNoticeData)
                {
                    var diffMinute = p.ClassTime.Subtract(request.NowTime).TotalMinutes;  //上课时间和当前时间相差的分钟数
                    if (diffMinute <= 0)
                    {
                        Log.Warn($"[NoticeStudentsOfClassTodayTenant]超时提醒:TenantId:{request.TenantId},ClassTimesId:{p.ClassTimesId},ClassOt:{request.ClassOt}", this.GetType());
                        ids.Add(p.Id);
                        _eventPublisher.Publish(new NoticeStudentsOfClassTodayClassTimesEvent(request.TenantId)
                        {
                            IsSendSms = tenantConfig.StudentNoticeConfig.StartClassSms,
                            IsSendWeChat = tenantConfig.StudentNoticeConfig.StartClassWeChat,
                            ClassTimesId = p.ClassTimesId
                        });
                    }
                    else if (diffMinute - beforeMinute <= 5)  //5分钟的容错时间
                    {
                        ids.Add(p.Id);
                        _eventPublisher.Publish(new NoticeStudentsOfClassTodayClassTimesEvent(request.TenantId)
                        {
                            IsSendSms = tenantConfig.StudentNoticeConfig.StartClassSms,
                            IsSendWeChat = tenantConfig.StudentNoticeConfig.StartClassWeChat,
                            ClassTimesId = p.ClassTimesId
                        });
                    }
                }
                if (ids.Count > 0)
                {
                    await _tempStudentClassNoticeDAL.SetProcessed(ids, request.ClassOt);
                }
            }
            else
            {
                Log.Debug($"[NoticeStudentsOfClassTodayTenant]未查询到需要提醒的课次信息:TenantId:{request.TenantId},ClassOt:{request.ClassOt}", this.GetType());
            }
        }

        public async Task NoticeStudentsOfClassTodayClassTimes(NoticeStudentsOfClassTodayClassTimesEvent request)
        {
            if (!request.IsSendSms && !request.IsSendWeChat)
            {
                Log.Debug($"[NoticeStudentsOfClassTodayClassTimes]未开启发送上课提醒服务:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimesId}", this.GetType());
                return;
            }
            var classTimes = await _jobAnalyzeDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes.Status == EmClassTimesStatus.BeRollcall)
            {
                Log.Warn($"[NoticeStudentsOfClassTodayClassTimes]已点名,无需发送上课通知:TenantId:{request.TenantId},ClassTimesId:{request.ClassTimesId}", this.GetType());
                return;
            }
            var classTimesStudents = await _jobAnalyzeDAL.GetClassTimesStudent(classTimes.Id);
            var classBucket = await _classDAL.GetClassBucket(classTimes.ClassId);
            if (classBucket == null)
            {
                Log.Debug($"[NoticeStudentsOfClassTodayClassTimes]未找到对应的班级信息:TenantId:{request.TenantId},ClassTimesId:{classTimes.Id}", this.GetType());
                return;
            }
            var classStudent = classBucket.EtClassStudents;
            if (!classTimesStudents.Any() && !classStudent.Any())
            {
                Log.Debug($"[NoticeStudentsOfClassTodayClassTimes]未查询到要提醒的学生:TenantId:{request.TenantId},ClassTimesId:{classTimes.Id}", this.GetType());
                return;
            }
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var stringClassRoom = string.Empty;
            if (!string.IsNullOrEmpty(classTimes.ClassRoomIds))
            {
                var allClassRoom = await _classRoomDAL.GetAllClassRoom();
                stringClassRoom = ComBusiness.GetClassRoomDesc(allClassRoom, classTimes.ClassRoomIds);
            }
            var smsReq = new NoticeStudentsOfClassTodayRequest()
            {
                ClassRoom = stringClassRoom,
                ClassTimeDesc = EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime, "-"),
                Students = new List<NoticeStudentsOfClassTodayStudent>()
            };
            if (classTimesStudents != null && classTimesStudents.Any())
            {
                foreach (var p in classTimesStudents)
                {
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    if (string.IsNullOrEmpty(student.Phone))
                    {
                        continue;
                    }
                    var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                    smsReq.Students.Add(new NoticeStudentsOfClassTodayStudent()
                    {
                        CourseName = courseName,
                        Phone = student.Phone,
                        StudentName = student.Name
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        smsReq.Students.Add(new NoticeStudentsOfClassTodayStudent()
                        {
                            CourseName = courseName,
                            Phone = student.PhoneBak,
                            StudentName = student.Name
                        });
                    }
                }
            }
            if (classStudent != null && classStudent.Any())
            {
                foreach (var p in classStudent)
                {
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    var student = studentBucket.Student;
                    if (string.IsNullOrEmpty(student.Phone))
                    {
                        continue;
                    }
                    var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                    smsReq.Students.Add(new NoticeStudentsOfClassTodayStudent()
                    {
                        CourseName = courseName,
                        Phone = student.Phone,
                        StudentName = student.Name
                    });
                    if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                    {
                        smsReq.Students.Add(new NoticeStudentsOfClassTodayStudent()
                        {
                            CourseName = courseName,
                            Phone = student.PhoneBak,
                            StudentName = student.Name
                        });
                    }
                }
            }
            if (smsReq.Students.Any())
            {
                if (request.IsSendSms)
                {
                    await _smsService.NoticeStudentsOfClassToday(smsReq);
                }
                if (request.IsSendWeChat)
                {
                    // TODA
                }
            }
        }

        public async Task NoticeStudentsCheckSign(NoticeStudentsCheckSignEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.ClassCheckSignSms && !tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat)
            {
                return;
            }
            var classRecord = await _jobAnalyzeDAL.GetClassRecord(request.ClassRecordId);
            if (classRecord == null)
            {
                Log.Warn($"[NoticeStudentsCheckSign]未找到点名记录,ClassRecordId:{request.ClassRecordId}", this.GetType());
                return;
            }
            var classRecordStudent = await _jobAnalyzeDAL.GetClassRecordStudent(request.ClassRecordId);
            if (classRecordStudent == null || classRecordStudent.Count == 0)
            {
                Log.Warn($"[NoticeStudentsCheckSign]未找到上课学员,ClassRecordId:{request.ClassRecordId}", this.GetType());
                return;
            }
            var req = new NoticeClassCheckSignRequest()
            {
                ClassTimeDesc = $"{classRecord.ClassOt.EtmsToDateString()} {EtmsHelper.GetTimeDesc(classRecord.StartTime, classRecord.EndTime, "-")}",
                Students = new List<NoticeClassCheckSignStudent>()
            };
            var tempBoxCourse = new DataTempBox<EtCourse>();
            foreach (var p in classRecordStudent)
            {
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    continue;
                }
                var student = studentBucket.Student;
                if (string.IsNullOrEmpty(student.Phone))
                {
                    continue;
                }
                var courseName = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId);
                var mySurplusClassTimes = await _studentCourseDAL.GetStudentCourse(p.StudentId, p.CourseId);
                var surplusClassTimesDesc = "0课时";
                if (mySurplusClassTimes != null && mySurplusClassTimes.Any())
                {
                    var temp = new StringBuilder();
                    var deClassTimes = mySurplusClassTimes.FirstOrDefault(j => j.DeType == EmDeClassTimesType.ClassTimes);
                    if (deClassTimes != null && deClassTimes.BuyQuantity > 0)
                    {
                        temp.Append(ComBusiness.GetSurplusQuantityDesc(deClassTimes.SurplusQuantity, deClassTimes.SurplusSmallQuantity, deClassTimes.DeType));
                    }
                    var deDay = mySurplusClassTimes.FirstOrDefault(j => j.DeType == EmDeClassTimesType.Day);
                    if (deDay != null && deDay.BuyQuantity > 0)
                    {
                        temp.Append(ComBusiness.GetSurplusQuantityDesc(deDay.SurplusQuantity, deDay.SurplusSmallQuantity, deDay.DeType));
                    }
                    if (temp.Length == 0)
                    {
                        surplusClassTimesDesc = "0课时";
                    }
                    else
                    {
                        surplusClassTimesDesc = temp.ToString();
                    }
                }
                req.Students.Add(new NoticeClassCheckSignStudent()
                {
                    CourseName = courseName,
                    Name = student.Name,
                    Phone = student.Phone,
                    DeClassTimesDesc = p.DeClassTimes.ToString(),
                    StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                    SurplusClassTimesDesc = surplusClassTimesDesc
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new NoticeClassCheckSignStudent()
                    {
                        CourseName = courseName,
                        Name = student.Name,
                        Phone = student.PhoneBak,
                        DeClassTimesDesc = p.DeClassTimes.ToString(),
                        StudentCheckStatusDesc = EmClassStudentCheckStatus.GetClassStudentCheckStatus(p.StudentCheckStatus),
                        SurplusClassTimesDesc = surplusClassTimesDesc
                    });
                }
            }
            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.ClassCheckSignSms)
                {
                    await _smsService.NoticeClassCheckSign(req);
                }
                if (tenantConfig.StudentNoticeConfig.ClassCheckSignWeChat)
                {
                    // TODA
                }
            }
        }

        public async Task NoticeStudentLeaveApply(NoticeStudentLeaveApplyEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckSms && !tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckWeChat)
            {
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(request.StudentLeaveApplyLog.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Warn($"[NoticeStudentLeaveApply]未找到学员信息,StudentLeaveApplyLogId:{request.StudentLeaveApplyLog.Id}", this.GetType());
                return;
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }
            var req = new NoticeStudentLeaveApplyRequest()
            {
                TimeDesc = $"{request.StudentLeaveApplyLog.StartDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(request.StudentLeaveApplyLog.StartTime)}-{request.StudentLeaveApplyLog.EndDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(request.StudentLeaveApplyLog.EndTime)}",
                Students = new List<NoticeStudentLeaveApplyStudent>()
            };
            var handleStatusDesc = request.StudentLeaveApplyLog.HandleStatus == EmStudentLeaveApplyHandleStatus.Pass ? "已经通过" : "审核未通过";
            req.Students.Add(new NoticeStudentLeaveApplyStudent()
            {
                HandleStatusDesc = handleStatusDesc,
                Name = student.Name,
                Phone = student.Phone
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentLeaveApplyStudent()
                {
                    HandleStatusDesc = handleStatusDesc,
                    Name = student.Name,
                    Phone = student.PhoneBak,
                });
            }
            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckSms)
                {
                    await _smsService.NoticeStudentLeaveApply(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentAskForLeaveCheckWeChat)
                {
                    // TODA
                }
            }
        }

        public async Task NoticeStudentContracts(NoticeStudentContractsEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.OrderBySms && !tenantConfig.StudentNoticeConfig.OrderByWeChat)
            {
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(request.Order.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Warn($"[NoticeStudentContracts]未找到学员信息,OrderId:{request.Order.Id}", this.GetType());
                return;
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }
            var req = new NoticeStudentContractsRequest()
            {
                AptSumDesc = request.Order.AptSum.ToString("F2"),
                PaySumDesc = request.Order.PaySum.ToString("F2"),
                TimeDedc = request.Order.Ot.EtmsToDateString(),
                Students = new List<NoticeStudentContractsStudent>()
            };
            var buyDesc = new StringBuilder();
            var buyCourse = request.OrderDetails.Where(p => p.ProductType == EmOrderProductType.Course);
            if (buyCourse.Any())
            {
                var tempCount = buyCourse.Count();
                if (tempCount == 1)
                {
                    var course = await _courseDAL.GetCourse(buyCourse.First().ProductId);
                    buyDesc.Append($"报读了课程（{course.Item1.Name}）");
                }
                else
                {
                    buyDesc.Append($"报读了{tempCount}门课程");
                }
            }
            var buyGoods = request.OrderDetails.Where(p => p.ProductType == EmOrderProductType.Goods);
            if (buyGoods.Any())
            {
                var tempCount = buyGoods.Count();
                if (buyDesc.Length > 0)
                {
                    buyDesc.Append($"、{tempCount}件物品");
                }
                else
                {
                    buyDesc.Append($"购买了{tempCount}件物品");
                }
            }
            var buyCost = request.OrderDetails.Where(p => p.ProductType == EmOrderProductType.Cost);
            if (buyCost.Any())
            {
                var tempCount = buyCost.Count();
                if (buyDesc.Length > 0)
                {
                    buyDesc.Append($"、{tempCount}笔费用");
                }
                else
                {
                    buyDesc.Append($"购买了{tempCount}笔费用");
                }
            }
            req.BuyDesc = buyDesc.ToString();
            req.Students.Add(new NoticeStudentContractsStudent()
            {
                Name = student.Name,
                Phone = student.Phone
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentContractsStudent()
                {
                    Name = student.Name,
                    Phone = student.PhoneBak
                });
            }
            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.OrderBySms)
                {
                    await _smsService.NoticeStudentContracts(req);
                }
                if (tenantConfig.StudentNoticeConfig.OrderByWeChat)
                {
                    // TODA
                }
            }
        }
    }
}
