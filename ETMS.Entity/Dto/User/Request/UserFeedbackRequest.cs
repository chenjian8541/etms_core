using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserFeedbackRequest : RequestBase
    {
        public string LinkPhone { get; set; }

        public string ProblemType { get; set; }

        public string ProblemLevel { get; set; }

        public string ProblemTheme { get; set; }

        public string ProblemContent { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(ProblemContent))
            {
                return "请输入反馈内容";
            }
            return base.Validate();
        }
    }
}
