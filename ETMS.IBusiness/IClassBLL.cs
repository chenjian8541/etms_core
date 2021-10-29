using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Event.DataContract;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassBLL : IBaseBLL
    {
        Task<ResponseBase> ClassAdd(ClassAddRequest request);

        Task<ResponseBase> ClassEdit(ClassEditRequest request);

        Task<ResponseBase> ClassGet(ClassGetRequest request);

        Task<ResponseBase> ClassViewGet(ClassViewGetRequest request);

        Task<ResponseBase> ClassBascGet(ClassBascGetRequest request);

        Task<ResponseBase> ClassDel(ClassDelRequest request);

        Task<ResponseBase> ClassOverOneToMany(ClassOverRequest request);

        Task<ResponseBase> ClassOverOneToOne(ClassOverOneToOneRequest request);

        Task<ResponseBase> ClassSetTeachers(SetClassTeachersRequest request);

        Task<ResponseBase> ClassGetPaging(ClassGetPagingRequest request);

        Task<ResponseBase> ClassGetPagingSimple(ClassGetPagingRequest request);

        Task<ResponseBase> ClassStudentAdd(ClassStudentAddRequest request);

        Task<ResponseBase> ClassStudentRemove(ClassStudentRemoveRequest request);

        Task<ResponseBase> ClassStudentGet(ClassStudentGetRequest request);

        Task<ResponseBase> ClassTimesRuleAdd1(ClassTimesRuleAdd1Request request);

        Task<ResponseBase> ClassTimesRuleAdd2(ClassTimesRuleAdd2Request request);

        Task<ResponseBase> ClassTimesRuleDel(ClassTimesRuleDelRequest request);

        Task SyncClassInfoProcessEvent(SyncClassInfoEvent request);

        Task<ResponseBase> ClassTimesRuleGet(ClassTimesRuleGetRequest request);

        Task<ResponseBase> ClassMyGet(ClassMyGetRequest request);

        Task<ResponseBase> ClassStudentTransfer(ClassStudentTransferRequest request);

        Task<ResponseBase> ClassPlacement(ClassPlacementRequest request);

        Task<ResponseBase> ClassTimesRuleDefiniteGet(ClassTimesRuleDefiniteGetRequest request);

        Task<ResponseBase> ClassTimesRuleEdit(ClassTimesRuleEditRequest request);

        Task<ResponseBase> ClassChangeOnlineSelStatus(ClassChangeOnlineSelStatusRequest request);
    }
}
