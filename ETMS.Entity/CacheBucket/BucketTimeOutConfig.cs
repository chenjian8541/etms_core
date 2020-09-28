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
    }
}
