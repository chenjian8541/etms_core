using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassCheckSignRequest : RequestBase
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public long ClassId { get; set; }

        /// <summary>
        /// 课次ID  null则为直接点名
        /// </summary>
        public long? ClassTimesId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public DateTime ClassOt { get; set; }

        /// <summary>
        /// 上课时间 开始
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 上课时间  结束
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 授课课程
        /// </summary>
        public List<long> CourseIds { get; set; }

        /// <summary>
        /// 上课老师
        /// </summary>
        public List<long> TeacherIds { get; set; }

        /// <summary>
        /// 授课课时
        /// </summary>
        public decimal ClassTimes { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public List<long> ClassRoomIds { get; set; }

        /// <summary>
        /// 上课内容
        /// </summary>
        public string ClassContent { get; set; }

        /// <summary>
        /// 上课学员
        /// </summary>
        public List<ClassCheckSignStudent> Students { get; set; }

        public override string Validate()
        {
            if (ClassId < 0)
            {
                return "无效的班级";
            }
            if (ClassOt == null)
            {
                return "请选择上课日期";
            }
            if (ClassOt.Date > DateTime.Now.Date)
            {
                return "上课日期不能大于当前日期";
            }
            if (StartTime <= 0 || EndTime <= 0)
            {
                return "请选择上课时间";
            }
            if (StartTime >= EndTime)
            {
                return "上课时间格式不正确";
            }
            if (TeacherIds == null || !TeacherIds.Any())
            {
                return "请选择上课老师";
            }
            if (CourseIds == null || !CourseIds.Any())
            {
                return "请选择授课课程";
            }
            if (ClassTimes <= 0)
            {
                return "授课课时必须大于0";
            }
            if (Students == null || !Students.Any())
            {
                return "请选择上课学员";
            }
            foreach (var s in Students)
            {
                var msg = s.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    return msg;
                }
            }
            return string.Empty;
        }
    }


    public class ClassCheckSignStudent : IValidate
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 学员姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 消耗课程ID
        /// </summary>
        public long CourseId { get; set; }

        /// <summary>
        /// 学员类型 <see cref="ETMS.Entity.Enum.EmClassStudentType"/>
        /// </summary>
        public byte StudentType { get; set; }

        /// <summary>
        /// 到课状态 <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        /// <summary>
        /// 扣课时
        /// </summary>
        public decimal DeClassTimes { get; set; }

        /// <summary>
        /// 试听记录
        /// </summary>
        public long? StudentTryCalssLogId { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int RewardPoints { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 对象的有效性验证方法
        /// </summary>
        /// <returns></returns>
        public string Validate()
        {
            if (StudentId <= 0)
            {
                return "学员信息无效";
            }
            if (CourseId <= 0)
            {
                return "请选择消耗课程";
            }
            if (DeClassTimes < 0)
            {
                return "扣减的课时不能小于0";
            }
            if (RewardPoints < 0)
            {
                return "积分必须大于0";
            }
            return string.Empty;
        }
    }
}
