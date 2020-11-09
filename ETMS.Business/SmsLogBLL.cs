using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Marketing.Output;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Enum;
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

        private readonly IStudentDAL _studentDAL;

        public SmsLogBLL(IStudentSmsLogDAL studentSmsLogDAL, IStudentDAL studentDAL)
        {
            this._studentSmsLogDAL = studentSmsLogDAL;
            this._studentDAL = studentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentSmsLogDAL);
        }

        public async Task<ResponseBase> StudentSmsLogGetPaging(StudentSmsLogGetPagingRequest request)
        {
            var pagingData = await _studentSmsLogDAL.GetPaging(request);
            var output = new List<StudentSmsLogGetPagingOutput>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var studetName = string.Empty;
                if (p.StudentId != null)
                {
                    var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId.Value);
                    if (student != null)
                    {
                        studetName = student.Name;
                    }
                }
                output.Add(new StudentSmsLogGetPagingOutput()
                {
                    CId = p.Id,
                    DeCount = p.DeCount,
                    Ot = p.Ot,
                    Phone = p.Phone,
                    SmsContent = p.SmsContent,
                    Status = p.Status,
                    StatusDesc = EmSmsLogStatus.GetSmsLogStatusDesc(p.Status),
                    StudentId = p.StudentId,
                    StudentName = studetName,
                    Type = p.Type,
                    TypeDesc = EmStudentSmsLogType.GetStudentSmsLogTypeDesc(p.Type)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentSmsLogGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
