using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassRecordAbsenceLogPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 老师
        /// </summary>
        public string TeachersDesc { get; set; }

        /// <summary>
        /// 学员类型  <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        public string StudentTypeDesc { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        public string StudentCheckStatusDesc { get; set; }

        public DateTime CheckOt { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        /// <summary>
        /// 消耗的课程ID
        /// </summary>
        public string CourseDesc { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        /// <summary>
        /// 扣课时规则  <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        public string DeTypeDesc { get; set; }

        /// <summary>
        /// 扣的课时
        /// </summary>
        public int DeClassTimes { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public int ExceedClassTimes { get; set; }

        /// <summary>
        /// 点名老师
        /// </summary>
        public long CheckUserId { get; set; }

        public string CheckUserName { get; set; }

        /// <summary>
        /// 处理状态  <see cref="ETMS.Entity.Enum.EmClassRecordAbsenceHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        public string HandleStatusDesc { get; set; }

        /// <summary>
        /// 标记内容
        /// </summary>
        public string HandleContent { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public string HandleOtDesc { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public string HandleUserName { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmClassRecordStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string DeClassTimesDesc { get; set; }

        public string WeekDesc { get; set; }
    }
}
