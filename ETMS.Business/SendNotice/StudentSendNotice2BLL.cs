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
using ETMS.Entity.Enum.EtmsManage;
using ETMS.IBusiness.EventConsumer;

namespace ETMS.Business.SendNotice
{
    public class StudentSendNotice2BLL : SendStudentNoticeBase, IStudentSendNotice2BLL
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

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly ISysSmsTemplate2BLL _sysSmsTemplate2BLL;

        public StudentSendNotice2BLL(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL,
            IWxService wxService, IAppConfigurtaionServices appConfigurtaionServices, IUserDAL userDAL, ITenantConfigDAL tenantConfigDAL,
            IActiveHomeworkDetailDAL activeHomeworkDetailDAL, IStudentDAL studentDAL, IActiveGrowthRecordDAL activeGrowthRecordDAL,
            IClassDAL classDAL, IClassRecordDAL classRecordDAL, ICourseDAL courseDAL, IStudentCourseDAL studentCourseDAL,
            IClassTimesDAL classTimesDAL, ISmsService smsService, IStudentCheckOnLogDAL studentCheckOnLogDAL, ISysSmsTemplate2BLL sysSmsTemplate2BLL
            , ITenantLibBLL tenantLibBLL)
            : base(studentWechatDAL, componentAccessBLL, sysTenantDAL, tenantLibBLL)
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
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._sysSmsTemplate2BLL = sysSmsTemplate2BLL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantLibBLL.InitTenantId(tenantId);
            this._sysSmsTemplate2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentWechatDAL, _userDAL, _tenantConfigDAL, _activeHomeworkDetailDAL,
                _studentDAL, _activeGrowthRecordDAL, _classDAL, _classRecordDAL, _courseDAL, _studentCourseDAL, _classTimesDAL,
                _studentCheckOnLogDAL);
        }

        public async Task NoticeStudentsOfHomeworkAddConsumeEvent(NoticeStudentsOfHomeworkAddEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentHomeworkWeChat)
            {
                return;
            }
            var date = DateTime.Now.Date;
            var homeworkDetails = await _activeHomeworkDetailDAL.GetActiveHomeworkDetail(request.HomeworkId, date);
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

            await this.InitNoticeConfig(EmNoticeConfigScenesType.StudentHomework);
            foreach (var myHomeWorkDetail in homeworkDetails)
            {
                if (this.CheckLimitNoticeClass(myHomeWorkDetail.ClassId))
                {
                    continue;
                }
                if (this.CheckLimitNoticeStudent(myHomeWorkDetail.StudentId))
                {
                    continue;
                }
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
                    OpendId = await GetOpenId(true, student.Phone),
                    Phone = student.Phone,
                    StudentId = student.Id,
                    Url = url
                });
                if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
                {
                    req.Students.Add(new HomeworkAddStudent()
                    {
                        Name = student.Name,
                        OpendId = await GetOpenId(true, student.PhoneBak),
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
            await this.InitNoticeConfig(EmNoticeConfigScenesType.StudentHomeworkComment);
            if (this.CheckLimitNoticeClass(homeworkDetail.ClassId))
            {
                return;
            }
            if (this.CheckLimitNoticeStudent(homeworkDetail.StudentId))
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
                OpendId = await GetOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id,
                Url = url
            });

            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new HomeworkCommentStudent()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, student.PhoneBak),
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
            await this.InitNoticeConfig(EmNoticeConfigScenesType.StudentGrowUpRecord);
            if (activeGrowthRecord.Type == EmActiveGrowthRecordType.Class)
            {
                var strClass = activeGrowthRecord.RelatedIds.Trim(',').Split(',');
                if (!string.IsNullOrEmpty(strClass[0]))
                {
                    var classId = strClass[0].ToLong();
                    if (this.CheckLimitNoticeClass(classId))
                    {
                        return;
                    }
                    var myClass = await _classDAL.GetClassBucket(classId);
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
                if (this.CheckLimitNoticeStudent(myGrowthRecordDetail.StudentId))
                {
                    continue;
                }
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
                    OpendId = await GetOpenId(true, student.Phone),
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
                        OpendId = await GetOpenId(true, student.PhoneBak),
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

            await this.InitNoticeConfig(EmNoticeConfigScenesType.TeacherClassEvaluate);
            if (this.CheckLimitNoticeClass(classRecordStudentLog.ClassId))
            {
                return;
            }
            if (this.CheckLimitNoticeStudent(classRecordStudentLog.StudentId))
            {
                return;
            }
            if (this.CheckLimitNoticeCourse(classRecordStudentLog.CourseId))
            {
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
                OpendId = await GetOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id,
                Url = url
            });

            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new StudentEvaluateItem()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                    Url = url
                });
            }

            _wxService.StudentEvaluate(req);
        }

        public async Task NoticeStudentsOfHomeworkExDateConsumeEvent(NoticeStudentsOfHomeworkExDateEvent request)
        {
            var homeworkDetailTomorrowExDate = await _activeHomeworkDetailDAL.GetHomeworkDetailTomorrowSingleWorkExDate();
            await NoticeStudentsHomeworkExpireRemind(homeworkDetailTomorrowExDate, request);
        }

        public async Task NoticeStudentsOfHomeworkNotAnswerConsumeEvent(NoticeStudentsOfHomeworkNotAnswerEvent request)
        {
            var hourAndMinute = EtmsHelper.GetTimeHourAndMinuteDesc(request.MyNow);
            var minValue = hourAndMinute - 10;  //10分钟的时间误差
            var maxValue = hourAndMinute + 10;
            var homeworkDetail = await _activeHomeworkDetailDAL.GetHomeworkDetailContinuousWorkTodayNotAnswer(request.MyNow.Date, maxValue, minValue);
            await NoticeStudentsHomeworkExpireRemind(homeworkDetail, request);
        }

        private async Task NoticeStudentsHomeworkExpireRemind(IEnumerable<EtActiveHomeworkDetail> homeworkDetailTomorrowExDate,
            ETMS.Event.DataContract.Event request)
        {
            if (homeworkDetailTomorrowExDate == null || !homeworkDetailTomorrowExDate.Any())
            {
                Log.Info($"[NoticeStudentsOfHomeworkExDateConsumeEvent]未发现需要提醒的未交作业的学员:{request.TenantId}", this.GetType());
                return;
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

            await this.InitNoticeConfig(EmNoticeConfigScenesType.StudentHomework);
            var tempBoxClass = new DataTempBox<EtClass>();
            foreach (var myHomeWorkDetail in homeworkDetailTomorrowExDate)
            {
                if (this.CheckLimitNoticeClass(myHomeWorkDetail.ClassId))
                {
                    continue;
                }
                if (this.CheckLimitNoticeStudent(myHomeWorkDetail.StudentId))
                {
                    continue;
                }
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
                var exDate = string.Empty;
                if (myHomeWorkDetail.ExDate == null)
                {
                    if (myHomeWorkDetail.Type == EmActiveHomeworkType.SingleWork)
                    {
                        continue;
                    }
                }
                else
                {
                    exDate = myHomeWorkDetail.ExDate.EtmsToString();
                }
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, myHomeWorkDetail.ClassId);
                var className = myClass?.Name;

                var url = string.Format(wxConfig.TemplateNoticeConfig.StudentHomeworkDetailUrl, myHomeWorkDetail.Id, myHomeWorkDetail.AnswerStatus);
                req.Students.Add(new HomeworkExpireRemindStudent()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, student.Phone),
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
                        OpendId = await GetOpenId(true, student.PhoneBak),
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
            if (!tenantConfig.StudentNoticeConfig.StudentCourseSurplusChangedWeChat &&
                !tenantConfig.StudentNoticeConfig.StudentCourseSurplusChangedSms)
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

            await this.InitNoticeConfig(EmNoticeConfigScenesType.StudentCourseSurplusChanged);
            if (this.CheckLimitNoticeStudent(request.StudentId))
            {
                return;
            }
            if (this.CheckLimitNoticeCourse(request.CourseId))
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
                Log.Fatal($"[NoticeStudentCourseSurplusConsumerEvent]未找到学员课程信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var myCourseName = myCourse.Item1.Name;
            var mySurplusQuantityDesc = ComBusiness.GetStudentCourseDesc(myStudentCourse);

            var myStudentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId, request.CourseId);
            var expireDateDesc = ComBusiness.GetStudentCourseExpireDateDesc(myStudentCourseDetail);

            var req = new StudentCourseSurplusRequest(await GetNoticeRequestBase(request.TenantId,
                tenantConfig.StudentNoticeConfig.StudentCourseSurplusChangedWeChat))
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
                OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCourseSurplusChangedWeChat, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id,
                Url = url,
                CourseName = myCourseName,
                SurplusQuantityDesc = mySurplusQuantityDesc,
                ExTimeDesc = expireDateDesc
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new StudentCourseSurplusItem()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCourseSurplusChangedWeChat, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                    Url = url,
                    CourseName = myCourseName,
                    SurplusQuantityDesc = mySurplusQuantityDesc,
                    ExTimeDesc = expireDateDesc
                });
            }

            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentCourseSurplusChangedSms)
                {
                    req.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.StudentCourseSurplus);
                    await _smsService.StudentCourseSurplus(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentCourseSurplusChangedWeChat)
                {
                    _wxService.StudentCourseSurplus(req);
                }
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

            await this.InitNoticeConfig(EmNoticeConfigScenesType.StartClass);
            if (this.CheckLimitNoticeClass(classTimes.ClassId))
            {
                return;
            }
            if (this.CheckLimitNoticeCourse(request.CourseId))
            {
                return;
            }
            if (this.CheckLimitNoticeStudent(request.StudentId))
            {
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
                OpendId = await GetOpenId(true, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new StudentMakeupItem()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(true, student.PhoneBak),
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
            if (!request.IsOwnTrigger && !tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat
                && !tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms)
            {
                return;
            }
            if (request.IsOwnTrigger)
            {
                await NoticeStudentCourseNotEnoughConsumerEvent2(request, tenantConfig);
            }
            else
            {
                await NoticeStudentCourseNotEnoughConsumerEvent1(request, tenantConfig);
            }
        }

        public async Task NoticeStudentCourseNotEnoughConsumerEvent1(NoticeStudentCourseNotEnoughEvent request, TenantConfig tenantConfig)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeStudentCourseNotEnoughConsumerEvent1]未找到学员信息:{JsonConvert.SerializeObject(request)}", this.GetType());
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
                Log.Error($"[NoticeStudentCourseNotEnoughConsumerEvent1]未找到课程信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            await this.InitNoticeConfig(EmNoticeConfigScenesType.StudentCourseNotEnough);
            if (this.CheckLimitNoticeStudent(request.StudentId))
            {
                return;
            }
            if (this.CheckLimitNoticeCourse(request.CourseId))
            {
                return;
            }
            if (await this.CheckLimitNoticeClassOfStudent(request.StudentId))
            {
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
            var deType = EmDeClassTimesType.ClassTimes;
            var expireDateDesc = string.Empty;
            var surplusDesc = string.Empty;
            var nowDate = DateTime.Now.Date;
            if (deClassTimes != null && deClassTimes.SurplusQuantity <= tenantConfig.StudentCourseRenewalConfig.LimitClassTimes)
            {
                notEnoughDesc = $"{tenantConfig.StudentCourseRenewalConfig.LimitClassTimes}课时";
                surplusDesc = $"{deClassTimes.SurplusQuantity.EtmsToString()}课时";
            }
            else
            {
                notEnoughDesc = $"{tenantConfig.StudentCourseRenewalConfig.LimitDay}天";
                deType = EmDeClassTimesType.Day;

                var myStudentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId, request.CourseId);
                var dayNotEnough = myStudentCourseDetail.FirstOrDefault(j => j.DeType == EmDeClassTimesType.Day
                && j.Status != EmStudentCourseStatus.EndOfClass && j.EndTime != null && j.EndTime >= nowDate);
                if (dayNotEnough != null)
                {
                    expireDateDesc = dayNotEnough.EndTime.EtmsToDateString();
                }
            }

            if (string.IsNullOrEmpty(surplusDesc))
            {
                surplusDesc = "0课时";
            }

            var req = new NoticeStudentCourseNotEnoughRequest(await GetNoticeRequestBase(request.TenantId, tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat))
            {
                CourseName = myCourse.Item1.Name,
                NotEnoughDesc = notEnoughDesc,
                Students = new List<NoticeStudentCourseNotEnoughStudent>(),
                DeType = deType,
                ExpireDateDesc = expireDateDesc,
                SurplusDesc = surplusDesc
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (deType == EmDeClassTimesType.ClassTimes)
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentCourseNotEnoughClassTimes;
            }
            else
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentCourseNotEnoughClassDay;
            }
            req.Url = wxConfig.TemplateNoticeConfig.StudentCourseUrl;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            req.Students.Add(new NoticeStudentCourseNotEnoughStudent()
            {
                StudentName = student.Name,
                OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentCourseNotEnoughStudent()
                {
                    StudentName = student.Name,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id
                });
            }

            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat)
                {
                    _wxService.NoticeStudentCourseNotEnough2(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms)
                {
                    req.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.StudentCourseNotEnough);
                    await _smsService.NoticeStudentCourseNotEnough(req);
                }
            }

            await _studentCourseDAL.UpdateStudentCourseNotEnoughRemindInfo(request.StudentId, request.CourseId);
        }

        /// <summary>
        /// 主动点击提醒
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tenantConfig"></param>
        /// <returns></returns>
        public async Task NoticeStudentCourseNotEnoughConsumerEvent2(NoticeStudentCourseNotEnoughEvent request, TenantConfig tenantConfig)
        {
            tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat = true;
            tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms = true;

            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeStudentCourseNotEnoughConsumerEvent2]未找到学员信息:{JsonConvert.SerializeObject(request)}", this.GetType());
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
                Log.Error($"[NoticeStudentCourseNotEnoughConsumerEvent2]未找到课程信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }

            var deType = EmDeClassTimesType.ClassTimes;
            var expireDateDesc = string.Empty;
            var surplusDesc = string.Empty;
            var nowDate = DateTime.Now.Date;

            var myStudentCourses = await _studentCourseDAL.GetStudentCourse(request.StudentId, request.CourseId);
            var myDeDayCourses = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day && (p.SurplusQuantity > 0 || p.SurplusSmallQuantity > 0));
            if (myDeDayCourses != null)
            {
                deType = EmDeClassTimesType.Day;
                var myStudentCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId, request.CourseId);
                var dayNotEnough = myStudentCourseDetail.FirstOrDefault(j => j.DeType == EmDeClassTimesType.Day
                && j.Status != EmStudentCourseStatus.EndOfClass && j.EndTime != null && j.EndTime >= nowDate);
                if (dayNotEnough != null)
                {
                    expireDateDesc = dayNotEnough.EndTime.EtmsToDateString();
                }
            }
            else
            {
                var deClassTimes = myStudentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
                if (deClassTimes != null)
                {
                    surplusDesc = $"{deClassTimes.SurplusQuantity.EtmsToString()}课时";
                }
            }
            if (string.IsNullOrEmpty(surplusDesc))
            {
                surplusDesc = "0课时";
            }

            var req = new NoticeStudentCourseNotEnoughRequest(await GetNoticeRequestBase(request.TenantId, tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat))
            {
                CourseName = myCourse.Item1.Name,
                NotEnoughDesc = string.Empty,
                Students = new List<NoticeStudentCourseNotEnoughStudent>(),
                DeType = deType,
                ExpireDateDesc = expireDateDesc,
                SurplusDesc = surplusDesc
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            if (deType == EmDeClassTimesType.ClassTimes)
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentCourseNotEnoughClassTimes;
            }
            else
            {
                req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentCourseNotEnoughClassDay;
            }
            req.Url = wxConfig.TemplateNoticeConfig.StudentCourseUrl;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            req.Students.Add(new NoticeStudentCourseNotEnoughStudent()
            {
                StudentName = student.Name,
                OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id
            });
            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentCourseNotEnoughStudent()
                {
                    StudentName = student.Name,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id
                });
            }

            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat)
                {
                    _wxService.NoticeStudentCourseNotEnough3(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms)
                {
                    req.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(request.TenantId, EmSysSmsTemplateType.StudentCourseNotEnough);
                    await _smsService.NoticeStudentCourseNotEnough(req);
                }
            }

            await _studentCourseDAL.UpdateStudentCourseNotEnoughRemindInfo(request.StudentId, request.CourseId);
        }

        public async Task NoticeStudentsCheckOnConsumerEvent(NoticeStudentsCheckOnEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat
                && !tenantConfig.StudentNoticeConfig.StudentCheckOnSms)
            {
                return;
            }

            var log = await _studentCheckOnLogDAL.GetStudentCheckOnLog(request.StudentCheckOnLogId);
            if (log == null)
            {
                Log.Error($"[NoticeStudentsCheckOnConsumerEvent]未找到考勤记录:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var studentBucket = await _studentDAL.GetStudent(log.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                Log.Error($"[NoticeStudentsCheckOnConsumerEvent]未找到学员信息:{JsonConvert.SerializeObject(request)}", this.GetType());
                return;
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.Phone))
            {
                return;
            }

            await this.InitNoticeConfig(EmNoticeConfigScenesType.StudentCheckOn);
            if (this.CheckLimitNoticeStudent(student.Id))
            {
                return;
            }
            if (await this.CheckLimitNoticeClassOfStudent(student.Id))
            {
                return;
            }

            if (log.CheckType == EmStudentCheckOnLogCheckType.CheckIn)
            {
                await NoticeStudentsCheckIn(request.TenantId, tenantConfig, log, student);
            }
            else
            {
                await NoticeStudentsCheckOut(request.TenantId, tenantConfig, log, student);
            }
        }

        private async Task NoticeStudentsCheckIn(int tenantId, TenantConfig tenantConfig, EtStudentCheckOnLog log, EtStudent student)
        {
            var req = new NoticeStudentCheckInRequest(await GetNoticeRequestBase(tenantId, tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat))
            {
                Students = new List<NoticeStudentCheckInStudent>(),
                CheckOtDesc = log.CheckOt.EtmsToMinuteString(),
                DeClassTimesDesc = string.Empty
            };
            if (log.Status == EmStudentCheckOnLogStatus.NormalAttendClass)
            {
                var myCourse = await _courseDAL.GetCourse(log.CourseId.Value);
                if (myCourse != null && myCourse.Item1 != null)
                {
                    var myStudentCourse = await _studentCourseDAL.GetStudentCourse(log.StudentId, log.CourseId.Value);
                    if (myStudentCourse != null && myStudentCourse.Count > 0)
                    {
                        var mySurplusQuantityDesc = ComBusiness.GetStudentCourseDesc(myStudentCourse);
                        req.DeClassTimesDesc = $"课程({myCourse.Item1.Name})，消耗{log.DeClassTimes.EtmsToString()}课时，剩余{mySurplusQuantityDesc}";
                    }
                }
            }
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentCheckIn;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            var url = string.Empty;
            if (log.CheckForm == EmStudentCheckOnLogCheckForm.Face)
            {
                url = string.Format(wxConfig.TemplateNoticeConfig.StudentCheckLogUrl, log.Id);
            }

            req.Students.Add(new NoticeStudentCheckInStudent()
            {
                Name = student.Name,
                OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id,
                Url = url,
                Points = log.Points
            });

            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentCheckInStudent()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                    Url = url,
                    Points = log.Points
                });
            }

            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat)
                {
                    _wxService.NoticeStudentCheckIn(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentCheckOnSms)
                {
                    req.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(tenantId, EmSysSmsTemplateType.StudentCheckOnLogCheckIn);
                    await _smsService.NoticeStudentCheckIn(req);
                }
            }
        }

        private async Task NoticeStudentsCheckOut(int tenantId, TenantConfig tenantConfig, EtStudentCheckOnLog log, EtStudent student)
        {
            var req = new NoticeStudentCheckOutRequest(await GetNoticeRequestBase(tenantId, tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat))
            {
                Students = new List<NoticeStudentCheckOutStudent>(),
                CheckOtDesc = log.CheckOt.EtmsToMinuteString()
            };
            var wxConfig = _appConfigurtaionServices.AppSettings.WxConfig;
            req.TemplateIdShort = wxConfig.TemplateNoticeConfig.StudentCheckOut;
            req.Remark = tenantConfig.StudentNoticeConfig.WeChatNoticeRemark;

            var url = string.Empty;
            if (log.CheckForm == EmStudentCheckOnLogCheckForm.Face)
            {
                url = string.Format(wxConfig.TemplateNoticeConfig.StudentCheckLogUrl, log.Id);
            }

            req.Students.Add(new NoticeStudentCheckOutStudent()
            {
                Name = student.Name,
                OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat, student.Phone),
                Phone = student.Phone,
                StudentId = student.Id,
                Url = url
            });

            if (!string.IsNullOrEmpty(student.PhoneBak) && EtmsHelper.IsMobilePhone(student.PhoneBak))
            {
                req.Students.Add(new NoticeStudentCheckOutStudent()
                {
                    Name = student.Name,
                    OpendId = await GetOpenId(tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat, student.PhoneBak),
                    Phone = student.PhoneBak,
                    StudentId = student.Id,
                    Url = url
                });
            }

            if (req.Students.Count > 0)
            {
                if (tenantConfig.StudentNoticeConfig.StudentCheckOnWeChat)
                {
                    _wxService.NoticeStudentCheckOut(req);
                }
                if (tenantConfig.StudentNoticeConfig.StudentCheckOnSms)
                {
                    req.SmsTemplate = await _sysSmsTemplate2BLL.GetSmsTemplate(tenantId, EmSysSmsTemplateType.StudentCheckOnLogCheckOut);
                    await _smsService.NoticeStudentCheckOut(req);
                }
            }
        }
    }
}
