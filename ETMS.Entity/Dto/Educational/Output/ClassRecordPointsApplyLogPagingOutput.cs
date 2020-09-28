using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassRecordPointsApplyLogPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 点名记录
        /// </summary>
        public long ClassRecordId { get; set; }

        /// <summary>
        /// 老师ID
        /// </summary>
        public long TeacherId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string ClassRoomIds { get; set; }

        /// <summary>
        /// 学员类型 <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        /// <summary>
        /// 试听记录 
        /// 试听学员对应的试听记录
        /// </summary>
        public long? StudentTryCalssLogId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public DateTime ClassOt { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseId { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 积分数
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyOt { get; set; }

        /// <summary>
        /// 点名时间
        /// </summary>
        public DateTime CheckOt { get; set; }

        /// <summary>
        /// 点名老师
        /// </summary>
        public long CheckUserId { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmClassRecordPointsApplyHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? HandleOt { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public long? HandleUser { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmClassRecordStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 申请老师
        /// </summary>
        public string TeacherDesc { get; set; }

        public string StudentTypeDesc { get; set; }

        public string StudentCheckStatusDesc { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        /// <summary>
        /// 消耗的课程ID
        /// </summary>
        public string CourseDesc { get; set; }

        public string HandleStatusDesc { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public string HandleOtDesc { get; set; }

        public string HandleContent { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public string HandleUserName { get; set; }

        public string StatusDesc { get; set; }

        public string WeekDesc { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }
    }
}
