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

        private readonly IAiTenantFaceAccess _aiTenantFaceAccess;

        private readonly IEventPublisher _eventPublisher;

        private readonly IAiBaiduFaceAccess _aiBaiduFaceAccess;

        private readonly ISysAIFaceBiduAccountDAL _sysAIFaceBiduAccountDAL;

        private readonly ISysAITenantAccountDAL _sysAITenantAccountDAL;

        /// <summary>
        /// 云AI类型 <see cref="EmSysTenantAICloudType"/>
        /// </summary>
        public int AICloudType { get; set; }

        public Aiface(ISysTenantDAL sysTenantDAL, IAiTenantFaceAccess aiFaceAccess, IEventPublisher eventPublisher,
            IAiBaiduFaceAccess aiBaiduFaceAccess, ISysAIFaceBiduAccountDAL sysAIFaceBiduAccountDAL, ISysAITenantAccountDAL sysAITenantAccountDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._aiTenantFaceAccess = aiFaceAccess;
            this._eventPublisher = eventPublisher;
            this._aiBaiduFaceAccess = aiBaiduFaceAccess;
            this._sysAIFaceBiduAccountDAL = sysAIFaceBiduAccountDAL;
            this._sysAITenantAccountDAL = sysAITenantAccountDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantId = tenantId;
        }

        private async Task InitTenantCloudConfig()
        {
            if (_isInitTenantTencentCloudConfig)
            {
                return;
            }
            var myTenant = await _sysTenantDAL.GetTenant(_tenantId);
            this.AICloudType = myTenant.AICloudType;
            if (this.AICloudType == EmSysTenantAICloudType.TencentCloud)
            {
                var tencentCloudAccount = await _sysAITenantAccountDAL.GetSysAITenantAccount(myTenant.TencentCloudId);
                _aiTenantFaceAccess.InitTencentCloudConfig(_tenantId, tencentCloudAccount.SecretId, tencentCloudAccount.SecretKey,
                    tencentCloudAccount.Endpoint, tencentCloudAccount.Region);
            }
            else
            {
                var baiduCloudAccount = await _sysAIFaceBiduAccountDAL.GetSysAIFaceBiduAccount(myTenant.BaiduCloudId);
                _aiBaiduFaceAccess.InitBaiduCloudConfig(_tenantId, baiduCloudAccount.Appid, baiduCloudAccount.ApiKey,
                    baiduCloudAccount.SecretKey);
            }
        }

        public async Task StudentDelete(long studentId)
        {
            try
            {
                await InitTenantCloudConfig();
                if (this.AICloudType == EmSysTenantAICloudType.TencentCloud)
                {
                    _aiTenantFaceAccess.StudentDel(studentId);
                }
                else
                {
                    _aiBaiduFaceAccess.StudentDel(studentId);
                }
            }
            catch (Exception ex)
            {
                LOG.Log.Error($"[Aiface]删除学员出错，studentId:{studentId}", ex, this.GetType());
            }
        }

        public async Task<Tuple<bool, string>> StudentInitFace(long studentId, string imageBase64)
        {
            await InitTenantCloudConfig();
            Tuple<bool, string> result;
            if (this.AICloudType == EmSysTenantAICloudType.TencentCloud)
            {
                result = _aiTenantFaceAccess.StudentInitFace(studentId, imageBase64);
            }
            else
            {
                result = _aiBaiduFaceAccess.StudentInitFace(studentId, imageBase64);
            }
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
            await InitTenantCloudConfig();
            if (this.AICloudType == EmSysTenantAICloudType.TencentCloud)
            {
                _aiTenantFaceAccess.StudentClearFace(studentId);
            }
            else
            {
                _aiBaiduFaceAccess.StudentClearFace(studentId);
            }
            return true;
        }

        public async Task<Tuple<long, string>> SearchPerson(string imageBase64)
        {
            await InitTenantCloudConfig();
            Tuple<long, string> result;
            if (this.AICloudType == EmSysTenantAICloudType.TencentCloud)
            {
                result = _aiTenantFaceAccess.SearchPerson(imageBase64);
            }
            else
            {
                result = _aiBaiduFaceAccess.SearchPerson(imageBase64);
            }
            if (result.Item1 > 0)
            {
                _eventPublisher.Publish(new TenantTxCloudUCountEvent(_tenantId)
                {
                    Type = EmSysTenantTxCloudUCountType.SearchPerson,
                    AddUseCount = 1,
                    StudentId = result.Item1
                });
            }
            return result;
        }

        public void Gr0oupDelete(int tenantId)
        {
            if (this.AICloudType == EmSysTenantAICloudType.BaiduCloud)
            {
                _aiBaiduFaceAccess.Gr0oupDelete(tenantId);
            }
        }
    }
}
