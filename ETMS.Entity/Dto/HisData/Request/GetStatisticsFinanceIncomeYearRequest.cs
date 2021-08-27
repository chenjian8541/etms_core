using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class GetStatisticsFinanceIncomeYearRequest : RequestBase
    {
        public int? Year { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmIncomeLogType"/>
        /// </summary>
        public byte Type { get; set; }

        public override string Validate()
        {
            return base.Validate();
        }
    }
}