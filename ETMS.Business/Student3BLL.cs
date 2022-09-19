using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Dto.Student.Output;
using Microsoft.AspNetCore.Http;
using ETMS.Utility;
using ETMS.Entity.Config;
using ETMS.Business.Common;
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.IBusiness.IncrementLib;
using ETMS.Entity.Dto.Common.Output;
using ETMS.Entity.View;
using ETMS.Event.DataContract.Statistics;

namespace ETMS.Business
{
    public class Student3BLL : IStudent3BLL
    {
        private readonly IEventPublisher _eventPublisher;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public Student3BLL(IEventPublisher eventPublisher, IUserOperationLogDAL userOperationLogDAL)
        {
            this._eventPublisher = eventPublisher;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _userOperationLogDAL);
        }

        public async Task<ResponseBase> SendToClassNotice(SendToClassNoticeRequest request)
        {
            _eventPublisher.Publish(new NoticeStudentToClassEvent(request.LoginTenantId)
            {
                StudentIds = request.StudentIds
            });

            await _userOperationLogDAL.AddUserLog(request, $"未上课提醒-给{request.StudentIds.Count}位学员发送上课提醒", EmUserOperationType.StudentManage);

            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SendArrearageNoticeBatch(SendArrearageNoticeBatchRequest request)
        {
            _eventPublisher.Publish(new NoticeSendArrearageBatchEvent(request.LoginTenantId)
            {
                OrderIds = request.OrderIds
            });

            await _userOperationLogDAL.AddUserLog(request, $"学员欠费提醒-给{request.OrderIds.Count}位学员发送欠费提醒", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> SendStudentCourseNotEnoughBatch(SendStudentCourseNotEnoughBatchRequest request)
        {
            _eventPublisher.Publish(new NoticeStudentCourseNotEnoughBatchEvent(request.LoginTenantId)
            {
                StudentCourses = request.StudentCourses
            });

            await _userOperationLogDAL.AddUserLog(request, $"续费提醒-给{request.StudentCourses.Count}位学员发送课程不足待续费提醒", EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }
    }
}
