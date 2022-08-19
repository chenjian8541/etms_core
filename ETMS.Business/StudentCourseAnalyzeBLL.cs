using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.IEventProvider;
using ETMS.Entity.Database.Source;
using ETMS.Utility;
using ETMS.Business.Common;
using ETMS.Entity.View;
using ETMS.Event.DataContract.Statistics;
using ETMS.Entity.Config;

namespace ETMS.Business
{
    public class StudentCourseAnalyzeBLL : IStudentCourseAnalyzeBLL
    {
        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IStudentCourseConsumeLogDAL _studentCourseConsumeLogDAL;

        public StudentCourseAnalyzeBLL(IStudentCourseDAL studentCourseDAL, IEventPublisher eventPublisher, ITenantConfigDAL tenantConfigDAL,
            IStudentCourseConsumeLogDAL studentCourseConsumeLogDAL)
        {
            this._studentCourseDAL = studentCourseDAL;
            this._eventPublisher = eventPublisher;
            this._tenantConfigDAL = tenantConfigDAL;
            this._studentCourseConsumeLogDAL = studentCourseConsumeLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentCourseDAL, _tenantConfigDAL, _studentCourseConsumeLogDAL);
        }

        public async Task CourseAnalyze(StudentCourseAnalyzeEvent request)
        {
            var myCourseIds = await _studentCourseDAL.GetStudentCourseId(request.StudentId);
            var studentCourseIds = await _studentCourseDAL.GetStudentBuyCourseId(request.StudentId);
            var allCourseIds = new List<long>();
            if (myCourseIds != null && myCourseIds.Any())
            {
                allCourseIds = myCourseIds;
            }
            if (studentCourseIds != null && studentCourseIds.Any())
            {
                allCourseIds.AddRange(studentCourseIds.Select(j => j.CourseId));
            }
            var vaildCourseIds = allCourseIds.Distinct();
            foreach (var courseId in vaildCourseIds)
            {
                _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.TenantId)
                {
                    CourseId = courseId,
                    StudentId = request.StudentId,
                    IsJobExecute = request.IsJobExecute
                });
            }
        }

        private EtStudentCourse GetCourseClassTimesDefault(int tenantId, long studentId, long courseId)
        {
            return new EtStudentCourse()
            {
                BugUnit = EmCourseUnit.ClassTimes,
                BuyQuantity = 0,
                CourseId = courseId,
                DeType = EmDeClassTimesType.ClassTimes,
                ExceedTotalClassTimes = 0,
                GiveQuantity = 0,
                GiveSmallQuantity = 0,
                IsDeleted = EmIsDeleted.Normal,
                LastJobProcessTime = DateTime.Now,
                RestoreTime = null,
                Status = EmStudentCourseStatus.Normal,
                StopTime = null,
                StudentId = studentId,
                SurplusQuantity = 0,
                SurplusSmallQuantity = 0,
                TenantId = tenantId,
                UseQuantity = 0,
                UseUnit = EmCourseUnit.ClassTimes
            };
        }

        private EtStudentCourse GetCourseDayDefault(int tenantId, long studentId, long courseId)
        {
            return new EtStudentCourse()
            {
                BugUnit = EmCourseUnit.Month,
                BuyQuantity = 0,
                UseUnit = EmCourseUnit.Day,
                RestoreTime = null,
                CourseId = courseId,
                DeType = EmDeClassTimesType.Day,
                ExceedTotalClassTimes = 0,
                GiveQuantity = 0,
                GiveSmallQuantity = 0,
                IsDeleted = EmIsDeleted.Normal,
                LastJobProcessTime = DateTime.Now,
                Status = EmStudentCourseStatus.Normal,
                StopTime = null,
                StudentId = studentId,
                SurplusQuantity = 0,
                SurplusSmallQuantity = 0,
                TenantId = tenantId,
                UseQuantity = 0
            };
        }

        /// <summary>
        /// 1.统计学员课程信息；
        /// 2.课程过期状态修改；
        /// 3.停课/复课状态；
        /// 4.停课时要讲所有课程详情的截止时间置空，复课后需要重新设置起止时间和课程有效期；
        /// 
        /// 购买单位（课时/月/天）
        /// 赠送单位（课时/月/天）
        /// 剩余单位（课时/月/天）
        /// 消耗单位（课时/天）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CourseDetailAnalyzeRes> CourseDetailAnalyze(StudentCourseDetailAnalyzeEvent request)
        {
            var myCourse = await _studentCourseDAL.GetStudentCourseDb(request.StudentId, request.CourseId);
            var courseClassTimes = myCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
            var courseDay = myCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
            if (myCourse.Count > 1)
            {
                var delIds = new List<long>();
                foreach (var p in myCourse)
                {
                    if (p.DeType == EmDeClassTimesType.ClassTimes && p.Id != courseClassTimes.Id)
                    {
                        delIds.Add(p.Id);
                    }
                    if (p.DeType == EmDeClassTimesType.Day && p.Id != courseDay.Id)
                    {
                        delIds.Add(p.Id);
                    }
                }
                if (delIds.Count > 0)
                {
                    await _studentCourseDAL.DelStudentCourse(delIds);
                }
            }

            var myCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId, request.CourseId);
            var surplusMoney = 0M;
            if (courseClassTimes == null)
            {
                courseClassTimes = GetCourseClassTimesDefault(request.TenantId, request.StudentId, request.CourseId);
            }
            else
            {
                courseClassTimes.BuyQuantity = 0;
                courseClassTimes.BuySmallQuantity = 0;
                courseClassTimes.GiveQuantity = 0;
                courseClassTimes.GiveSmallQuantity = 0;
                courseClassTimes.SurplusQuantity = 0;
                courseClassTimes.SurplusSmallQuantity = 0;
                courseClassTimes.UseQuantity = 0;
                courseClassTimes.StartTime = null;
                courseClassTimes.EndTime = null;
            }

            if (courseDay == null)
            {
                courseDay = GetCourseDayDefault(request.TenantId, request.StudentId, request.CourseId);
            }
            else
            {
                courseDay.BuyQuantity = 0;
                courseDay.BuySmallQuantity = 0;
                courseDay.GiveQuantity = 0;
                courseDay.GiveSmallQuantity = 0;
                courseDay.SurplusQuantity = 0;
                courseDay.SurplusSmallQuantity = 0;
                courseDay.UseQuantity = 0;
                courseDay.StartTime = null;
                courseDay.EndTime = null;
            }

            //判断课程停课/复课状态
            var studentCourseStatus = EmStudentCourseStatus.Normal;
            if (myCourse.Any())
            {
                var firstCourseLog = myCourse.First();
                if (firstCourseLog.Status == EmStudentCourseStatus.StopOfClass)
                {
                    if (firstCourseLog.RestoreTime == null || firstCourseLog.RestoreTime.Value > DateTime.Now.Date)
                    {
                        studentCourseStatus = EmStudentCourseStatus.StopOfClass;
                    }
                    if (firstCourseLog.RestoreTime != null && firstCourseLog.RestoreTime.Value <= DateTime.Now.Date)
                    {
                        //自动复课
                        await ComBusiness3.RestoreStudentCourse(_studentCourseDAL, request.TenantId, request.StudentId, request.CourseId,
                           firstCourseLog.RestoreTime.Value);
                        _eventPublisher.Publish(request);
                        return new CourseDetailAnalyzeRes()
                        {
                            NewCourse = myCourse,
                            IsCourseNotEnough = false
                        };
                    }
                }
                courseClassTimes.StopTime = courseDay.StopTime = firstCourseLog.StopTime;
                courseClassTimes.RestoreTime = courseDay.RestoreTime = firstCourseLog.RestoreTime;
            }
            courseClassTimes.Status = courseDay.Status = studentCourseStatus;

            var classTimeEndTimeList = new List<DateTime?>();
            //按课时
            var myStudentCourseDetailClassTimes = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes);
            foreach (var myDetailClassTimes in myStudentCourseDetailClassTimes)
            {
                courseClassTimes.BuyQuantity += myDetailClassTimes.BuyQuantity;
                courseClassTimes.GiveQuantity += myDetailClassTimes.GiveQuantity;
                courseClassTimes.UseQuantity += myDetailClassTimes.UseQuantity;
                if (myDetailClassTimes.Status == EmStudentCourseStatus.EndOfClass)
                {
                    continue;
                }
                //自动结课 （剩余课时=0  过期）
                if (myDetailClassTimes.SurplusQuantity == 0)
                {
                    myDetailClassTimes.Status = EmStudentCourseStatus.EndOfClass;
                    continue;
                }
                if (studentCourseStatus != EmStudentCourseStatus.StopOfClass && myDetailClassTimes.EndTime != null && myDetailClassTimes.EndTime.Value < DateTime.Now.Date)
                {
                    myDetailClassTimes.Status = EmStudentCourseStatus.EndOfClass;
                    continue;
                }
                myDetailClassTimes.Status = studentCourseStatus;
                courseClassTimes.SurplusQuantity += myDetailClassTimes.SurplusQuantity;
                surplusMoney += ComBusiness2.GetStudentCourseDetailSurplusMoney(myDetailClassTimes);
                classTimeEndTimeList.Add(myDetailClassTimes.EndTime);
            }

            //按月
            var dayTimeStartTime = new List<DateTime?>();
            var dayTimeEndTime = new List<DateTime?>();
            var myStudentCourseDetailDay = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day);
            foreach (var myDetailDay in myStudentCourseDetailDay)
            {
                if (myDetailDay.BugUnit == EmCourseUnit.Month)
                {
                    courseDay.BuyQuantity += myDetailDay.BuyQuantity;
                }
                else
                {
                    courseDay.BuySmallQuantity += myDetailDay.BuyQuantity;
                }

                courseDay.UseQuantity += myDetailDay.UseQuantity;  //使用按天
                if (myDetailDay.GiveUnit == EmCourseUnit.Month)  //赠送可能按月和天
                {
                    courseDay.GiveQuantity += myDetailDay.GiveQuantity;
                }
                else
                {
                    courseDay.GiveSmallQuantity += myDetailDay.GiveQuantity;
                }
                if (myDetailDay.Status == EmStudentCourseStatus.EndOfClass)
                {
                    continue;
                }
                //自动结课 （剩余课时=0  过期）
                //if (myDetailDay.SurplusQuantity == 0 && myDetailDay.SurplusSmallQuantity == 0)
                //{
                //    myDetailDay.Status = EmStudentCourseStatus.EndOfClass;
                //    continue;
                //}
                if (studentCourseStatus != EmStudentCourseStatus.StopOfClass && myDetailDay.EndTime != null && myDetailDay.EndTime.Value < DateTime.Now.Date)
                {
                    myDetailDay.Status = EmStudentCourseStatus.EndOfClass;
                    continue;
                }
                myDetailDay.Status = studentCourseStatus;
                courseDay.SurplusQuantity += myDetailDay.SurplusQuantity;
                courseDay.SurplusSmallQuantity += myDetailDay.SurplusSmallQuantity;
                surplusMoney += ComBusiness2.GetStudentCourseDetailSurplusMoney(myDetailDay);
                dayTimeStartTime.Add(myDetailDay.StartTime);
                dayTimeEndTime.Add(myDetailDay.EndTime);
            }

            var newCourseDetail = new List<EtStudentCourseDetail>();
            newCourseDetail.AddRange(myStudentCourseDetailClassTimes);
            newCourseDetail.AddRange(myStudentCourseDetailDay);

            var isClassOver = false;
            if (!newCourseDetail.Where(p => p.Status != EmStudentCourseStatus.EndOfClass).Any()) //所有课程已结课
            {
                courseClassTimes.Status = courseDay.Status = EmStudentCourseStatus.EndOfClass;
                isClassOver = true;
                surplusMoney = 0;
                if (request.IsClassOfOneAutoOver)
                {
                    _eventPublisher.Publish(new ClassOfOneAutoOverEvent(request.TenantId)
                    {
                        CourseId = request.CourseId,
                        StudentId = request.StudentId
                    });
                }
            }

            //处理过期时间
            if (classTimeEndTimeList.Any())
            {
                var isHasNotExpired = classTimeEndTimeList.Exists(p => p == null); //不过期
                if (!isHasNotExpired)
                {
                    var maxDate = classTimeEndTimeList.OrderByDescending(p => p).FirstOrDefault();
                    courseClassTimes.EndTime = maxDate;
                }
            }
            if (dayTimeStartTime.Any())
            {
                var minDate = dayTimeStartTime.Where(p => p != null).OrderBy(p => p).FirstOrDefault();
                if (minDate != null)
                {
                    courseDay.StartTime = minDate;
                }
            }
            if (dayTimeEndTime.Any())
            {
                var maxDate = dayTimeEndTime.Where(p => p != null).OrderByDescending(p => p).FirstOrDefault();
                if (maxDate != null)
                {
                    courseDay.EndTime = maxDate;
                }
            }

            if (request.IsJobExecute) //处理最后一次消课时间
            {
                var lastDeTime = await _studentCourseConsumeLogDAL.GetLastConsumeTime(request.StudentId, request.CourseId);
                if (lastDeTime != null)
                {
                    var oldLasDeTime = courseClassTimes.LastDeTime ?? courseDay.LastDeTime;
                    if (oldLasDeTime == null || lastDeTime > oldLasDeTime)
                    {
                        courseClassTimes.LastDeTime = lastDeTime;
                        courseDay.LastDeTime = lastDeTime;
                    }
                }
            }

            var newCourse = new List<EtStudentCourse>();
            if (courseClassTimes.BuyQuantity > 0 || courseClassTimes.ExceedTotalClassTimes > 0)
            {
                newCourse.Add(courseClassTimes);
            }
            else if (courseClassTimes.Id > 0)
            {
                courseClassTimes.IsDeleted = EmIsDeleted.Deleted;
                newCourse.Add(courseClassTimes);
            }

            if (courseDay.BuyQuantity > 0 || courseDay.BuySmallQuantity > 0)
            {
                newCourse.Add(courseDay);
            }
            else if (courseDay.Id > 0)
            {
                courseDay.IsDeleted = EmIsDeleted.Deleted;
                newCourse.Add(courseDay);
            }


            bool isDelOldStudentCourse = false;
            if (newCourse.Count == 0 && myCourseDetail.Count == 0) //订单作废 可能造成要删除课程
            {
                isDelOldStudentCourse = true;
            }
            await _studentCourseDAL.EditStudentCourse(request.StudentId, newCourse, newCourseDetail, myCourse, isDelOldStudentCourse,
                surplusMoney);

            if (request.IsSendNoticeStudent)
            {
                _eventPublisher.Publish(new NoticeStudentCourseSurplusEvent(request.TenantId)
                {
                    CourseId = request.CourseId,
                    StudentId = request.StudentId
                });
            }

            if (isClassOver)
            {
                _eventPublisher.Publish(new StudentAutoMarkGraduationEvent(request.TenantId)
                {
                    StudentId = request.StudentId
                });
            }

            var now = DateTime.Now;
            TenantConfig tenantConfig = null;
            if (!request.IsJobExecute)
            {
                //课程不足提醒
                tenantConfig = await _tenantConfigDAL.GetTenantConfig();
                if (tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat
                    || tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms)
                {
                    var isNeedRemindCourseNotEnough = ComBusiness2.CheckStudentCourseNeedRemind(newCourse, tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughCount,
                         tenantConfig.StudentCourseRenewalConfig.LimitClassTimes, tenantConfig.StudentCourseRenewalConfig.LimitDay);
                    if (isNeedRemindCourseNotEnough)
                    {
                        _eventPublisher.Publish(new NoticeStudentCourseNotEnoughEvent(request.TenantId)
                        {
                            StudentId = request.StudentId,
                            CourseId = request.CourseId
                        });
                    }
                }
            }

            var isCourseNotEnough = false;
            if (request.IsNeedCheckCourseIsNotEnough)
            {
                if (tenantConfig == null)
                {
                    tenantConfig = await _tenantConfigDAL.GetTenantConfig();
                }
                isCourseNotEnough = ComBusiness2.CheckStudentCourseIsNotEnough(newCourse, tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughCount,
                       tenantConfig.StudentCourseRenewalConfig.LimitClassTimes, tenantConfig.StudentCourseRenewalConfig.LimitDay);
            }

            if (!request.IsJobExecute)
            {
                _eventPublisher.Publish(new SyncStudentStudentCourseIdsEvent(request.TenantId, request.StudentId));
                _eventPublisher.Publish(new SysTenantStatistics2Event(request.TenantId));
                _eventPublisher.Publish(new SyncStudentCourseStatusEvent(request.TenantId)
                {
                    StudentId = request.StudentId
                });
                if (!request.IsNotProcessStudentCourseExTimeDe)
                {
                    _eventPublisher.Publish(new StudentCourseExTimeDeEvent(request.TenantId)
                    {
                        StudentId = request.StudentId,
                        CourseId = request.CourseId
                    });
                }
            }

            return new CourseDetailAnalyzeRes()
            {
                NewCourse = newCourse,
                IsCourseNotEnough = isCourseNotEnough
            };
        }

        public async Task TenantStudentCourseNotEnoughConsumerEvent(TenantStudentCourseNotEnoughEvent request)
        {
            var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
            if (!tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat
                && !tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms)
            {
                return;
            }

            var myNeedRemindStudent = await _studentCourseDAL.GetStudentCourseNotEnoughNeedRemind(tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughCount,
                tenantConfig.StudentCourseRenewalConfig.LimitClassTimes, tenantConfig.StudentCourseRenewalConfig.LimitDay);
            if (!myNeedRemindStudent.Any())
            {
                LOG.Log.Info($"[TenantStudentCourseNotEnoughConsumerEvent]学员课时不足续费提醒，未查询到需要提醒的数据,tenantId:{request.TenantId}", this.GetType());
                return;
            }
            foreach (var p in myNeedRemindStudent)
            {
                _eventPublisher.Publish(new NoticeStudentCourseNotEnoughEvent(request.TenantId)
                {
                    StudentId = p.StudentId,
                    CourseId = p.CourseId
                });
            }
        }
    }
}
