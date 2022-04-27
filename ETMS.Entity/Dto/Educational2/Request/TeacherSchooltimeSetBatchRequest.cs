using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Utility;

namespace ETMS.Entity.Dto.Educational2.Request
{
    public class TeacherSchooltimeSetBatchRequest : RequestBase
    {
        public List<long> TeacherIds { get; set; }

        public List<DateTime> ExcludeDate { get; set; }

        public List<TeacherSchooltimeSetBatchItem> Items { get; set; }

        public override string Validate()
        {
            if (TeacherIds == null || TeacherIds.Count == 0)
            {
                return "请选择老师";
            }
            if (TeacherIds.Count > 20)
            {
                return "一次性最多设置20位老师";
            }
            if (Items == null || Items.Count == 0)
            {
                return "请设置老师上课时间";
            }
            if (Items.Count > 10)
            {
                return "最多设置10条规则";
            }
            var tempWeekDatas = new List<TeacherSchooltimeSetBatchWeek>();
            foreach (var item in Items)
            {
                foreach (var i in item.Weeks)
                {
                    var overlappingTimeLog = tempWeekDatas.Where(j => j.Week == i && !(item.NewIntStartTime >= j.NewIntEndTime || item.NewIntEndTime <= j.NewIntStartTime)).FirstOrDefault();
                    if (overlappingTimeLog != null)
                    {
                        return $"周{EtmsHelper.GetWeekDesc(i)}{EtmsHelper.GetTimeDesc(item.NewIntStartTime, item.NewIntEndTime)}存在重叠的时间段，起重新设置";
                    }
                    tempWeekDatas.Add(new TeacherSchooltimeSetBatchWeek()
                    {
                        Week = i,
                        NewIntStartTime = item.NewIntStartTime,
                        NewIntEndTime = item.NewIntEndTime,
                        IsJumpHoliday = item.IsJumpHoliday
                    });
                }
            }
            return base.Validate();
        }
    }

    public class TeacherSchooltimeSetBatchItem
    {
        public List<byte> Weeks { get; set; }

        public bool IsJumpHoliday { get; set; }

        public int NewIntStartTime { get; set; }

        public int NewIntEndTime { get; set; }

        public long? CourseId { get; set; }

        public string RuleDesc { get; set; }

        public string TimeDesc { get; set; }

        public string StrWeeks { get; set; }
    }

    public class TeacherSchooltimeSetBatchWeek
    {
        public byte Week { get; set; }

        public bool IsJumpHoliday { get; set; }

        public int NewIntStartTime { get; set; }

        public int NewIntEndTime { get; set; }
    }
}
