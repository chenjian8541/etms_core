using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.External.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IImportBLL : IBaseBLL
    {
        Task<List<EtStudentExtendField>> StudentExtendFieldAllGet();

        Task<ResponseBase> GetImportStudentExcelTemplate(GetImportStudentExcelTemplateRequest request);

        Task<ResponseBase> ImportStudent(ImportStudentRequest request);

        Task<ResponseBase> GetImportCourseTimesExcelTemplate(GetImportCourseTimesExcelTemplateRequest request);

        Task<ResponseBase> ImportCourseTimes(ImportCourseTimesRequest request);

        Task<ResponseBase> GetImportCourseDayExcelTemplate(GetImportCourseDayExcelTemplateRequest request);

        Task<ResponseBase> ImportCourseDay(ImportCourseDayRequest request);
    }
}
