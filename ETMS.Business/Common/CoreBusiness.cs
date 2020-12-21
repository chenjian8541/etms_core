using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    public static class CoreBusiness
    {
        private static EtStudentCourse GetExceedClassTimes(DeStudentClassTimesTempRequest classRecordStudent, decimal addExceedTotalClassTimes)
        {
            return new EtStudentCourse()
            {
                ExceedTotalClassTimes = addExceedTotalClassTimes,
                DeType = EmDeClassTimesType.ClassTimes,
                BugUnit = EmCourseUnit.ClassTimes,
                BuyQuantity = 0,
                CourseId = classRecordStudent.CourseId,
                GiveQuantity = 0,
                GiveSmallQuantity = 0,
                IsDeleted = EmIsDeleted.Normal,
                LastJobProcessTime = DateTime.Now,
                RestoreTime = null,
                Status = EmStudentCourseStatus.Normal,
                StopTime = null,
                StudentId = classRecordStudent.StudentId,
                SurplusQuantity = 0,
                SurplusSmallQuantity = 0,
                TenantId = classRecordStudent.TenantId,
                UseQuantity = 0,
                UseUnit = EmCourseUnit.ClassTimes
            };
        }

        /// <summary>
        /// 扣减学员课程
        /// </summary>
        /// <param name="studentCourseDAL"></param>
        /// <param name="classRecordStudent"></param>
        /// <returns></returns>
        public static async Task<DeStudentClassTimesResult> DeStudentClassTimes(IStudentCourseDAL studentCourseDAL, DeStudentClassTimesTempRequest classRecordStudent)
        {
            if (classRecordStudent.DeClassTimes == 0)
            {
                return DeStudentClassTimesResult.GetNotDeEntity();
            }
            var classDate = classRecordStudent.ClassOt.Date;
            var myCourseDetail = await studentCourseDAL.GetStudentCourseDetail(classRecordStudent.StudentId, classRecordStudent.CourseId);
            if (myCourseDetail == null || !myCourseDetail.Any())
            {
                //记录超上课时
                await studentCourseDAL.SaveNotbuyStudentExceedClassTimes(GetExceedClassTimes(classRecordStudent, classRecordStudent.DeClassTimes));
                return new DeStudentClassTimesResult()
                {
                    DeSum = 0,
                    DeType = EmDeClassTimesType.NotDe,
                    ExceedClassTimes = classRecordStudent.DeClassTimes,
                    OrderId = null,
                    OrderNo = string.Empty
                };
            }

            var dayCourseDetail = myCourseDetail.FirstOrDefault(p => p.DeType == EmDeClassTimesType.Day && p.StartTime != null && classDate >= p.StartTime
            && p.EndTime != null && classDate <= p.EndTime && p.Status == EmStudentCourseStatus.Normal);
            if (dayCourseDetail != null)
            {
                //按天消耗
                return new DeStudentClassTimesResult()
                {
                    DeSum = dayCourseDetail.Price,
                    DeType = EmDeClassTimesType.Day,
                    ExceedClassTimes = 0,
                    OrderId = dayCourseDetail.OrderId,
                    OrderNo = dayCourseDetail.OrderNo
                };
            }

            var timesCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes && (p.EndTime == null || classDate <= p.EndTime)
            && p.SurplusQuantity > 0 && p.Status == EmStudentCourseStatus.Normal);
            if (!timesCourseDetail.Any())
            {
                //无剩余课时 记录超上课时
                await studentCourseDAL.SaveNotbuyStudentExceedClassTimes(GetExceedClassTimes(classRecordStudent, classRecordStudent.DeClassTimes));
                return new DeStudentClassTimesResult()
                {
                    DeSum = 0,
                    DeType = EmDeClassTimesType.NotDe,
                    ExceedClassTimes = classRecordStudent.DeClassTimes,
                    OrderId = null,
                    OrderNo = string.Empty
                };
            }

            var enoughTimesCourseDetail = timesCourseDetail.FirstOrDefault(p => p.SurplusQuantity >= classRecordStudent.DeClassTimes);
            if (enoughTimesCourseDetail != null)   //存在课时足够扣的记录
            {
                await studentCourseDAL.DeClassTimesOfStudentCourseDetail(enoughTimesCourseDetail.Id, classRecordStudent.DeClassTimes);
                return new DeStudentClassTimesResult()
                {
                    DeSum = enoughTimesCourseDetail.Price * classRecordStudent.DeClassTimes,
                    DeType = EmDeClassTimesType.ClassTimes,
                    ExceedClassTimes = 0,
                    OrderId = enoughTimesCourseDetail.OrderId,
                    OrderNo = enoughTimesCourseDetail.OrderNo,
                    DeStudentCourseDetailId = enoughTimesCourseDetail.Id,
                    DeClassTimes = classRecordStudent.DeClassTimes
                };
            }
            var notEnoughTimesCourseDetail = timesCourseDetail.First();
            var exceedClassTimes = classRecordStudent.DeClassTimes - notEnoughTimesCourseDetail.SurplusQuantity;
            await studentCourseDAL.DeClassTimesOfStudentCourseDetail(notEnoughTimesCourseDetail.Id, notEnoughTimesCourseDetail.SurplusQuantity);
            await studentCourseDAL.SaveNotbuyStudentExceedClassTimes(GetExceedClassTimes(classRecordStudent, exceedClassTimes));
            return new DeStudentClassTimesResult()
            {
                DeSum = notEnoughTimesCourseDetail.Price * notEnoughTimesCourseDetail.SurplusQuantity,
                DeType = EmDeClassTimesType.ClassTimes,
                ExceedClassTimes = exceedClassTimes,
                OrderId = notEnoughTimesCourseDetail.OrderId,
                OrderNo = notEnoughTimesCourseDetail.OrderNo,
                DeStudentCourseDetailId = notEnoughTimesCourseDetail.Id,
                DeClassTimes = notEnoughTimesCourseDetail.SurplusQuantity
            };
        }
    }
}
