using Microsoft.AspNetCore.Http;
using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.External.Request;
using ETMS.IBusiness;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers.External
{
    public class ImportCourseDayAction
    {
        public async Task<ResponseBase> ProcessAction(IFormCollection collection,
           AppSettings appSettings,
           ImportCourseDayRequest request,
           IImportBLL importBLL)
        {
            if (collection.Files.Count == 0)
            {
                return ResponseBase.CommonError("请选择需要导入的excel文件");
            }
            var file = collection.Files[0];
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (string.IsNullOrEmpty(fileExtension) || Array.IndexOf(appSettings.StaticFilesConfig.UploadExcelFileTypeLimit.Split('|'), fileExtension) == -1)
            {
                return ResponseBase.CommonError("请上传正确的excel文件");
            }
            if (file.Length > appSettings.StaticFilesConfig.UploadFileSizeLimit)
            {
                return ResponseBase.CommonError("文件大小被限制");
            }
            using (var excelStream = file.OpenReadStream())
            {
                var excelContent = ExcelLib.ReadImportCourseDayExcelContent(excelStream, 0, 2);
                if (!string.IsNullOrEmpty(excelContent.Item1))
                {
                    return ResponseBase.CommonError(excelContent.Item1);
                }
                else
                {
                    request.ImportCourseDays = excelContent.Item2;
                    return await importBLL.ImportCourseDay(request);
                }
            }
        }
    }
}
