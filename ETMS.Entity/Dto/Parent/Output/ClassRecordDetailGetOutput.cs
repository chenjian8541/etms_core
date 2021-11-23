using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class ClassRecordDetailGetOutput
    {
        public ClassRecordBascInfo ClassRecordBascInfo { get; set; }

        public List<ClassRecordEvaluateStudentInfo> EvaluateStudentInfos { get; set; }
    }

    public class ClassRecordBascInfo {
        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentAvatarUrl { get; set; }

        public string TeachersDesc { get; set; }

        /// <summary>
        /// 老师个数
        /// </summary>
        public int TeacherNum { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string ClassRoomIdsDesc { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        public string StudentTypeDesc { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        public string StudentCheckStatusDesc { get; set; }

        public string ClassOtDesc { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseId { get; set; }

        public string CourseDesc { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        public string WeekDesc { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int RewardPoints { get; set; }

        public string DeClassTimesDesc { get; set; }
    }

    public class ClassRecordEvaluateStudentInfo {

        public long EvaluateStudentId { get; set; }

        public long TeacherId { get; set; }

        public string TeacherName { get; set; }

        public string TeacherAvatar { get; set; }

        public string EvaluateOtDesc { get; set; }

        public string EvaluateContent { get; set; }

        public List<string> EvaluateMedias { get; set; }
    }
}
