using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class HolidaySettingAddRequest : RequestBase
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (StartTime == null)
            {
                return "请选择开始日期";
            }
            if (EndTime == null)
            {
                return "请选择结束日期";
            }
            if (StartTime > EndTime)
            {
                return "开始日期不能大于结束日期";
            }
            return string.Empty;
        }
    }
}
