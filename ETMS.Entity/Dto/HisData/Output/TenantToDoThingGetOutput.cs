using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class TenantToDoThingGetOutput
    {
        /// <summary>
        /// 学员课程续费
        /// </summary>
        public int StudentCourseNotEnough { get; set; }

        /// <summary>
        /// 欠费补交
        /// </summary>
        public int StudentOrderArrears { get; set; }

        /// <summary>
        /// 班级未排课
        /// </summary>
        public int ClassNotScheduled { get; set; }

        /// <summary>
        /// 课次超时未点名
        /// </summary>
        public int ClassTimesTimeOutNotCheckSign { get; set; }

        /// <summary>
        /// 物品库存提醒
        /// </summary>
        public int GoodsInventoryNotEnough { get; set; }

        /// <summary>
        /// 缺课
        /// </summary>
        public int ClassRecordAbsent { get; set; }

        /// <summary>
        /// 请假申请
        /// </summary>
        public int StudentLeaveApplyLogCount { get; set; }

        /// <summary>
        /// 试听申请
        /// </summary>
        public int TryCalssApplyLogCount { get; set; }
    }
}
