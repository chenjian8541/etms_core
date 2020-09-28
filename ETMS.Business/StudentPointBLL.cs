using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StudentPointBLL : IStudentPointBLL
    {
        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        public StudentPointBLL(IStudentPointsLogDAL studentPointsLogDAL)
        {
            this._studentPointsLogDAL = studentPointsLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentPointsLogDAL);
        }

        public async Task<ResponseBase> StudentPointLogPaging(StudentPointLogPagingRequest request)
        {
            var pagingData = await _studentPointsLogDAL.GetPaging(request);
            var output = new List<StudentPointLogPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new StudentPointLogPagingOutput()
                {
                    No = p.No,
                    Ot = p.Ot,
                    Remark = p.Remark,
                    StudentId = p.StudentId,
                    Type = p.Type,
                    TypeDesc = EmStudentPointsLogType.GetStudentPointsLogType(p.Type),
                    PointsDesc = EmStudentPointsLogType.GetStudentPointsLogChangPointsDesc(p.Type, p.Points)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentPointLogPagingOutput>(pagingData.Item2, output));
        }
    }
}
