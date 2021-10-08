using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class LcsPayQueryOutput
    {
        /// <summary>
        /// <see cref="EmLcsPayQueryPayStatus"/>
        /// </summary>
        public int PayStatus { get; set; }
    }

    public struct EmLcsPayQueryPayStatus
    {
        public const int Success = 1;

        public const int Fail = 2;

        public const int Paying = 3;
    }
}
