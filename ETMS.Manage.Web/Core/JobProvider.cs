﻿using ETMS.IOC;
using ETMS.Manage.Entity.Config;
using ETMS.Manage.Jobs;
using Hangfire;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ETMS.Manage.Web.Core
{
    public class JobProvider
    {
        public static void InitJobs()
        {
            var hangfireJobsJson = File.ReadAllText("hangfireJobs.json");
            var hangfireJobConfigs = JsonConvert.DeserializeObject<List<HangfireJobConfig>>(hangfireJobsJson);
            var jobsAssemblyTypes = Assembly.Load("ETMS.Manage.Jobs").GetTypes();
            foreach (var t in jobsAssemblyTypes)
            {
                if (!t.IsClass)
                {
                    continue;
                }
                if (t.BaseType != typeof(BaseJob))
                {
                    continue;
                }
                var jobConfig = hangfireJobConfigs.FirstOrDefault(p => p.Name == t.Name);
                RecurringJob.AddOrUpdate(() => ((BaseJob)CustomServiceLocator.GetInstance(t)).Execute(new JobExecutionContext()
                {
                    JobConfig = jobConfig
                }), jobConfig.Cron, TimeZoneInfo.Local);
            }
        }
    }
}
