using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 上课记录学员信息
    /// </summary>
    [Table("EtClassRecordStudent")]
    public class EtClassRecordStudent : Entity<long>
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
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 老师
        /// 如果是一位则直接记录，如果是多为以”,”隔开
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
        /// 类型  <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
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
        /// 从哪个学员课程详情扣的
        /// </summary>
        public long? DeStudentCourseDetailId { get; set; }

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
        /// （评价老师）点评了多少位老师
        /// </summary>
        public int EvaluateTeacherNum { get; set; }

        /// <summary>
        /// （评价老师）是否点评了老师
        /// </summary>
        public bool IsBeEvaluate { get; set; }

        /// <summary>
        /// （评价学员）老师评价此学员的数量
        /// </summary>
        public int EvaluateCount { get; set; }

        /// <summary>
        /// （评价学员）学员读取老师评价的数量
        /// </summary>
        public int EvaluateReadCount { get; set; }

        /// <summary>
        /// 是否奖励积分
        /// </summary>
        public bool IsRewardPoints { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int RewardPoints { get; set; }

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
