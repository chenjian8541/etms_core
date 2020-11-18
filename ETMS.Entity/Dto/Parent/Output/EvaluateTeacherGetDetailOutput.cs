using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class EvaluateTeacherGetDetailOutput
    {
        public long Id { get; set; }

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

        public string TeachersDesc { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string ClassRoomIdsDesc { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        public string StudentCheckStatusDesc { get; set; }

        public string ClassOtDesc { get; set; }

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

        /// <summary>
        /// 评价信息
        /// </summary>
        public List<EvaluateTeacherGetDetailTeacherOutput> EvaluateTeachers { get; set; }
    }

    public class EvaluateTeacherGetDetailTeacherOutput
    {
        /// <summary>
        /// 老师ID
        /// </summary>
        public long TeacherId { get; set; }

        /// <summary>
        /// 老师名称
        /// </summary>
        public string TeacherName { get; set; }

        /// <summary>
        /// 是否点评
        /// </summary>
        public bool IsBeEvaluate { get; set; }

        /// <summary>
        /// 星级
        /// </summary>
        public int StarValue { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        public string EvaluateContent { get; set; }
    }
}
