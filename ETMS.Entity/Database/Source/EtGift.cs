using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 礼品
    /// </summary>
    [Table("EtGift")]
    public class EtGift : Entity<long>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///礼品类别
        /// </summary>
        public long? GiftCategoryId { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int Nums { get; set; }

        /// <summary>
        /// 库存不足是否允许兑换
        /// </summary>
        public bool IsLimitNums { get; set; }

        /// <summary>
        /// 所需积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 图片地址
        /// 多个图片以”|”隔开
        /// </summary>
        public string ImgPath { get; set; }

        /// <summary>
        /// 图片详情
        /// </summary>
        public string GiftContent { get; set; }

        /// <summary>
        /// 限领数量
        /// 每人最多领取数量，0表示不限制
        /// </summary>
        public int NumsLimit { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
