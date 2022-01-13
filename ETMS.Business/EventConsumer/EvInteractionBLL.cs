using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IDataAccess.ElectronicAlbum;
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

        public EvInteractionBLL(IElectronicAlbumDAL electronicAlbumDAL, IElectronicAlbumDetailDAL electronicAlbumDetailDAL,
            IClassDAL classDAL)
        {
            this._electronicAlbumDAL = electronicAlbumDAL;
            this._electronicAlbumDetailDAL = electronicAlbumDetailDAL;
            this._classDAL = classDAL;
        }
        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _electronicAlbumDAL, _electronicAlbumDetailDAL, _classDAL);
        }

        public async Task ElectronicAlbumInitConsumerEvent(ElectronicAlbumInitEvent request)
        {
            var p = request.MyElectronicAlbum;
            if (p.Type == EmElectronicAlbumType.Student)
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
        }
    }
}
