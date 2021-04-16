using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentDelOutput
    {
        public StudentDelOutput(bool isDelFinish, bool isWarningHisData = false)
        {
            this.IsDelFinish = isDelFinish;
            this.IsWarningHisData = isWarningHisData;
        }

        public bool IsDelFinish { get; set; }

        public bool IsWarningHisData { get; set; }
    }
}
