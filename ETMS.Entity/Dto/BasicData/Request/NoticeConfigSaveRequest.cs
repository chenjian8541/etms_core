using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class NoticeConfigSaveRequest : RequestBase
    {
        public byte PeopleType { get; set; }

        public int ScenesType { get; set; }

        public int ExType { get; set; }

        public List<long> ConfigValues { get; set; }

        public override string Validate()
        {
            return base.Validate();
        }
    }
}
