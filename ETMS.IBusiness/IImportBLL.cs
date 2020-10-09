using ETMS.Entity.Common;
using ETMS.Entity.Dto.External.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IImportBLL : IBaseBLL
    {
        Task<ResponseBase> GetImportStudentExcelTemplate(GetImportStudentExcelTemplateRequest request);

        Task<ResponseBase> ImportStudent(ImportStudentRequest request);
    }
}
