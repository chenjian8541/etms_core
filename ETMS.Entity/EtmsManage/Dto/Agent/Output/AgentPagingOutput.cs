using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Output
{
    public class AgentPagingOutput
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string Phone { get; set; }

        public string IdCard { get; set; }

        public string Address { get; set; }

        public int EtmsSmsCount { get; set; }

        public DateTime Ot { get; set; }

        public byte IsLock { get; set; }

        public string IsLockDesc { get; set; }

        public string LastLoginOtDesc { get; set; }

        public string Remark { get; set; }

        public string KefuQQ { get; set; }

        public string KefuPhone { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }

        public string UserName { get; set; }

        public List<MyEtmsAccountOutput> MyAccounts { get; set; }
    }

    public class MyEtmsAccountOutput
    {
        public int VersionId { get; set; }

        public int EtmsCount { get; set; }

        public string VersionName { get; set; }
    }
}
