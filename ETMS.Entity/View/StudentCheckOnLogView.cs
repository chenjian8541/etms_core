using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class StudentCheckOnLogView
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary> 
        /// 考勤形式  <see cref="ETMS.Entity.Enum.EmStudentCheckOnLogCheckForm"/>
        /// </summary>
        public byte CheckForm { get; set; }

        /// <summary>
        /// 考勤类型  <see cref="ETMS.Entity.Enum.EmStudentCheckOnLogCheckType"/>
        /// </summary>
        public byte CheckType { get; set; }

        /// <summary>
        /// 考勤介质
        /// 磁卡卡号/人脸图片key
        /// </summary>
        public string CheckMedium { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        public DateTime CheckOt { get; set; }

        /// <summary>
        /// 考勤日期
        /// </summary>
        public DateTime? CheckOtDate { get; set; }

        /// <summary>
        /// 上课课次
        /// </summary>
        public long? ClassTimesId { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public long? ClassId { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public long? CourseId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public string ClassOtDesc { get; set; }

        /// <summary>
        /// 扣课方式  <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        /// <summary>
        /// 扣减的课时
        /// </summary>
        public decimal DeClassTimes { get; set; }

        /// <summary>
        /// 课消金额
        /// </summary>
        public decimal DeSum { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public decimal ExceedClassTimes { get; set; }

        /// <summary>
        /// 扣学员剩余课时记录ID 
        /// </summary>
        public long? DeStudentCourseDetailId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmStudentCheckOnLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Points { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public long? TrackUser { get; set; }
    }
}
