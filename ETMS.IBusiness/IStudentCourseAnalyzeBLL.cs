using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentCourseAnalyzeBLL : IBaseBLL
    {
        Task CourseAnalyze(StudentCourseAnalyzeEvent request);

        Task CourseDetailAnalyze(StudentCourseDetailAnalyzeEvent request);
    }
}
