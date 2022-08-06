using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.Open3.Output;
using ETMS.Entity.Dto.Open3.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness.Open;
using ETMS.IDataAccess;
using ETMS.IDataAccess.ShareTemplate;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class Open3BLL : IOpen3BLL
    {
        private readonly IAchievementDAL _achievementDAL;

        private readonly ISubjectDAL _subjectDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IShareTemplateUseTypeDAL _shareTemplateUseTypeDAL;

        public Open3BLL(IAchievementDAL achievementDAL, ISubjectDAL subjectDAL, IStudentDAL studentDAL, IShareTemplateUseTypeDAL shareTemplateUseTypeDAL)
        {
            this._achievementDAL = achievementDAL;
            this._subjectDAL = subjectDAL;
            this._studentDAL = studentDAL;
            this._shareTemplateUseTypeDAL = shareTemplateUseTypeDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _achievementDAL, _subjectDAL, _studentDAL, _shareTemplateUseTypeDAL);
        }

        public async Task<ResponseBase> AchievementDetailGet(AchievementDetailGetRequest request)
        {
            var p = await _achievementDAL.GetAchievementDetailById(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("成绩单不存在");
            }
            var myAchievement = await _achievementDAL.GetAchievement(p.AchievementId);
            if (myAchievement == null)
            {
                return ResponseBase.CommonError("成绩单不存在");
            }
            var studentBucket = await _studentDAL.GetStudent(p.StudentId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            if (p.Status == EmAchievementStatus.Save)
            {
                return ResponseBase.CommonError("成绩单未发布");
            }
            if (p.ShowParent == EmBool.False)
            {
                return ResponseBase.CommonError("成绩单未开放");
            }
            if (p.ReadStatus == EmBool.False)
            {
                await _achievementDAL.SetAchievementDetailIsRead(p.AchievementId, p.Id);
            }
            var myAllSubject = await _subjectDAL.GetAllSubject();
            var mySubject = myAllSubject.FirstOrDefault(a => a.Id == p.SubjectId);
            var output = new AchievementDetailGetOutput()
            {
                CheckStatus = p.CheckStatus,
                Comment = p.Comment,
                ExamOt = p.ExamOt.EtmsToDateString(),
                Name = p.Name,
                RankMy = p.RankMy,
                ScoreAverage = myAchievement.ScoreAverage.EtmsToString3(),
                ScoreMax = myAchievement.ScoreMax.EtmsToString3(),
                ScoreTotal = myAchievement.ScoreTotal.EtmsToString3(),
                ScoreMy = p.ScoreMy.EtmsToString3(),
                ShowRankParent = p.ShowRankParent,
                StudentAvatar = AliyunOssUtil.GetAccessUrlHttps(studentBucket.Student.Avatar),
                StudentName = studentBucket.Student.Name,
                SubjectName = mySubject?.Name,
                StudentGender = studentBucket.Student.Gender
            };
            var shareTemplateBucket = await _shareTemplateUseTypeDAL.GetShareTemplate(EmShareTemplateUseType.Achievement);
            if (shareTemplateBucket != null && shareTemplateBucket.MyShareTemplateLink != null)
            {
                output.ShareContent = ShareTemplateHandler.TemplateLinkAchievement(shareTemplateBucket.MyShareTemplateLink,
                   studentBucket.Student.Name, output.Name, output.SubjectName, output.ExamOt, output.ScoreMy);
            }
            return ResponseBase.Success(output);
        }
    }
}
