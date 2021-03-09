using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentContractsBLL : IBaseBLL
    {
        Task<ResponseBase> StudentEnrolment(StudentEnrolmentRequest request);

        Task StudentEnrolmentEvent(StudentEnrolmentEvent request);
    }
}
