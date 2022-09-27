using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Output;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.Entity.Dto.Common;

namespace ETMS.Business
{
    public class ActiveHomeworkBLL : IActiveHomeworkBLL
    {
        private readonly IActiveHomeworkDAL _activeHomeworkDAL;

        private readonly IActiveHomeworkDetailDAL _activeHomeworkDetailDAL;

        private readonly IUserDAL _userDAL;

        private readonly IClassDAL _classDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IEventPublisher _eventPublisher;

        public ActiveHomeworkBLL(IActiveHomeworkDAL activeHomeworkDAL, IActiveHomeworkDetailDAL activeHomeworkDetailDAL,
            IUserDAL userDAL, IClassDAL classDAL, IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IUserOperationLogDAL userOperationLogDAL, IStudentDAL studentDAL, IEventPublisher eventPublisher)
        {
            this._activeHomeworkDAL = activeHomeworkDAL;
            this._activeHomeworkDetailDAL = activeHomeworkDetailDAL;
            this._userDAL = userDAL;
            this._classDAL = classDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._userOperationLogDAL = userOperationLogDAL;
            this._studentDAL = studentDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _activeHomeworkDAL, _activeHomeworkDetailDAL, _userDAL, _classDAL, _userOperationLogDAL, _studentDAL);
        }

        public async Task<ResponseBase> ActiveHomeworkGetPaging(ActiveHomeworkGetPagingRequest request)
        {
            var pagingData = await _activeHomeworkDAL.GetPaging(request);
            var output = new List<ActiveHomeworkGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxClass = new DataTempBox<EtClass>();
            foreach (var p in pagingData.Item1)
            {
                var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                var teacher = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.CreateUserId);
                var exDateDesc = string.Empty;
                var lxExTimeDesc = string.Empty;
                if (p.Type == EmActiveHomeworkType.SingleWork)
                {
                    exDateDesc = p.ExDate == null ? "未设置" : p.ExDate.EtmsToMinuteString();
                }
                else
                {
                    exDateDesc = $"{p.LxStartDate.EtmsToDateString()}~{p.LxEndDate.EtmsToDateString()}";
                    if (p.LxExTime != null)
                    {
                        lxExTimeDesc = EtmsHelper.GetTimeDesc(p.LxExTime.Value);
                    }
                }
                output.Add(new ActiveHomeworkGetPagingOutput()
                {
                    HomeworkId = p.Id,
                    ClassId = p.ClassId,
                    ClassName = myClass?.Name,
                    TeacherName = teacher.Name,
                    TeacherAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, teacher.Avatar),
                    ExDateDesc = exDateDesc,
                    FinishCount = p.FinishCount,
                    OtDesc = p.Ot.EtmsToMinuteString(),
                    ReadCount = p.ReadCount,
                    Title = p.Title,
                    TotalCount = p.TotalCount,
                    Type = p.Type,
                    TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                    WorkContent = p.WorkContent,
                    WorkMediasUrl = GetMediasUrl(p.WorkMedias),
                    LxExTimeDesc = lxExTimeDesc
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActiveHomeworkGetPagingOutput>(pagingData.Item2, output));
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

        public async Task<ResponseBase> ActiveHomeworkAdd(ActiveHomeworkAddRequest request)
        {
            var now = DateTime.Now;
            var date = now.Date;
            var workMedias = string.Empty;
            if (request.WorkMediasKeys != null && request.WorkMediasKeys.Count > 0)
            {
                workMedias = string.Join('|', request.WorkMediasKeys);
            }
            var details = new List<EtActiveHomeworkDetail>();
            var homeworkIds = new List<long>();
            foreach (var p in request.ClassInfos)
            {
                if (p.StudentIds == null || p.StudentIds.Count == 0)
                {
                    var tempClassStudent = await _classDAL.GetClassBucket(p.ClassId);
                    if (tempClassStudent == null || tempClassStudent.EtClass == null)
                    {
                        LOG.Log.Error("[ActiveHomeworkAdd]班级不存在", request, this.GetType());
                        return ResponseBase.CommonError("班级不存在");
                    }
                    if (tempClassStudent.EtClassStudents != null && tempClassStudent.EtClassStudents.Count > 0)
                    {
                        p.StudentIds = tempClassStudent.EtClassStudents.Select(j => j.StudentId).ToList();
                    }
                }
                var entity = new EtActiveHomework()
                {
                    ClassId = p.ClassId,
                    CreateUserId = request.LoginUserId,
                    ExDate = request.ExDate,
                    FinishCount = 0,
                    ReadCount = 0,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Status = EmActiveHomeworkStatus.Undone,
                    TenantId = request.LoginTenantId,
                    Title = request.Title,
                    TotalCount = p.StudentIds.Count,
                    Type = EmActiveHomeworkType.SingleWork,
                    WorkContent = request.WorkContent,
                    WorkMedias = workMedias,
                    StudentIds = EtmsHelper.GetMuIds(p.StudentIds)
                };
                await _activeHomeworkDAL.AddActiveHomework(entity);

                homeworkIds.Add(entity.Id);
                foreach (var studentId in p.StudentIds)
                {
                    details.Add(new EtActiveHomeworkDetail()
                    {
                        AnswerContent = string.Empty,
                        AnswerMedias = string.Empty,
                        AnswerOt = null,
                        AnswerStatus = EmActiveHomeworkDetailAnswerStatus.Unanswered,
                        ClassId = p.ClassId,
                        CreateUserId = request.LoginUserId,
                        ExDate = request.ExDate,
                        HomeworkId = entity.Id,
                        IsDeleted = EmIsDeleted.Normal,
                        Ot = now,
                        ReadStatus = EmActiveHomeworkDetailReadStatus.No,
                        StudentId = studentId,
                        TenantId = request.LoginTenantId,
                        Title = request.Title,
                        Type = EmActiveHomeworkType.SingleWork,
                        WorkContent = request.WorkContent,
                        WorkMedias = workMedias,
                        OtDate = date
                    });
                }
            }
            _activeHomeworkDetailDAL.AddActiveHomeworkDetail(details);

            foreach (var id in homeworkIds)
            {
                _eventPublisher.Publish(new NoticeStudentsOfHomeworkAddEvent(request.LoginTenantId)
                {
                    HomeworkId = id
                });
            }

            await _userOperationLogDAL.AddUserLog(request, $"布置单次作业-{request.Title}", EmUserOperationType.ActiveHomeworkMgr, now);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveHomeworkAdd2(ActiveHomeworkAdd2Request request)
        {
            var now = DateTime.Now;
            var date = now.Date;
            var workMedias = string.Empty;
            if (request.WorkMediasKeys != null && request.WorkMediasKeys.Count > 0)
            {
                workMedias = string.Join('|', request.WorkMediasKeys);
            }
            var details = new List<EtActiveHomeworkDetail>();
            var homeworkStudents = new List<EtActiveHomeworkStudent>();
            var homeworkIds = new List<long>();
            var isTodaySend = request.LxStartDate <= date;
            foreach (var p in request.ClassInfos)
            {
                if (p.StudentIds == null || p.StudentIds.Count == 0)
                {
                    var tempClassStudent = await _classDAL.GetClassBucket(p.ClassId);
                    if (tempClassStudent == null || tempClassStudent.EtClass == null)
                    {
                        LOG.Log.Error("[ActiveHomeworkAdd]班级不存在", request, this.GetType());
                        return ResponseBase.CommonError("班级不存在");
                    }
                    if (tempClassStudent.EtClassStudents != null && tempClassStudent.EtClassStudents.Count > 0)
                    {
                        p.StudentIds = tempClassStudent.EtClassStudents.Select(j => j.StudentId).ToList();
                    }
                }
                var entity = new EtActiveHomework()
                {
                    ClassId = p.ClassId,
                    CreateUserId = request.LoginUserId,
                    ExDate = null,
                    FinishCount = 0,
                    ReadCount = 0,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Status = EmActiveHomeworkStatus.Undone,
                    TenantId = request.LoginTenantId,
                    Title = request.Title,
                    TotalCount = p.StudentIds.Count,
                    Type = EmActiveHomeworkType.ContinuousWork,
                    WorkContent = request.WorkContent,
                    WorkMedias = workMedias,
                    LxStartDate = request.LxStartDate,
                    LxEndDate = request.LxEndDate,
                    LxExTime = request.LxExTime,
                    LxTotalCount = request.LxTotalCount.ToInt(),
                    StudentIds = EtmsHelper.GetMuIds(p.StudentIds)
                };
                await _activeHomeworkDAL.AddActiveHomework(entity);

                homeworkIds.Add(entity.Id);
                foreach (var studentId in p.StudentIds)
                {
                    if (isTodaySend)
                    {
                        details.Add(new EtActiveHomeworkDetail()
                        {
                            AnswerContent = string.Empty,
                            AnswerMedias = string.Empty,
                            AnswerOt = null,
                            AnswerStatus = EmActiveHomeworkDetailAnswerStatus.Unanswered,
                            ClassId = p.ClassId,
                            CreateUserId = request.LoginUserId,
                            ExDate = null,
                            HomeworkId = entity.Id,
                            IsDeleted = EmIsDeleted.Normal,
                            Ot = now,
                            ReadStatus = EmActiveHomeworkDetailReadStatus.No,
                            StudentId = studentId,
                            TenantId = request.LoginTenantId,
                            Title = request.Title,
                            Type = EmActiveHomeworkType.ContinuousWork,
                            WorkContent = request.WorkContent,
                            WorkMedias = workMedias,
                            OtDate = date,
                            LxExTime = request.LxExTime
                        });
                    }
                    homeworkStudents.Add(new EtActiveHomeworkStudent()
                    {
                        AnswerStatus = EmActiveHomeworkDetailAnswerStatus.Unanswered,
                        CreateUserId = request.LoginUserId,
                        HomeworkId = entity.Id,
                        IsDeleted = EmIsDeleted.Normal,
                        ReadStatus = EmActiveHomeworkDetailReadStatus.No,
                        StudentId = studentId,
                        TenantId = request.LoginTenantId
                    });
                }
            }
            _activeHomeworkDetailDAL.AddActiveHomeworkDetail(details);
            _activeHomeworkDAL.AddActiveHomeworkStudent(homeworkStudents);

            foreach (var id in homeworkIds)
            {
                _eventPublisher.Publish(new NoticeStudentsOfHomeworkAddEvent(request.LoginTenantId)
                {
                    HomeworkId = id
                });
            }

            await _userOperationLogDAL.AddUserLog(request, $"布置连续作业-{request.Title}", EmUserOperationType.ActiveHomeworkMgr, now);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveHomeworkGetBasc(ActiveHomeworkGetBascRequest request)
        {
            var p = await _activeHomeworkDAL.GetActiveHomework(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var myClassBucket = await _classDAL.GetClassBucket(p.ClassId);
            var teacher = await _userDAL.GetUser(p.CreateUserId);
            var output = new ActiveHomeworkGetBascOutput()
            {
                HomeworkId = p.Id,
                ClassId = p.ClassId,
                ClassName = myClassBucket?.EtClass?.Name,
                TeacherName = teacher.Name,
                TeacherAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, teacher.Avatar),
                ExDateDesc = p.ExDate == null ? "未设置" : p.ExDate.EtmsToMinuteString(),
                FinishCount = p.FinishCount,
                OtDesc = p.Ot.EtmsToMinuteString(),
                ReadCount = p.ReadCount,
                Title = p.Title,
                TotalCount = p.TotalCount,
                Type = p.Type,
                TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                WorkContent = p.WorkContent,
                WorkMediasUrl = GetMediasUrl(p.WorkMedias)
            };
            if (p.Type == EmActiveHomeworkType.ContinuousWork)
            {
                output.LxStartDateDesc = p.LxStartDate.EtmsToDateString();
                output.LxEndDateDesc = p.LxEndDate.EtmsToDateString();
                output.LxTotalCount = p.LxTotalCount.Value;
                output.Students = new List<HomeworkStudent>();
                var ids = EtmsHelper.AnalyzeMuIds(p.StudentIds);
                foreach (var studentId in ids)
                {
                    var studentBucket = await _studentDAL.GetStudent(studentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    output.Students.Add(new HomeworkStudent()
                    {
                        StudentId = studentId,
                        StudentName = studentBucket.Student.Name
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActiveHomeworkGetForEdit(ActiveHomeworkGetForEditRequest request)
        {
            var p = await _activeHomeworkDAL.GetActiveHomework(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var output = new ActiveHomeworkGetForEditOutput()
            {
                HomeworkId = p.Id,
                Title = p.Title,
                WorkContent = p.WorkContent,
                WorkMediasKeys = new List<Img>()
            };
            if (!string.IsNullOrEmpty(p.WorkMedias))
            {
                var myMedias = p.WorkMedias.Split('|');
                foreach (var item in myMedias)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        output.WorkMediasKeys.Add(new Img()
                        {
                            Key = item,
                            Url = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, item)
                        });
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActiveHomeworkStudentGetAnswered(ActiveHomeworkGetAnsweredRequest request)
        {
            var activeHomeworkDetails = await _activeHomeworkDetailDAL.GetActiveHomeworkDetail(request.CId,
                EmActiveHomeworkDetailAnswerStatus.Answered, request.OtDate, request.StudentId);
            var output = new List<ActiveHomeworkStudentGetAnsweredOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var detail in activeHomeworkDetails)
            {
                var p = await _activeHomeworkDetailDAL.GetActiveHomeworkDetailBucket(detail.Id);
                var studentBucket = await _studentDAL.GetStudent(detail.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    continue;
                }
                var student = studentBucket.Student;
                var answerStatus = AnsweredOutputAnswerStatus.GetAnswerStatus(detail.ExDate, detail.AnswerOt.Value);
                var myDetail = new ActiveHomeworkStudentGetAnsweredOutput()
                {
                    HomeworkDetailId = detail.Id,
                    AnswerContent = detail.AnswerContent,
                    AnswerMediasUrl = GetMediasUrl(detail.AnswerMedias),
                    AnswerOtDesc = detail.AnswerOt.EtmsToMinuteString(),
                    StudentId = detail.StudentId,
                    StudentName = student.Name,
                    AnswerStatus = answerStatus.Item1,
                    AnswerStatusDesc = answerStatus.Item2,
                    StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar),
                    CommentOutputs = await GetCommentOutput(p.ActiveHomeworkDetailComments, tempBoxUser)
                };
                output.Add(myDetail);
            }
            return ResponseBase.Success(output);
        }

        private async Task<List<CommentOutput>> GetCommentOutput(List<EtActiveHomeworkDetailComment> activeHomeworkDetailComments, DataTempBox<EtUser> tempBoxUser)
        {
            var commentOutputs = new List<CommentOutput>();
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
                            firstrelatedManName = myRelatedUser.Name;
                            firstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                        }
                    }
                    else
                    {
                        firstrelatedManName = myFirstComment.Nickname;
                        firstrelatedManAvatar = myFirstComment.Headimgurl;
                    }
                    commentOutputs.Add(new CommentOutput()
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
                                secondfirstrelatedManName = myRelatedUser.Name;
                                secondfirstrelatedManAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myRelatedUser.Avatar);
                            }
                        }
                        else
                        {
                            secondfirstrelatedManName = mySecondComment.Nickname;
                            secondfirstrelatedManAvatar = mySecondComment.Headimgurl;
                        }
                        commentOutputs.Add(new CommentOutput()
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

        public async Task<ResponseBase> ActiveHomeworkStudentGetUnanswered(ActiveHomeworkGetUnansweredRequest request)
        {
            var activeHomeworkDetails = await _activeHomeworkDetailDAL.GetActiveHomeworkDetail(request.CId,
                EmActiveHomeworkDetailAnswerStatus.Unanswered, request.OtDate, request.StudentId);
            var output = new List<ActiveHomeworkStudentGetUnansweredOutput>();
            foreach (var p in activeHomeworkDetails)
            {
                var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                if (studentBucket == null || studentBucket.Student == null)
                {
                    continue;
                }
                output.Add(new ActiveHomeworkStudentGetUnansweredOutput()
                {
                    HomeworkDetailId = p.Id,
                    ReadStatus = p.ReadStatus,
                    ReadStatusDesc = EmActiveHomeworkDetailReadStatus.GetActiveHomeworkDetailReadStatusDesc(p.ReadStatus),
                    StudentAvatar = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, studentBucket.Student.Avatar),
                    StudentId = p.StudentId,
                    StudentName = studentBucket.Student.Name,
                    StudentPhone = ComBusiness3.PhoneSecrecy(studentBucket.Student.Phone, request.SecrecyType, request.SecrecyDataBag),
                    OtDateDesc = p.OtDate.EtmsToDateString()
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ActiveHomeworkDel(ActiveHomeworkDelRequest request)
        {
            var p = await _activeHomeworkDAL.GetActiveHomework(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            await _activeHomeworkDAL.DelActiveHomework(request.CId);
            AliyunOssUtil.DeleteObject2(p.WorkMedias);
            _eventPublisher.Publish(new CloudFileDelEvent(request.LoginTenantId)
            {
                SceneType = CloudFileScenes.ActiveHomework,
                RelatedId = request.CId
            });

            await _userOperationLogDAL.AddUserLog(request, $"删除作业-{p.Title}", EmUserOperationType.ActiveHomeworkMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveHomeworkCommentAdd(ActiveHomeworkCommentAddRequest request)
        {
            var p = await _activeHomeworkDAL.GetActiveHomework(request.HomeworkId);
            if (p == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var now = DateTime.Now;
            var comment = new EtActiveHomeworkDetailComment()
            {
                CommentContent = request.CommentContent,
                Headimgurl = string.Empty,
                Nickname = string.Empty,
                HomeworkDetailId = request.HomeworkDetailId,
                HomeworkId = request.HomeworkId,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                ReplyId = request.ReplyId,
                SourceId = request.LoginUserId,
                SourceType = EmActiveCommentSourceType.User,
                TenantId = request.LoginTenantId
            };
            await _activeHomeworkDetailDAL.AddActiveHomeworkDetailComment(comment);

            _eventPublisher.Publish(new NoticeStudentsOfHomeworkAddCommentEvent(request.LoginTenantId)
            {
                HomeworkDetailId = request.HomeworkDetailId,
                AddUserId = request.LoginUserId,
                MyOt = now
            });

            await _userOperationLogDAL.AddUserLog(request, $"添加作业评语-作业[{p.Title}]，添加评语[{request.CommentContent}]",
                EmUserOperationType.ActiveHomeworkMgr, now);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveHomeworkEdit(ActiveHomeworkEditRequest request)
        {
            var p = await _activeHomeworkDAL.GetActiveHomework(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("作业不存在");
            }
            var workMedias = string.Empty;
            if (request.WorkMediasKeys != null && request.WorkMediasKeys.Count > 0)
            {
                workMedias = string.Join('|', request.WorkMediasKeys);
            }
            p.Title = request.Title;
            p.WorkContent = request.WorkContent;
            p.WorkMedias = workMedias;
            await _activeHomeworkDAL.EditActiveHomework(p);

            _eventPublisher.Publish(new ActiveHomeworkEditEvent(request.LoginTenantId)
            {
                ActiveHomework = p
            });
            await _userOperationLogDAL.AddUserLog(request, $"编辑作业-{request.Title}", EmUserOperationType.ActiveHomeworkMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveHomeworkCommentDel(ActiveHomeworkCommentDelRequest request)
        {
            await _activeHomeworkDetailDAL.DelActiveHomeworkDetailComment(request.HomeworkDetailId, request.CommentId);
            await _userOperationLogDAL.AddUserLog(request, "删除作业评语", EmUserOperationType.ActiveHomeworkMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ActiveHomeworkStudentGetPaging(ActiveHomeworkStudentGetPagingRequest request)
        {
            var pagingData = await _activeHomeworkDetailDAL.GetPaging(request);
            var output = new List<ActiveHomeworkStudentGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxUser = new DataTempBox<EtUser>();
                var tempBoxClass = new DataTempBox<EtClass>();
                var limitDateTime = DateTime.Now.AddDays(-7);
                foreach (var p in pagingData.Item1)
                {
                    var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId);
                    output.Add(new ActiveHomeworkStudentGetPagingOutput()
                    {
                        ClassId = p.ClassId,
                        ClassName = myClass?.Name,
                        TeacherName = await ComBusiness.GetParentTeacher(tempBoxUser, _userDAL, p.CreateUserId),
                        OtDesc = p.Ot.EtmsToMinuteString(),
                        Title = p.Title,
                        Type = p.Type,
                        TypeDesc = EmActiveHomeworkType.GetActiveHomeworkTypeDesc(p.Type),
                        AnswerStatus = p.AnswerStatus,
                        HomeworkDetailId = p.Id,
                        HomeworkId = p.HomeworkId,
                        AnswerStatusDesc = EmActiveHomeworkDetailAnswerStatus.ActiveHomeworkDetailAnswerStatusDesc(p.AnswerStatus),
                        AnswerOtDesc = p.AnswerOt.EtmsToMinuteString(),
                        WorkContent = p.WorkContent,
                        WorkMediasUrl = GetMediasUrl(p.WorkMedias),
                        ExDateDesc = p.ExDate == null ? string.Empty : p.ExDate.EtmsToMinuteString(),
                        IsCanModify = p.Ot >= limitDateTime
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ActiveHomeworkStudentGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ActiveHomeworkStudentGet(ActiveHomeworkStudentGetRequest request)
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
            var output = new ActiveHomeworkStudentGetOutput()
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
                output.AnswerInfo = new HomeworkDetailAnswerInfo2()
                {
                    AnswerContent = p.AnswerContent,
                    AnswerMediasUrl = GetMediasUrl(p.AnswerMedias),
                    AnswerOtDesc = p.AnswerOt.EtmsToMinuteString()
                };
            }
            return ResponseBase.Success(output);
        }
    }
}
