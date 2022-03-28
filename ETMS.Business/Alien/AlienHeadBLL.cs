using ETMS.Entity.Alien.Common;
using ETMS.Entity.Alien.Dto.Head.Output;
using ETMS.Entity.Common;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.IBusiness.Alien;
using ETMS.IDataAccess.Alien;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public class AlienHeadBLL : IAlienHeadBLL
    {
        private readonly IMgHeadDAL _mgHeadDAL;

        private readonly IMgUserOpLogDAL _mgUserOpLogDAL;

        private readonly IMgTenantsDAL _mgTenantsDAL;

        private readonly ISysTenantDAL _sysTenantDAL;
        public AlienHeadBLL(IMgHeadDAL mgHeadDAL, IMgUserOpLogDAL mgUserOpLogDAL, IMgTenantsDAL mgTenantsDAL,
            ISysTenantDAL sysTenantDAL)
        {
            this._mgHeadDAL = mgHeadDAL;
            this._mgUserOpLogDAL = mgUserOpLogDAL;
            this._mgTenantsDAL = mgTenantsDAL;
            this._sysTenantDAL = sysTenantDAL;
        }

        public void InitHeadId(int headId)
        {
            this.InitDataAccess(headId, _mgUserOpLogDAL, _mgTenantsDAL);
        }

        public async Task<ResponseBase> HeadAllGet(AlienRequestBase request)
        {
            var output = new List<HeadAllGetOut>();
            var allTenants = await _mgTenantsDAL.GetMgTenants();
            if (allTenants != null && allTenants.Any())
            {
                foreach (var item in allTenants)
                {
                    var p = await _sysTenantDAL.GetTenant(item.TenantId);
                    if (p == null)
                    {
                        continue;
                    }
                    output.Add(new HeadAllGetOut()
                    {
                        CId = p.Id,
                        Name = p.Name,
                        Ot = p.Ot,
                        Phone = p.Phone,
                        SmsCount = p.SmsCount,
                        Status = EmSysTenantStatus.GetSysTenantStatus(p.Status, p.ExDate),
                        StatusDesc = EmSysTenantStatus.GetSysTenantStatusDesc(p.Status, p.ExDate),
                        TenantCode = p.TenantCode,
                        Address = p.Address,
                        IdCard = p.IdCard,
                        LinkMan = p.LinkMan,
                        Value = p.Id,
                        Label = p.Name,
                        ExDate = p.ExDate,
                        SmsSignature = p.SmsSignature
                    });
                }
            }
            return ResponseBase.Success(output);
        }
    }
}
