using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.ElectronicAlbum;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.ElectronicAlbum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.ElectronicAlbum
{
    public class ElectronicAlbumStatisticsDAL : DataAccessBase, IElectronicAlbumStatisticsDAL
    {
        public ElectronicAlbumStatisticsDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task AddReadCount(long electronicAlbumId, DateTime dateTime)
        {
            var date = dateTime.Date;
            var log = await _dbWrapper.Find<EtElectronicAlbumReadLogDay>(
                p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.ElectronicAlbumId == electronicAlbumId && p.Ot == date);
            if (log != null)
            {
                log.ReadCount += 1;
                await _dbWrapper.Update(log);
            }
            else
            {
                await _dbWrapper.Insert(new EtElectronicAlbumReadLogDay()
                {
                    ElectronicAlbumId = electronicAlbumId,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = date,
                    ReadCount = 1,
                    TenantId = _tenantId
                });
            }
        }

        public async Task AddShareCount(long electronicAlbumId, DateTime dateTime)
        {
            var date = dateTime.Date;
            var log = await _dbWrapper.Find<EtElectronicAlbumShareLogDay>(
                p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.ElectronicAlbumId == electronicAlbumId && p.Ot == date);
            if (log != null)
            {
                log.ShareCount += 1;
                await _dbWrapper.Update(log);
            }
            else
            {
                await _dbWrapper.Insert(new EtElectronicAlbumShareLogDay()
                {
                    ElectronicAlbumId = electronicAlbumId,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = date,
                    ShareCount = 1,
                    TenantId = _tenantId
                });
            }
        }

        public async Task<List<EtElectronicAlbumReadLogDay>> GetReadLog(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtElectronicAlbumReadLogDay>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }

        public async Task<List<EtElectronicAlbumShareLogDay>> GetShareLog(DateTime startTime, DateTime endTime)
        {
            return await _dbWrapper.FindList<EtElectronicAlbumShareLogDay>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Ot >= startTime && p.Ot <= endTime);
        }
    }
}
