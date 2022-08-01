using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Output;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Dto.Educational3.Request;
using ETMS.Entity.Dto.Educational3.Output;
using ETMS.Event.DataContract.Achievement;

namespace ETMS.Business
{
    public class AchievementBLL : IAchievementBLL
    {
        private readonly IAchievementDAL _achievementDAL;

        private readonly ISubjectDAL _subjectDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IEventPublisher _eventPublisher;

        public AchievementBLL(IAchievementDAL achievementDAL, ISubjectDAL subjectDAL, IStudentDAL studentDAL, IUserOperationLogDAL userOperationLogDAL,
            IEventPublisher eventPublisher)
        {
            this._achievementDAL = achievementDAL;
            this._subjectDAL = subjectDAL;
            this._studentDAL = studentDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _achievementDAL, _subjectDAL, _studentDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> AchievementGetPaging(AchievementGetPagingRequest request)
        {
            var pagingData = await _achievementDAL.GetPaging(request);
            var output = new List<AchievementGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var allSubject = await _subjectDAL.GetAllSubject();
                foreach (var p in pagingData.Item1)
                {
                    var mySubject = allSubject.FirstOrDefault(j => j.Id == p.SubjectId);
                    output.Add(new AchievementGetPagingOutput()
                    {
                        CId = p.Id,
                        ExamOt = p.ExamOt.EtmsToDateString(),
                        Name = p.Name,
                        ScoreAverage = p.ScoreAverage,
                        ScoreMax = p.ScoreMax,
                        ScoreMin = p.ScoreMin,
                        ScoreTotal = p.ScoreTotal,
                        ShowParent = p.ShowParent,
                        ShowRankParent = p.ShowRankParent,
                        SourceType = p.SourceType,
                        SourceTypeDesc = EmAchievementSourceType.GetAchievementSourceTypeDesc(p.SourceType),
                        Status = p.Status,
                        StatusDesc = EmAchievementStatus.GetAchievementStatusDesc(p.Status),
                        StudentCount = p.StudentCount,
                        StudentInCount = p.StudentInCount,
                        StudentMissCount = p.StudentMissCount,
                        SubjectId = p.SubjectId,
                        SubjectName = mySubject?.Name,
                        IsCalculate = p.IsCalculate,
                        StudenReadCount = p.StudenReadCount,
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AchievementGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AchievementDetailGetPaging(AchievementDetailGetPagingRequest request)
        {
            var pagingData = await _achievementDAL.GetPagingDetail(request);
            var output = new List<AchievementDetailGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var allSubject = await _subjectDAL.GetAllSubject();
                var tempBoxStudent = new DataTempBox<EtStudent>();
                foreach (var p in pagingData.Item1)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    if (myStudent == null)
                    {
                        continue;
                    }
                    var mySubject = allSubject.FirstOrDefault(j => j.Id == p.SubjectId);
                    output.Add(new AchievementDetailGetPagingOutput()
                    {
                        ShowRankParent = p.ShowRankParent,
                        ShowParent = p.ShowParent,
                        AchievementId = p.AchievementId,
                        CheckStatus = p.CheckStatus,
                        Comment = p.Comment,
                        ExamOt = p.ExamOt.EtmsToDateString(),
                        Name = p.Name,
                        RankMy = p.RankMy,
                        ReadStatus = p.ReadStatus,
                        ScoreMy = p.ScoreMy,
                        ScoreTotal = p.ScoreTotal,
                        SourceType = p.SourceType,
                        Status = p.Status,
                        StudentId = p.StudentId,
                        StudentName = myStudent.Name,
                        SubjectId = p.SubjectId,
                        SubjectName = mySubject?.Name,
                        CId = p.Id,
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AchievementDetailGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AchievementGet(AchievementGetRequest request)
        {
            var p = await _achievementDAL.GetAchievement(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("成绩单不存在");
            }
            var output = new AchievementGetOutput()
            {
                CId = p.Id,
                ExamOt = p.ExamOt.EtmsToDateString(),
                Name = p.Name,
                ScoreAverage = p.ScoreAverage,
                ScoreMax = p.ScoreMax,
                ScoreMin = p.ScoreMin,
                ScoreTotal = p.ScoreTotal,
                ShowParent = p.ShowParent,
                ShowRankParent = p.ShowRankParent,
                SourceType = p.SourceType,
                Status = p.Status,
                StudentCount = p.StudentCount,
                StudentInCount = p.StudentInCount,
                StudentMissCount = p.StudentMissCount,
                SubjectId = p.SubjectId,
                Students = new List<AchievementStudentOutput>()
            };
            var myAchievementDetail = await _achievementDAL.GetAchievementDetail(request.CId);
            if (myAchievementDetail.Any())
            {
                var tempDetail = myAchievementDetail.OrderBy(p => p.RankMy);
                foreach (var item in tempDetail)
                {
                    var studentBucket = await _studentDAL.GetStudent(item.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    output.Students.Add(new AchievementStudentOutput()
                    {
                        CheckStatus = item.CheckStatus,
                        Comment = item.Comment,
                        ScoreMy = item.ScoreMy,
                        StudentId = item.StudentId,
                        StudentName = studentBucket.Student.Name,
                        StudentPhone = ComBusiness3.PhoneSecrecy(studentBucket.Student.Phone, request.SecrecyType),
                        ReadStatus = item.ReadStatus,
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> AchievementAdd(AchievementAddRequest request)
        {
            var status = request.IsPublish ? EmAchievementStatus.Publish : EmAchievementStatus.Save;
            var now = DateTime.Now;
            var myAchievement = new EtAchievement()
            {
                IsCalculate = EmBool.True,
                ExamOt = request.ExamOt.Value,
                CreateTime = now,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                ScoreAverage = 0,
                ScoreMax = 0,
                ScoreMin = 0,
                ScoreTotal = request.ScoreTotal,
                ShowParent = request.ShowParent,
                ShowRankParent = request.ShowRankParent,
                SourceType = request.SourceType,
                Status = status,
                StudentCount = request.Students.Count,
                SubjectId = request.SubjectId.Value,
                TenantId = request.LoginTenantId,
                UserId = request.LoginUserId
            };
            var myAchievementDetails = new List<EtAchievementDetail>();
            foreach (var p in request.Students)
            {
                if (p.CheckStatus == EmAchievementDetailCheckStatus.Join)
                {
                    myAchievement.StudentInCount++;
                }
                else
                {
                    myAchievement.StudentMissCount++;
                }
                myAchievementDetails.Add(new EtAchievementDetail()
                {
                    ExamOt = myAchievement.ExamOt,
                    CreateTime = now,
                    Comment = p.Comment,
                    CheckStatus = p.CheckStatus,
                    IsDeleted = myAchievement.IsDeleted,
                    Name = myAchievement.Name,
                    RankMy = 0,
                    ScoreMy = p.ScoreMy,
                    ScoreTotal = myAchievement.ScoreTotal,
                    ShowParent = myAchievement.ShowParent,
                    ShowRankParent = myAchievement.ShowRankParent,
                    SourceType = myAchievement.SourceType,
                    Status = status,
                    StudentId = p.StudentId,
                    SubjectId = myAchievement.SubjectId,
                    TenantId = myAchievement.TenantId,
                    UserId = myAchievement.UserId
                });
            }
            await _achievementDAL.AddAchievement(myAchievement, myAchievementDetails);

            _eventPublisher.Publish(new SyncAchievementAllEvent(request.LoginTenantId)
            {
                AchievementId = myAchievement.Id,
                IsSendStudentNotice = request.ShowParent == EmBool.True && status == EmAchievementStatus.Publish
            });

            await _userOperationLogDAL.AddUserLog(request, $"添加成绩单—{request.Name}", EmUserOperationType.Achievement);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AchievementDel(AchievementDelRequest request)
        {
            var p = await _achievementDAL.GetAchievement(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("成绩单不存在");
            }
            await _achievementDAL.DelAchievement(p.Id);

            await _userOperationLogDAL.AddUserLog(request, $"删除成绩单—{p.Name}", EmUserOperationType.Achievement);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AchievementEdit(AchievementEditRequest request)
        {
            var p = await _achievementDAL.GetAchievement(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("成绩单不存在");
            }
            var isChanged = false;
            if (request.StudentChange != null)
            {
                var studentChange = request.StudentChange;
                if (studentChange.DelDetailIds != null && studentChange.DelDetailIds.Any())
                {
                    await _achievementDAL.DelAchievementDetail(studentChange.DelDetailIds);
                    isChanged = true;
                }
                if (studentChange.Edits != null && studentChange.Edits.Any())
                {
                    isChanged = true;
                    var editStudents = studentChange.Edits.Where(j => j.DetailId > 0);
                    if (editStudents.Any())
                    {
                        //编辑
                        var myHisDetails = await _achievementDAL.GetAchievementDetail(request.CId);
                        var myAchievementDetailEdits = new List<EtAchievementDetail>();
                        foreach (var j in editStudents)
                        {
                            var hisItem = myHisDetails.FirstOrDefault(a => a.Id == j.DetailId);
                            if (hisItem == null)
                            {
                                continue;
                            }
                            hisItem.CheckStatus = j.CheckStatus;
                            hisItem.ScoreMy = j.ScoreMy;
                            hisItem.Comment = j.Comment;
                            myAchievementDetailEdits.Add(hisItem);
                        }
                        if (myAchievementDetailEdits.Any())
                        {
                            await _achievementDAL.EditAchievementDetails(myAchievementDetailEdits);
                        }
                    }

                    var addStudents = studentChange.Edits.Where(j => j.DetailId == 0);
                    if (addStudents.Any())
                    {
                        //新增
                        var myAchievementDetailAdds = new List<EtAchievementDetail>();
                        foreach (var j in addStudents)
                        {
                            myAchievementDetailAdds.Add(new EtAchievementDetail()
                            {
                                AchievementId = p.Id,
                                Comment = j.Comment,
                                CheckStatus = j.CheckStatus,
                                CreateTime = p.CreateTime,
                                ExamOt = p.ExamOt,
                                IsDeleted = p.IsDeleted,
                                Name = p.Name,
                                RankMy = 0,
                                ScoreMy = j.ScoreMy,
                                ScoreTotal = p.ScoreTotal,
                                ShowParent = p.ShowParent,
                                ShowRankParent = p.ShowRankParent,
                                SourceType = p.SourceType,
                                Status = p.Status,
                                StudentId = j.StudentId,
                                SubjectId = p.SubjectId,
                                TenantId = p.TenantId,
                                UserId = p.UserId
                            });
                        }
                        await _achievementDAL.AddAchievementDetails(myAchievementDetailAdds);
                    }
                }
            }

            if (isChanged || p.Name != request.Name || p.ShowRankParent != request.ShowRankParent)
            {
                if (isChanged)
                {
                    p.IsCalculate = EmBool.True;
                }
                p.Name = request.Name;
                p.ShowRankParent = request.ShowRankParent;
                await _achievementDAL.EditAchievement(p);
                if (p.Name != request.Name || p.ShowRankParent != request.ShowRankParent)
                {
                    await _achievementDAL.UpdateAchievementDetail(request.CId, request.Name, request.ShowRankParent);
                }
                isChanged = true;
            }

            if (isChanged)
            {
                _eventPublisher.Publish(new SyncAchievementAllEvent(request.LoginTenantId)
                {
                    AchievementId = p.Id,
                    IsSendStudentNotice = p.ShowParent == EmBool.True && p.Status == EmAchievementStatus.Publish
                });
            }

            await _userOperationLogDAL.AddUserLog(request, $"编辑成绩单—{p.Name}", EmUserOperationType.Achievement);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AchievementPush(AchievementPushRequest request)
        {
            var p = await _achievementDAL.GetAchievement(request.CId);
            if (p == null)
            {
                return ResponseBase.CommonError("成绩单不存在");
            }
            await _achievementDAL.PushAchievement(p.Id);
            _eventPublisher.Publish(new NoticeStudentsAchievementEvent(request.LoginTenantId)
            {
                AchievementId = request.CId
            });

            await _userOperationLogDAL.AddUserLog(request, $"发布成绩单—{p.Name}", EmUserOperationType.Achievement);
            return ResponseBase.Success();
        }
    }
}
