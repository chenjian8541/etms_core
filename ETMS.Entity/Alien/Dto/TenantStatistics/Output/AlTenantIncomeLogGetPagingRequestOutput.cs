using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlTenantIncomeLogGetPagingRequestOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual long CId { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmIncomeLogType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        /// <summary>
        /// 关联单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 关联订单
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// 项目名称  <see cref=" ETMS.Entity.Enum.EmIncomeLogProjectType"/>
        /// </summary>
        public long ProjectType { get; set; }

        public string ProjectTypeDesc { get; set; }

        /// <summary>
        /// 收支金额
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmIncomeLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public DateTime Ot { get; set; }

        public string OtDesc { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateOt { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
