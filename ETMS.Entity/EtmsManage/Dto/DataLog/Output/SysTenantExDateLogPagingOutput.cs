﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Output
{
    public class SysTenantExDateLogPagingOutput
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public string BeforeDateDesc { get; set; }

        public string AfterDateDesc { get; set; }

        public byte ChangeType { get; set; }

        public string ChangeDesc { get; set; }

        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
