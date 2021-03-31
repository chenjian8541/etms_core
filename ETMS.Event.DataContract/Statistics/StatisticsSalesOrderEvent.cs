using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    public class StatisticsSalesOrderEvent : Event
    {
        public StatisticsSalesOrderEvent(int tenantId) : base(tenantId)
        { }

        public EtOrder Order1 { get; set; }

        public string OldCommissionUser { get; set; }

        public StatisticsSalesOrderOpType OpType { get; set; }
    }

    public enum StatisticsSalesOrderOpType
    {
        /// <summary>
        /// 报名
        /// </summary>
        StudentEnrolment,

        /// <summary>
        /// 退单
        /// </summary>
        ReturnOrder,

        /// <summary>
        /// 转课
        /// </summary>
        TransferCourse,

        /// <summary>
        /// 作废
        /// </summary>
        Repeal,

        /// <summary>
        /// 更换提成人
        /// </summary>
        ChangeCommissionUser
    }
}

