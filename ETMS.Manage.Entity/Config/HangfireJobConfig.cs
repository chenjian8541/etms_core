using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Manage.Entity.Config
{
    public class HangfireJobConfig
    {
        public string Name { get; set; }

        public string Cron { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 是否允许自动重试
        /// </summary>
        public bool IsCanTryAgain { get; set; }

        /// <summary>
        /// 是否允许并行执行
        /// </summary>
        public bool IsCanParallelProcess { get; set; }
    }
}
