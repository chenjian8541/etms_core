using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class TeacherClassTimesView
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
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 一号时间
        /// </summary>
        public DateTime FirstTime { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public decimal ClassTimes { get; set; }

        /// <summary>
        /// 课次
        /// </summary>
        public int ClassCount { get; set; }

        public string UserName { get; set; }

        public string UserPhone { get; set; }
    }
}
