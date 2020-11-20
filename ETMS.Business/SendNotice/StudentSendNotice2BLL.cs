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

        public StudentSendNotice2BLL(IStudentWechatDAL studentWechatDAL, IComponentAccessBLL componentAccessBLL, ISysTenantDAL sysTenantDAL,
            IWxService wxService, IAppConfigurtaionServices appConfigurtaionServices, IUserDAL userDAL, ITenantConfigDAL tenantConfigDAL,
            IActiveHomeworkDetailDAL activeHomeworkDetailDAL, IStudentDAL studentDAL, IActiveGrowthRecordDAL activeGrowthRecordDAL,
            IClassDAL classDAL, IClassRecordDAL classRecordDAL, ICourseDAL courseDAL)
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
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentWechatDAL, _userDAL, _tenantConfigDAL, _activeHomeworkDetailDAL,
                _studentDAL, _activeGrowthRecordDAL, _classDAL, _classRecordDAL, _courseDAL);
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
    }
}
