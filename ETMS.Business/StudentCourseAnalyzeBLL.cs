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

namespace ETMS.Business
{
    public class StudentCourseAnalyzeBLL : IStudentCourseAnalyzeBLL
    {
        private readonly IStudentCourseDAL _studentCourseDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        public StudentCourseAnalyzeBLL(IStudentCourseDAL studentCourseDAL, IEventPublisher eventPublisher, ITenantConfigDAL tenantConfigDAL)
        {
            this._studentCourseDAL = studentCourseDAL;
            this._eventPublisher = eventPublisher;
            this._tenantConfigDAL = tenantConfigDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentCourseDAL, _tenantConfigDAL);
        }

        public async Task CourseAnalyze(StudentCourseAnalyzeEvent request)
        {
            var studentCourseIds = await _studentCourseDAL.GetStudentBuyCourseId(request.StudentId);
            foreach (var course in studentCourseIds)
            {
                _eventPublisher.Publish(new StudentCourseDetailAnalyzeEvent(request.TenantId)
                {
                    CourseId = course.CourseId,
                    StudentId = request.StudentId
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
            var myCourseDetail = await _studentCourseDAL.GetStudentCourseDetail(request.StudentId, request.CourseId);
            var courseClassTimes = myCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.ClassTimes);
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
            var courseDay = myCourse.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day);
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
                if (myDetailClassTimes.EndTime != null && myDetailClassTimes.EndTime.Value < DateTime.Now.Date)
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
                if (myDetailDay.EndTime != null && myDetailDay.EndTime.Value < DateTime.Now.Date)
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
            var isCheckCourseIsNotEnough = false;
            var isCourseNotEnough = false;
            if (EtmsHelper.CheckIsDaytime(now))
            {
                //课程不足提醒
                var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
                if (tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughWeChat
                    || tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughSms)
                {
                    isCourseNotEnough = ComBusiness2.CheckStudentCourseNeedRemind(newCourse, tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughCount,
                        tenantConfig.StudentCourseRenewalConfig.LimitClassTimes, tenantConfig.StudentCourseRenewalConfig.LimitDay);
                    isCheckCourseIsNotEnough = true;
                    if (isCourseNotEnough)
                    {
                        _eventPublisher.Publish(new NoticeStudentCourseNotEnoughEvent(request.TenantId)
                        {
                            StudentId = request.StudentId,
                            CourseId = request.CourseId
                        });
                    }
                }
            }
            if (request.IsNeedCheckCourseIsNotEnough && !isCheckCourseIsNotEnough)
            {
                var tenantConfig = await _tenantConfigDAL.GetTenantConfig();
                isCourseNotEnough = ComBusiness2.CheckStudentCourseNeedRemind(newCourse, tenantConfig.StudentNoticeConfig.StudentCourseNotEnoughCount,
                       tenantConfig.StudentCourseRenewalConfig.LimitClassTimes, tenantConfig.StudentCourseRenewalConfig.LimitDay);
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
