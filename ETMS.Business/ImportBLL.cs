using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.External.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ImportBLL : IImportBLL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        private readonly IStudentSourceDAL _studentSourceDAL;

        private readonly IStudentRelationshipDAL _studentRelationshipDAL;

        private readonly IGradeDAL _gradeDAL;

        private readonly ISysTenantDAL _sysTenantDAL;


        public ImportBLL(IHttpContextAccessor httpContextAccessor, IAppConfigurtaionServices appConfigurtaionServices,
            IStudentSourceDAL studentSourceDAL, IStudentRelationshipDAL studentRelationshipDAL, IGradeDAL gradeDAL, ISysTenantDAL sysTenantDAL)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._appConfigurtaionServices = appConfigurtaionServices;
            this._studentSourceDAL = studentSourceDAL;
            this._studentRelationshipDAL = studentRelationshipDAL;
            this._gradeDAL = gradeDAL;
            this._sysTenantDAL = sysTenantDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSourceDAL, _studentRelationshipDAL, _gradeDAL);
        }

        public async Task<ResponseBase> GetImportStudentExcelTemplate(GetImportStudentExcelTemplateRequest request)
        {
            var tenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var checkImportStudentTemplateFileResult = ExcelLib.CheckImportStudentExcelTemplate(tenant.TenantCode, _appConfigurtaionServices.AppSettings.StaticFilesConfig.ServerPath);
            if (!checkImportStudentTemplateFileResult.IsExist)
            {
                ExcelLib.GenerateImportStudentExcelTemplate(new ImportStudentExcelTemplateRequest()
                {
                    CheckResult = checkImportStudentTemplateFileResult,
                    GradeAll = await _gradeDAL.GetAllGrade(),
                    StudentRelationshipAll = await _studentRelationshipDAL.GetAllStudentRelationship(),
                    StudentSourceAll = await _studentSourceDAL.GetAllStudentSource(),
                });
            }
            return ResponseBase.Success(UrlHelper.GetUrl(_httpContextAccessor, _appConfigurtaionServices.AppSettings.StaticFilesConfig.VirtualPath, checkImportStudentTemplateFileResult.UrlKey));
        }
    }
}
