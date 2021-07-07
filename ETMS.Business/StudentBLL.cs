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
using ETMS.Entity.Dto.Common.Output;

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

        private readonly IOrderDAL _orderDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        public StudentBLL(IStudentDAL studentDAL, IStudentExtendFieldDAL studentExtendFieldDAL, IUserOperationLogDAL userOperationLogDAL,
            IStudentTagDAL studentTagDAL, IStudentRelationshipDAL studentRelationshipDAL, IStudentSourceDAL studentSourceDAL,
            IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices, IUserDAL userDAL, IGradeDAL gradeDAL,
            IStudentTrackLogDAL studentTrackLogDAL, IStudentOperationLogDAL studentOperationLogDAL,
            IStudentLeaveApplyLogDAL studentLeaveApplyLogDAL, INoticeBLL noticeBLL, IEventPublisher eventPublisher,
            IStudentCourseDAL studentCourseDAL, IClassDAL classDAL, IAiface aiface, IStudentPointsLogDAL studentPointsLogDAL,
            IOrderDAL orderDAL, IIncomeLogDAL incomeLogDAL)
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
            this._orderDAL = orderDAL;
            this._incomeLogDAL = incomeLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._noticeBLL.InitTenantId(tenantId);
            this._aiface.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _studentExtendFieldDAL, _studentTagDAL,
                _userOperationLogDAL, _studentRelationshipDAL, _studentSourceDAL, _userDAL, _gradeDAL, _studentTrackLogDAL, _studentOperationLogDAL,
                _studentLeaveApplyLogDAL, _studentCourseDAL, _classDAL, _studentPointsLogDAL, _orderDAL, _incomeLogDAL);
        }

        public async Task<ResponseBase> StudentDuplicateCheck(StudentDuplicateCheckRequest request)
        {
            if (await _studentDAL.ExistStudent(request.Name, request.Phone))
            {
                return ResponseBase.CommonError("该学员已经存在");
            }
            var logStudents = await _studentDAL.GetStudentsByPhone(request.Phone);
            if (logStudents.Any())
            {
                return ResponseBase.Success(logStudents.Select(p => new StudentDuplicateCheckOutput()
                {
                    StudentId = p.Id,
                    Name = p.Name,
                    Phone = p.Phone,
                    StudentType = p.StudentType,
                    StudentTypeDesc = EmStudentType.GetStudentTypeDesc(p.StudentType)
                }));
            }
            return ResponseBase.Success();
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
            var trackUser = request.TrackUser;
            if (trackUser == null)
            {
                trackUser = request.LoginUserId;
            }
            int? birthdayMonth = null;
            int? birthdayDay = null;
            if (request.Birthday != null)
            {
                birthdayMonth = request.Birthday.Value.Month;
                birthdayDay = request.Birthday.Value.Day;
            }
            var etStudent = new EtStudent()
            {
                BirthdayMonth = birthdayMonth,
                BirthdayDay = birthdayDay,
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
                TrackUser = trackUser,
                NamePinyin = PinyinHelper.GetPinyinInitials(request.Name).ToLower(),
                RecommendStudentId = request.RecommendStudentId
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

            CoreBusiness.ProcessStudentPhoneAboutAdd(etStudent, _eventPublisher);
            SyncParentStudents(etStudent.TenantId, etStudent.Phone, etStudent.PhoneBak);

            if (etStudent.RecommendStudentId != null)
            {
                _eventPublisher.Publish(new StudentRecommendRewardEvent(request.LoginTenantId)
                {
                    Student = etStudent,
                    Type = StudentRecommendRewardType.Registered
                });
            }
            await _userOperationLogDAL.AddUserLog(request, $"添加学员-姓名:{request.Name},手机号码:{request.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success(studentId);
        }

        private void SyncParentStudents(int tenantId, params string[] phones)
        {
            _eventPublisher.Publish(new SyncParentStudentsEvent(tenantId)
            {
                Phones = phones
            }); ;
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
            if (request.RecommendStudentId != null && request.RecommendStudentId.Value == request.CId)
            {
                return ResponseBase.CommonError("推荐人不能选择自己");
            }

            var tags = string.Empty;
            if (request.Tags != null && request.Tags.Any())
            {
                tags = $",{string.Join(',', request.Tags)},";
            }
            int? birthdayMonth = null;
            int? birthdayDay = null;
            if (request.Birthday != null)
            {
                birthdayMonth = request.Birthday.Value.Month;
                birthdayDay = request.Birthday.Value.Day;
            }

            var etStudent = studentBucket.Student;
            var oldAvatar = etStudent.Avatar;
            var oldPhone = etStudent.Phone;
            var oldPhoneBak = etStudent.PhoneBak;
            var isChangeStudentSource = request.SourceId != etStudent.SourceId;

            etStudent.BirthdayMonth = birthdayMonth;
            etStudent.BirthdayDay = birthdayDay;
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
            etStudent.RecommendStudentId = request.RecommendStudentId;
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

            CoreBusiness.ProcessStudentPhoneAboutEdit(oldPhone, oldPhoneBak, etStudent, _eventPublisher);
            SyncParentStudents(etStudent.TenantId, etStudent.Phone, etStudent.PhoneBak, oldPhone, oldPhoneBak);
            await _userOperationLogDAL.AddUserLog(request, $"编辑学员-姓名:{request.Name},手机号码:{request.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        private async Task<ResponseBase> StudentDel(RequestBase request, long studentId, bool isIgnoreCheck, bool isAddLog = true)
        {
            var studentBucket = await _studentDAL.GetStudent(studentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var etStudent = studentBucket.Student;
            if (!isIgnoreCheck)
            {
                if (await _studentDAL.CheckStudentHasOrder(studentId))
                {
                    return ResponseBase.Success(new DelOutput(false, true));
                }
            }
            if (isIgnoreCheck)
            {
                //删除此学员购买课程对应的支付信息
                var myUserOrder = await _orderDAL.GetOrderStudentOt(studentId);
                var ot = new List<DateTime>();
                if (myUserOrder.Any())
                {
                    var ids = new List<long>();
                    foreach (var p in myUserOrder)
                    {
                        ids.Add(p.Id);
                        ot.Add(p.Ot);
                    }
                    await _incomeLogDAL.DelIncomeLog(ids);
                }
                await _studentDAL.DelStudentDepth(studentId);
                if (ot.Any())
                {
                    var myot = ot.Distinct();
                    foreach (var item in myot)
                    {
                        _eventPublisher.Publish(new StatisticsSalesProductEvent(request.LoginTenantId)
                        {
                            StatisticsDate = item
                        });
                        _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
                        {
                            StatisticsDate = item
                        });
                        _eventPublisher.Publish(new StatisticsSalesCourseEvent(request.LoginTenantId)
                        {
                            StatisticsDate = item
                        });
                    }
                }
            }
            else
            {
                await _studentDAL.DelStudent(studentId);
            }
            SyncStatisticsStudentInfo(new StatisticsStudentCountEvent(request.LoginTenantId)
            {
                Time = etStudent.Ot,
                OpType = StatisticsStudentOpType.Deduction,
                ChangeCount = 1
            }, request, etStudent.Ot, true);
            AliyunOssUtil.DeleteObject(etStudent.Avatar, etStudent.FaceGreyKey, etStudent.FaceKey);
            await _aiface.StudentDelete(etStudent.Id);

            CoreBusiness.ProcessStudentPhoneAboutDel(etStudent, _eventPublisher);
            SyncParentStudents(etStudent.TenantId, etStudent.Phone, etStudent.PhoneBak);
            _eventPublisher.Publish(new StudentCourseAnalyzeEvent(request.LoginTenantId)
            {
                StudentId = studentId
            });

            if (isAddLog)
            {
                await _userOperationLogDAL.AddUserLog(request, $"删除学员-姓名:{etStudent.Name},手机号码:{etStudent.Phone}", EmUserOperationType.StudentManage);
            }
            return ResponseBase.Success(new DelOutput(true));
        }

        public async Task<ResponseBase> StudentDel(StudentDelRequest request)
        {
            return await StudentDel(request, request.CId, request.IsIgnoreCheck);
        }

        public async Task<ResponseBase> StudentDelList(StudentDelListRequest request)
        {
            foreach (var studentId in request.CIds)
            {
                if (await _studentDAL.CheckStudentHasOrder(studentId))
                {
                    await StudentDel(request, studentId, true, false);
                }
                else
                {
                    await StudentDel(request, studentId, false, false);
                }
            }
            await _userOperationLogDAL.AddUserLog(request, $"批量删除{request.CIds.Count}个学员", EmUserOperationType.StudentManage);
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
            var recommendStudentDesc = string.Empty;
            if (student.RecommendStudentId != null)
            {
                var recommendStudentBucket = await _studentDAL.GetStudent(student.RecommendStudentId.Value);
                if (recommendStudentBucket != null && recommendStudentBucket.Student != null)
                {
                    recommendStudentDesc = ComBusiness3.GetStudentDescPC(recommendStudentBucket.Student);
                }
            }
            var studentGetOutput = new StudentGetOutput()
            {
                CId = student.Id,
                RecommendStudentId = student.RecommendStudentId,
                RecommendStudentDesc = recommendStudentDesc,
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
                Phone = ComBusiness3.PhoneSecrecy(student.Phone, request.LoginClientType),
                PhoneBak = ComBusiness3.PhoneSecrecy(student.PhoneBak, request.LoginClientType),
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
            var recommendStudentName = string.Empty;
            if (student.RecommendStudentId != null)
            {
                var recommendStudentBucket = await _studentDAL.GetStudent(student.RecommendStudentId.Value);
                if (recommendStudentBucket != null && recommendStudentBucket.Student != null)
                {
                    recommendStudentName = recommendStudentBucket.Student.Name;
                }
            }
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
                RecommendStudentId = student.RecommendStudentId,
                RecommendStudentName = recommendStudentName
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
                Phone = ComBusiness3.PhoneSecrecy(student.Phone, request.LoginClientType),
                PhoneBak = ComBusiness3.PhoneSecrecy(student.PhoneBak, request.LoginClientType),
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
                TrackContent = request.TrackContent,
                TrackImg = EtmsHelper2.GetImgKeys(request.TrackImgKey)
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
                TrackUserName = trackUser?.Name,
                TrackImgUrl = EtmsHelper2.GetImgUrl(lastTrackLog.TrackImg),
                TrackUserAvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, trackUser?.Avatar),
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
                    NextTrackTimeDesc = log.NextTrackTime.EtmsToDateString(),
                    TrackContent = log.TrackContent,
                    TrackUserAvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, user?.Avatar),
                    TrackUserName = user?.Name,
                    TrackImgUrl = EtmsHelper2.GetImgUrl(log.TrackImg)
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
                var myUser = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.TrackUserId);
                trackLog.Add(new StudentTrackLogGetPagingOutput()
                {
                    CId = p.Id,
                    ContentTypeDesc = EmStudentTrackContentType.GetContentTypeDesc(p.ContentType),
                    TrackTimeDesc = p.TrackTime.EtmsToMinuteString(),
                    NextTrackTimeDesc = p.NextTrackTime.EtmsToDateString(),
                    TrackContent = p.TrackContent,
                    StudentDesc = EtmsHelper.PeopleDesc(p.StudentName, p.StudentPhone),
                    TrackUserName = myUser?.Name,
                    TrackImgUrl = EtmsHelper2.GetImgUrl(p.TrackImg),
                    TrackUserAvatarUrl = UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, myUser?.Avatar),
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

        public async Task<ResponseBase> StudentLeaveApplyLogGet(StudentLeaveApplyLogGetRequest request)
        {
            var applyLog = await _studentLeaveApplyLogDAL.GetStudentLeaveApplyLog(request.StudentLeaveApplyLogId);
            if (applyLog == null)
            {
                return ResponseBase.CommonError("请假记录不存在");
            }
            var studentBucket = await _studentDAL.GetStudent(applyLog.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("请假学员不存在");
            }
            var handleUserDesc = string.Empty;
            if (applyLog.HandleUser != null)
            {
                var user = await _userDAL.GetUser(applyLog.HandleUser.Value);
                handleUserDesc = user?.Name;
            }
            var student = studentBucket.Student;
            return ResponseBase.Success(new StudentLeaveApplyLogPagingOutput()
            {
                ApplyOt = applyLog.ApplyOt,
                CId = applyLog.Id,
                EndDateDesc = applyLog.EndDate.EtmsToDateString(),
                EndTimeDesc = EtmsHelper.GetTimeDesc(applyLog.EndTime),
                StartDateDesc = applyLog.StartDate.EtmsToDateString(),
                StartTimeDesc = EtmsHelper.GetTimeDesc(applyLog.StartTime),
                HandleRemark = applyLog.HandleRemark,
                HandleOtDesc = applyLog.HandleOt == null ? string.Empty : applyLog.HandleOt.EtmsToString(),
                HandleStatus = applyLog.HandleStatus,
                HandleStatusDesc = EmStudentLeaveApplyHandleStatus.GetStudentLeaveApplyHandleStatusDesc(applyLog.HandleStatus),
                HandleUser = applyLog.HandleUser,
                HandleUserDesc = handleUserDesc,
                LeaveContent = applyLog.LeaveContent,
                StudentName = student.Name,
                StudentPhone = ComBusiness3.PhoneSecrecy(student.Phone, request.LoginClientType)
            });
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
                    StudentPhone = ComBusiness3.PhoneSecrecy(p.StudentPhone, request.LoginClientType)
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
            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));

            await _userOperationLogDAL.AddUserLog(request, "审核请假记录", EmUserOperationType.StudentLeaveApplyManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentLeaveApplyAdd(StudentLeaveApplyAddRequest request)
        {
            var log = new EtStudentLeaveApplyLog()
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
            };
            await _studentLeaveApplyLogDAL.AddStudentLeaveApplyLog(log);
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

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            _eventPublisher.Publish(new NoticeUserStudentLeaveApplyEvent(request.LoginTenantId)
            {
                StudentLeaveApplyLog = log
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
            _eventPublisher.Publish(new StudentCourseAnalyzeEvent(request.LoginTenantId)
            {
                StudentId = request.CId
            });
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

        public async Task<ResponseBase> StudentChangePwd(StudentChangePwdRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            var newPwd = CryptogramHelper.Encrypt3DES(request.NewPwd, SystemConfig.CryptogramConfig.Key);
            await _studentDAL.ChangePwd(student.Id, newPwd);

            await _userOperationLogDAL.AddUserLog(request, $"修改家长端登录密码-学员:{student.Name},手机号码:{student.Phone}", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }
    }
}
