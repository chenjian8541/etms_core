using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.LOG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;
using ETMS.IEventProvider;

namespace ETMS.Business
{
    public class JobAnalyzeBLL : IJobAnalyzeBLL
    {
        private readonly IJobAnalyzeDAL _analyzeClassTimesDAL;

        private readonly IClassDAL _classDAL;

        private readonly IHolidaySettingDAL _holidaySettingDAL;

        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IJobAnalyzeDAL _jobAnalyzeDAL;

        private readonly IParentStudentDAL _parentStudentDAL;

        public JobAnalyzeBLL(IJobAnalyzeDAL analyzeClassTimesDAL, IClassDAL classDAL, IHolidaySettingDAL holidaySettingDAL, IClassRecordDAL classRecordDAL,
            IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL, IEventPublisher eventPublisher, IJobAnalyzeDAL jobAnalyzeDAL,
            IParentStudentDAL parentStudentDAL)
        {
            this._analyzeClassTimesDAL = analyzeClassTimesDAL;
            this._classDAL = classDAL;
            this._holidaySettingDAL = holidaySettingDAL;
            this._classRecordDAL = classRecordDAL;
            this._eventPublisher = eventPublisher;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
            this._jobAnalyzeDAL = jobAnalyzeDAL;
            this._parentStudentDAL = parentStudentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _analyzeClassTimesDAL, _classDAL,
                _holidaySettingDAL, _classRecordDAL, _studentCourseConsumeLogDAL, _jobAnalyzeDAL, _parentStudentDAL);
        }

        public void ResetTenantId(int tenantId)
        {
            this.ResetDataAccess(tenantId, _analyzeClassTimesDAL, _classDAL, _holidaySettingDAL, _classRecordDAL, _studentCourseConsumeLogDAL, _jobAnalyzeDAL);
        }

        public async Task UpdateClassTimesRuleLoopStatus()
        {
            await _analyzeClassTimesDAL.UpdateClassTimesRuleLoopStatus();
        }

        public async Task<Tuple<IEnumerable<LoopClassTimesRule>, int>> GetNeedLoopClassTimesRule(int pageSize, int pageCurrent)
        {
            return await _analyzeClassTimesDAL.GetNeedLoopClassTimesRule(pageSize, pageCurrent);
        }

        public async Task GenerateClassTimesEvent(GenerateClassTimesEvent request)
        {
            var classRule = await _analyzeClassTimesDAL.GetClassTimesRule(request.ClassTimesRuleId);
            if (classRule == null || !classRule.IsNeedLoop)
            {
                return;
            }
            if (classRule.EndDate != null && classRule.LastJobProcessTime >= classRule.EndDate)
            {
                return;
            }
            var etClass = await _classDAL.GetClassBucket(request.ClassId);
            if (etClass == null || etClass.EtClass == null)
            {
                Log.Error($"[定时生成课次]排课规则所属班级不存在],tenantId:{request.TenantId},ruleId:{request.ClassTimesRuleId},classId:{request.ClassId}", this.GetType());
                return;
            }
            var currentDate = classRule.LastJobProcessTime.Date;
            var holidaySettings = new List<EtHolidaySetting>();
            if (classRule.IsJumpHoliday)
            {
                holidaySettings = await _holidaySettingDAL.GetAllHolidaySetting();
            }
            while (true)
            {
                currentDate = currentDate.AddDays(1);
                if (classRule.EndDate != null && currentDate > classRule.EndDate)
                {
                    break;
                }
                if (classRule.IsJumpHoliday && holidaySettings != null && holidaySettings.Any())  //节假日 限制
                {
                    var isHday = holidaySettings.Where(p => currentDate >= p.StartTime && currentDate <= p.EndTime);
                    if (isHday.Any())
                    {
                        continue;
                    }
                }
                if ((int)currentDate.DayOfWeek == classRule.Week) //同一周
                {
                    var classRoomIds = classRule.ClassRoomIds;
                    var classRoomIdsIsAlone = true;
                    if (string.IsNullOrEmpty(classRule.ClassRoomIds))
                    {
                        classRoomIds = etClass.EtClass.ClassRoomIds;
                        classRoomIdsIsAlone = false;
                    }

                    var courseList = classRule.CourseList;
                    var courseListIsAlone = true;
                    if (string.IsNullOrEmpty(classRule.CourseList))
                    {
                        courseList = etClass.EtClass.CourseList;
                        courseListIsAlone = false;
                    }

                    var teachers = classRule.Teachers;
                    var teachersIsAlone = true;
                    if (string.IsNullOrEmpty(classRule.Teachers))
                    {
                        teachers = etClass.EtClass.Teachers;
                        teachersIsAlone = false;
                    }

                    var studentIdsClass = string.Empty;
                    if (etClass.EtClassStudents != null && etClass.EtClassStudents.Any())
                    {
                        studentIdsClass = EtmsHelper.GetMuIds(etClass.EtClassStudents.Select(p => p.StudentId));
                    }

                    await _analyzeClassTimesDAL.AddClassTimes(new EtClassTimes()
                    {
                        ClassContent = classRule.ClassContent,
                        ClassId = classRule.ClassId,
                        ClassOt = currentDate,
                        ClassRecordId = null,
                        ClassRoomIds = classRoomIds,
                        ClassRoomIdsIsAlone = classRoomIdsIsAlone,
                        CourseList = courseList,
                        CourseListIsAlone = courseListIsAlone,
                        Teachers = teachers,
                        TeachersIsAlone = teachersIsAlone,
                        TeacherNum = teachers.Trim(',').Length,
                        StartTime = classRule.StartTime,
                        EndTime = classRule.EndTime,
                        IsDeleted = EmIsDeleted.Normal,
                        RuleId = classRule.Id,
                        Status = EmClassTimesStatus.UnRollcall,
                        TenantId = classRule.TenantId,
                        Week = classRule.Week,
                        StudentIdsTemp = string.Empty,
                        StudentIdsClass = studentIdsClass
                    });
                    break;
                }
            }
            await _analyzeClassTimesDAL.UpdateClassTimesRule(request.ClassTimesRuleId, currentDate);
        }

        public async Task<Tuple<IEnumerable<StudentCourseConsume>, int>> GetNeedConsumeStudentCourse(int pageSize, int pageCurrent, DateTime time)
        {
            return await _analyzeClassTimesDAL.GetNeedConsumeStudentCourse(pageSize, pageCurrent, time);
        }

        public async Task ConsumeStudentCourseProcessEvent(ConsumeStudentCourseEvent request)
        {
            var studentCourseDetail = await _analyzeClassTimesDAL.GetStudentCourseDetail(request.StudentCourseDetailId);
            if (studentCourseDetail == null)
            {
                Log.Error($"[定时扣减学员剩余课程,课程记录不存在],tenantId:{request.TenantId},StudentCourseDetailId:{request.StudentCourseDetailId}", this.GetType());
                return;
            }
            if (studentCourseDetail.LastJobProcessTime != null && studentCourseDetail.LastJobProcessTime >= request.DeTime)
            {
                Log.Info($"[定时扣减学员剩余课程,重复处理],tenantId:{request.TenantId},StudentCourseDetailId:{request.StudentCourseDetailId}", this.GetType());
                return;
            }
            if (studentCourseDetail.DeType != EmDeClassTimesType.Day)
            {
                return;
            }
            if (studentCourseDetail.StartTime == null || studentCourseDetail.EndTime == null)
            {
                return;
            }
            if (studentCourseDetail.StartTime > request.DeTime)
            {
                return;
            }
            if (studentCourseDetail.EndTime < request.DeTime)
            {
                return;
            }
            var mokJobProcessTime = studentCourseDetail.LastJobProcessTime;
            var firstDate = request.DeTime.AddDays(1).Date;
            var endDate = studentCourseDetail.EndTime.Value.Date;
            var dffTime = EtmsHelper.GetDffTime(firstDate, endDate);
            studentCourseDetail.SurplusQuantity = dffTime.Item1;
            studentCourseDetail.SurplusSmallQuantity = dffTime.Item2;
            studentCourseDetail.UseQuantity += 1;
            studentCourseDetail.LastJobProcessTime = request.DeTime;
            await _analyzeClassTimesDAL.EditStudentCourseDetail(studentCourseDetail);
            if (mokJobProcessTime == null)
            {
                mokJobProcessTime = studentCourseDetail.StartTime.Value.AddDays(-1);
            }
            var studentCourseConsumeLogs = new List<EtStudentCourseConsumeLog>();
            while (true)
            {
                mokJobProcessTime = mokJobProcessTime.Value.AddDays(1).Date;
                if (mokJobProcessTime > request.DeTime)
                {
                    break;
                }
                studentCourseConsumeLogs.Add(new EtStudentCourseConsumeLog()
                {
                    CourseId = studentCourseDetail.CourseId,
                    DeClassTimes = 0,
                    DeClassTimesSmall = 1,
                    DeType = EmDeClassTimesType.Day,
                    IsDeleted = EmIsDeleted.Normal,
                    OrderId = studentCourseDetail.OrderId,
                    OrderNo = studentCourseDetail.OrderNo,
                    Ot = mokJobProcessTime.Value,
                    SourceType = EmStudentCourseConsumeSourceType.AutoConsumeDay,
                    StudentId = studentCourseDetail.StudentId,
                    TenantId = studentCourseDetail.TenantId
                });
            }
            _studentCourseConsumeLogDAL.AddStudentCourseConsumeLog(studentCourseConsumeLogs);
            _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(studentCourseDetail.TenantId)
            {
                CourseId = studentCourseDetail.CourseId,
                StudentId = studentCourseDetail.StudentId
            });
        }

        public async Task<Tuple<IEnumerable<HasCourseStudent>, int>> GetHasCourseStudent(int pageSize, int pageCurrent)
        {
            return await _analyzeClassTimesDAL.GetHasCourseStudent(pageSize, pageCurrent);
        }

        public async Task TenantClassTimesTodayConsumerEvent(TenantClassTimesTodayEvent request)
        {
            var classTimes = await _jobAnalyzeDAL.GetClassTimesUnRollcall(request.ClassOt.Date);
            if (classTimes.Count > 0)
            {
                _eventPublisher.Publish(new TempStudentNeedCheckGenerateEvent(request.TenantId) //待考勤数据
                {
                    ClassTimesIds = classTimes.Select(p => p.Id).ToList(),
                    ClassOt = request.ClassOt
                });
            }
        }

        public async Task SyncParentStudentsConsumerEvent(SyncParentStudentsEvent request)
        {
            var myPhone = request.Phones.Distinct();
            foreach (var p in myPhone)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                await _parentStudentDAL.UpdateCacheAndGetParentStudents(request.TenantId, p);
            }
        }
    }
}
