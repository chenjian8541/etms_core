using ETMS.Entity.Common;
using ETMS.Entity.Dto.Interaction.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassRecordEvaluateBLL : IBaseBLL
    {
        Task<ResponseBase> TeacherClassRecordEvaluateGetPaging(TeacherClassRecordEvaluateGetPagingRequest request);

        Task<ResponseBase> TeacherClassRecordEvaluateGetDetail(TeacherClassRecordEvaluateGetDetailRequest request);

        Task<ResponseBase> TeacherClassRecordEvaluateStudent(TeacherClassRecordEvaluateStudentRequest request);

        Task<ResponseBase> TeacherClassRecordEvaluateStudentDetail(TeacherClassRecordEvaluateStudentDetailRequest request);

        Task<ResponseBase> TeacherClassRecordEvaluateSubmit(TeacherClassRecordEvaluateSubmitRequest request);

        Task<ResponseBase> TeacherEvaluateLogGetPaging(TeacherEvaluateLogGetPagingRequest request);
    }
}
