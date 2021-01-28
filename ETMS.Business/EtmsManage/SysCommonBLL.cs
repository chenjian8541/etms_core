using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Dto.SysCommon.Output;
using ETMS.Entity.EtmsManage.Dto.SysCommon.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysCommonBLL : ISysCommonBLL
    {
        private readonly ISysAppsettingsDAL _sysAppsettingsDAL;

        private readonly ISysAppsettingsBLL _sysAppsettingsBLL;

        private readonly ISysAgentLogDAL _sysAgentLogDAL;

        public SysCommonBLL(ISysAppsettingsDAL sysAppsettingsDAL, ISysAppsettingsBLL sysAppsettingsBLL, ISysAgentLogDAL sysAgentLogDAL)
        {
            this._sysAppsettingsDAL = sysAppsettingsDAL;
            this._sysAppsettingsBLL = sysAppsettingsBLL;
            this._sysAgentLogDAL = sysAgentLogDAL;
        }

        public async Task<ResponseBase> EtmsGlobalConfigGet(EtmsGlobalConfigGetRequest request)
        {
            var log = await _sysAppsettingsBLL.GetEtmsGlobalConfig();
            return ResponseBase.Success(new EtmsGlobalConfigGetOutput()
            {
                KefuPhone = log.KefuPhone,
                KefuQQ = log.KefuQQ,
                Kefu53 = log.Kefu53,
                Phone404 = log.Phone404
            });
        }

        public async Task<ResponseBase> EtmsGlobalConfigSave(EtmsGlobalConfigSaveRequest request)
        {
            var log = await _sysAppsettingsBLL.GetEtmsGlobalConfig();
            log.KefuPhone = request.KefuPhone;
            log.KefuQQ = request.KefuQQ;
            log.Phone404 = request.Phone404;
            log.Kefu53 = request.Kefu53;
            var data = JsonConvert.SerializeObject(log);
            await _sysAppsettingsDAL.SaveSysAppsettings(data, EmSysAppsettingsType.EtmsGlobalConfig);

            await _sysAgentLogDAL.AddSysAgentOpLog(new SysAgentOpLog()
            {
                AgentId = request.LoginAgentId,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = "修改主系统配置信息",
                Ot = DateTime.Now,
                Remark = string.Empty,
                Type = EmSysAgentOpLogType.SysExplainMgr
            });
            return ResponseBase.Success();
        }
    }
}
