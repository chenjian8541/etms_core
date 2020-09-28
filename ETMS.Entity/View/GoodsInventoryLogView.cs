using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class GoodsInventoryLogView
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }

        /// <summary>
        /// 物品ID
        /// </summary>
        public long GoodsId { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmGoodsInventoryType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 变动数量
        /// </summary>
        public int ChangeQuantity { get; set; }

        /// <summary>
        /// 出入库单价
        /// </summary>
        public decimal Prince { get; set; }

        /// <summary>
        /// 出入口总价
        /// </summary>
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string GoodsName { get; set; }

        public string UserName { get; set; }
    }
}
