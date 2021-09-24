using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage.Lcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class BascDataInfoBLL : IBascDataInfoBLL
    {
        private readonly ISysLcswAreaProvinceDAL _sysLcswAreaProvinceDAL;

        private readonly ISysLcswAreaCityDAL _sysLcswAreaCityDAL;

        private readonly ISysLcswAreaRegionDAL _sysLcswAreaRegionDAL;

        private readonly ISysLcsBankMCC3DAL _sysLcsBankMCC3DAL;

        private readonly ISysLcsBankMCC2DAL _sysLcsBankMCC2DAL;

        private readonly ISysLcsBankMCC1DAL _sysLcsBankMCC1DAL;

        private readonly ISysLcsBankDAL _sysLcsBankDAL;

        public BascDataInfoBLL(ISysLcswAreaRegionDAL sysLcswAreaRegionDAL, ISysLcswAreaProvinceDAL sysLcswAreaProvinceDAL,
            ISysLcswAreaCityDAL sysLcswAreaCityDAL, ISysLcsBankMCC3DAL sysLcsBankMCC3DAL, ISysLcsBankMCC2DAL sysLcsBankMCC2DAL,
            ISysLcsBankMCC1DAL sysLcsBankMCC1DAL, ISysLcsBankDAL sysLcsBankDAL)
        {
            this._sysLcswAreaRegionDAL = sysLcswAreaRegionDAL;
            this._sysLcswAreaProvinceDAL = sysLcswAreaProvinceDAL;
            this._sysLcswAreaCityDAL = sysLcswAreaCityDAL;
            this._sysLcsBankMCC3DAL = sysLcsBankMCC3DAL;
            this._sysLcsBankMCC2DAL = sysLcsBankMCC2DAL;
            this._sysLcsBankMCC1DAL = sysLcsBankMCC1DAL;
            this._sysLcsBankDAL = sysLcsBankDAL;
        }

        public async Task<ResponseBase> GetRegions(GetRegionsRequrst requrst)
        {
            switch (requrst.Type)
            {
                case 0:
                    return ResponseBase.Success(await _sysLcswAreaProvinceDAL.GetAllProvince());
                case 1:
                    return ResponseBase.Success(await _sysLcswAreaCityDAL.GetCity(requrst.Code));
                case 2:
                    return ResponseBase.Success(await _sysLcswAreaRegionDAL.GetRegion(requrst.Code));
            }
            return ResponseBase.CommonError("请求数据格式错误");
        }

        public async Task<ResponseBase> GetBanks(GetBanksRequrst requrst)
        {
            if (requrst.Type == 0)
            {
                return ResponseBase.Success(await _sysLcsBankDAL.GetAllBankHead());
            }
            return ResponseBase.Success(await _sysLcsBankDAL.GetBank(requrst.BankCode, requrst.CityCode));
        }

        public async Task<ResponseBase> GetIndustry(GetIndustryRequrst requrst)
        {
            switch (requrst.Type)
            {
                case 0:
                    return ResponseBase.Success(await _sysLcsBankMCC1DAL.GetAllLcsBankMCC1());
                case 1:
                    return ResponseBase.Success(await _sysLcsBankMCC2DAL.GetLcsBankMCC2(requrst.Code));
                case 2:
                    return ResponseBase.Success(await _sysLcsBankMCC3DAL.GetLcsBankMCC3(requrst.Code));
            }
            return ResponseBase.CommonError("请求数据格式错误");
        }
    }
}
