﻿using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using Microsoft.AspNetCore.Http;
using ETMS.Entity.Config;
using ETMS.LOG;
using Newtonsoft.Json;

namespace ETMS.Business
{
    public class ParentDataBLL : IParentDataBLL
    {
        private readonly IStudentLeaveApplyLogDAL _studentLeaveApplyLogDAL;

        private readonly IParentStudentDAL _parentStudentDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IClassRoomDAL _classRoomDAL;

        private readonly IUserDAL _userDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IGiftCategoryDAL _giftCategoryDAL;

        private readonly IGiftDAL _giftDAL;

        private readonly IActiveHomeworkDAL _activeHomeworkDAL;

        private readonly IActiveHomeworkDetailDAL _activeHomeworkDetailDAL;

        private readonly IStudentWechatDAL _studentWechatDAL;

        public ParentDataBLL(IStudentLeaveApplyLogDAL studentLeaveApplyLogDAL, IParentStudentDAL parentStudentDAL, IStudentDAL studentDAL,
            IStudentOperationLogDAL studentOperationLogDAL, IClassTimesDAL classTimesDAL, IClassRoomDAL classRoomDAL, IUserDAL userDAL,
            ICourseDAL courseDAL, IClassDAL classDAL, ITenantConfigDAL tenantConfigDAL, IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IGiftCategoryDAL giftCategoryDAL, IGiftDAL giftDAL, IActiveHomeworkDAL activeHomeworkDAL, IActiveHomeworkDetailDAL activeHomeworkDetailDAL,
           IStudentWechatDAL studentWechatDAL)
        {
            this._studentLeaveApplyLogDAL = studentLeaveApplyLogDAL;
            this._parentStudentDAL = parentStudentDAL;
            this._studentDAL = studentDAL;
            this._studentOperationLogDAL = studentOperationLogDAL;
            this._classTimesDAL = classTimesDAL;
            this._classRoomDAL = classRoomDAL;
            this._userDAL = userDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._giftCategoryDAL = giftCategoryDAL;
            this._giftDAL = giftDAL;
            this._activeHomeworkDAL = activeHomeworkDAL;
            this._activeHomeworkDetailDAL = activeHomeworkDetailDAL;
            this._studentWechatDAL = studentWechatDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentLeaveApplyLogDAL, _parentStudentDAL, _studentDAL,
                _studentOperationLogDAL, _classTimesDAL, _classRoomDAL, _userDAL, _courseDAL, _classDAL,
                _tenantConfigDAL, _giftCategoryDAL, _giftDAL, _activeHomeworkDAL, _activeHomeworkDetailDAL, _studentWechatDAL);
        }

        public async Task<ResponseBase> StudentLeaveApplyGet(StudentLeaveApplyGetRequest request)
        {
            var pagingData = await _studentLeaveApplyLogDAL.GetPaging(request);
            var output = new List<StudentLeaveApplyGetOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new StudentLeaveApplyGetOutput()
                {
                    TitleDesc = $"{p.StudentName}的请假",
                    ApplyOt = p.ApplyOt,
                    StartDate = p.StartDate.EtmsToDateString(),
                    StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                    EndDate = p.EndDate.EtmsToDateString(),
                    EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                    HandleStatus = p.HandleStatus,
                    LeaveContent = p.LeaveContent,
                    HandleStatusDesc = EmStudentLeaveApplyHandleStatus.GetStudentLeaveApplyHandleStatusDescParent(p.HandleStatus),
                    Id = p.Id
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentLeaveApplyGetOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentListGet(StudentListGetRequest request)
        {
            var myStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
            var output = new List<StudentListGetOutput>();
            foreach (var p in myStudents)
            {
                output.Add(new StudentListGetOutput()
                {
                    Name = p.Name,
                    StudentId = p.Id,
                    Gender = p.Gender,
                    AvatarKey = p.Avatar,
                    AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p.Avatar),
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentLeaveApplyDetailGet(StudentLeaveApplyDetailGetRequest request)
        {
            var p = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyLog(request.Id);
            var student = await _studentDAL.GetStudent(p.StudentId);
            return ResponseBase.Success(new StudentLeaveApplyDetailGetOutput()
            {
                TitleDesc = $"{student.Student.Name}的请假",
                ApplyOt = p.ApplyOt,
                StartDate = p.StartDate.EtmsToDateString(),
                StartTime = EtmsHelper.GetTimeDesc(p.StartTime),
                EndDate = p.EndDate.EtmsToDateString(),
                EndTime = EtmsHelper.GetTimeDesc(p.EndTime),
                HandleStatus = p.HandleStatus,
                LeaveContent = p.LeaveContent,
                HandleStatusDesc = EmStudentLeaveApplyHandleStatus.GetStudentLeaveApplyHandleStatusDescParent(p.HandleStatus),
                Id = p.Id,
                HandleOt = p.HandleOt.EtmsToString()
            });
        }

        public async Task<ResponseBase> StudentLeaveApplyRevoke(StudentLeaveApplyRevokeRequest request)
        {
            var p = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyLog(request.Id);
            if (p.HandleStatus != EmStudentLeaveApplyHandleStatus.Unreviewed)
            {
                return ResponseBase.CommonError("无法撤销");
            }
            p.HandleStatus = EmStudentLeaveApplyHandleStatus.IsRevoke;
            await _studentLeaveApplyLogDAL.EditStudentLeaveApplyLog(p);
            await _studentOperationLogDAL.AddStudentLog(p.StudentId, request.LoginTenantId, $"撤销请假申请", EmStudentOperationLogType.StudentLeaveApply);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentClassTimetableGet(StudentClassTimetableRequest request)
        {
            var classTimesData = (await _classTimesDAL.GetList(request)).OrderBy(p => p.ClassOt).ThenBy(p => p.StartTime);
            return ResponseBase.Success(await GetStudentClassTimetableOutput(request, classTimesData));
        }

        public async Task<ResponseBase> StudentClassTimetableDetailGet(StudentClassTimetableDetailGetRequest request)
        {
            var classTimes = await _classTimesDAL.GetClassTimes(request.Id);
            var output = await GetStudentClassTimetableOutput(request, new List<EtClassTimes>() { classTimes });
            return ResponseBase.Success(output.First());
        }

        private async Task<List<StudentClassTimetableOutput>> GetStudentClassTimetableOutput(ParentRequestBase request, IEnumerable<EtClassTimes> classTimesData)
        {
            var output = new List<StudentClassTimetableOutput>();
            if (!classTimesData.Any())
            {
                return output;
            }
            var myStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, request.LoginPhone);
            var myStudentCount = myStudents.Count();
            var allClassRoom = await _classRoomDAL.GetAllClassRoom();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var classTimes in classTimesData)
            {
                var classRoomIdsDesc = string.Empty;
                var courseListDesc = string.Empty;
                var courseStyleColor = string.Empty;
                var className = string.Empty;
                var teachersDesc = string.Empty;
                var etClass = await _classDAL.GetClassBucket(classTimes.ClassId);
                var courseInfo = await ComBusiness.GetCourseNameAndColor(tempBoxCourse, _courseDAL, classTimes.CourseList);
                classRoomIdsDesc = ComBusiness.GetDesc(allClassRoom, classTimes.ClassRoomIds);
                className = etClass.EtClass.Name;
                courseListDesc = courseInfo.Item1;
                courseStyleColor = courseInfo.Item2;
                teachersDesc = await ComBusiness.GetParentTeachers(tempBoxUser, _userDAL, classTimes.Teachers);
                var studentName = string.Empty;
                if (myStudentCount == 1)
                {
                    studentName = myStudents.First().Name;
                }
                else
                {
                    var allClassTimesStudent = $"{classTimes.StudentIdsClass}{classTimes.StudentIdsTemp}";
                    var tempStudent = new StringBuilder();
                    foreach (var p in myStudents)
                    {
                        if (allClassTimesStudent.IndexOf($",{p.Id},") != -1)
                        {
                            tempStudent.Append($"{p.Name},");
                        }
                    }
                    studentName = tempStudent.ToString().TrimEnd(',');
                }
                output.Add(new StudentClassTimetableOutput()
                {
                    Id = classTimes.Id,
                    ClassId = classTimes.ClassId,
                    ClassName = className,
                    ClassOt = classTimes.ClassOt.EtmsToDateString(),
                    ClassRoomIds = classTimes.ClassRoomIds,
                    ClassRoomIdsDesc = classRoomIdsDesc,
                    CourseList = classTimes.CourseList,
                    CourseListDesc = courseListDesc,
                    CourseStyleColor = courseStyleColor,
                    EndTime = EtmsHelper.GetTimeDesc(classTimes.EndTime),
                    StartTime = EtmsHelper.GetTimeDesc(classTimes.StartTime),
                    Status = classTimes.Status,
                    Week = classTimes.Week,
                    WeekDesc = $"星期{EtmsHelper.GetWeekDesc(classTimes.Week)}",
                    Teachers = classTimes.Teachers,
                    TeachersDesc = teachersDesc,
                    StudentName = studentName,
                    ClassOtShort = classTimes.ClassOt.EtmsToDateShortString(),
                    ClassContent = classTimes.ClassContent
                });
            }
            return output;
        }

        public async Task<ResponseBase> IndexBannerGet(IndexBannerGetRequest request)
        {
            var config = await _tenantConfigDAL.GetTenantConfig();
            var outPut = new List<IndexBannerGetOutput>();
            if (config.ParentSetConfig.ParentBanners.Any())
            {
                foreach (var p in config.ParentSetConfig.ParentBanners)
                {
                    if (string.IsNullOrEmpty(p.ImgKey))
                    {
                        continue;
                    }
                    outPut.Add(new IndexBannerGetOutput()
                    {
                        ImgUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p.ImgKey),
                        LinkUrl = p.UrlKey
                    });
                }
            }
            return ResponseBase.Success(outPut);
        }

        public async Task<ResponseBase> HomeworkUnansweredGetPaging(HomeworkUnansweredGetPagingRequest request)
        {
            var pagingData = await _activeHomeworkDetailDAL.GetPaging(request);
            var output = new List<HomeworkUnansweredGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                var overdueStatusOutput = EmOverdueStatusOutput.GetOverdueStatusOutput(p.ExDate, DateTime.Now);
                output.Add(new HomeworkUnansweredGetPagingOutput()
                {
                    ClassId = p.ClassId,
                    ClassName = myClass?.Name,
                    TeacherName = await ComBusiness.GetParentTeacher(tempBoxUser, _userDAL, p.CreateUserId),
                    ExDateDesc = p.ExDate == null ? string.Empty : p.ExDate.EtmsToMinuteString(),
                    OtDesc = p.Ot.EtmsToMinuteString(),
                    Title = p.Title,
                    Type = p.Type,
                    TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                    AnswerStatus = p.AnswerStatus,
                    ReadStatus = p.ReadStatus,
                    StudentId = p.StudentId,
                    HomeworkDetailId = p.Id,
                    HomeworkId = p.HomeworkId,
                    StudentName = student.Name,
                    AnswerStatusDesc = EmActiveHomeworkDetailAnswerStatus.ActiveHomeworkDetailAnswerStatusDesc(p.AnswerStatus),
                    OverdueStatus = overdueStatusOutput.Item1,
                    OverdueStatusDesc = overdueStatusOutput.Item2
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<HomeworkUnansweredGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> HomeworkAnsweredGetPaging(HomeworkAnsweredGetPagingRequest request)
        {
            var pagingData = await _activeHomeworkDetailDAL.GetPaging(request);
            var output = new List<HomeworkAnsweredGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                output.Add(new HomeworkAnsweredGetPagingOutput()
                {
                    ClassId = p.ClassId,
                    ClassName = myClass?.Name,
                    TeacherName = await ComBusiness.GetParentTeacher(tempBoxUser, _userDAL, p.CreateUserId),
                    OtDesc = p.Ot.EtmsToMinuteString(),
                    Title = p.Title,
                    Type = p.Type,
                    TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                    AnswerStatus = p.AnswerStatus,
                    StudentId = p.StudentId,
                    HomeworkDetailId = p.Id,
                    HomeworkId = p.HomeworkId,
                    StudentName = student.Name,
                    AnswerStatusDesc = EmActiveHomeworkDetailAnswerStatus.ActiveHomeworkDetailAnswerStatusDesc(p.AnswerStatus),
                    AnswerOtDesc = p.AnswerOt.EtmsToMinuteString()
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<HomeworkAnsweredGetPagingOutput>(pagingData.Item2, output));
        }

        private List<string> GetMediasUrl(string workMedias)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(workMedias))
            {
                return result;
            }
            var myMedias = workMedias.Split('|');
            foreach (var p in myMedias)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    result.Add(UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, p));
                }
            }
            return result;
        }

        public async Task<ResponseBase> HomeworkDetailGet(HomeworkDetailGetRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var p = homeworkDetailBucket.ActiveHomeworkDetail;
            var classInfo = await _classDAL.GetClassBucket(p.ClassId);
            var tempBoxUser = new DataTempBox<EtUser>();
            var teacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.CreateUserId);
            var output = new HomeworkDetailGetOutput()
            {
                ClassId = p.ClassId,
                ClassName = classInfo?.EtClass.Name,
                ExDateDesc = p.ExDate == null ? string.Empty : p.ExDate.EtmsToMinuteString(),
                HomeworkDetailId = p.Id,
                HomeworkId = p.HomeworkId,
                OtDesc = p.Ot.EtmsToMinuteString(),
                ReadStatus = p.ReadStatus,
                TeacherName = ComBusiness2.GetParentTeacherName(teacher),
                TeacherAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, teacher.Avatar),
                Title = p.Title,
                Type = p.Type,
                TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                WorkContent = p.WorkContent,
                WorkMediasUrl = GetMediasUrl(p.WorkMedias),
                AnswerStatus = p.AnswerStatus,
                AnswerStatusDesc = EmActiveHomeworkDetailAnswerStatus.ActiveHomeworkDetailAnswerStatusDesc(p.AnswerStatus),
                AnswerInfo = null,
                CommentOutputs = await GetCommentOutput(homeworkDetailBucket.ActiveHomeworkDetailComments, tempBoxUser)
            };
            if (p.AnswerStatus == EmActiveHomeworkDetailAnswerStatus.Answered)
            {
                output.AnswerInfo = new HomeworkDetailAnswerInfo()
                {
                    AnswerContent = p.AnswerContent,
                    AnswerMediasUrl = GetMediasUrl(p.AnswerMedias),
                    AnswerOtDesc = p.AnswerOt.EtmsToMinuteString()
                };
            }
            return ResponseBase.Success(output);
        }

        private async Task<List<ParentCommentOutput>> GetCommentOutput(List<EtActiveHomeworkDetailComment> activeHomeworkDetailComments, DataTempBox<EtUser> tempBoxUser)
        {
            var commentOutputs = new List<ParentCommentOutput>();
            if (activeHomeworkDetailComments != null || activeHomeworkDetailComments.Count > 0)
            {
                var first = activeHomeworkDetailComments.Where(j => j.ReplyId == null).OrderBy(j => j.Ot);
                foreach (var myFirstComment in first)
                {
                    var firstrelatedManName = string.Empty;
                    var firstrelatedManAvatar = string.Empty;
                    if (myFirstComment.SourceType == EmActiveCommentSourceType.User)
                    {
                        var myRelatedUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, myFirstComment.SourceId);
                        if (myRelatedUser != null)
                        {
                            firstrelatedManName = ComBusiness2.GetParentTeacherName(myRelatedUser);
                            firstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                        }
                    }
                    else
                    {
                        firstrelatedManName = myFirstComment.Nickname;
                        firstrelatedManAvatar = myFirstComment.Headimgurl;
                    }
                    commentOutputs.Add(new ParentCommentOutput()
                    {
                        CommentContent = myFirstComment.CommentContent,
                        CommentId = myFirstComment.Id,
                        Ot = myFirstComment.Ot.EtmsToMinuteString(),
                        ReplyId = myFirstComment.ReplyId,
                        SourceType = myFirstComment.SourceType,
                        RelatedManAvatar = firstrelatedManAvatar,
                        RelatedManName = firstrelatedManName,
                        OtDesc = EtmsHelper.GetOtFriendlyDesc(myFirstComment.Ot)
                    });
                    var second = activeHomeworkDetailComments.Where(p => p.ReplyId == myFirstComment.Id).OrderBy(j => j.Ot);
                    foreach (var mySecondComment in second)
                    {
                        var secondfirstrelatedManName = string.Empty;
                        var secondfirstrelatedManAvatar = string.Empty;
                        if (mySecondComment.SourceType == EmActiveCommentSourceType.User)
                        {
                            var myRelatedUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, mySecondComment.SourceId);
                            if (myRelatedUser != null)
                            {
                                secondfirstrelatedManName = ComBusiness2.GetParentTeacherName(myRelatedUser);
                                secondfirstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                            }
                        }
                        else
                        {
                            secondfirstrelatedManName = mySecondComment.Nickname;
                            secondfirstrelatedManAvatar = mySecondComment.Headimgurl;
                        }
                        commentOutputs.Add(new ParentCommentOutput()
                        {
                            CommentContent = mySecondComment.CommentContent,
                            CommentId = mySecondComment.Id,
                            Ot = mySecondComment.Ot.EtmsToMinuteString(),
                            OtDesc = EtmsHelper.GetOtFriendlyDesc(mySecondComment.Ot),
                            RelatedManAvatar = secondfirstrelatedManAvatar,
                            RelatedManName = secondfirstrelatedManName,
                            ReplyId = mySecondComment.ReplyId,
                            SourceType = mySecondComment.SourceType,
                            ReplyRelatedManName = firstrelatedManName
                        });
                    }
                }
            }
            return commentOutputs;
        }

        public async Task<ResponseBase> HomeworkDetailSetRead(HomeworkDetailSetReadRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            if (homeworkDetailBucket.ActiveHomeworkDetail.ReadStatus == EmActiveHomeworkDetailReadStatus.Yes)
            {
                Log.Warn($"[HomeworkDetailSetRead]重复提交设置作业已读请求:HomeworkDetailId:{JsonConvert.SerializeObject(request)}", this.GetType());
                return ResponseBase.Success();
            }
            var activeHomeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            activeHomeworkDetail.ReadStatus = EmActiveHomeworkDetailReadStatus.Yes;
            await _activeHomeworkDetailDAL.EditActiveHomeworkDetail(activeHomeworkDetail);

            var activeHomework = await _activeHomeworkDAL.GetActiveHomework(activeHomeworkDetail.HomeworkId);
            activeHomework.ReadCount += 1;
            await _activeHomeworkDAL.EditActiveHomework(activeHomework);

            await _studentOperationLogDAL.AddStudentLog(activeHomeworkDetail.StudentId, request.LoginTenantId, $"阅读课后作业：{activeHomeworkDetail.Title}", EmStudentOperationLogType.Homework);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HomeworkSubmitAnswer(HomeworkSubmitAnswerRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            if (homeworkDetailBucket.ActiveHomeworkDetail.AnswerStatus == EmActiveHomeworkDetailAnswerStatus.Answered)
            {
                Log.Warn($"[HomeworkSubmitAnswer]重复提交作业:HomeworkDetailId:{JsonConvert.SerializeObject(request)}", this.GetType());
                return ResponseBase.CommonError("请勿重复提交作业");
            }
            var activeHomeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            activeHomeworkDetail.AnswerStatus = EmActiveHomeworkDetailAnswerStatus.Answered;
            activeHomeworkDetail.AnswerContent = request.AnswerContent;
            activeHomeworkDetail.AnswerMedias = string.Empty;
            if (request.AnswerMediasKeys != null && request.AnswerMediasKeys.Count > 0)
            {
                activeHomeworkDetail.AnswerMedias = string.Join('|', request.AnswerMediasKeys);
            }
            activeHomeworkDetail.AnswerOt = DateTime.Now;
            await _activeHomeworkDetailDAL.EditActiveHomeworkDetail(activeHomeworkDetail);

            var activeHomework = await _activeHomeworkDAL.GetActiveHomework(activeHomeworkDetail.HomeworkId);
            activeHomework.FinishCount += 1;
            await _activeHomeworkDAL.EditActiveHomework(activeHomework);

            await _studentOperationLogDAL.AddStudentLog(activeHomeworkDetail.StudentId, request.LoginTenantId, $"提交课后作业：{activeHomeworkDetail.Title}", EmStudentOperationLogType.Homework);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HomeworkAddComment(HomeworkAddCommentRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var activeHomeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            var myStudentWechat = await _studentWechatDAL.GetStudentWechatByPhone(request.LoginPhone);
            var comment = new EtActiveHomeworkDetailComment()
            {
                CommentContent = request.CommentContent,
                Headimgurl = myStudentWechat?.Headimgurl,
                Nickname = myStudentWechat?.Nickname,
                HomeworkDetailId = request.HomeworkDetailId,
                HomeworkId = activeHomeworkDetail.HomeworkId,
                IsDeleted = EmIsDeleted.Normal,
                Ot = DateTime.Now,
                ReplyId = null,
                SourceId = activeHomeworkDetail.StudentId,
                SourceType = EmActiveCommentSourceType.Student,
                TenantId = request.LoginTenantId
            };
            await _activeHomeworkDetailDAL.AddActiveHomeworkDetailComment(comment);

            await _studentOperationLogDAL.AddStudentLog(activeHomeworkDetail.StudentId, request.LoginTenantId, $"评论课后作业：{activeHomeworkDetail.Title}", EmStudentOperationLogType.Homework);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HomeworkDelComment(HomeworkDelCommentRequest request)
        {
            var homeworkDetailBucket = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(request.HomeworkDetailId);
            if (homeworkDetailBucket == null || homeworkDetailBucket.ActiveHomeworkDetail == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var activeHomeworkDetail = homeworkDetailBucket.ActiveHomeworkDetail;
            await _activeHomeworkDetailDAL.DelActiveHomeworkDetailComment(request.HomeworkDetailId, request.CommentId);
            await _studentOperationLogDAL.AddStudentLog(activeHomeworkDetail.StudentId, request.LoginTenantId, "删除课后作业评论", EmStudentOperationLogType.Homework);
            return ResponseBase.Success();
        }
    }
}
