using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ICommonHandlerBLL: IBaseBLL
    {
        Task AnalyzeStudentCourseDetailRestoreNormalStatus(long deStudentCourseDetailId);

        Task DelOrdersRefreshAboutStatus(IEnumerable<OrderStudentOt> orders);
    }
}
