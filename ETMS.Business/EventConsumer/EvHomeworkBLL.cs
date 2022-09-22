using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvHomeworkBLL : IEvHomeworkBLL
    {
        private readonly IActiveHomeworkDAL _activeHomeworkDAL;

        private readonly IActiveHomeworkDetailDAL _activeHomeworkDetailDAL;

        private readonly IEventPublisher _eventPublisher;

        public EvHomeworkBLL(IActiveHomeworkDAL activeHomeworkDAL, IActiveHomeworkDetailDAL activeHomeworkDetailDAL,
            IEventPublisher eventPublisher)
        {
            this._activeHomeworkDAL = activeHomeworkDAL;
            this._activeHomeworkDetailDAL = activeHomeworkDetailDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _activeHomeworkDAL, _activeHomeworkDetailDAL);
        }

        public async Task GenerateContinuousHomeworkConsumerEvent(GenerateContinuousHomeworkEvent request)
        {
            var now = DateTime.Now;
            var ot = request.MyDate.Date;
            var needCreateHomeword = await _activeHomeworkDAL.GetNeedCreateContinuousHomework(ot);
            if (needCreateHomeword == null || needCreateHomeword.Count == 0)
            {
                return;
            }
            foreach (var p in needCreateHomeword)
            {
                await GenerateContinuousHomeworkProcess(p, ot, now);
            }
        }

        private async Task GenerateContinuousHomeworkProcess(EtActiveHomework homework,
            DateTime date, DateTime now)
        {
            if (string.IsNullOrEmpty(homework.StudentIds))
            {
                LOG.Log.Error("[GenerateContinuousHomeworkProcess]未找到存储的学员Id信息", homework, this.GetType());
                return;
            }
            var isExecuted = await _activeHomeworkDetailDAL.ExistHomeworkDetail(homework.Id, date);
            if (isExecuted)
            {
                return;
            }
            var ids = EtmsHelper.AnalyzeMuIds(homework.StudentIds);
            var details = new List<EtActiveHomeworkDetail>();
            foreach (var studentId in ids)
            {
                details.Add(new EtActiveHomeworkDetail()
                {
                    AnswerContent = string.Empty,
                    AnswerMedias = string.Empty,
                    AnswerOt = null,
                    AnswerStatus = EmActiveHomeworkDetailAnswerStatus.Unanswered,
                    ClassId = homework.ClassId,
                    CreateUserId = homework.CreateUserId,
                    ExDate = null,
                    HomeworkId = homework.Id,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    ReadStatus = EmActiveHomeworkDetailReadStatus.No,
                    StudentId = studentId,
                    TenantId = homework.TenantId,
                    Title = homework.Title,
                    Type = EmActiveHomeworkType.ContinuousWork,
                    WorkContent = homework.WorkContent,
                    WorkMedias = homework.WorkMedias,
                    OtDate = date,
                    LxExTime = homework.LxExTime
                });
            }
            _activeHomeworkDetailDAL.AddActiveHomeworkDetail(details);
            await _activeHomeworkDAL.ResetHomeworkStudentAnswerStatus(homework.Id);
            _eventPublisher.Publish(new NoticeStudentsOfHomeworkAddEvent(homework.TenantId)
            {
                HomeworkId = homework.Id
            });
        }

        public async Task SyncHomeworkReadAndFinishCountConsumerEvent(SyncHomeworkReadAndFinishCountEvent request)
        {
            if (request.OpType == SyncHomeworkReadAndFinishCountOpType.Read)
            {
                await _activeHomeworkDAL.HomeworkStudentSetReadStatus(request.HomeworkId, request.StudentId,
                    EmActiveHomeworkDetailReadStatus.Yes);
            }
            else
            {
                var newAnswerStatus = await _activeHomeworkDetailDAL.GetHomeworkStudentAnswerStatus(request.HomeworkId, request.StudentId);
                await _activeHomeworkDAL.HomeworkStudentSetAnswerStatus(request.HomeworkId, request.StudentId,
                    newAnswerStatus);
            }
            var answerAndReadInfo = await _activeHomeworkDAL.GetAnswerAndReadCount(request.HomeworkId);
            await _activeHomeworkDAL.UpdateHomeworkAnswerAndReadCount(request.HomeworkId,
                answerAndReadInfo.ReadCount, answerAndReadInfo.AnswerCount);
        }

        public async Task ActiveHomeworkEditConsumerEvent(ActiveHomeworkEditEvent request)
        {
            var myHomework = request.ActiveHomework;
            await _activeHomeworkDetailDAL.UpdateHomeworkDetail(myHomework.Id, myHomework.Title,
                myHomework.WorkContent, myHomework.WorkMedias);

            _eventPublisher.Publish(new NoticeStudentsOfHomeworkEditEvent(request.TenantId)
            {
                HomeworkId = myHomework.Id
            });
        }
    }
}
