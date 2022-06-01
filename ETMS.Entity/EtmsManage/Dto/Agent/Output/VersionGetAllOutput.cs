using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class VersionGetAllOutput
    {
        public int CId { get; set; }

        public string Name { get; set; }

        public string DetailInfo { get; set; }

        public string Remark { get; set; }

        public int Value { get; set; }

        public string Label { get; set; }

        public bool IsLimitLoginPC { get; set; }

        public bool IsLimitLoginWxParent { get; set; }

        public bool IsLimitLoginWxTeacher { get; set; }

        public bool IsLimitLoginAppTeacher { get; set; }
    }
}
