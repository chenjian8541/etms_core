using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IDataAccess.ElectronicAlbum;
using ETMS.IEventProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvInteractionBLL : IEvInteractionBLL
    {
        private readonly IElectronicAlbumDAL _electronicAlbumDAL;

        private readonly IElectronicAlbumDetailDAL _electronicAlbumDetailDAL;

        private readonly IClassDAL _classDAL;

        private readonly IElectronicAlbumStatisticsDAL _electronicAlbumStatisticsDAL;

        private readonly IEventPublisher _eventPublisher;

        public EvInteractionBLL(IElectronicAlbumDAL electronicAlbumDAL, IElectronicAlbumDetailDAL electronicAlbumDetailDAL,
            IClassDAL classDAL, IElectronicAlbumStatisticsDAL electronicAlbumStatisticsDAL, IEventPublisher eventPublisher)
        {
            this._electronicAlbumDAL = electronicAlbumDAL;
            this._electronicAlbumDetailDAL = electronicAlbumDetailDAL;
            this._classDAL = classDAL;
            this._electronicAlbumStatisticsDAL = electronicAlbumStatisticsDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _electronicAlbumDAL, _electronicAlbumDetailDAL, _classDAL,
                _electronicAlbumStatisticsDAL);
        }

        public async Task ElectronicAlbumInitConsumerEvent(ElectronicAlbumInitEvent request)
        {
            var p = request.MyElectronicAlbum;
            if (p.Type == EmElectronicAlbumMyType.Student)
            {
                await _electronicAlbumDetailDAL.AddElectronicAlbumDetail(new EtElectronicAlbumDetail()
                {
                    CoverKey = p.CoverKey,
                    ElectronicAlbumId = p.Id,
                    IsDeleted = EmIsDeleted.Normal,
                    Name = p.Name,
                    ReadCount = 0,
                    ShareCount = 0,
                    Status = p.Status,
                    StudentId = p.RelatedId,
                    TenantId = p.TenantId,
                    UserId = p.UserId
                });
            }
            else
            {
                var myClassBucket = await _classDAL.GetClassBucket(p.RelatedId);
                if (myClassBucket == null || myClassBucket.EtClass == null)
                {
                    LOG.Log.Error("[ElectronicAlbumInitConsumerEvent]班级不存在", request, this.GetType());
                    return;
                }
                if (myClassBucket.EtClassStudents != null && myClassBucket.EtClassStudents.Count > 0)
                {
                    var entitys = new List<EtElectronicAlbumDetail>();
                    foreach (var myItem in myClassBucket.EtClassStudents)
                    {
                        entitys.Add(new EtElectronicAlbumDetail()
                        {
                            CoverKey = p.CoverKey,
                            ElectronicAlbumId = p.Id,
                            IsDeleted = EmIsDeleted.Normal,
                            Name = p.Name,
                            ReadCount = 0,
                            ShareCount = 0,
                            Status = p.Status,
                            StudentId = myItem.StudentId,
                            TenantId = p.TenantId,
                            UserId = p.UserId,
                            RenderKey = p.RenderKey
                        });
                    }
                    _electronicAlbumDetailDAL.AddElectronicAlbumDetail(entitys);
                }
            }
            if (p.Status == EmElectronicAlbumStatus.Push)
            {
                _eventPublisher.Publish(new NoticeStudentAlbumEvent(request.TenantId)
                {
                    AlbumId = p.Id,
                    Name = p.Name,
                    Type = p.Type,
                    RelatedId = p.RelatedId,
                    Time = p.CreateTime
                });
            }
        }

        public async Task ElectronicAlbumStatisticsConsumerEvent(ElectronicAlbumStatisticsEvent request)
        {
            var p = await _electronicAlbumDetailDAL.GetElectronicAlbumDetail(request.ElectronicAlbumDetailId);
            if (p == null)
            {
                return;
            }
            if (request.OpType == ElectronicAlbumStatisticsOpType.Read)
            {
                await _electronicAlbumDetailDAL.AddReadCount(request.ElectronicAlbumDetailId, 1);
                await _electronicAlbumDAL.AddReadCount(p.ElectronicAlbumId, 1);
                await _electronicAlbumStatisticsDAL.AddReadCount(p.ElectronicAlbumId, request.Ot);
            }
            else
            {
                await _electronicAlbumDetailDAL.AddShareCount(request.ElectronicAlbumDetailId, 1);
                await _electronicAlbumDAL.AddShareCount(p.ElectronicAlbumId, 1);
                await _electronicAlbumStatisticsDAL.AddShareCount(p.ElectronicAlbumId, request.Ot);
            }
        }
    }
}
