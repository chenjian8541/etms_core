using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Enum;
using ETMS.IEventProvider;
using ETMS.Utility;
using ETMS.IDataAccess.Statistics;
using ETMS.Event.DataContract.Statistics;
using ETMS.Entity.Temp.View;
using Newtonsoft.Json;
using ETMS.Entity.Database.Source;
using ETMS.Business.Common;
using ETMS.Entity.Temp;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.IBusiness;

namespace ETMS.Business.EventConsumer
{
    public class EvStudentAccountRechargeBLL : IEvStudentAccountRechargeBLL
    {
        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentAccountRechargeCoreBLL _studentAccountRechargeCoreBLL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;
        public EvStudentAccountRechargeBLL(IStudentAccountRechargeDAL studentAccountRechargeDAL, IStudentDAL studentDAL, IEventPublisher eventPublisher,
            IStudentAccountRechargeCoreBLL studentAccountRechargeCoreBLL, IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL)
        {
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentDAL = studentDAL;
            this._eventPublisher = eventPublisher;
            this._studentAccountRechargeCoreBLL = studentAccountRechargeCoreBLL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeCoreBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentAccountRechargeDAL, _studentDAL, _studentAccountRechargeLogDAL);
        }

        public async Task SyncStudentAccountRechargeRelationStudentIdsConsumerEvent(SyncStudentAccountRechargeRelationStudentIdsEvent request)
        {
            var myStudentAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRechargeStudentIds(request.StudentAccountRechargeId);
            var strIds = string.Empty;
            if (myStudentAccountRecharge.Any())
            {
                var ids = myStudentAccountRecharge.Select(p => p.StudentId);
                strIds = EtmsHelper.GetMuIds(ids);
            }
            await _studentAccountRechargeDAL.UpdatetStudentAccountRechargeStudentIds(request.StudentAccountRechargeId, strIds);
        }

        public async Task StudentAutoAddAccountRechargeConsumerEvent(StudentAutoAddAccountRechargeEvent request)
        {
            if (request.AddMoney <= 0)
            {
                return;
            }
            var myStudentBucket = await _studentDAL.GetStudent(request.StudentId);
            if (myStudentBucket == null || myStudentBucket.Student == null)
            {
                return;
            }
            var myStudent = myStudentBucket.Student;
            EtStudentAccountRecharge myStudentAccountRecharge = null;
            //判断是否存在储值账户
            var studentAccountRechargeBinder = await _studentAccountRechargeDAL.GetAccountRechargeBinderByStudentId(request.StudentId);
            if (studentAccountRechargeBinder != null)
            {
                myStudentAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRecharge(studentAccountRechargeBinder.StudentAccountRechargeId);
            }

            //判断手机号是否存在储值账户
            if (myStudentAccountRecharge == null)
            {
                var phoneAccountRechargeBucket = await _studentAccountRechargeDAL.GetStudentAccountRecharge(myStudent.Phone);
                if (phoneAccountRechargeBucket != null && phoneAccountRechargeBucket.StudentAccountRecharge != null)
                {
                    myStudentAccountRecharge = phoneAccountRechargeBucket.StudentAccountRecharge;
                    //新增关联学员
                    await _studentAccountRechargeDAL.StudentAccountRechargeBinderAdd(myStudent.Phone, new EtStudentAccountRechargeBinder()
                    {
                        IsDeleted = EmIsDeleted.Normal,
                        StudentAccountRechargeId = myStudentAccountRecharge.Id,
                        StudentId = request.StudentId,
                        TenantId = request.TenantId
                    });

                    _eventPublisher.Publish(new SyncStudentAccountRechargeRelationStudentIdsEvent(request.TenantId)
                    {
                        StudentAccountRechargeId = myStudentAccountRecharge.Id
                    });
                }
            }

            //创建储值账户
            if (myStudentAccountRecharge == null)
            {
                myStudentAccountRecharge = new EtStudentAccountRecharge()
                {
                    BalanceSum = 0,
                    Phone = myStudent.Phone,
                    BalanceGive = 0,
                    BalanceReal = 0,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = DateTime.Now,
                    RechargeGiveSum = 0,
                    RechargeSum = 0,
                    TenantId = request.TenantId
                };
                await _studentAccountRechargeDAL.AddStudentAccountRecharge(myStudentAccountRecharge);
                //新增关联学员
                await _studentAccountRechargeDAL.StudentAccountRechargeBinderAdd(myStudent.Phone, new EtStudentAccountRechargeBinder()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    StudentAccountRechargeId = myStudentAccountRecharge.Id,
                    StudentId = request.StudentId,
                    TenantId = request.TenantId
                });

                _eventPublisher.Publish(new SyncStudentAccountRechargeRelationStudentIdsEvent(request.TenantId)
                {
                    StudentAccountRechargeId = myStudentAccountRecharge.Id
                });
            }

            //账户充值
            var now = DateTime.Now;
            await _studentAccountRechargeCoreBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(request.TenantId)
            {
                AddBalanceReal = request.AddMoney,
                AddBalanceGive = 0,
                AddRechargeSum = request.AddMoney,
                AddRechargeGiveSum = 0,
                StudentAccountRechargeId = myStudentAccountRecharge.Id,
                TryCount = 0
            });


            await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
            {
                UserId = request.UserId,
                StudentAccountRechargeId = myStudentAccountRecharge.Id,
                Phone = myStudentAccountRecharge.Phone,
                CgBalanceGive = 0,
                CgBalanceReal = request.AddMoney,
                CgNo = request.OrderNo,
                CgServiceCharge = 0,
                CommissionUser = null,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                RelatedOrderId = request.OrderId,
                Remark = request.Remark,
                Status = EmStudentAccountRechargeLogStatus.Normal,
                TenantId = request.TenantId,
                Type = request.RechargeLogType
            });
        }
    }
}
