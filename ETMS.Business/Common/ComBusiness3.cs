using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Utility;
using ETMS.IBusiness;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ETMS.Entity.Dto.Product.Output;
using ETMS.Entity.Dto.User.Output;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Event.DataContract;
using ETMS.IDataAccess;
using ETMS.IEventProvider;

namespace ETMS.Business.Common
{
    internal static class ComBusiness3
    {
        internal static string GetStudentDescPC(EtStudent student)
        {
            return $"{student.Name}({student.Phone})";
        }

        internal static bool CheckStudentCourseHasSurplus(IEnumerable<EtStudentCourse> studentCourses)
        {
            if (studentCourses == null || !studentCourses.Any())
            {
                return false;
            }
            var now = DateTime.Now.Date;
            var deClassTimes = studentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            if (deClassTimes != null && deClassTimes.SurplusQuantity > 0 && deClassTimes.Status != EmStudentCourseStatus.EndOfClass)
            {
                return true;
            }

            var courseDay = studentCourses.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
            if (courseDay != null && (courseDay.SurplusQuantity > 0 || courseDay.SurplusSmallQuantity > 0)
                && courseDay.Status != EmStudentCourseStatus.EndOfClass)
            {
                return true;
            }

            return false;
        }

        internal static ClassReservationSettingDesc GetClassReservationSettingDesc(ClassReservationSettingView rule)
        {
            var result = new ClassReservationSettingDesc();
            switch (rule.StartClassReservaLimitType)
            {
                case EmStartClassReservaLimitType.NotLimit:
                    result.RuleStartClassReservaLimitDesc = "不限制";
                    break;
                case EmStartClassReservaLimitType.LimitHour:
                    result.RuleStartClassReservaLimitDesc = $"上课前{rule.StartClassReservaLimitValue}小时内可预约";
                    break;
                case EmStartClassReservaLimitType.LimitDay:
                    result.RuleStartClassReservaLimitDesc = $"上课前{rule.StartClassReservaLimitValue}天内可预约";
                    break;
            }

            switch (rule.DeadlineClassReservaLimitType)
            {
                case EmDeadlineClassReservaLimitType.NotLimit:
                    result.RuleDeadlineClassReservaLimitDesc = "不限制";
                    break;
                case EmDeadlineClassReservaLimitType.LimitMinute:
                    result.RuleDeadlineClassReservaLimitDesc = $"上课前{rule.DeadlineClassReservaLimitValue}分钟截止预约";
                    break;
                case EmDeadlineClassReservaLimitType.LimitHour:
                    result.RuleDeadlineClassReservaLimitDesc = $"上课前{rule.DeadlineClassReservaLimitValue}小时截止预约";
                    break;
                case EmDeadlineClassReservaLimitType.LimitDay:
                    result.RuleDeadlineClassReservaLimitDesc = $"上课前{rule.DeadlineClassReservaLimitValue}天的{EtmsHelper.GetTimeDesc(rule.DeadlineClassReservaLimitDayTimeValue)}截止预约";
                    break;
            }

            switch (rule.MaxCountClassReservaLimitType)
            {
                case EmMaxCountClassReservaLimitType.NotLimit:
                    result.RuleMaxCountClassReservaLimitDesc = "不限制";
                    break;
                case EmMaxCountClassReservaLimitType.SameCourseLimit:
                    result.RuleMaxCountClassReservaLimitDesc = $"同一门课程最多可约{rule.MaxCountClassReservaLimitValue}节课";
                    break;
            }

            switch (rule.CancelClassReservaType)
            {
                case EmCancelClassReservaType.LimitMinute:
                    break;
                case EmCancelClassReservaType.LimitHour:
                    break;
                case EmCancelClassReservaType.LimitDay:
                    break;
            }

            return result;
        }

        internal static bool CheckStudentInClassTimes(EtClassTimes classTimes, long studentId)
        {
            if (!string.IsNullOrEmpty(classTimes.StudentIdsClass) && classTimes.StudentIdsClass.Split(',').FirstOrDefault(p => p == studentId.ToString()) != null)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(classTimes.StudentIdsTemp) && classTimes.StudentIdsTemp.Split(',').FirstOrDefault(p => p == studentId.ToString()) != null)
            {
                return true;
            }
            if (!string.IsNullOrEmpty(classTimes.StudentIdsReservation) && classTimes.StudentIdsReservation.Split(',').FirstOrDefault(p => p == studentId.ToString()) != null)
            {
                return true;
            }
            return false;
        }

        internal static async Task<StudentAccountRechargeView> GetStudentAccountRechargeView(DataTempBox2<StudentAccountRechargeView> tempBox,
            IStudentAccountRechargeCoreBLL coreBLL, string phone, long id)
        {
            return await tempBox.GetData(id, async () =>
            {
                return await coreBLL.GetStudentAccountRechargeByPhone(phone);
            });
        }

        internal static string PhoneSecrecy(string phone, int loginClientType)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return string.Empty;
            }
            if (loginClientType == EmUserOperationLogClientType.WeChat)
            {
                try
                {
                    return Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[PhoneSecrecy]{phone}", ex, typeof(ComBusiness3));
                }
            }
            return phone;
        }

        internal static string GetName<T>(List<T> entitys, long? id) where T : Entity<long>, IHasName
        {
            if (id == null)
            {
                return string.Empty;
            }
            var myData = entitys.FirstOrDefault(p => p.Id == id.Value);
            return myData?.Name;
        }

        internal static List<string> GetMediasUrl(string workMedias)
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
                    result.Add(AliyunOssUtil.GetAccessUrlHttps(p));
                }
            }
            return result;
        }

        internal static List<PriceRuleDesc> GetPriceRuleDescs(List<EtCoursePriceRule> priceRules)
        {
            if (priceRules == null || !priceRules.Any())
            {
                return new List<PriceRuleDesc>();
            }
            var myPriceRules = priceRules.OrderBy(p => p.PriceType);
            var roles = new List<PriceRuleDesc>();
            foreach (var p in myPriceRules)
            {
                roles.Add(ComBusiness.GetPriceRuleDesc(p));
            }
            return roles;
        }

        internal static RoleNoticeSettingOutput AnalyzeNoticeSetting(string noticeSetting, bool isAdmin = false)
        {
            if (isAdmin)
            {
                return new RoleNoticeSettingOutput() //这里，一些接收微信消息的权限给了true，因为前端没用到暂且这么处理
                {
                    IsAllowAppLogin = true,
                    IsAllowLookStatistics = true,
                    IsReceiveInteractiveStudent = true,
                    IsStudentContractsNotArrived = true,
                    IsStudentLeaveApply = true,
                    IsTryCalssApply = true,
                    IsAllowPCLogin = true,
                    IsAllowWebchatLogin = true
                };
            }
            var output = new RoleNoticeSettingOutput()
            {
                IsAllowPCLogin = true,
                IsAllowWebchatLogin = true
            };
            if (string.IsNullOrEmpty(noticeSetting))
            {
                return output;
            }
            var settings = noticeSetting.Split(',');
            foreach (var p in settings)
            {
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }
                var temp = p.ToInt();
                switch (temp)
                {
                    case RoleOtherSetting.StudentLeaveApply:
                        output.IsStudentLeaveApply = true;
                        break;
                    case RoleOtherSetting.StudentContractsNotArrived:
                        output.IsStudentContractsNotArrived = true;
                        break;
                    case RoleOtherSetting.TryCalssApply:
                        output.IsTryCalssApply = true;
                        break;
                    case RoleOtherSetting.ReceiveInteractiveStudent:
                        output.IsReceiveInteractiveStudent = true;
                        break;
                    case RoleOtherSetting.AllowAppLogin:
                        output.IsAllowAppLogin = true;
                        break;
                    case RoleOtherSetting.AllowLookStatistics:
                        output.IsAllowLookStatistics = true;
                        break;
                    case RoleOtherSetting.AllowPCLogin:
                        output.IsAllowPCLogin = true;
                        break;
                    case RoleOtherSetting.AllowWebchatLogin:
                        output.IsAllowWebchatLogin = true;
                        break;
                    case RoleOtherSetting.StudentCheckOnWeChat:
                        output.IsStudentCheckOnWeChat = true;
                        break;
                }
            }
            return output;
        }

        internal static TenantConfigGetSimpleOutput GetTenantConfigGetSimple(TenantConfig config)
        {
            var output = new TenantConfigGetSimpleOutput()
            {
                IsCanDeDecimal = config.ClassCheckSignConfig.IsCanDeDecimal,
                IsEnableStudentCheckDeClassTimes = false
            };
            var cardCheckIn = config.StudentCheckInConfig.StudentUseCardCheckIn;
            var faceCheckIn = config.StudentCheckInConfig.StudentUseFaceCheckIn;
            if (cardCheckIn.IsRelationClassTimesCard == EmBool.True
                && cardCheckIn.RelationClassTimesCardType == EmAttendanceRelationClassTimesType.GoDeStudentCourse)
            {
                output.IsEnableStudentCheckDeClassTimes = true;
            }
            if (faceCheckIn.IsRelationClassTimesFace == EmBool.True
                && faceCheckIn.RelationClassTimesFaceType == EmAttendanceRelationClassTimesType.GoDeStudentCourse)
            {
                output.IsEnableStudentCheckDeClassTimes = true;
            }
            return output;
        }

        internal static void ClassRecordAbsenceProcess(List<EtClassRecordAbsenceLog> classRecordAbsenceLogs, EtClassRecordStudent student, long recordId)
        {
            if (student.StudentType == EmClassStudentType.TryCalssStudent || student.StudentType == EmClassStudentType.MakeUpStudent)
            {
                return;
            }
            if (student.StudentCheckStatus == EmClassStudentCheckStatus.NotArrived || student.StudentCheckStatus == EmClassStudentCheckStatus.Leave)
            {
                classRecordAbsenceLogs.Add(new EtClassRecordAbsenceLog()
                {
                    Remark = student.Remark,
                    IsDeleted = EmIsDeleted.Normal,
                    CheckOt = student.CheckOt,
                    CheckUserId = student.CheckUserId,
                    ClassContent = student.ClassContent,
                    ClassId = student.ClassId,
                    ClassOt = student.ClassOt,
                    ClassRecordId = recordId,
                    ClassRecordStudentId = student.Id,
                    Week = student.Week,
                    StartTime = student.StartTime,
                    Status = EmClassRecordStatus.Normal,
                    HandleStatus = EmClassRecordAbsenceHandleStatus.Unprocessed,
                    ClassRoomIds = student.ClassRoomIds,
                    CourseId = student.CourseId,
                    DeClassTimes = student.DeClassTimes,
                    DeSum = student.DeSum,
                    DeType = student.DeType,
                    EndTime = student.EndTime,
                    ExceedClassTimes = student.ExceedClassTimes,
                    HandleContent = string.Empty,
                    HandleOt = null,
                    HandleUser = null,
                    StudentCheckStatus = student.StudentCheckStatus,
                    StudentId = student.StudentId,
                    StudentTryCalssLogId = student.StudentTryCalssLogId,
                    StudentType = student.StudentType,
                    TeacherNum = student.TeacherNum,
                    Teachers = student.Teachers,
                    TenantId = student.TenantId
                });
            }
        }

        internal static async Task TryCalssStudentProcess(ITryCalssLogDAL tryCalssLogDAL, IStudentDAL studentDAL, IEventPublisher eventPublisher, List<EtStudentTrackLog> studentTrackLogs,
            EtClassRecord classRecord, EtClassRecordStudent student, long recordId, bool tryCalssNoticeTrackUser)
        {
            //更新试听记录
            if (student.StudentCheckStatus == EmClassStudentCheckStatus.Arrived || student.StudentCheckStatus == EmClassStudentCheckStatus.BeLate)
            {
                await tryCalssLogDAL.UpdateStatus(student.StudentTryCalssLogId.Value, EmTryCalssLogStatus.IsTry);
            }
            else
            {
                await tryCalssLogDAL.UpdateStatus(student.StudentTryCalssLogId.Value, EmTryCalssLogStatus.IsExpired);
            }

            //通知此学员的跟进人
            if (tryCalssNoticeTrackUser)
            {
                eventPublisher.Publish(new NoticeUserOfStudentTryClassFinishEvent(classRecord.TenantId)
                {
                    StudentId = student.StudentId,
                    CourseId = student.CourseId,
                    ClassRecord = classRecord
                });
            }

            //生成跟进记录
            var myStudent = await studentDAL.GetStudent(student.StudentId);
            if (myStudent.Student.TrackUser != null)
            {
                byte tryLogContentType = 0;
                if (student.StudentCheckStatus == EmClassStudentCheckStatus.Arrived || student.StudentCheckStatus == EmClassStudentCheckStatus.BeLate)
                {
                    tryLogContentType = EmStudentTrackContentType.IsTryClassFinish;
                }
                else
                {
                    tryLogContentType = EmStudentTrackContentType.IsTryClassInvalid;
                }
                studentTrackLogs.Add(new EtStudentTrackLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    NextTrackTime = null,
                    RelatedInfo = recordId,
                    StudentId = student.StudentId,
                    TenantId = student.TenantId,
                    TrackTime = student.ClassOt,
                    TrackUserId = myStudent.Student.TrackUser.Value,
                    ContentType = tryLogContentType,
                    TrackContent = $"预约试听的课程已结束，学员到课状态({EmClassStudentCheckStatus.GetClassStudentCheckStatus(student.StudentCheckStatus)})"
                });
            }
        }

        internal static Tuple<bool, string> IsStopOfClass2(List<EtStudentCourse> myStudentCourses, DateTime? verifyDate = null)
        {
            var isStopCourse = IsStopOfClass(myStudentCourses, verifyDate);
            if (isStopCourse)
            {
                var myFirstCourse = myStudentCourses.First();
                if (myFirstCourse.RestoreTime == null)
                {
                    return Tuple.Create(true, $"停课开始时间：{myFirstCourse.StopTime.EtmsToDateString()}");
                }
                else
                {
                    return Tuple.Create(true, $"停课时间：{myFirstCourse.StopTime.EtmsToDateString()}到{myFirstCourse.RestoreTime.EtmsToDateString()}");
                }
            }
            return Tuple.Create(false, string.Empty);
        }

        internal static bool IsStopOfClass(List<EtStudentCourse> myStudentCourses, DateTime? verifyDate = null)
        {
            if (myStudentCourses == null || myStudentCourses.Count == 0)
            {
                return false;
            }
            var myFirstCourse = myStudentCourses.First();
            if (verifyDate == null)
            {
                return myFirstCourse.Status == EmStudentCourseStatus.StopOfClass;
            }
            else
            {
                if (myFirstCourse.Status == EmStudentCourseStatus.StopOfClass)
                {
                    if (myFirstCourse.RestoreTime == null)
                    {
                        return verifyDate.Value >= myFirstCourse.StopTime.Value;
                    }
                    else
                    {
                        return verifyDate.Value >= myFirstCourse.StopTime.Value && verifyDate.Value < myFirstCourse.RestoreTime.Value;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 学员课程复课
        /// </summary>
        /// <param name="studentCourseDAL"></param>
        /// <param name="tenantId"></param>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        internal static async Task RestoreStudentCourse(IStudentCourseDAL studentCourseDAL, long tenantId, long studentId,
            long courseId, DateTime restoreTime)
        {
            var myCourse = await studentCourseDAL.GetStudentCourseDb(studentId, courseId);
            if (myCourse == null || myCourse.Count == 0)
            {
                LOG.Log.Error($"[RestoreStudentCourse]未找到复课的课程,{tenantId},{studentId},{courseId}", typeof(ComBusiness3));
                return;
            }
            var firstStopLog = myCourse.First();
            if (firstStopLog.Status != EmStudentCourseStatus.StopOfClass)
            {
                LOG.Log.Error($"[RestoreStudentCourse]课程不需要复课,{tenantId},{studentId},{courseId}", typeof(ComBusiness3));
                return;
            }
            var stopTime = firstStopLog.StopTime.Value.Date;

            //复课后，自动处理课程的有效期
            var stopCourseDetail = await studentCourseDAL.GetStudentCourseDetailStop(studentId, courseId);
            if (stopCourseDetail.Any())
            {
                bool isChanged = false;
                foreach (var p in stopCourseDetail)
                {
                    if (p.EndTime != null && p.EndTime.Value > stopTime)
                    {
                        var diff = (p.EndTime.Value - stopTime).TotalDays; //计算停课那天 还剩余多久的课程
                        p.EndTime = restoreTime.AddDays(diff);
                        isChanged = true;
                    }
                }
                if (isChanged)
                {
                    await studentCourseDAL.UpdateStudentCourseDetail(stopCourseDetail);
                }
            }
            await studentCourseDAL.StudentCourseRestoreTime(studentId, courseId);
        }
    }
}
