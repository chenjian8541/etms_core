using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class IncomeLogAddRequest : RequestBase
    {
        public long ProjectType { get; set; }

        public byte Type { get; set; }

        public byte PayType { get; set; }

        public decimal Sum { get; set; }

        public string AccountNo { get; set; }

        public DateTime Ot { get; set; }

        public string Remark { get; set; }

        public override string Validate()
        {
            if (Sum <= 0)
            {
                return "请输入收支金额";
            }
            return string.Empty;
        }
    }
}
