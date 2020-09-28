using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 缺勤记录
    /// </summary>
    [Table("EtClassRecordAbsenceLog")]
    public class EtClassRecordAbsenceLog : Entity<long>
    {
        /// <summary>
		/// 班级ID
		/// </summary>
		public long ClassId { get; set; }

        /// <summary>
        /// 点名记录
        /// </summary>
        public long ClassRecordId { get; set; }

        /// <summary>
        /// 上课记录学员表Id
        /// </summary>
        public long ClassRecordStudentId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 老师
        /// </summary>
        public string Teachers { get; set; }

        /// <summary>
        /// 老师个数
        /// </summary>
        public int TeacherNum { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string ClassRoomIds { get; set; }

        /// <summary>
        /// 学员类型  <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        /// <summary>
        /// 试听记录Id
        /// </summary>
        public long? StudentTryCalssLogId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public DateTime ClassOt { get; set; }

        /// <summary>
        /// 消耗的课程ID
        /// </summary>
        public long CourseId { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        /// <summary>
        /// 开始点
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 结束点
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        /// <summary>
        /// 扣课时规则  <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        /// <summary>
        /// 扣的课时
        /// </summary>
        public int DeClassTimes { get; set; }

        /// <summary>
        /// 课消金额
        /// </summary>
        public decimal DeSum { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public int ExceedClassTimes { get; set; }

        /// <summary>
        /// 点名时间
        /// </summary>
        public DateTime CheckOt { get; set; }

        /// <summary>
        /// 点名老师
        /// </summary>
        public long CheckUserId { get; set; }

        /// <summary>
        /// 处理状态  <see cref="ETMS.Entity.Enum.EmClassRecordAbsenceHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        /// <summary>
        /// 标记内容
        /// </summary>
        public string HandleContent { get; set; }

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
    }
}
