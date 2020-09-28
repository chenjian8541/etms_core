using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentSetLearningManagerRequest : RequestBase
    {
        public List<long> StudentCIds { get; set; }

        public long NewLearningManager { get; set; }

        public override string Validate()
        {
            if (StudentCIds == null || !StudentCIds.Any())
            {
                return "请求数据不合法";
            }
            if (NewLearningManager <= 0)
            {
                return "请选择学管师";
            }
            return base.Validate();
        }
    }
}