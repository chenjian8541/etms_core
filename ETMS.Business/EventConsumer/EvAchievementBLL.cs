using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Achievement;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvAchievementBLL : IEvAchievementBLL
    {
        private readonly IAchievementDAL _achievementDAL;

        private readonly IEventPublisher _eventPublisher;

        public EvAchievementBLL(IAchievementDAL achievementDAL, IEventPublisher eventPublisher)
        {
            this._achievementDAL = achievementDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _achievementDAL);
        }

        public async Task SyncAchievementAllConsumerEvent(SyncAchievementAllEvent request)
        {
            var p = await _achievementDAL.GetAchievement(request.AchievementId);
            if (p == null)
            {
                return;
            }
            var temp = await _achievementDAL.GetAchievementDetail(request.AchievementId);
            if (temp.Count == 0)
            {
                return;
            }
            var myDetails = temp.OrderByDescending(p => p.ScoreMy).ToList();

            var rankMy = 0;
            var thisScore = 0M;
            var studentCount = 0;
            var studentInCount = 0;
            var studentMissCount = 0;
            var totalScore = 0M;
            foreach (var item in myDetails)
            {
                studentCount++;
                totalScore += item.ScoreMy;
                if (item.CheckStatus == EmAchievementDetailCheckStatus.Join)
                {
                    studentInCount++;
                }
                else
                {
                    studentMissCount++;
                }

                if (rankMy == 0)
                {
                    rankMy = 1;
                    item.RankMy = rankMy;
                    thisScore = item.ScoreMy;
                }
                else
                {
                    if (thisScore == item.ScoreMy)
                    {
                        item.RankMy = rankMy;
                    }
                    else
                    {
                        rankMy++;
                        item.RankMy = rankMy;
                        thisScore = item.ScoreMy;
                    }
                }
                item.Name = p.Name;
                item.SubjectId = p.SubjectId;
                item.ScoreTotal = p.ScoreTotal;
                item.ExamOt = p.ExamOt;
                item.ShowParent = p.ShowParent;
                item.ShowRankParent = p.ShowRankParent;
                item.Status = p.Status;
            }

            p.IsCalculate = EmBool.False;
            p.StudentCount = studentCount;
            p.StudentInCount = studentInCount;
            p.StudentMissCount = studentMissCount;
            p.ScoreMax = temp.Min(j => j.ScoreMy);
            p.ScoreMax = temp.Max(j => j.ScoreMy);
            if (studentInCount > 0)
            {
                p.ScoreAverage = totalScore / studentInCount;
            }
            else
            {
                p.ScoreAverage = 0;
            }

            await _achievementDAL.EditAchievement(p);
            await _achievementDAL.EditAchievementDetails(myDetails);

            if (request.IsSendStudentNotice)
            {
                _eventPublisher.Publish(new NoticeStudentsAchievementEvent(request.TenantId)
                {
                    AchievementId = request.AchievementId,
                    AchievementDetailList = myDetails
                });
            }
        }
    }
}
