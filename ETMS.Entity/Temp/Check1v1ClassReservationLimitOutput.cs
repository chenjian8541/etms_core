using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Temp
{
    public class Check1v1ClassReservationLimitOutput
    {
        public Check1v1ClassReservationLimitOutput() { }

        public Check1v1ClassReservationLimitOutput(string errMsg)
        {
            this.IsCanReservation = false;
            this.ErrMsg = errMsg;
        }

        public bool IsCanReservation { get; set; }

        public string ErrMsg { get; set; }
    }
}
