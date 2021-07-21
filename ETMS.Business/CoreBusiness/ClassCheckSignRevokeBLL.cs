using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Event.DataContract;
using ETMS.Entity.Database.Source;
using ETMS.Utility;
using ETMS.IEventProvider;
using ETMS.Event.DataContract.Statistics;

namespace ETMS.Business
{
    public class ClassCheckSignRevokeBLL : IClassCheckSignRevokeBLL
    {
        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IClassTimesDAL _classTimesDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly ICommonHandlerBLL _commonHandlerBLL;

        public ClassCheckSignRevokeBLL(IClassRecordDAL classRecordDAL, IStudentCourseDAL studentCourseDAL,
            IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL, IStudentDAL studentDAL, IStudentPointsLogDAL studentPointsLogDAL,
            IUserDAL userDAL, IEventPublisher eventPublisher, IClassTimesDAL classTimesDAL, IStudentCheckOnLogDAL studentCheckOnLogDAL,
            ICommonHandlerBLL commonHandlerBLL)
        {
            this._classRecordDAL = classRecordDAL;
            this._studentCourseDAL = studentCourseDAL;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
            this._studentDAL = studentDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._userDAL = userDAL;
            this._eventPublisher = eventPublisher;
            this._classTimesDAL = classTimesDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._commonHandlerBLL = commonHandlerBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this._commonHandlerBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _classRecordDAL, _studentCourseDAL, _studentDAL, _studentCourseConsumeLogDAL,
                _studentPointsLogDAL, _userDAL, _classTimesDAL, _studentCheckOnLogDAL);
        }

        public async Task<ResponseBase> ClassCheckSignRevoke(ClassCheckSignRevokeRequest request)
        {
            var classRecord = await _classRecordDAL.GetClassRecord(request.CId);
            if (classRecord == null)
            {
                return ResponseBase.CommonError("点名记录不存在");
            }
            if (classRecord.Status == EmClassRecordStatus.Revoked)
            {
                return ResponseBase.CommonError("此点名记录已撤销");
            }
            if (DateTime.Now.Date.Subtract(classRecord.CheckOt.Date).TotalDays > 30)  //无法撤销30天前的点名记录
            {
                return ResponseBase.CommonError("无法撤销30天前的点名记录");
            }
            var classRecordAbsenceLogs = await _classRecordDAL.GetClassRecordAbsenceLogByClassRecordId(request.CId);
            var isMarkFinish = classRecordAbsenceLogs.FirstOrDefault(p => p.HandleStatus == EmClassRecordAbsenceHandleStatus.MarkFinish);
            if (isMarkFinish != null)
            {
                return ResponseBase.CommonError("存在补课记录无法撤销");
            }
            await _classRecordDAL.SetClassRecordRevoke(request.CId);
            _eventPublisher.Publish(new ClassCheckSignRevokeEvent(request.LoginTenantId)
            {
                ClassRecordId = classRecord.Id,
                UserId = request.LoginUserId
            });
            return ResponseBase.Success();
        }

        public async Task ClassCheckSignRevokeEvent(ClassCheckSignRevokeEvent request)
        {
            var now = DateTime.Now;
            var classRecord = await _classRecordDAL.GetClassRecord(request.ClassRecordId);
            var classRecordStudents = await _classRecordDAL.GetClassRecordStudents(request.ClassRecordId);
            var studentCourseConsumeLogs = new List<EtStudentCourseConsumeLog>();
            foreach (var p in classRecordStudents)
            {
                //退还扣的课时
                if (p.DeType == EmDeClassTimesType.ClassTimes && p.DeClassTimes > 0 && p.DeStudentCourseDetailId != null)
                {
                    await _studentCourseDAL.AddClassTimesOfStudentCourseDetail(p.DeStudentCourseDetailId.Value, p.DeClassTimes);

                    //课消记录
                    studentCourseConsumeLogs.Add(new EtStudentCourseConsumeLog()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        DeClassTimes = p.DeClassTimes,
                        DeClassTimesSmall = 0,
                        CourseId = p.CourseId,
                        DeType = EmDeClassTimesType.ClassTimes,
                        OrderId = 0,
                        OrderNo = string.Empty,
                        Ot = now,
                        SourceType = EmStudentCourseConsumeSourceType.UndoStudentClassRecord,
                        StudentId = p.StudentId,
                        TenantId = p.TenantId
                    });

                    await _commonHandlerBLL.AnalyzeStudentCourseDetailRestoreNormalStatus(p.DeStudentCourseDetailId.Value);
                }
                //超上课时
                if (p.ExceedClassTimes > 0)
                {
                    await _studentCourseDAL.DeExceedTotalClassTimes(p.StudentId, p.CourseId, p.ExceedClassTimes);
                }
                _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(p.TenantId)
                {
                    StudentId = p.StudentId,
                    CourseId = p.CourseId,
                    IsSendNoticeStudent = true
                });
            }
            if (studentCourseConsumeLogs.Any())
            {
                _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(studentCourseConsumeLogs); //课消记录
            }

            //积分申请
            var studentPointsLogs = new List<EtStudentPointsLog>();
            var classRecordPointsApplyLogs = await _classRecordDAL.GetClassRecordPointsApplyLogByClassRecordId(classRecord.Id);
            if (classRecordPointsApplyLogs.Any())
            {
                foreach (var p in classRecordPointsApplyLogs)
                {
                    if (p.HandleStatus == EmClassRecordPointsApplyHandleStatus.CheckPass)
                    {
                        await _studentDAL.DeductionPoint(p.StudentId, p.Points);
                        studentPointsLogs.Add(new EtStudentPointsLog()
                        {
                            IsDeleted = EmIsDeleted.Normal,
                            No = string.Empty,
                            Ot = now,
                            Points = p.Points,
                            Remark = string.Empty,
                            StudentId = p.StudentId,
                            TenantId = p.TenantId,
                            Type = EmStudentPointsLogType.ClassCheckSignRevoke
                        });
                    }
                }
            }
            if (studentPointsLogs.Any())
            {
                _studentPointsLogDAL.AddStudentPointsLog(studentPointsLogs);
            }

            //老师课时统计
            var teachers = classRecord.Teachers.Trim(',').Split(',');
            foreach (var t in teachers)
            {
                var teacherId = t.ToLong();
                await _userDAL.DeTeacherClassTimesInfo(teacherId, classRecord.ClassTimes, 1);
                await _userDAL.DeTeacherMonthClassTimes(teacherId, classRecord.ClassOt, classRecord.ClassTimes, 1);
            }

            await _classRecordDAL.AddClassRecordOperationLog(new EtClassRecordOperationLog()
            {
                ClassId = classRecord.ClassId,
                ClassRecordId = classRecord.Id,
                IsDeleted = classRecord.IsDeleted,
                OpContent = "撤销点名记录",
                OpType = EmClassRecordOperationType.UndoClassRecord,
                Ot = now,
                Remark = string.Empty,
                Status = classRecord.Status,
                TenantId = classRecord.TenantId,
                UserId = request.UserId
            });

            //排课课次
            if (classRecord.ClassTimesId != null)
            {
                await _classTimesDAL.UpdateClassTimesClassCheckSignRevoke(classRecord.ClassTimesId.Value, EmClassTimesStatus.UnRollcall);
                //撤销考勤记上课
                await _studentCheckOnLogDAL.RevokeCheckSign(classRecord.ClassTimesId.Value);
                _eventPublisher.Publish(new SyncClassTimesStudentEvent(classRecord.TenantId)
                {
                    ClassTimesId = classRecord.ClassTimesId.Value
                });
            }
            _eventPublisher.Publish(new NoticeStudentClassCheckSignRevokeEvent(classRecord.TenantId)
            {
                ClassRecord = classRecord,
                ClassRecordStudent = classRecordStudents
            });

            //统计信息
            _eventPublisher.Publish(new StatisticsClassRevokeEvent(request.TenantId)
            {
                ClassRecord = classRecord
            });

            _eventPublisher.Publish(new StatisticsClassFinishCountEvent(request.TenantId)
            {
                ClassId = classRecord.ClassId
            });

            if (!EtmsHelper2.IsThisMonth(classRecord.ClassOt))
            {
                _eventPublisher.Publish(new StatisticsEducationEvent(request.TenantId)
                {
                    Time = classRecord.ClassOt
                });
            }
        }
    }
}
