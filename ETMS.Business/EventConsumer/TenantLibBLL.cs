using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class TenantLibBLL : ITenantLibBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly ICourseDAL _courseDAL;

        private readonly IClassDAL _classDAL;

        private readonly INoticeConfigDAL _noticeConfigDAL;

        private readonly IComDAL _comDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysTenantCloudStorageDAL _sysTenantCloudStorageDAL;

        public TenantLibBLL(IStudentDAL studentDAL, ICourseDAL courseDAL, IClassDAL classDAL, INoticeConfigDAL noticeConfigDAL,
            IComDAL comDAL, IUserOperationLogDAL userOperationLogDAL, ISysTenantDAL sysTenantDAL,
            ISysTenantCloudStorageDAL sysTenantCloudStorageDAL)
        {
            this._studentDAL = studentDAL;
            this._courseDAL = courseDAL;
            this._classDAL = classDAL;
            this._noticeConfigDAL = noticeConfigDAL;
            this._comDAL = comDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._sysTenantCloudStorageDAL = sysTenantCloudStorageDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentDAL, _courseDAL, _classDAL, _noticeConfigDAL, _comDAL,
                _userOperationLogDAL);
        }

        public async Task<EtNoticeConfig> NoticeConfigGet(int type, byte peopleType, int scenesType)
        {
            return await _noticeConfigDAL.GetNoticeConfig(type, peopleType, scenesType);
        }

        public async Task<IEnumerable<EtClass>> GetStudentInClass(long studentId)
        {
            return await _classDAL.GetStudentClass(studentId);
        }

        public async Task ComSqlHandleConsumerEvent(ComSqlHandleEvent request)
        {
            await _comDAL.ExecuteSql(request.Sql);
        }

        public async Task SyncTenantLastOpTimeConsumerEvent(SyncTenantLastOpTimeEvent request)
        {
            var lastOpTime = await _userOperationLogDAL.GetLastOpTime(request.TenantId);
            if (lastOpTime != null)
            {
                await _sysTenantDAL.UpdateTenantLastOpTime(request.TenantId, lastOpTime.Value);
            }
        }

        public async Task CloudStorageAnalyzeConsumerEvent(CloudStorageAnalyzeEvent request)
        {
            var now = DateTime.Now;
            var unitCvtGb = 1024;
            var tenantId = request.TenantId;
            var tenantCloudStorageList = new List<SysTenantCloudStorage>();
            var aliyunOssCall = new AliyunOssCall();
            var lastOldPrefix = $"{SystemConfig.ComConfig.OSSRootFolderProd}/{tenantId}/";
            var lastOldSizeMb = aliyunOssCall.Process(lastOldPrefix);
            var lastOldSizeGb = lastOldSizeMb / unitCvtGb;
            tenantCloudStorageList.Add(new SysTenantCloudStorage()
            {
                IsDeleted = EmIsDeleted.Normal,
                AgentId = request.AgentId,
                LastModified = now,
                Remark = null,
                TenantId = tenantId,
                Type = 0,
                ValueMB = lastOldSizeMb,
                ValueGB = lastOldSizeGb
            });
            var totalMb = lastOldSizeMb;
            var totalGb = lastOldSizeGb;
            foreach (var itemTag in EmTenantCloudStorageType.TenantCloudStorageTypeTags)
            {
                var itemPrefix = $"{SystemConfig.ComConfig.OSSRootNewFolder}/{itemTag.Tag}/{SystemConfig.ComConfig.OSSRootFolderProd}/{tenantId}/";
                var itemSizeMb = aliyunOssCall.Process(itemPrefix);
                var itemSizeGb = itemSizeMb / unitCvtGb;
                tenantCloudStorageList.Add(new SysTenantCloudStorage()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    AgentId = request.AgentId,
                    LastModified = now,
                    Remark = null,
                    TenantId = tenantId,
                    Type = itemTag.Type,
                    ValueMB = totalMb,
                    ValueGB = itemSizeGb
                });
                totalMb += itemSizeMb;
                totalGb += itemSizeGb;
            }
            await _sysTenantDAL.UpdateTenantCloudStorage(tenantId, totalMb, totalGb);
            await _sysTenantCloudStorageDAL.SaveCloudStorage(tenantId, tenantCloudStorageList);
        }
    }
}
