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
using ETMS.Entity.Config;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IBusiness.Wechart;
using ETMS.Business.WxCore;
using ETMS.IBusiness.SendNotice;
using Newtonsoft.Json;

namespace ETMS.Business.SendNotice
{
    public class StudentSendNotice2BLL : SendNoticeBase, IStudentSendNotice2BLL
    {
        private readonly IWxService _wxService;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IUserDAL _userDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IActiveHomeworkDetailDAL _activeHomeworkDetailDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IActiveGrowthRecordDAL _activeGrowthRecordDAL;

        private readonly IClassDAL _classDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly ISmsService _smsService;

        public StudentSendNotice2BLL(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL,
            IWxService wxService, IAppConfigurtaionServices appConfigurtaionServices, IUserDAL userDAL, ITenantConfigDAL tenantConfigDAL,
            IActiveHomeworkDetailDAL activeHomeworkDetailDAL, IStudentDAL studentDAL, IActiveGrowthRecordDAL activeGrowthRecordDAL,
            IClassDAL classDAL, IClassRecordDAL classRecordDAL, ICourseDAL courseDAL, IStudentCourseDAL studentCourseDAL,
            IClassTimesDAL classTimesDAL, ISmsService smsService)
            : base(studentWechatDAL, componentAccessBLL, sysTenantDAL)
        {
            this._wxService = wxService;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._userDAL = userDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._activeHomeworkDetailDAL = activeHomeworkDetailDAL;
            this._studentDAL = studentDAL;
            this._activeGrowthRecordDAL = activeGrowthRecordDAL;
            this._classDAL = classDAL;
            this._classRecordDAL = classRecordDAL;
            this._courseDAL = courseDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._classTimesDAL = classTimesDAL;
            this._smsService = smsService;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentWechatDAL, _userDAL, _tenantConfigDAL, _activeHomeworkDetailDAL,
                _studentDAL, _activeGrowthRecordDAL, _classDAL, _classRecordDAL, _courseDAL, _studentCourseDAL, _classTimesDAL);
        }

        public async Task NoticeStudentsOfHomeworkAddConsumeEvent(NoticeStudentsOfHomeworkAddEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentHomeworkWeChat)
            {
                return;
            }
            var homeworkDetails = await _activeHomeworkDetailDAL.GetActiveHomeworkDetail(request.HomeworkId);
            if (homeworkDetails.Count == 0)
            {
                return;
            }
            var exDateDesc = string.Empty;
            if (homeworkDetails[0].ExDate != null)
            {
                exDateDesc = homeworkDetails[0].ExDate.Value.EtmsToMinuteString();
            }
            var req = new HomeworkAddRequest(await GetNoticeRequestBase(request.TenantId))
            {
                HomeworkTitle = homeworkDetails[0].Title,
                ExDateDesc = exDateDesc,
                Students = new List<HomeworkAddStudent>()
            };

            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.HomeworkAdd;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            foreach (var myHomeWorkDetail in homeworkDetails)
            {
                var studentBucket = await _studentDAL.GetStudent(myHomeWorkDetail.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    Log.Warn($"[NoticeStudentsOfHomeworkAddConsumeEvent]未找到学员信息,StudentId:{myHomeWorkDetail.StudentId}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                if (string.IsNullOrEmpty(student.Phone))
                {
                    continue;
                }
                var url = string.Format(wxConfig.TemplateNoticeConfig.StudentHomeworkDetailUrl, myHomeWorkDetail.Id, myHomeWorkDetail.AnswerStatus);
                req.Students.Add(new HomeworkAddStudent()
                {
                    Name = student.Name,
                    OpendId = await GetStudentOpenId(true, student.Phone),
                    Phone = student.Phone,
                    StudentId = student.Id,
                    Url = url
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new HomeworkAddStudent()
                    {
                        Name = student.Name,
                        OpendId = await GetStudentOpenId(true, student.PhoneBak),
                        Phone = student.PhoneBak,
                        StudentId = student.Id,
                        Url = url
                    });
                }
            }

            if (req.Students.Count > 0)
            {
                _wxService.HomeworkAdd(req);
            }
        }

        public async Task NoticeStudentsOfHomeworkAddCommentConsumeEvent(NoticeStudentsOfHomeworkAddCommentEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentHomeworkCommentWeChat)
            {
                return;
            }

            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                Log.Error($"[NoticeStudentsOfHomeworkAddCommentConsumeEvent]未找到作业信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var homeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            var studentBucket = await _studentDAL.GetStudent(homeworkDetail.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeStudentsOfHomeworkAddCommentConsumeEvent]未找到学员信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }

            var user = await _userDAL.GetUser(request.AddUserId);
            var req = new HomeworkCommentRequest(await GetNoticeRequestBase(request.TenantId))
            {
                HomeworkTitle = homeworkDetail.Title,
                OtDesc = request.MyOt.EtmsToMinuteString(),
                UserName = ComBusiness2.GetParentTeacherName(user),
                Students = new List<HomeworkCommentStudent>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.HomeworkComment;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            var url = string.Format(wxConfig.TemplateNoticeConfig.StudentHomeworkDetailUrl, homeworkDetail.Id, homeworkDetail.AnswerStatus);
            req.Students.Add(new HomeworkCommentStudent()
            {
                Name = student.Name,
                OpendId = await GetStudentOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id,
                Url = url
            });

            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new HomeworkCommentStudent()
                {
                    Name = student.Name,
                    OpendId = await GetStudentOpenId(true, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                    Url = url
                });
            }

            _wxService.HomeworkComment(req);
        }

        public async Task NoticeStudentsOfGrowthRecordConsumeEvent(NoticeStudentsOfGrowthRecordEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentGrowUpRecordWeChat)
            {
                return;
            }

            var activeGrowthRecordBucket = await _activeGrowthRecordDAL.GetActiveGrowthRecord(request.GrowthRecordId);
            if (activeGrowthRecordBucket == null || activeGrowthRecordBucket.ActiveGrowthRecord == null)
            {
                Log.Error($"[NoticeStudentsOfGrowthRecordConsumeEvent]成长档案不存在:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var activeGrowthRecord = activeGrowthRecordBucket.ActiveGrowthRecord;
            if (activeGrowthRecord.SendType == EmActiveGrowthRecordSendType.No)
            {
                return;
            }

            var className = string.Empty;
            if (activeGrowthRecord.Type == EmActiveGrowthRecordType.Class)
            {
                var strClass = activeGrowthRecord.RelatedIds.Trim(',').Split(',');
                if (!string.IsNullOrEmpty(strClass[0]))
                {
                    var myClass = await _classDAL.GetClassBucket(strClass[0].ToLong());
                    className = myClass?.EtClass.Name;
                }
            }

            var growthRecordDetails = await _activeGrowthRecordDAL.GetGrowthRecordDetailView(request.GrowthRecordId);
            if (!growthRecordDetails.Any())
            {
                return;
            }

            var req = new GrowthRecordAddRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Students = new List<GrowthRecordAddStudent>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.GrowthRecordAdd;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            foreach (var myGrowthRecordDetail in growthRecordDetails)
            {
                var studentBucket = await _studentDAL.GetStudent(myGrowthRecordDetail.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    Log.Warn($"[NoticeStudentsOfGrowthRecordConsumeEvent]未找到学员信息,StudentId:{myGrowthRecordDetail.StudentId}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                if (string.IsNullOrEmpty(student.Phone))
                {
                    continue;
                }

                var url = string.Format(wxConfig.TemplateNoticeConfig.StudentGrowthRecordDetailUrl, myGrowthRecordDetail.Id, request.TenantId);
                req.Students.Add(new GrowthRecordAddStudent()
                {
                    ClassName = className,
                    Name = student.Name,
                    OpendId = await GetStudentOpenId(true, student.Phone),
                    Phone = student.Phone,
                    StudentId = student.Id,
                    Url = url
                });

                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new GrowthRecordAddStudent()
                    {
                        ClassName = className,
                        Name = student.Name,
                        OpendId = await GetStudentOpenId(true, student.PhoneBak),
                        Phone = student.PhoneBak,
                        StudentId = student.Id,
                        Url = url
                    });
                }
            }

            if (req.Students.Count > 0)
            {
                _wxService.GrowthRecordAdd(req);
            }
        }

        public async Task NoticeStudentsOfStudentEvaluateConsumeEvent(NoticeStudentsOfStudentEvaluateEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.TeacherClassEvaluateWeChat)
            {
                return;
            }
            var classRecordStudentLog = await _classRecordDAL.GetEtClassRecordStudentById(request.ClassRecordStudentId);

            var studentBucket = await _studentDAL.GetStudent(classRecordStudentLog.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeStudentsOfStudentEvaluateConsumeEvent]未找到学员信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }
            var tempBoxUser = new DataTempBox<EtUser>();
            var myCourse = await _courseDAL.GetCourse(classRecordStudentLog.CourseId);
            var courseName = string.Empty;
            if (myCourse != null && myCourse.Item1 != null)
            {
                courseName = myCourse.Item1.Name;
            }
            var req = new StudentEvaluateRequest(await GetNoticeRequestBase(request.TenantId))
            {
                TeacherName = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, classRecordStudentLog.Teachers),
                CourseName = courseName,
                Students = new List<StudentEvaluateItem>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentEvaluate;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            var url = string.Format(wxConfig.TemplateNoticeConfig.ClassRecordDetailFrontUrl, classRecordStudentLog.Id);
            req.Students.Add(new StudentEvaluateItem()
            {
                Name = student.Name,
                OpendId = await GetStudentOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id,
                Url = url
            });

            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new StudentEvaluateItem()
                {
                    Name = student.Name,
                    OpendId = await GetStudentOpenId(true, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                    Url = url
                });
            }

            _wxService.StudentEvaluate(req);
        }

        public async Task NoticeStudentsOfHomeworkExDateConsumeEvent(NoticeStudentsOfHomeworkExDateEvent request)
        {
            var homeworkDetailTomorrowExDate = await _activeHomeworkDetailDAL.GetHomeworkDetailTomorrowExDate();
            if (!homeworkDetailTomorrowExDate.Any())
            {
                Log.Info($"[NoticeStudentsOfHomeworkExDateConsumeEvent]未发现需要提醒的未交作业的学员:{request.TenantId}", this.GetType());
            }
            var req = new HomeworkExpireRemindRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Students = new List<HomeworkExpireRemindStudent>()
            };

            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.HomeworkExpireRemind;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            var tempBoxClass = new DataTempBox<EtClass>();
            foreach (var myHomeWorkDetail in homeworkDetailTomorrowExDate)
            {
                var studentBucket = await _studentDAL.GetStudent(myHomeWorkDetail.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    Log.Warn($"[NoticeStudentsOfHomeworkExDateConsumeEvent]未找到学员信息,StudentId:{myHomeWorkDetail.StudentId}", this.GetType());
                    continue;
                }
                var student = studentBucket.Student;
                if (string.IsNullOrEmpty(student.Phone))
                {
                    continue;
                }
                if (myHomeWorkDetail.ExDate == null)
                {
                    continue;
                }
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, myHomeWorkDetail.ClassId);
                var className = myClass?.Name;

                var url = string.Format(wxConfig.TemplateNoticeConfig.StudentHomeworkDetailUrl, myHomeWorkDetail.Id, myHomeWorkDetail.AnswerStatus);
                var exDate = myHomeWorkDetail.ExDate.EtmsToString();
                req.Students.Add(new HomeworkExpireRemindStudent()
                {
                    Name = student.Name,
                    OpendId = await GetStudentOpenId(true, student.Phone),
                    Phone = student.Phone,
                    StudentId = student.Id,
                    Url = url,
                    ExDateDesc = exDate,
                    HomeworkTitle = myHomeWorkDetail.Title,
                    ClassName = className
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new HomeworkExpireRemindStudent()
                    {
                        Name = student.Name,
                        OpendId = await GetStudentOpenId(true, student.PhoneBak),
                        Phone = student.PhoneBak,
                        StudentId = student.Id,
                        Url = url,
                        ExDateDesc = exDate,
                        HomeworkTitle = myHomeWorkDetail.Title,
                        ClassName = className
                    });
                }
            }

            if (req.Students.Count > 0)
            {
                _wxService.HomeworkExpireRemind(req);
            }
        }

        public async Task NoticeStudentCourseSurplusConsumerEvent(NoticeStudentCourseSurplusEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.ClassRecordStudentChangeWeChat)
            {
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeStudentCourseSurplusConsumerEvent]未找到学员信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }

            var myCourse = await _courseDAL.GetCourse(request.CourseId);
            if (myCourse == null || myCourse.Item1 == null)
            {
                Log.Error($"[NoticeStudentCourseSurplusConsumerEvent]未找到课程信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var myStudentCourse = await _studentCourseDAL.GetStudentCourse(request.StudentId, request.CourseId);
            if (myStudentCourse == null || myStudentCourse.Count == 0)
            {
                Log.Error($"[NoticeStudentCourseSurplusConsumerEvent]未找到学员课程信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var myCourseName = myCourse.Item1.Name;
            var mySurplusQuantityDesc = ComBusiness.GetStudentCourseDesc(myStudentCourse);

            var req = new StudentCourseSurplusRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Students = new List<StudentCourseSurplusItem>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentCourseSurplus;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;
            var url = wxConfig.TemplateNoticeConfig.StudentCourseUrl;

            req.Students.Add(new StudentCourseSurplusItem()
            {
                Name = student.Name,
                OpendId = await GetStudentOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id,
                Url = url,
                CourseName = myCourseName,
                SurplusQuantityDesc = mySurplusQuantityDesc
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new StudentCourseSurplusItem()
                {
                    Name = student.Name,
                    OpendId = await GetStudentOpenId(true, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                    Url = url,
                    CourseName = myCourseName,
                    SurplusQuantityDesc = mySurplusQuantityDesc
                });
            }

            if (req.Students.Count > 0)
            {
                _wxService.StudentCourseSurplus(req);
            }
        }

        public async Task NoticeStudentsOfMakeupConsumerEvent(NoticeStudentsOfMakeupEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StartClassWeChat)
            {
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeStudentsOfMakeupConsumerEvent]未找到学员信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }

            var myCourse = await _courseDAL.GetCourse(request.CourseId);
            if (myCourse == null || myCourse.Item1 == null)
            {
                Log.Error($"[NoticeStudentsOfMakeupConsumerEvent]未找到课程信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var classTimes = await _classTimesDAL.GetClassTimes(request.ClassTimesId);
            if (classTimes == null)
            {
                Log.Error($"[NoticeStudentsOfMakeupConsumerEvent]未找到课次信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var tempBoxUser = new DataTempBox<EtUser>();
            var req = new StudentMakeupRequest(await GetNoticeRequestBase(request.TenantId))
            {
                Students = new List<StudentMakeupItem>(),
                CourseName = myCourse.Item1.Name,
                ClassOt = classTimes.ClassOt.EtmsToDateString(),
                ClassTime = $"{EtmsHelper.GetTimeDesc(classTimes.StartTime)}~{EtmsHelper.GetTimeDesc(classTimes.EndTime)}",
                TeacherDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, classTimes.Teachers)
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentMakeup;
            req.Url = string.Empty;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            req.Students.Add(new StudentMakeupItem()
            {
                Name = student.Name,
                OpendId = await GetStudentOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new StudentMakeupItem()
                {
                    Name = student.Name,
                    OpendId = await GetStudentOpenId(true, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id
                });
            }

            if (req.Students.Count > 0)
            {
                _wxService.StudentMakeup(req);
            }
        }

        public async Task NoticeStudentCourseNotEnoughConsumerEvent(NoticeStudentCourseNotEnoughEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat
                && !tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms)
            {
                return;
            }

            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeStudentCourseNotEnoughConsumerEvent]未找到学员信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }

            var myCourse = await _courseDAL.GetCourse(request.CourseId);
            if (myCourse == null || myCourse.Item1 == null)
            {
                Log.Error($"[NoticeStudentCourseNotEnoughConsumerEvent]未找到课程信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var myStudentCourses = await _studentCourseDAL.GetStudentCourse(request.StudentId, request.CourseId);
            if (!ComBusiness2.CheckStudentCourseNeedRemind(myStudentCourses, tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughCount,
                tenantConfig.StudentCourseRenewalConfig.LimitClassTimes, tenantConfig.StudentCourseRenewalConfig.LimitDay))
            {
                return;
            }

            var notEnoughDesc = string.Empty;
            var deClassTimes = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            if (deClassTimes != null && deClassTimes.SurplusQuantity <= tenantConfig.StudentCourseRenewalConfig.LimitClassTimes)
            {
                notEnoughDesc = $"{tenantConfig.StudentCourseRenewalConfig.LimitClassTimes}课时";
            }
            else
            {
                notEnoughDesc = $"{tenantConfig.StudentCourseRenewalConfig.LimitDay}天";
            }

            var req = new NoticeStudentCourseNotEnoughRequest(await GetNoticeRequestBase(request.TenantId, tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat))
            {
                CourseName = myCourse.Item1.Name,
                NotEnoughDesc = notEnoughDesc,
                Students = new List<NoticeStudentCourseNotEnoughStudent>()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentCourseNotEnough;
            req.Url = wxConfig.TemplateNoticeConfig.StudentCourseUrl;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            req.Students.Add(new NoticeStudentCourseNotEnoughStudent()
            {
                StudentName = student.Name,
                OpendId = await GetStudentOpenId(tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentCourseNotEnoughStudent()
                {
                    StudentName = student.Name,
                    OpendId = await GetStudentOpenId(tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id
                });
            }

            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat)
                {
                    _wxService.NoticeStudentCourseNotEnough(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms)
                {
                    await _smsService.NoticeStudentCourseNotEnough(req);
                }
            }

            await _studentCourseDAL.UpdateStudentCourseNotEnoughRemindInfo(request.StudentId, request.CourseId);
        }
    }
}
