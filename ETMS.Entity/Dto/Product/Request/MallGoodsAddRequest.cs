using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Entity.View.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class MallGoodsAddRequest : RequestBase
    {
        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        public long RelatedId { get; set; }

        public string RelatedName { get; set; }

        public string Name { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal Price { get; set; }

        public int Points { get; set; }

        public List<MallGoodsTagItem> TagItems { get; set; }

        public List<MallGoodsSpecItem> SpecItems { get; set; }

        public string ImgCoverKey { get; set; }

        public List<string> ImgDetailKeys { get; set; }

        public string GsContent { get; set; }

        public CoursePriceRule CoursePriceRules { get; set; }

        public override string Validate()
        {
            if (RelatedId <= 0)
            {
                return "请选择关联的商品";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入商品展示名称";
            }
            if (ProductType != EmProductType.Course && Price <= 0)
            {
                return "请输入售卖价格";
            }
            if (ProductType == EmProductType.Course)
            {
                if (CoursePriceRules == null)
                {
                    return "请设置售卖价格";
                }
                var msg = CoursePriceRules.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    return msg;
                }
            }
            if (string.IsNullOrEmpty(ImgCoverKey))
            {
                return "请选择商品封面图";
            }
            if (TagItems != null && TagItems.Count > 2)
            {
                return "一件商品最多设置两个标签";
            }
            if (SpecItems != null && SpecItems.Count > 10)
            {
                return "一件商品最多设置10种规格";
            }
            return string.Empty;
        }
    }
}
