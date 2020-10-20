using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class SmsLogBLL : ISmsLogBLL
    {
        private readonly IStudentSmsLogDAL _studentSmsLogDAL;

        public SmsLogBLL(IStudentSmsLogDAL studentSmsLogDAL)
        {
            this._studentSmsLogDAL = studentSmsLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSmsLogDAL);
        }

        public async Task<ResponseBase> StudentSmsLogGetPaging(StudentSmsLogGetPagingRequest request)
        {
            return null;
        }
    }
}
