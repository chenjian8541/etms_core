using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Output
{
    public class CouponsStudentUsrPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 核销时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string CouponsTitle { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string CouponsValueDesc { get; set; }

        public string CouponsTypeDesc { get; set; }
    }
}
