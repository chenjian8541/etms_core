namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmActivityType
    {
        /// <summary>
        /// 团购
        /// </summary>
        public const int GroupPurchase = 0;

        /// <summary>
        /// 砍价
        /// </summary>
        public const int Haggling = 1;

        /// <summary>
        /// 秒杀
        /// </summary>
        public const int Seckill = 2;

        /// <summary>
        /// 分销
        /// </summary>
        public const int Distribution = 3;

        public static string GetActivityTypeDesc(int t)
        {
            switch (t)
            {
                case GroupPurchase:
                    return "团购";
                case Haggling:
                    return "砍价";
                case Seckill:
                    return "秒杀";
                case Distribution:
                    return "分销";
            }
            return string.Empty;
        }
    }
}
