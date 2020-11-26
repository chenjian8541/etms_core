using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.CacheBucket
{
    public struct BucketTimeOutConfig
    {
        /// <summary>
        /// 一般数据超时时间
        /// </summary>
        public const int ComTimeOutDay = 7;

        /// <summary>
        /// 机构数据超时时间
        /// </summary>
        public const int TenantDataTimeOutDay = 30;

        /// <summary>
        /// 家长保留学员信息 (家长端登陆)
        /// </summary>
        public const int ParentStudentTimeOutDay = 1;

        /// <summary>
        /// 上课通知
        /// </summary>
        public const int NoticeStudentsOfClassBeforeDay = 2;

        /// <summary>
        /// 上课提醒数据
        /// </summary>
        public const int TempStudentClassNotice = 4;

        /// <summary>
        /// 微信access_token过期时间(2小时)
        /// </summary>
        public const int WxAccessTokenBucket = 2;

        /// <summary>
        /// 用户登录信息
        /// </summary>
        public const int UserLoginOnlineDay = 60;

        /// <summary>
        /// 2天的数据
        /// </summary>
        public const int TempTwoDays = 2;

        /// <summary>
        /// 清除数据
        /// </summary>
        public const int ClearDataSaveDays = 60;
    }
}
