using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassTimesBLL : IBaseBLL
    {
        Task<ResponseBase> ClassTimesGetView(ClassTimesGetViewRequest request);

        Task<ResponseBase> ClassTimesClassStudentGet(ClassTimesClassStudentGetRequest request);

        Task<ResponseBase> ClassTimesStudentGet(ClassTimesStudentGetRequest request);

        Task<ResponseBase> ClassTimesGetEditView(ClassTimesGetEditViewRequest request);

        Task<ResponseBase> ClassTimesEdit(ClassTimesEditRequest request);

        Task<ResponseBase> ClassTimesDel(ClassTimesDelRequest request);

        Task<ResponseBase> ClassTimesAddTempStudent(ClassTimesAddTempStudentRequest request);

        Task<ResponseBase> ClassTimesAddTryStudent2(ClassTimesAddTryStudent2Request request);

        Task<ResponseBase> ClassTimesAddTryStudent(ClassTimesAddTryStudentRequest request);

        Task<ResponseBase> ClassTimesAddMakeupStudent(ClassTimesAddMakeupStudentRequest request);

        Task<ResponseBase> ClassTimesAddTryStudentOneToOne(ClassTimesAddTryStudentOneToOneRequest request);

        Task<ResponseBase> ClassTimesDelTempOrTryStudent(ClassTimesDelTempOrTryStudentRequest request);

        Task<ResponseBase> ClassTimesGetPaging(ClassTimesGetPagingRequest request);

        Task<ResponseBase> ClassTimesGetOfWeekTime(ClassTimesGetOfWeekTimeRequest request);

        Task<ResponseBase> ClassTimesCancelTryClassStudent(ClassTimesCancelTryClassStudentRequest request);

        Task<ResponseBase> ClassTimesGetMyWeek(ClassTimesGetMyRequest request);

        Task<ResponseBase> ClassTimesGetMyOt(ClassTimesGetMyOtRequest request);

        Task<ResponseBase> ClassTimesGetOfWeekTimeTeacher(ClassTimesGetOfWeekTimeTeacherRequest request);

        Task<ResponseBase> ClassTimesGetOfWeekTimeRoom(ClassTimesGetOfWeekTimeRoomRequest request);

    }
}
