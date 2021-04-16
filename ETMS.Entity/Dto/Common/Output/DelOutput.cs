using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Common.Output
{
    public class DelOutput
    {
        public DelOutput(bool isDelFinish, bool isWarningHisData = false)
        {
            this.IsDelFinish = isDelFinish;
            this.IsWarningHisData = isWarningHisData;
        }

        public bool IsDelFinish { get; set; }

        public bool IsWarningHisData { get; set; }
    }
}
