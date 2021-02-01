using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Dto.Student.Output;
using Microsoft.AspNetCore.Http;
using ETMS.Utility;
using ETMS.Entity.Config;
using ETMS.Business.Common;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.IBusiness.IncrementLib;

namespace ETMS.Business
{
    public class StudentBLL : IStudentBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly IStudentExtendFieldDAL _studentExtendFieldDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IStudentTagDAL _studentTagDAL;

        private readonly IStudentRelationshipDAL _studentRelationshipDAL;

        private readonly IStudentSourceDAL _studentSourceDAL;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IUserDAL _userDAL;

        private readonly IGradeDAL _gradeDAL;

        private readonly IStudentTrackLogDAL _studentTrackLogDAL;

        private readonly IStudentOperationLogDAL _studentOperationLogDAL;

        private readonly IStudentLeaveApplyLogDAL _studentLeaveApplyLogDAL;

        private readonly INoticeBLL _noticeBLL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IClassDAL _classDAL;

        private readonly IAiface _aiface;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        public StudentBLL(IStudentDAL studentDAL, IStudentExtendFieldDAL studentExtendFieldDAL, IUserOperationLogDAL userOperationLogDAL,
            IStudentTagDAL studentTagDAL, IStudentRelationshipDAL studentRelationshipDAL, IStudentSourceDAL studentSourceDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IUserDAL userDAL, IGradeDAL gradeDAL,
            IStudentTrackLogDAL studentTrackLogDAL, IStudentOperationLogDAL studentOperationLogDAL,
            IStudentLeaveApplyLogDAL studentLeaveApplyLogDAL, INoticeBLL noticeBLL, IEventPublisher eventPublisher,
            IStudentCourseDAL studentCourseDAL, IClassDAL classDAL, IAiface aiface, IStudentPointsLogDAL studentPointsLogDAL)
        {
            this._studentDAL = studentDAL;
            this._studentExtendFieldDAL = studentExtendFieldDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._studentTagDAL = studentTagDAL;
            this._studentRelationshipDAL = studentRelationshipDAL;
            this._studentSourceDAL = studentSourceDAL;
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._userDAL = userDAL;
            this._gradeDAL = gradeDAL;
            this._studentTrackLogDAL = studentTrackLogDAL;
            this._studentOperationLogDAL = studentOperationLogDAL;
            this._studentLeaveApplyLogDAL = studentLeaveApplyLogDAL;
            this._noticeBLL = noticeBLL;
            this._eventPublisher = eventPublisher;
            this._studentCourseDAL = studentCourseDAL;
            this._classDAL = classDAL;
            this._aiface = aiface;
            this._studentPointsLogDAL = studentPointsLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._noticeBLL.InitTenantId(tenantId);
            this._aiface.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _studentExtendFieldDAL, _studentTagDAL,
                _userOperationLogDAL, _studentRelationshipDAL, _studentSourceDAL, _userDAL, _gradeDAL, _studentTrackLogDAL, _studentOperationLogDAL,
                _studentLeaveApplyLogDAL, _studentCourseDAL, _classDAL, _studentPointsLogDAL);
        }

        public async Task<ResponseBase> StudentAdd(StudentAddRequest request)
        {
            if (await _studentDAL.ExistStudent(request.Name, request.Phone))
            {
                return ResponseBase.CommonError("该学员已经存在");
            }
            var tags = string.Empty;
            if (request.Tags != null && request.Tags.Any())
            {
                tags = $",{string.Join(',', request.Tags)},";
            }
            var now = DateTime.Now;
            var etStudent = new EtStudent()
            {
                Age = request.Birthday.EtmsGetAge(),
                Name = request.Name,
                Avatar = request.AvatarKey,
                Birthday = request.Birthday,
                //CardNo = request.CardNo,
                CreateBy = request.LoginUserId,
                EndClassOt = null,
                Gender = request.Gender,
                GradeId = request.GradeId,
                HomeAddress = request.HomeAddress,
                IntentionLevel = request.IntentionLevel,
                IsBindingWechat = EmIsBindingWechat.No,
                IsDeleted = EmIsDeleted.Normal,
                LastJobProcessTime = now,
                LastTrackTime = null,
                LearningManager = null,
                NextTrackTime = null,
                Ot = now.Date,
                Phone = request.Phone,
                PhoneBak = request.PhoneBak,
                PhoneBakRelationship = request.PhoneBakRelationship,
                PhoneRelationship = request.PhoneRelationship ?? 0,
                Points = 0,
                Remark = request.Remark,
                SchoolName = request.SchoolName,
                SourceId = request.SourceId,
                StudentType = EmStudentType.HiddenStudent,
                Tags = tags,
                TenantId = request.LoginTenantId,
                TrackStatus = EmStudentTrackStatus.NotTrack,
                TrackUser = request.TrackUser,
                NamePinyin = PinyinHelper.GetPinyinInitials(request.Name).ToLower()
            };
            var studentExtendInfos = new List<EtStudentExtendInfo>();
            if (request.StudentExtendItems != null && request.StudentExtendItems.Any())
            {
                foreach (var s in request.StudentExtendItems)
                {
                    studentExtendInfos.Add(new EtStudentExtendInfo()
                    {
                        ExtendFieldId = s.CId,
                        IsDeleted = EmIsDeleted.Normal,
                        Remark = string.Empty,
                        StudentId = 0,
                        TenantId = request.LoginTenantId,
                        Value1 = s.Value
                    });
                }
            }
            var studentId = await _studentDAL.AddStudent(etStudent, studentExtendInfos);
            SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
            {
                ChangeCount = 1,
                OpType = StatisticsStudentOpType.Add,
                Time = now
            }, request, etStudent.Ot, true);
            await _userOperationLogDAL.AddUserLog(request, $"添加学员-姓名:{request.Name},手机号码:{request.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success(studentId);
        }

        private void SyncStatisticsStudentInfo(StatisticsStudentCountEvent studentCountEvent, RequestBase request, DateTime ot, bool isChangeStudentSource)
        {
            if (studentCountEvent != null)
            {
                _eventPublisher.Publish(studentCountEvent);
            }
            if (isChangeStudentSource)
            {
                _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentSource, StatisticsDate = ot });
            }
            _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentType, StatisticsDate = ot });
        }

        public async Task<ResponseBase> StudentEdit(StudentEditRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            if (await _studentDAL.ExistStudent(request.Name, request.Phone, request.CId))
            {
                return ResponseBase.CommonError("存在相同姓名和手机号的学员");
            }
            var tags = string.Empty;
            if (request.Tags != null && request.Tags.Any())
            {
                tags = $",{string.Join(',', request.Tags)},";
            }
            var etStudent = studentBucket.Student;
            var oldAvatar = etStudent.Avatar;
            var isChangeStudentSource = request.SourceId != etStudent.SourceId;

            etStudent.Name = request.Name;
            etStudent.Avatar = request.AvatarKey;
            etStudent.Birthday = request.Birthday;
            //etStudent.CardNo = request.CardNo;
            etStudent.Gender = request.Gender;
            etStudent.GradeId = request.GradeId;
            etStudent.HomeAddress = request.HomeAddress;
            etStudent.IntentionLevel = request.IntentionLevel;
            etStudent.Phone = request.Phone;
            etStudent.PhoneBak = request.PhoneBak;
            etStudent.PhoneBakRelationship = request.PhoneBakRelationship;
            etStudent.PhoneRelationship = request.PhoneRelationship ?? 0;
            etStudent.Remark = request.Remark;
            etStudent.SchoolName = request.SchoolName;
            etStudent.SourceId = request.SourceId;
            etStudent.Tags = tags;
            etStudent.TrackUser = request.TrackUser;
            etStudent.NamePinyin = PinyinHelper.GetPinyinInitials(request.Name).ToLower();
            etStudent.Age = request.Birthday.EtmsGetAge();
            var studentExtendInfos = new List<EtStudentExtendInfo>();
            if (request.StudentExtendItems != null && request.StudentExtendItems.Any())
            {
                foreach (var s in request.StudentExtendItems)
                {
                    studentExtendInfos.Add(new EtStudentExtendInfo()
                    {
                        ExtendFieldId = s.CId,
                        IsDeleted = EmIsDeleted.Normal,
                        Remark = string.Empty,
                        StudentId = etStudent.Id,
                        TenantId = request.LoginTenantId,
                        Value1 = s.Value
                    });
                }
            }
            await _studentDAL.EditStudent(etStudent, studentExtendInfos);
            SyncStatisticsStudentInfo(null, request, etStudent.Ot, isChangeStudentSource);
            if (oldAvatar != request.AvatarKey)
            {
                AliyunOssUtil.DeleteObject(oldAvatar);
            }

            await _userOperationLogDAL.AddUserLog(request, $"编辑学员-姓名:{request.Name},手机号码:{request.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentDel(StudentDelRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var etStudent = studentBucket.Student;
            if (await _studentDAL.CheckStudentHasOrder(request.CId))
            {
                return ResponseBase.CommonError("学员存在订单记录，无法删除");
            }
            await _studentDAL.DelStudent(request.CId);
            SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
            {
                Time = etStudent.Ot,
                OpType = StatisticsStudentOpType.Deduction,
                ChangeCount = 1
            }, request, etStudent.Ot, true);
            AliyunOssUtil.DeleteObject(etStudent.Avatar, etStudent.FaceGreyKey, etStudent.FaceKey);
            await _aiface.StudentDelete(etStudent.Id);

            await _userOperationLogDAL.AddUserLog(request, $"删除学员-姓名:{etStudent.Name},手机号码:{etStudent.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentGet(StudentGetRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            var studentRelationship = await _studentRelationshipDAL.GetAllStudentRelationship();
            var tempBoxUser = new DataTempBox<EtUser>();
            var studentGetOutput = new StudentGetOutput()
            {
                CId = student.Id,
                AvatarKey = student.Avatar,
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar),
                CardNo = student.CardNo,
                Gender = student.Gender,
                TrackUser = student.TrackUser,
                GradeId = student.GradeId,
                IntentionLevel = student.IntentionLevel,
                IsBindingWechat = student.IsBindingWechat,
                Tags = student.Tags,
                StudentType = student.StudentType,
                HomeAddress = student.HomeAddress,
                LearningManager = student.LearningManager,
                TrackStatus = student.TrackStatus,
                Name = student.Name,
                Phone = student.Phone,
                PhoneBak = student.PhoneBak,
                SourceId = student.SourceId,
                SchoolName = student.SchoolName,
                Points = student.Points,
                Remark = student.Remark,
                PhoneRelationship = student.PhoneRelationship,
                PhoneBakRelationship = student.PhoneBakRelationship,
                BirthdayDesc = student.Birthday.EtmsToDateString(),
                Age = student.Age,
                EndClassOtDesc = student.EndClassOt == null ? string.Empty : student.EndClassOt.EtmsToDateString(),
                GenderDesc = EmGender.GetGenderDesc(student.Gender),
                IntentionLevelDesc = EmStudentIntentionLevel.GetIntentionLevelDesc(student.IntentionLevel),
                LastTrackTimeDesc = student.LastTrackTime.EtmsToDateString(),
                NextTrackTimeDesc = student.NextTrackTime.EtmsToDateString(),
                StudentTypeDesc = EmStudentType.GetStudentTypeDesc(student.StudentType),
                TrackStatusDesc = EmStudentTrackStatus.GetTrackStatusDesc(student.TrackStatus),
                TrackUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, student.TrackUser),
                LearningManagerDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, student.LearningManager),
                GradeIdDesc = await GetGradeDesc(student.GradeId),
                PhoneBakRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneBakRelationship, "备用号码"),
                PhoneRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneRelationship, "手机号码"),
                SourceIdDesc = await GetSourceDesc(student.SourceId),
                TagsDesc = await GetStudentTagsDesc(student.Tags),
                StudentExtendItems = new List<StudentExtendItemOutput>(),
                OtDesc = student.Ot.EtmsToDateString(),
                IsBindingCard = !string.IsNullOrEmpty(student.CardNo),
                IsBindingFaceKey = !string.IsNullOrEmpty(student.FaceKey),
                FaceKeyUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.FaceKey),
            };
            var studentExtendFileds = await _studentExtendFieldDAL.GetAllStudentExtendField();
            foreach (var file in studentExtendFileds)
            {
                studentGetOutput.StudentExtendItems.Add(new StudentExtendItemOutput()
                {
                    FieldDisplayName = file.DisplayName,
                    FieldId = file.Id,
                    Value = studentBucket.StudentExtendInfos.FirstOrDefault(p => p.ExtendFieldId == file.Id)?.Value1
                });
            }
            return ResponseBase.Success(studentGetOutput);
        }

        public async Task<ResponseBase> StudentGetForEdit(StudentGetForEditReuqest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            string[] tags = null;
            if (!string.IsNullOrEmpty(student.Tags))
            {
                tags = student.Tags.Trim(',').Split(',');
            }
            var tempBoxUser = new DataTempBox<EtUser>();
            var output = new StudentGetForEditOutput()
            {
                AvatarKey = student.Avatar,
                Birthday = student.Birthday.EtmsToDateString(),
                CardNo = student.CardNo,
                CId = student.Id,
                Gender = student.Gender,
                GradeId = student.GradeId,
                HomeAddress = student.HomeAddress,
                IntentionLevel = student.IntentionLevel,
                Name = student.Name,
                Phone = student.Phone,
                PhoneBak = student.PhoneBak,
                PhoneBakRelationship = student.PhoneBakRelationship,
                PhoneRelationship = student.PhoneRelationship,
                Remark = student.Remark,
                SchoolName = student.SchoolName,
                SourceId = student.SourceId,
                TrackUser = student.TrackUser,
                Tags = tags,
                StudentExtendItems = new List<StudentExtendItem>(),
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar),
                TrackUserName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, student.TrackUser),
            };
            var studentExtendFileds = await _studentExtendFieldDAL.GetAllStudentExtendField();
            foreach (var file in studentExtendFileds)
            {
                output.StudentExtendItems.Add(new StudentExtendItem()
                {
                    CId = file.Id,
                    Value = studentBucket.StudentExtendInfos.FirstOrDefault(p => p.ExtendFieldId == file.Id)?.Value1
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentGetPaging(StudentGetPagingRequest request)
        {
            var studentPagingInfo = await _studentDAL.GetStudentPaging(request);
            var studentTags = await _studentTagDAL.GetAllStudentTag();
            var sources = await _studentSourceDAL.GetAllStudentSource();
            var studentRelationship = await _studentRelationshipDAL.GetAllStudentRelationship();
            var grades = await _gradeDAL.GetAllGrade();
            var tempBoxUser = new DataTempBox<EtUser>();
            return ResponseBase.Success(new ResponsePagingDataBase<StudentGetPagingOutput>(studentPagingInfo.Item2, studentPagingInfo.Item1.Select(student => new StudentGetPagingOutput()
            {
                CId = student.Id,
                AvatarKey = student.Avatar,
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar),
                CardNo = student.CardNo,
                Gender = student.Gender,
                TrackUser = student.TrackUser,
                GradeId = student.GradeId,
                IntentionLevel = student.IntentionLevel,
                IsBindingWechat = student.IsBindingWechat,
                Tags = student.Tags,
                StudentType = student.StudentType,
                HomeAddress = student.HomeAddress,
                LearningManager = student.LearningManager,
                TrackStatus = student.TrackStatus,
                Name = student.Name,
                Phone = student.Phone,
                PhoneBak = student.PhoneBak,
                SourceId = student.SourceId,
                SchoolName = student.SchoolName,
                Points = student.Points,
                Remark = student.Remark,
                PhoneRelationship = student.PhoneRelationship,
                PhoneBakRelationship = student.PhoneBakRelationship,
                BirthdayDesc = student.Birthday.EtmsToDateString(),
                Age = student.Age,
                EndClassOtDesc = student.EndClassOt == null ? string.Empty : student.EndClassOt.EtmsToDateString(),
                GenderDesc = EmGender.GetGenderDesc(student.Gender),
                IntentionLevelDesc = EmStudentIntentionLevel.GetIntentionLevelDesc(student.IntentionLevel),
                LastTrackTimeDesc = student.LastTrackTime.EtmsToDateString(),
                NextTrackTimeDesc = student.NextTrackTime.EtmsToDateString(),
                StudentTypeDesc = EmStudentType.GetStudentTypeDesc(student.StudentType),
                TrackStatusDesc = EmStudentTrackStatus.GetTrackStatusDesc(student.TrackStatus),
                TrackUserDesc = ComBusiness.GetUserName(tempBoxUser, _userDAL, student.TrackUser).Result,
                LearningManagerDesc = ComBusiness.GetUserName(tempBoxUser, _userDAL, student.LearningManager).Result,
                GradeIdDesc = GetGradeDesc(grades, student.GradeId),
                PhoneBakRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneBakRelationship, "手机号码"),
                PhoneRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneRelationship, "备用号码"),
                SourceIdDesc = GetSourceDesc(sources, student.SourceId),
                TagsDesc = GetStudentTagsDesc(studentTags, student.Tags),
                OtDesc = student.Ot.EtmsToDateString(),
                Label = student.Name,
                Value = student.Id,
                IsBindingCard = !string.IsNullOrEmpty(student.CardNo),
                IsBindingFaceKey = !string.IsNullOrEmpty(student.FaceKey),
                FaceKeyUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.FaceKey)
            })));
        }

        private async Task<string> GetStudentTagsDesc(string tags)
        {
            if (string.IsNullOrEmpty(tags))
            {
                return string.Empty;
            }
            var studentTags = await _studentTagDAL.GetAllStudentTag();
            return GetStudentTagsDesc(studentTags, tags);
        }

        private string GetStudentTagsDesc(List<EtStudentTag> studentTags, string tags)
        {
            if (string.IsNullOrEmpty(tags))
            {
                return string.Empty;
            }
            var ids = tags.Trim(',').Split(',');
            var tagDesc = new StringBuilder();
            foreach (var id in ids)
            {
                var myTag = studentTags.FirstOrDefault(p => p.Id == id.ToLong());
                if (myTag != null)
                {
                    tagDesc.Append($"{myTag.Name},");
                }
            }
            return tagDesc.ToString().TrimEnd(',');
        }

        private async Task<string> GetSourceDesc(long? id)
        {
            if (id == null)
            {
                return string.Empty;
            }
            var sources = await _studentSourceDAL.GetAllStudentSource();
            return sources.FirstOrDefault(p => p.Id == id.Value)?.Name;
        }

        private string GetSourceDesc(List<EtStudentSource> sources, long? id)
        {
            if (id == null)
            {
                return string.Empty;
            }
            return sources.FirstOrDefault(p => p.Id == id.Value)?.Name;
        }

        private async Task<string> GetGradeDesc(long? gradeId)
        {
            if (gradeId == null)
            {
                return string.Empty;
            }
            var grades = await _gradeDAL.GetAllGrade();
            return grades.FirstOrDefault(p => p.Id == gradeId.Value)?.Name;
        }

        private string GetGradeDesc(List<EtGrade> grades, long? gradeId)
        {
            if (gradeId == null)
            {
                return string.Empty;
            }
            return grades.FirstOrDefault(p => p.Id == gradeId.Value)?.Name;
        }

        public async Task<ResponseBase> StudentSetTrackUser(StudentSetTrackUserRequest request)
        {
            await _studentDAL.EditStudentTrackUser(request.StudentCIds, request.NewTrackUser);
            await _userOperationLogDAL.AddUserLog(request, "分配跟进人", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentSetLearningManager(StudentSetLearningManagerRequest request)
        {
            await _studentDAL.EditStudentLearningManager(request.StudentCIds, request.NewLearningManager);
            await _userOperationLogDAL.AddUserLog(request, "分配学管师", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentTrackLogAdd(StudentTrackLogAddRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var trackTime = DateTime.Now;
            var trackLog = new EtStudentTrackLog()
            {
                ContentType = EmStudentTrackContentType.Customize,
                IsDeleted = EmIsDeleted.Normal,
                NextTrackTime = request.NextTrackTime,
                TrackTime = trackTime,
                RelatedInfo = null,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                TrackUserId = request.LoginUserId,
                TrackContent = request.TrackContent
            };
            await _studentTrackLogDAL.AddStudentTrackLog(trackLog);
            var student = studentBucket.Student;
            student.LastTrackTime = trackTime;
            if (request.NextTrackTime != null)
            {
                student.NextTrackTime = request.NextTrackTime.Value;
            }
            if (student.TrackStatus == EmStudentTrackStatus.NotTrack)
            {
                student.TrackStatus = EmStudentTrackStatus.IsTracking;
            }
            await _studentDAL.EditStudent(student);
            _eventPublisher.Publish(new StatisticsStudentTrackCountEvent(request.LoginTenantId)
            {
                Time = trackTime,
                ChangeCount = 1,
                OpType = StatisticsStudentTrackCountOpType.Add
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加跟进记录-姓名:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone},跟进内容:{request.TrackContent}",
                EmUserOperationType.StudentTrackLog);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentTrackLogGetLast(StudentTrackLogGetLastRequest request)
        {
            var studentTrackLogs = await _studentTrackLogDAL.GetStudentTrackLog(request.StudentId);
            if (studentTrackLogs == null || !studentTrackLogs.Any())
            {
                return ResponseBase.Success();
            }
            var lastTrackLog = studentTrackLogs.First();
            var trackUser = await _userDAL.GetUser(lastTrackLog.TrackUserId);
            return ResponseBase.Success(new StudentTrackLogGetLastOutput()
            {
                CId = lastTrackLog.Id,
                TrackTimeDesc = lastTrackLog.TrackTime.EtmsToMinuteString(),
                NextTrackTimeDesc = lastTrackLog.NextTrackTime.EtmsToMinuteString(),
                TrackContent = lastTrackLog.TrackContent,
                TrackUserName = trackUser?.Name
            });
        }

        public async Task<ResponseBase> StudentTrackLogGetList(StudentTrackLogGetListRequest request)
        {
            var studentTrackLogs = await _studentTrackLogDAL.GetStudentTrackLog(request.StudentId);
            var logOutput = new List<StudentTrackLogGetListOutput>();
            if (studentTrackLogs == null || !studentTrackLogs.Any())
            {
                return ResponseBase.Success(logOutput);
            }
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var log in studentTrackLogs)
            {
                var user = await ComBusiness.GetUser(tempBoxUser, _userDAL, log.TrackUserId);
                logOutput.Add(new StudentTrackLogGetListOutput()
                {
                    CId = log.Id,
                    TrackTimeDesc = log.TrackTime.EtmsToMinuteString(),
                    NextTrackTimeDesc = log.NextTrackTime.EtmsToMinuteString(),
                    TrackContent = log.TrackContent,
                    TrackUserAvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, user?.Avatar),
                    TrackUserName = user?.Name
                });
            }
            return ResponseBase.Success(logOutput);
        }

        public async Task<ResponseBase> StudentTrackLogDel(StudentTrackLogDelRequest request)
        {
            var log = await _studentTrackLogDAL.GetTrackLog(request.CId);
            if (log == null)
            {
                return ResponseBase.CommonError("不存在此记录");
            }
            await _studentTrackLogDAL.DelStudentTrackLog(log.Id, log.StudentId);
            _eventPublisher.Publish(new StatisticsStudentTrackCountEvent(request.LoginTenantId)
            {
                Time = log.TrackTime,
                ChangeCount = 1,
                OpType = StatisticsStudentTrackCountOpType.Deduction
            });
            await _userOperationLogDAL.AddUserLog(request, $"删除跟进记录-跟进内容:{log.TrackContent}", EmUserOperationType.StudentTrackLog);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentTrackLogGetPaging(StudentTrackLogGetPagingRequest request)
        {
            var gtudentTrackLogGetPagingInfo = await _studentTrackLogDAL.GetPaging(request);
            var trackLog = new List<StudentTrackLogGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in gtudentTrackLogGetPagingInfo.Item1)
            {
                var userName = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.TrackUserId);
                trackLog.Add(new StudentTrackLogGetPagingOutput()
                {
                    CId = p.Id,
                    ContentTypeDesc = EmStudentTrackContentType.GetContentTypeDesc(p.ContentType),
                    TrackTimeDesc = p.TrackTime.EtmsToMinuteString(),
                    NextTrackTimeDesc = p.NextTrackTime.EtmsToDateString(),
                    TrackContent = p.TrackContent,
                    StudentDesc = EtmsHelper.PeopleDesc(p.StudentName, p.StudentPhone),
                    TrackUserName = userName
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentTrackLogGetPagingOutput>(gtudentTrackLogGetPagingInfo.Item2, trackLog));
        }

        public async Task<ResponseBase> StudentOperationLogPaging(StudentOperationLogPagingRequest request)
        {
            var opLog = await _studentOperationLogDAL.GetPaging(request);
            return ResponseBase.Success(new ResponsePagingDataBase<StudentOperationLogPagingOutput>(opLog.Item2, opLog.Item1.Select(p => new StudentOperationLogPagingOutput()
            {
                IpAddress = p.IpAddress,
                OpContent = p.OpContent,
                Ot = p.Ot,
                Remark = p.Remark,
                StudentName = p.StudentName,
                StudentPhone = p.StudentPhone,
                TypeDesc = EnumDataLib.GetStudentOperationTypeDesc.FirstOrDefault(j => j.Value == p.Type)?.Label
            })));
        }

        public ResponseBase StudentOperationLogTypeGet(RequestBase request)
        {
            return ResponseBase.Success(EnumDataLib.GetStudentOperationTypeDesc);
        }

        public async Task<ResponseBase> StudentLeaveApplyLogPaging(StudentLeaveApplyLogPagingRequest request)
        {
            var pagingData = await _studentLeaveApplyLogDAL.GetPaging(request);
            var output = new List<StudentLeaveApplyLogPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new StudentLeaveApplyLogPagingOutput()
                {
                    ApplyOt = p.ApplyOt,
                    CId = p.Id,
                    EndDateDesc = p.EndDate.EtmsToDateString(),
                    EndTimeDesc = EtmsHelper.GetTimeDesc(p.EndTime),
                    StartDateDesc = p.StartDate.EtmsToDateString(),
                    StartTimeDesc = EtmsHelper.GetTimeDesc(p.StartTime),
                    HandleRemark = p.HandleRemark,
                    HandleOtDesc = p.HandleOt == null ? string.Empty : p.HandleOt.EtmsToString(),
                    HandleStatus = p.HandleStatus,
                    HandleStatusDesc = EmStudentLeaveApplyHandleStatus.GetStudentLeaveApplyHandleStatusDesc(p.HandleStatus),
                    HandleUser = p.HandleUser,
                    HandleUserDesc = p.HandleUser == null ? string.Empty : await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.HandleUser.Value),
                    LeaveContent = p.LeaveContent,
                    StudentName = p.StudentName,
                    StudentPhone = p.StudentPhone
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentLeaveApplyLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentLeaveApplyHandle(StudentLeaveApplyHandleRequest request)
        {
            var applyLog = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyLog(request.StudentLeaveApplyLogId);
            if (applyLog == null)
            {
                return ResponseBase.CommonError("请假记录不存在");
            }
            if (applyLog.HandleStatus != EmStudentLeaveApplyHandleStatus.Unreviewed)
            {
                return ResponseBase.CommonError("此记录已审核");
            }
            applyLog.HandleStatus = request.NewHandleStatus;
            applyLog.HandleOt = DateTime.Now;
            applyLog.HandleRemark = request.HandleRemark;
            applyLog.HandleUser = request.LoginUserId;
            await _studentLeaveApplyLogDAL.EditStudentLeaveApplyLog(applyLog);
            _eventPublisher.Publish(new NoticeStudentLeaveApplyEvent(request.LoginTenantId)
            {
                StudentLeaveApplyLog = applyLog
            });
            await _userOperationLogDAL.AddUserLog(request, "审核请假记录", EmUserOperationType.StudentLeaveApplyManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentLeaveApplyAdd(StudentLeaveApplyAddRequest request)
        {
            await _studentLeaveApplyLogDAL.AddStudentLeaveApplyLog(new EtStudentLeaveApplyLog()
            {
                ApplyOt = DateTime.Now,
                EndDate = request.EndDate,
                EndTime = request.EndTime,
                HandleOt = null,
                HandleRemark = string.Empty,
                HandleStatus = EmStudentLeaveApplyHandleStatus.Unreviewed,
                HandleUser = null,
                IsDeleted = EmIsDeleted.Normal,
                LeaveContent = request.LeaveContent,
                StartDate = request.StartDate,
                StartTime = request.StartTime,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId
            });
            await _studentOperationLogDAL.AddStudentLog(new EtStudentOperationLog()
            {
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"添加请假申请",
                Ot = DateTime.Now,
                Remark = string.Empty,
                StudentId = request.StudentId,
                TenantId = request.LoginTenantId,
                Type = (int)EmStudentOperationLogType.StudentLeaveApply
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentExtendFieldInit(StudentExtendFieldInitRequest request)
        {
            var holidays = await _studentExtendFieldDAL.GetAllStudentExtendField();
            var output = new StudentExtendFieldInitOutput()
            {
                FieldItems = new List<StudentExtendFieldItems>(),
                FieldValues = new List<StudentExtendFieldValues>()
            };
            foreach (var p in holidays)
            {
                output.FieldItems.Add(new StudentExtendFieldItems()
                {
                    CId = p.Id,
                    Name = p.DisplayName
                });
                output.FieldValues.Add(new StudentExtendFieldValues()
                {
                    CId = p.Id,
                    Value = string.Empty
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentMarkGraduation(StudentMarkGraduationRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            await _studentDAL.EditStudentType(request.CId, EmStudentType.HistoryStudent, DateTime.Now);
            await _studentCourseDAL.StudentMarkGraduation(request.CId);
            await _classDAL.RemoveStudent(request.CId);
            _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentType });
            await _userOperationLogDAL.AddUserLog(request, $"标记毕业-姓名:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentMarkReading(StudentMarkReadingRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            await _studentDAL.EditStudentType(request.CId, EmStudentType.ReadingStudent, DateTime.Now);
            _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentType });
            await _userOperationLogDAL.AddUserLog(request, $"标记在读-姓名:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentMarkHidden(StudentMarkHiddenRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            await _studentDAL.EditStudentType(request.CId, EmStudentType.HiddenStudent, DateTime.Now);
            _eventPublisher.Publish(new StatisticsStudentEvent(request.LoginTenantId) { OpType = EmStatisticsStudentType.StudentType });
            await _userOperationLogDAL.AddUserLog(request, $"转为潜在学员-姓名:{studentBucket.Student.Name},手机号码:{studentBucket.Student.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentLeaveApplyPassGet(StudentLeaveApplyPassGetRequest request)
        {
            var studentLeave = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyPassLog(request.Ot.Value);
            var output = new List<StudentLeaveApplyPassGetOutput>();
            if (studentLeave != null && studentLeave.Any())
            {
                foreach (var p in studentLeave)
                {
                    output.Add(new StudentLeaveApplyPassGetOutput()
                    {
                        StudentId = p.StudentId,
                        LeaveDesc = $"{p.StartDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(p.StartTime)}~{p.EndDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(p.EndTime)}"
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentLeaveAboutClassCheckSignGet(StudentLeaveAboutClassCheckSignGetRequest request)
        {
            var studentLeave = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyPassLog(request.Ot.Value);
            var output = new List<StudentLeaveAboutClassCheckSignGetOutput>();
            if (studentLeave != null && studentLeave.Count > 0)
            {
                var studentLeaveCheck = new StudentIsLeaveCheck(studentLeave);
                var myCheckLeaveResult = studentLeaveCheck.GetStudentLeaveList(request.StartTime, request.EndTime, request.Ot.Value.Date);
                if (myCheckLeaveResult.Count > 0)
                {
                    foreach (var p in myCheckLeaveResult)
                    {
                        output.Add(new StudentLeaveAboutClassCheckSignGetOutput()
                        {
                            StudentId = p.StudentId,
                            LeaveDesc = $"{p.StartDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(p.StartTime)}~{p.EndDate.EtmsToDateString()} {EtmsHelper.GetTimeDesc(p.EndTime)}",
                            LeaveContent = p.LeaveContent
                        });
                    }
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentGetByCardNo(StudentGetByCardNoRequest request)
        {
            var student = await _studentDAL.GetStudent(request.CardNo);
            if (student == null)
            {
                return ResponseBase.CommonError("未找到此卡号对应的学员信息");
            }
            var studentRelationship = await _studentRelationshipDAL.GetAllStudentRelationship();
            var tempBoxUser = new DataTempBox<EtUser>();
            var output = new StudentGetByCardNoOutput()
            {
                CId = student.Id,
                AvatarKey = student.Avatar,
                AvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.Avatar),
                CardNo = student.CardNo,
                Gender = student.Gender,
                TrackUser = student.TrackUser,
                GradeId = student.GradeId,
                IntentionLevel = student.IntentionLevel,
                IsBindingWechat = student.IsBindingWechat,
                Tags = student.Tags,
                StudentType = student.StudentType,
                HomeAddress = student.HomeAddress,
                LearningManager = student.LearningManager,
                TrackStatus = student.TrackStatus,
                Name = student.Name,
                Phone = student.Phone,
                PhoneBak = student.PhoneBak,
                SourceId = student.SourceId,
                SchoolName = student.SchoolName,
                Points = student.Points,
                Remark = student.Remark,
                PhoneRelationship = student.PhoneRelationship,
                PhoneBakRelationship = student.PhoneBakRelationship,
                BirthdayDesc = student.Birthday.EtmsToDateString(),
                Age = student.Age,
                EndClassOtDesc = student.EndClassOt == null ? string.Empty : student.EndClassOt.EtmsToDateString(),
                GenderDesc = EmGender.GetGenderDesc(student.Gender),
                IntentionLevelDesc = EmStudentIntentionLevel.GetIntentionLevelDesc(student.IntentionLevel),
                LastTrackTimeDesc = student.LastTrackTime.EtmsToDateString(),
                NextTrackTimeDesc = student.NextTrackTime.EtmsToDateString(),
                StudentTypeDesc = EmStudentType.GetStudentTypeDesc(student.StudentType),
                TrackStatusDesc = EmStudentTrackStatus.GetTrackStatusDesc(student.TrackStatus),
                TrackUserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, student.TrackUser),
                LearningManagerDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, student.LearningManager),
                GradeIdDesc = await GetGradeDesc(student.GradeId),
                PhoneBakRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneBakRelationship, "备用号码"),
                PhoneRelationshipDesc = ComBusiness2.GetStudentRelationshipDesc(studentRelationship, student.PhoneRelationship, "手机号码"),
                SourceIdDesc = await GetSourceDesc(student.SourceId),
                TagsDesc = await GetStudentTagsDesc(student.Tags),
                OtDesc = student.Ot.EtmsToDateString(),
                IsBindingCard = !string.IsNullOrEmpty(student.CardNo),
                IsBindingFaceKey = !string.IsNullOrEmpty(student.FaceKey),
                FaceKeyUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, student.FaceKey)
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentRelieveCardNo(StudentRelieveCardNoRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.CardNo))
            {
                return ResponseBase.CommonError("此学员未绑定卡片");
            }
            await _studentDAL.StudentRelieveCardNo(request.CId, student.CardNo);

            await _userOperationLogDAL.AddUserLog(request, $"解绑学员卡片-姓名:{student.Name},手机号码:{student.Phone},卡号:{student.CardNo}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentBindingCardNo(StudentBindingCardNoRequest request)
        {
            var newCardNo = request.NewCardNo.Trim();
            var bindingCardNoStudent = await _studentDAL.GetStudent(newCardNo);
            if (bindingCardNoStudent != null)
            {
                return ResponseBase.CommonError($"此卡已被学员[{bindingCardNoStudent.Name}]绑定，请先解绑卡片");
            }
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            if (newCardNo == student.CardNo)
            {
                return ResponseBase.CommonError("学员已绑定此卡片");
            }
            await _studentDAL.StudentBindingCardNo(request.CId, newCardNo, student.CardNo);

            await _userOperationLogDAL.AddUserLog(request, $"绑定学员卡片-姓名:{student.Name},手机号码:{student.Phone},卡号:{newCardNo}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentChangePoints(StudentChangePointsRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            if (request.ChangeType == EmChangePointsType.Deduction && student.Points < request.ChangePoints)
            {
                return ResponseBase.CommonError("学员积分不足");
            }
            var type = 0;
            var desc = string.Empty;
            if (request.ChangeType == EmChangePointsType.Deduction)
            {
                await _studentDAL.DeductionPoint(request.CId, request.ChangePoints);
                type = EmStudentPointsLogType.StudentPointsAdjustDeduction;
                desc = "扣除";
            }
            else
            {
                await _studentDAL.AddPoint(request.CId, request.ChangePoints);
                type = EmStudentPointsLogType.StudentPointsAdjustAdd;
                desc = "增加";
            }
            var now = DateTime.Now;
            await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
            {
                IsDeleted = EmIsDeleted.Normal,
                No = string.Empty,
                Ot = now,
                Points = request.ChangePoints,
                Remark = request.Remark,
                StudentId = request.CId,
                TenantId = request.LoginTenantId,
                Type = type
            });

            await _userOperationLogDAL.AddUserLog(request, $"积分调整-学员:{student.Name},手机号码:{student.Phone},{desc}{request.ChangePoints}积分", EmUserOperationType.StudentManage, now);
            return ResponseBase.Success();
        }
    }
}
