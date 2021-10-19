using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Common;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    internal static class ComBusiness4
    {
        internal static Tuple<string, string> GetTeacherSalaryContractPerformanceSetDetailDesc(EtTeacherSalaryContractPerformanceSetDetail item)
        {
            var unitTag = string.Empty;
            switch (item.ComputeMode)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    unitTag = "元/课时";
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    unitTag = "元/人次";
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    unitTag = "%课消比";
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    unitTag = "元/课时";
                    break;
            }
            var desc = $"<div class='performance_set_rule_content_item'><span class='rule_interval_value'>{item.ComputeValue}{unitTag}</span></div>";
            return Tuple.Create(desc, EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc(item.ComputeMode));
        }

        internal static Tuple<string, string> GetTeacherSalaryContractPerformanceSetDetailDesc(List<EtTeacherSalaryContractPerformanceSetDetail> items)
        {
            if (items == null || items.Count == 0)
            {
                return Tuple.Create(string.Empty, string.Empty);
            }
            var myItem = items.OrderBy(p => p.MinLimit);
            var firstItem = myItem.First();
            var unitTag = string.Empty;
            switch (firstItem.ComputeMode)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    unitTag = "元/课时";
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    unitTag = "元/人次";
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    unitTag = "%课消比";
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    unitTag = "元/课时";
                    break;
            }
            var strDesc = new StringBuilder();
            if (items.Count == 1) //无梯度
            {
                strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_value'>{firstItem.ComputeValue}{unitTag}</span></div>");
            }
            else
            {
                //采用前开后闭
                foreach (var p in items)
                {
                    if (p.MinLimit == null || p.MinLimit == 0) //第一项
                    {
                        strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>0＜X≤{p.MaxLimit}</span><span class='rule_interval_value'>{p.ComputeValue}{unitTag}</span></div>");
                        continue;
                    }
                    if (p.MaxLimit == null || p.MaxLimit == 0) //最后一项
                    {
                        strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>{p.MinLimit}＜X</span><span class='rule_interval_value'>{p.ComputeValue}{unitTag}</span></div>");
                        continue;
                    }
                    strDesc.Append($"<div class='performance_set_rule_content_item'><span class='rule_interval_desc'>{p.MinLimit}＜X≤{p.MaxLimit}</span><span class='rule_interval_value'>{p.ComputeValue}{unitTag}</span></div>");
                }
            }
            return Tuple.Create(strDesc.ToString(), EmTeacherSalaryComputeMode.GetTeacherSalaryComputeModeDesc(firstItem.ComputeMode));
        }

        internal static bool GetIsOpenLcsPay(int lcswApplyStatus, byte lcswOpenStatus)
        {
            return EmLcswApplyStatus.IsSuccess(lcswApplyStatus) && lcswOpenStatus == EmBool.True;
        }

        internal static string GetLcsTerminalTime(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss");
        }

        internal static string CheckStudentCourseIsEnough(List<EtStudentCourseDetail> myCourseDetail, string studentName, DateTime classDate, decimal deClassTimes)
        {
            if (!string.IsNullOrEmpty(studentName))
            {
                studentName = $"[{studentName}]";
            }
            if (myCourseDetail == null || !myCourseDetail.Any())
            {
                return $"学员{studentName}未购买此课程，无法点名";
            }
            var dayCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day && (p.StartTime == null || classDate >= p.StartTime)
            && (p.EndTime == null || classDate <= p.EndTime) && p.Status == EmStudentCourseStatus.Normal);
            if (!dayCourseDetail.Any())
            {
                var timesCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes && p.Status == EmStudentCourseStatus.Normal
                && (p.EndTime == null || classDate <= p.EndTime) && p.SurplusQuantity >= deClassTimes);
                if (!timesCourseDetail.Any())
                {
                    var stopTimeCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.ClassTimes
                    && p.Status == EmStudentCourseStatus.StopOfClass && (p.EndTime == null || classDate <= p.EndTime) && p.SurplusQuantity >= deClassTimes);
                    if (stopTimeCourseDetail.Any())
                    {
                        return $"学员{studentName}已停课，无法点名";
                    }
                    var stopDayCourseDetail = myCourseDetail.Where(p => p.DeType == EmDeClassTimesType.Day && (p.StartTime == null || classDate >= p.StartTime)
                    && (p.EndTime == null || classDate <= p.EndTime) && p.Status == EmStudentCourseStatus.StopOfClass);
                    if (stopDayCourseDetail.Any())
                    {
                        return $"学员{studentName}已停课，无法点名";
                    }
                    return $"学员{studentName}剩余课时不足，无法点名";
                }
            }
            return string.Empty;
        }

        internal static string GetClassTimesOtDesc(EtClassTimes classTimes)
        {
            return $"{classTimes.ClassOt.EtmsToDateString()}(周{EtmsHelper.GetWeekDesc(classTimes.Week)}){EtmsHelper.GetTimeDesc(classTimes.StartTime, classTimes.EndTime, "-")}";
        }

        internal static List<Img> GetImgs(string keys)
        {
            if (string.IsNullOrEmpty(keys))
            {
                return new List<Img>();
            }
            var imgs = new List<Img>();
            var myMedias = keys.Split('|');
            foreach (var p in myMedias)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    imgs.Add(new Img(p, AliyunOssUtil.GetAccessUrlHttps(p)));
                }
            }
            return imgs;
        }
    }
}
