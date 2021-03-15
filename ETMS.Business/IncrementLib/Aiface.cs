using ETMS.Entity.Enum.EtmsManage;
using ETMS.Event.DataContract;
using ETMS.IAiFace;
using ETMS.IBusiness.EtmsManage;
using ETMS.IBusiness.IncrementLib;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.IncrementLib
{
    public class Aiface : IAiface
    {
        private bool _isInitTenantTencentCloudConfig = false;

        private int _tenantId;

        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ISysAppsettingsBLL _sysAppsettingsBLL;

        private readonly IAiFaceAccess _aiFaceAccess;

        private readonly IEventPublisher _eventPublisher;

        public Aiface(ISysTenantDAL sysTenantDAL, ISysAppsettingsBLL sysAppsettingsBLL, IAiFaceAccess aiFaceAccess, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._sysAppsettingsBLL = sysAppsettingsBLL;
            this._aiFaceAccess = aiFaceAccess;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantId = tenantId;
        }

        private async Task InitTenantTencentCloudConfig()
        {
            if (_isInitTenantTencentCloudConfig)
            {
                return;
            }
            var myTenant = await _sysTenantDAL.GetTenant(_tenantId);
            var tencentCloudAccount = await _sysAppsettingsBLL.GetTencentCloudAccount(myTenant.TencentCloudId);
            _aiFaceAccess.InitTencentCloudConfig(_tenantId, tencentCloudAccount.SecretId, tencentCloudAccount.SecretKey,
                tencentCloudAccount.Endpoint, tencentCloudAccount.Region);
        }

        public async Task StudentDelete(long studentId)
        {
            try
            {
                await InitTenantTencentCloudConfig();
                _aiFaceAccess.StudentDel(studentId);
            }
            catch (Exception ex)
            {
                LOG.Log.Error($"[Aiface]删除学员出错，studentId:{studentId}", ex, this.GetType());
            }
        }

        public async Task<Tuple<bool, string>> StudentInitFace(long studentId, string faceGreyKeyUrl)
        {
            await InitTenantTencentCloudConfig();
            var result = _aiFaceAccess.StudentInitFace(studentId, faceGreyKeyUrl);
            if (result.Item1)
            {
                _eventPublisher.Publish(new TenantTxCloudUCountEvent(_tenantId)
                {
                    Type = EmSysTenantTxCloudUCountType.PersonMgr,
                    AddUseCount = 1,
                    StudentId = studentId
                });
            }
            return result;
        }

        public async Task<bool> StudentClearFace(long studentId)
        {
            await InitTenantTencentCloudConfig();
            _aiFaceAccess.StudentClearFace(studentId);
            return true;
        }

        public async Task<long> SearchPerson(string imageBase64)
        {
            await InitTenantTencentCloudConfig();
            var studentId = _aiFaceAccess.SearchPerson(imageBase64);
            if (studentId > 0)
            {
                _eventPublisher.Publish(new TenantTxCloudUCountEvent(_tenantId)
                {
                    Type = EmSysTenantTxCloudUCountType.SearchPerson,
                    AddUseCount = 1,
                    StudentId = studentId
                });
            }
            return studentId;
        }
    }
}
