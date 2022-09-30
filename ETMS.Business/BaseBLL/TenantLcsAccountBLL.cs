using ETMS.Business.Common;
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

        private readonly ISysTenantSuixingAccountDAL _sysTenantSuixingAccountDAL;

        private readonly ISysTenantSuixingAccount2DAL _sysTenantSuixingAccount2DAL;

        public TenantLcsAccountBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, ISysTenantDAL sysTenantDAL,
            ITenantFubeiAccountDAL tenantFubeiAccountDAL, ISysTenantSuixingAccountDAL sysTenantSuixingAccountDAL,
            ISysTenantSuixingAccount2DAL sysTenantSuixingAccount2DAL)
        {
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
            this._sysTenantDAL = sysTenantDAL;
            this._tenantFubeiAccountDAL = tenantFubeiAccountDAL;
            this._sysTenantSuixingAccountDAL = sysTenantSuixingAccountDAL;
            this._sysTenantSuixingAccount2DAL = sysTenantSuixingAccount2DAL;
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
            SysTenantSuixingAccount2 mySysTenantSuixingAccount = null;
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
                case EmAgtPayType.Suixing:
                    mySysTenantSuixingAccount = await _sysTenantSuixingAccount2DAL.GetTenantSuixingAccount(myTenant.Id);
                    if (mySysTenantSuixingAccount == null)
                    {
                        LOG.Log.Error($"[CheckTenantLcsAccount]随行付账户异常tenantId:{tenantId}", this.GetType());
                        return new CheckTenantLcsAccountView("随行付账户异常，无法支付");
                    }
                    myAgtPayAccountInfo = new TenantAgtPayAccountInfo()
                    {
                        MerchantName = mySysTenantSuixingAccount.MerName,
                        MerchantType = 0,
                        MerchantNo = mySysTenantSuixingAccount.Mno,
                        TerminalId = mySysTenantSuixingAccount.Mno
                    };
                    break;
            }
            return new CheckTenantLcsAccountView()
            {
                MyTenant = myTenant,
                MyAgtPayAccountInfo = myAgtPayAccountInfo,
                MyLcsAccount = myLcsAccount,
                MyFubeiAccount = myFubeiAccount,
                MySysTenantSuixingAccount = mySysTenantSuixingAccount,
                ErrMsg = null
            };
        }

        protected async Task<CheckTenantLcsAccountView> CheckTenantAgtPayAccountAboutRefund(int tenantId, int agtPayType,
            int orderType)
        {
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            var myTenantAgtPayInfo = ComBusiness4.GetTenantAgtPayInfo(myTenant);
            var isOpenAgtPay = myTenantAgtPayInfo.IsOpenAgtPay;
            SysTenantLcsAccount myLcsAccount = null;
            SysTenantFubeiAccount myFubeiAccount = null;
            BaseTenantSuixingAccount mySysTenantSuixingAccount = null;
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
                case EmAgtPayType.Suixing:
                    if (orderType == EmLcsPayLogOrderType.Activity)
                    {
                        mySysTenantSuixingAccount = await _sysTenantSuixingAccountDAL.GetTenantSuixingAccount(myTenant.Id);
                    }
                    else
                    {
                        mySysTenantSuixingAccount = await _sysTenantSuixingAccount2DAL.GetTenantSuixingAccount(myTenant.Id);
                    }
                    if (mySysTenantSuixingAccount == null)
                    {
                        LOG.Log.Error($"[CheckTenantLcsAccount]随行付账户异常tenantId:{tenantId}", this.GetType());
                        return new CheckTenantLcsAccountView("随行付账户异常，无法支付");
                    }
                    myAgtPayAccountInfo = new TenantAgtPayAccountInfo()
                    {
                        MerchantName = mySysTenantSuixingAccount.MerName,
                        MerchantType = 0,
                        MerchantNo = mySysTenantSuixingAccount.Mno,
                        TerminalId = mySysTenantSuixingAccount.Mno
                    };
                    break;
            }
            return new CheckTenantLcsAccountView()
            {
                MyTenant = myTenant,
                MyAgtPayAccountInfo = myAgtPayAccountInfo,
                MyLcsAccount = myLcsAccount,
                MyFubeiAccount = myFubeiAccount,
                MySysTenantSuixingAccount = mySysTenantSuixingAccount,
                ErrMsg = null
            };
        }
    }
}
