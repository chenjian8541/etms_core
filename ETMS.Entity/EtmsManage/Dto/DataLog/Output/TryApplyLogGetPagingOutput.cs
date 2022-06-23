using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Output
{
    public class TryApplyLogGetPagingOutput
    {
        public long CId { get; set; }
        public string Name { get; set; }

        public string LinkPhone { get; set; }

        public DateTime Ot { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTryApplyLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string HandleRemark { get; set; }

        /// <summary>
        /// 客户端类型  <see cref="EmUserOperationLogClientType"/>
        /// </summary>
        public int ClientType { get; set; }

        public string ClientTypeDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public DateTime? HandleOt { get; set; }
    }
}
