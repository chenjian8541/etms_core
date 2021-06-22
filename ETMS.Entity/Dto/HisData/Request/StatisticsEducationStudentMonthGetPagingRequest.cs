using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class StatisticsEducationStudentMonthGetPagingRequest : RequestPagingBase
    {
        public long? StudentId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId}");
            }
            var firstDate = new DateTime(Year, Month, 1);
            condition.Append($" AND Ot = '{firstDate.EtmsToDateString()}'");
            return condition.ToString();
        }

        public override string Validate()
        {
            if (Year <= 0 || Month <= 0)
            {
                return "请选择月份";
            }
            return base.Validate();
        }
    }
}