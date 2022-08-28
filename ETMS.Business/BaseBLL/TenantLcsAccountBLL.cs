﻿using ETMS.Business.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Temp;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.BaseBLL
{
    public abstract class TenantLcsAccountBLL
    {
        protected readonly ITenantLcsAccountDAL _tenantLcsAccountDAL;

        protected readonly ISysTenantDAL _sysTenantDAL;

        protected readonly ITenantFubeiAccountDAL _tenantFubeiAccountDAL;

        public TenantLcsAccountBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, ISysTenantDAL sysTenantDAL,
            ITenantFubeiAccountDAL tenantFubeiAccountDAL)
        {
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._tenantFubeiAccountDAL = tenantFubeiAccountDAL;
        }

        protected async Task<CheckTenantLcsAccountView> CheckTenantAgtPayAccount(int tenantId)
        {
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            var myTenantAgtPayInfo = ComBusiness4.GetTenantAgtPayInfo(myTenant);
            var isOpenAgtPay = myTenantAgtPayInfo.IsOpenAgtPay;
            var agtPayType = myTenantAgtPayInfo.AgtPayType;
            if (agtPayType != EmAgtPayType.Suixing && !isOpenAgtPay)
            {
                return new CheckTenantLcsAccountView("机构未开通聚合支付");
            }
            SysTenantLcsAccount myLcsAccount = null;
            SysTenantFubeiAccount myFubeiAccount = null;
            TenantAgtPayAccountInfo myAgtPayAccountInfo = null;
            switch (agtPayType)
            {
                case EmAgtPayType.Lcsw:
                    myLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(myTenant.Id);
                    if (myLcsAccount == null)
                    {
                        LOG.Log.Error($"[CheckTenantLcsAccount]扫呗账户异常tenantId:{tenantId}", this.GetType());
                        return new CheckTenantLcsAccountView("扫呗账户异常，无法支付");
                    }
                    myAgtPayAccountInfo = new TenantAgtPayAccountInfo()
                    {
                        MerchantName = myLcsAccount.MerchantName,
                        MerchantNo = myLcsAccount.MerchantNo,
                        MerchantType = myLcsAccount.MerchantType,
                        TerminalId = myLcsAccount.TerminalId
                    };
                    break;
                case EmAgtPayType.Fubei:
                    myFubeiAccount = await _tenantFubeiAccountDAL.GetTenantFubeiAccount(myTenant.Id);
                    if (myFubeiAccount == null)
                    {
                        LOG.Log.Error($"[CheckTenantLcsAccount]付呗账户异常tenantId:{tenantId}", this.GetType());
                        return new CheckTenantLcsAccountView("付呗账户异常，无法支付");
                    }
                    myAgtPayAccountInfo = new TenantAgtPayAccountInfo()
                    {
                        MerchantName = myFubeiAccount.MerchantName,
                        MerchantType = 0,
                        MerchantNo = myFubeiAccount.MerchantCode,
                        TerminalId = myFubeiAccount.MerchantId.ToString()
                    };
                    break;
            }
            return new CheckTenantLcsAccountView()
            {
                MyTenant = myTenant,
                MyAgtPayAccountInfo = myAgtPayAccountInfo,
                MyLcsAccount = myLcsAccount,
                MyFubeiAccount = myFubeiAccount,
                ErrMsg = null
            };
        }
    }
}
