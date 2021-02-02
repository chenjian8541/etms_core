using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class TryCalssApplyLogBLL : ITryCalssApplyLogBLL
    {
        private readonly ITryCalssApplyLogDAL _tryCalssApplyLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly INoticeBLL _noticeBLL;

        private readonly IEventPublisher _eventPublisher;

        public TryCalssApplyLogBLL(ITryCalssApplyLogDAL tryCalssApplyLogDAL, IUserDAL userDAL, IStudentDAL studentDAL, IUserOperationLogDAL userOperationLogDAL,
            INoticeBLL noticeBLL, IEventPublisher eventPublisher)
        {
            this._tryCalssApplyLogDAL = tryCalssApplyLogDAL;
            this._userDAL = userDAL;
            this._studentDAL = studentDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._noticeBLL = noticeBLL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this._noticeBLL.InitDataAccess(tenantId);
            this.InitDataAccess(tenantId, _tryCalssApplyLogDAL, _userDAL, _studentDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> TryCalssApplyLogPaging(TryCalssApplyLogPagingRequest request)
        {
            var pagingData = await _tryCalssApplyLogDAL.GetPaging(request);
            var output = new List<TryCalssApplyLogPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            foreach (var p in pagingData.Item1)
            {
                var studentName = string.Empty;
                var phone = string.Empty;

                if (p.StudentId != null)
                {
                    var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId.Value);
                    if (student != null)
                    {
                        studentName = student.Name;
                        phone = student.Phone;
                    }
                }
                else
                {
                    phone = p.Phone;
                    studentName = p.TouristName;
                }
                var recommandStudentDesc = string.Empty;
                if (p.RecommandStudentId != null)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.RecommandStudentId.Value);
                    if (myStudent != null)
                    {
                        recommandStudentDesc = $"学员：{myStudent.Name},{myStudent.Phone}";
                    }
                }
                output.Add(new TryCalssApplyLogPagingOutput()
                {
                    ApplyOt = p.ApplyOt,
                    ClassOtDesc = p.ClassOt.EtmsToDateString(),
                    ClassTime = p.ClassTime,
                    CourseDesc = p.CourseDesc,
                    HandleOtDesc = p.HandleOt.EtmsToString(),
                    HandleRemark = p.HandleRemark,
                    HandleStatus = p.HandleStatus,
                    HandleStatusDesc = EmTryCalssApplyHandleStatus.GetTryCalssApplyHandleStatusDesc(p.HandleStatus),
                    HandleUserDesc = p.HandleUser == null ? string.Empty : await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.HandleUser.Value),
                    SourceType = p.SourceType,
                    SourceTypeDesc = EmTryCalssSourceType.GetTryCalssSourceTypeDesc(p.SourceType),
                    Phone = phone,
                    StudentName = studentName,
                    RecommandStudentDesc = recommandStudentDesc,
                    CId = p.Id,
                    TouristRemark = p.TouristRemark
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TryCalssApplyLogPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> TryCalssApplyLogHandle(TryCalssApplyLogHandleRequest request)
        {
            var applyLog = await _tryCalssApplyLogDAL.GetTryCalssApplyLog(request.CId);
            if (applyLog == null)
            {
                return ResponseBase.CommonError("试听申请记录不存在");
            }
            if (applyLog.HandleStatus != EmTryCalssApplyHandleStatus.Unreviewed)
            {
                return ResponseBase.CommonError("此记录已审核");
            }
            applyLog.HandleStatus = request.NewHandleStatus;
            applyLog.HandleOt = DateTime.Now;
            applyLog.HandleRemark = request.HandleRemark;
            applyLog.HandleUser = request.LoginUserId;
            await _tryCalssApplyLogDAL.EditTryCalssApplyLog(applyLog);
            await _noticeBLL.TryCalssApplyLogHandle();

            _eventPublisher.Publish(new ResetTenantToDoThingEvent(request.LoginTenantId));
            await _userOperationLogDAL.AddUserLog(request, "审核试听申请记录", EmUserOperationType.TryCalssApplyLogManage);
            return ResponseBase.Success();
        }
    }
}
