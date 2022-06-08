using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class TenantSuixingAccountGetOutput
    {
        public int PayUnionType { get; set; }

        public TenantSuixingAccountInfo AccountInfo { get; set; }
    }

    public class TenantSuixingAccountInfo
    {
        public string Mno { get; set; }

        public string MerName { get; set; }

        public string MecDisNm { get; set; }

        public string MblNo { get; set; }

        public string OperationalType { get; set; }

        public string HaveLicenseNo { get; set; }

        public string MecTypeFlag { get; set; }

        public string ParentMno { get; set; }
    }
}
