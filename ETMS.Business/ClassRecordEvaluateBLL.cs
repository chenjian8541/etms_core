using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business
{
    public class ClassRecordEvaluateBLL : IClassRecordEvaluateBLL
    {
        private readonly IClassRecordDAL _classRecordDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserDAL _userDAL;

        private readonly IClassDAL _classDAL;

        public ClassRecordEvaluateBLL(IClassRecordDAL classRecordDAL, IStudentDAL studentDAL, IUserDAL userDAL, IClassDAL classDAL)
        {
            this._classRecordDAL = classRecordDAL;
            this._studentDAL = studentDAL;
            this._userDAL = userDAL;
            this._classDAL = classDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _classRecordDAL, _studentDAL, _userDAL, _classDAL);
        }
    }
}
