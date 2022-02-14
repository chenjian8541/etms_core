using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Config
{
    public class ParentMenuConfig
    {
        /// <summary>
        /// 请假审批
        /// </summary>
        public const int LeaveApply = 1;

        /// <summary>
        /// 礼品兑换
        /// </summary>
        public const int GiftExchange = 2;

        /// <summary>
        /// 课后作业
        /// </summary>
        public const int Homework = 3;

        /// <summary>
        /// 评价老师
        /// </summary>
        public const int EvaluateTeacher = 4;

        /// <summary>
        /// 在线预约
        /// </summary>
        public const int ReservationMgr = 5;

        /// <summary>
        /// 微官网
        /// </summary>
        public const int MicroWeb = 6;

        /// <summary>
        /// 在线商城
        /// </summary>
        public const int MallGoods = 7;

        /// <summary>
        /// 电子相册
        /// </summary>
        public const int AlbumLog = 8;

        /// <summary>
        /// 余额
        /// </summary>
        public const int Balance = 20;

        /// <summary>
        /// 我的收藏
        /// </summary>
        public const int GrowingCollect = 21;

        /// <summary>
        /// 上课记录
        /// </summary>
        public const int ClassRecord = 22;

        /// <summary>
        /// 报读课程
        /// </summary>
        public const int MyBuyCourse = 23;

        /// <summary>
        /// 消费记录
        /// </summary>
        public const int ConsumeOrderLog = 24;

        /// <summary>
        /// 兑换记录
        /// </summary>
        public const int ExchangeLog = 25;

        /// <summary>
        /// 积分记录
        /// </summary>
        public const int PointsLog = 26;

        /// <summary>
        /// 优惠券
        /// </summary>
        public const int CouponsLog = 27;

        /// <summary>
        /// 推荐有奖
        /// </summary>
        public const int RecommendAward = 28;

        /// <summary>
        /// 机构通知
        /// </summary>
        public const int WxMessage = 29;

        /// <summary>
        /// 商城订单
        /// </summary>
        public const int MallOrder = 30;

        /// <summary>
        /// 考勤记录
        /// </summary>
        public const int CheckOnLog = 31;

        /// <summary>
        /// 点评记录
        /// </summary>
        public const int EvaluateStudentLog = 32;

        public static List<ParentMenuInfo> AllConfig
        {
            get;
            private set;
        }

        static ParentMenuConfig()
        {
            AllConfig = new List<ParentMenuInfo>();
            AllConfig.Add(new ParentMenuInfo(LeaveApply, "请假审批", 5));
            AllConfig.Add(new ParentMenuInfo(GiftExchange, "礼品兑换", 10));
            AllConfig.Add(new ParentMenuInfo(Homework, "课后作业", 15));
            AllConfig.Add(new ParentMenuInfo(EvaluateTeacher, "评价老师", 20));
            AllConfig.Add(new ParentMenuInfo(AlbumLog, "电子相册", 22));
            AllConfig.Add(new ParentMenuInfo(ReservationMgr, "在线预约", 25));
            AllConfig.Add(new ParentMenuInfo(MallGoods, "在线商城", 27));
            AllConfig.Add(new ParentMenuInfo(MicroWeb, "微官网", 30));

            AllConfig.Add(new ParentMenuInfo(Balance, "余额", 35));
            AllConfig.Add(new ParentMenuInfo(GrowingCollect, "我的收藏", 40));
            AllConfig.Add(new ParentMenuInfo(MyBuyCourse, "报读课程", 50));
            AllConfig.Add(new ParentMenuInfo(MallOrder, "商城订单", 53));
            AllConfig.Add(new ParentMenuInfo(ConsumeOrderLog, "消费记录", 55));
            AllConfig.Add(new ParentMenuInfo(ClassRecord, "上课记录", 56));
            AllConfig.Add(new ParentMenuInfo(EvaluateStudentLog, "点评记录", 57));

            AllConfig.Add(new ParentMenuInfo(ExchangeLog, "兑换记录", 60));
            AllConfig.Add(new ParentMenuInfo(PointsLog, "积分记录", 65));
            AllConfig.Add(new ParentMenuInfo(CheckOnLog, "考勤记录", 67));
            AllConfig.Add(new ParentMenuInfo(CouponsLog, "优惠券", 70));
            AllConfig.Add(new ParentMenuInfo(RecommendAward, "推荐有奖", 75));
            AllConfig.Add(new ParentMenuInfo(WxMessage, "机构通知", 80));
        }
    }

    public class ParentMenuConfigOutput
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string IcoKey { get; set; }

        public string IcoUrl { get; set; }

        public bool IsShow { get; set; }

        public int OrderIndex { get; set; }
    }
}
