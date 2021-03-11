using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class StudentRecommendConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// 注册
        /// </summary>
        public bool IsOpenRegistered { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int RegisteredGivePoints { get; set; }

        /// <summary>
        /// 奖励充值账户
        /// </summary>
        public decimal RegisteredGiveMoney { get; set; }

        /// <summary>
        /// 购课
        /// </summary>
        public bool IsOpenBuy { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int BuyGivePoints { get; set; }

        /// <summary>
        /// 奖励充值账户
        /// </summary>
        public decimal BuyGiveMoney { get; set; }

        public override string Validate()
        {
            //if (IsOpenRegistered)
            //{
            //    if (RegisteredGivePoints <= 0 && RegisteredGiveMoney <= 0)
            //    {
            //        return "请设置学员注册时的奖励信息";
            //    }
            //}
            //if (IsOpenBuy)
            //{
            //    if (BuyGivePoints <= 0 && BuyGiveMoney <= 0)
            //    {
            //        return "请设置学员购课时的奖励信息";
            //    }
            //}
            return base.Validate();
        }
    }
}
