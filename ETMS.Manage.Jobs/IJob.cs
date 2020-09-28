using ETMS.Manage.Entity.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    public interface IJob
    {
        void Execute(JobExecutionContext context);
    }
}
