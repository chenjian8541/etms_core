using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlienTenantClassRecordStudentGetOutput
    {
        public long CId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

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

        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseId { get; set; }

        public string CourseDesc { get; set; }

        /// <summary>
        /// 扣课时规则  <see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>
        /// </summary>
        public byte DeType { get; set; }

        public string DeTypeDesc { get; set; }

        /// <summary>
        /// 扣的课时
        /// </summary>
        public string DeClassTimes { get; set; }

        /// <summary>
        /// 课消金额
        /// </summary>
        public string DeSum { get; set; }

        /// <summary>
        /// 超上课时
        /// </summary>
        public string ExceedClassTimes { get; set; }

        public bool IsRewardPoints { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int RewardPoints { get; set; }

        /// <summary>
        /// 备注 
        /// </summary>
        public string Remark { get; set; }

        public string DeClassTimesDesc { get; set; }

        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public int ChangeRowState { get; set; }

        public int NewStudentCheckStatus { get; set; }

        public string NewRemark { get; set; }

        /// <summary>
        /// 扣的课时
        /// </summary>
        public string NewDeClassTimes { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int NewRewardPoints { get; set; }

        /// <summary>
        /// 点名或者考勤时默认赠送的积分
        /// </summary>
        public int CheckPointsDefault { get; set; }

        public string SurplusCourseDesc { get; set; }
    }
}
