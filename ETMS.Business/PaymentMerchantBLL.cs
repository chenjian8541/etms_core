using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.CoreBusiness.Request;
using ETMS.Entity.Dto.PaymentService.Output;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Pay.Lcsw.Dto;
using ETMS.Entity.Pay.Lcsw.Dto.Request;
using ETMS.Entity.Pay.Lcsw.Dto.Request.Response;
using ETMS.Entity.Temp.View;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Pay.Lcsw;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class PaymentMerchantBLL : IPaymentMerchantBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly ITenantLcsAccountDAL _tenantLcsAccountDAL;

        private readonly IPayLcswService _payLcswService;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly ITenantFubeiAccountDAL _tenantFubeiAccountDAL;

        private readonly IAgtPayServiceBLL _agtPayServiceBLL;

        private readonly IAppConfigurtaionServices _appConfigurtaionServices;

        public PaymentMerchantBLL(ISysTenantDAL sysTenantDAL, ITenantLcsAccountDAL tenantLcsAccountDAL, IPayLcswService payLcswService,
            IUserOperationLogDAL userOperationLogDAL, ITenantFubeiAccountDAL tenantFubeiAccountDAL,
            IAgtPayServiceBLL agtPayServiceBLL, IAppConfigurtaionServices appConfigurtaionServices)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
            this._payLcswService = payLcswService;
            this._userOperationLogDAL = userOperationLogDAL;
            this._tenantFubeiAccountDAL = tenantFubeiAccountDAL;
            this._agtPayServiceBLL = agtPayServiceBLL;
            this._appConfigurtaionServices = appConfigurtaionServices;
        }

        public async Task<ResponseBase> TenantPaymentSetGet(RequestBase request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var output = new TenantPaymentSetGetOutput()
            {
                AgtPayType = myTenant.AgtPayType
            };
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> TenantFubeiAccountBind(TenantFubeiAccountBindRequest request)
        {
            this._agtPayServiceBLL.InitTenantId(request.LoginTenantId);
            this._userOperationLogDAL.InitTenantId(request.LoginTenantId);
            var fubeiConfig = this._appConfigurtaionServices.AppSettings.PayConfig.FubeiConfig;
            var wxConfigResult = await _agtPayServiceBLL.WxConfig(new WxConfigRequest()
            {
                AccountType = request.AccountType,
                AppId = request.AppId,
                AppSecret = request.AppSecret,
                CashierId = request.CashierId,
                JsapiPath = fubeiConfig.JsapiPath,
                MerchantId = request.MerchantId,
                StoreId = request.StoreId,
                WxSubAppid = request.WxSubAppid,
                VendorSn = fubeiConfig.VendorSn,
                VendorSecret = fubeiConfig.VendorSecret
            });
            if (!wxConfigResult.IsSuccess)
            {
                return ResponseBase.CommonError(wxConfigResult.ErrMsg);
            }
            var callbackConfigResult = await _agtPayServiceBLL.CallbackConfig(new CallbackConfigRequest()
            {
                AccountType = request.AccountType,
                AppId = request.AppId,
                AppSecret = request.AppSecret,
                MerchantId = request.MerchantId,
                VendorSn = fubeiConfig.VendorSn,
                VendorSecret = fubeiConfig.VendorSecret,
                refund_callback_url = SysWebApiAddressConfig.FubeiRefundApiNotify
            });
            if (!callbackConfigResult.IsSuccess)
            {
                return ResponseBase.CommonError(callbackConfigResult.ErrMsg);
            }

            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            var now = DateTime.Now;
            var account = await _tenantFubeiAccountDAL.GetTenantFubeiAccount(request.LoginTenantId);
            if (account == null)
            {
                //新增
                var entity = new SysTenantFubeiAccount()
                {
                    AccountType = request.AccountType,
                    AgentId = myTenant.AgentId,
                    ApplyStatus = EmTenantFubeiAccountApplyStatus.Passed,
                    AppId = request.AppId,
                    AppSecret = request.AppSecret,
                    CashierId = request.CashierId,
                    MerchantCode = request.MerchantCode,
                    MerchantId = request.MerchantId,
                    MerchantName = request.MerchantName,
                    StoreId = request.StoreId,
                    VendorSn = fubeiConfig.VendorSn,
                    VendorSecret = fubeiConfig.VendorSecret,
                    WxJsapiPath = fubeiConfig.JsapiPath,
                    WxSubAppid = request.WxSubAppid,
                    TenantId = request.LoginTenantId,
                    Remark = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    ChangeTime = now,
                    CreationTime = now
                };
                await _tenantFubeiAccountDAL.AddTenantFubeiAccount(entity);
            }
            else
            {
                //编辑
                account.AppId = request.AppId;
                account.AppSecret = request.AppSecret;
                account.CashierId = request.CashierId;
                account.MerchantCode = request.MerchantCode;
                account.MerchantId = request.MerchantId;
                account.MerchantName = request.MerchantName;
                account.StoreId = request.StoreId;
                account.VendorSn = fubeiConfig.VendorSn;
                account.VendorSecret = fubeiConfig.VendorSecret;
                account.WxJsapiPath = fubeiConfig.JsapiPath;
                account.WxSubAppid = request.WxSubAppid;
                await _tenantFubeiAccountDAL.EditTenantFubeiAccount(account);
            }
            myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId); //获取最新的机构信息
            myTenant.AgtPayType = EmAgtPayType.Fubei;
            myTenant.LcswOpenStatus = EmBool.True;
            await _sysTenantDAL.EditTenant(myTenant);

            await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
            {
                ClientType = EmUserOperationLogClientType.PC,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = "绑定付呗支付账户",
                Ot = now,
                Remark = string.Empty,
                TenantId = myTenant.Id,
                Type = (int)EmUserOperationType.LcsMgr,
                UserId = request.LoginUserId
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> TenantFubeiAccountGet(RequestBase request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            if (myTenant.AgtPayType == EmAgtPayType.Lcsw)
            {
                return ResponseBase.CommonError("机构已申请了扫呗支付");
            }
            if (myTenant.AgtPayType == EmAgtPayType.NotApplied)
            {
                return ResponseBase.Success(new TenantFubeiAccountGetOutput()
                {
                    AgtPayType = myTenant.AgtPayType
                });
            }
            var account = await _tenantFubeiAccountDAL.GetTenantFubeiAccount(request.LoginTenantId);
            if (account == null)
            {
                return ResponseBase.CommonError("未找到机构绑定的付呗账户");
            }
            return ResponseBase.Success(new TenantFubeiAccountGetOutput()
            {
                AgtPayType = myTenant.AgtPayType,
                AccountInfo = new TenantFubeiAccountInfo()
                {
                    AccountType = account.AccountType,
                    AppId = account.AppId,
                    ApplyStatus = account.ApplyStatus,
                    AppSecret = account.AppSecret,
                    CashierId = account.CashierId,
                    MerchantCode = account.MerchantCode,
                    MerchantId = account.MerchantId,
                    MerchantName = account.MerchantName,
                    StoreId = account.StoreId,
                    VendorSn = account.VendorSn,
                    WxSubAppid = account.WxSubAppid
                }
            });
        }

        public ResponseBase MerchantCheckName(MerchantCheckNameRequest request)
        {
            var res = _payLcswService.CheckName(request.MerchantName);
            var output = new MerchantCheckNameOutput()
            {
                IsRepeat = true
            };
            if (res.IsSuccess())
            {
                output.IsRepeat = false;
            }
            return ResponseBase.Success(output);
        }

        private static LcswStatusView GetLcswStatus(string merchant_status)
        {
            var lcswApplyStatus = merchant_status.ToInt();
            var lcswOpenStatus = EmBool.False;
            if (MerchantStatus.IsPass(merchant_status))
            {
                lcswOpenStatus = EmBool.True;
            }
            return new LcswStatusView()
            {
                LcswApplyStatus = lcswApplyStatus,
                LcswOpenStatus = lcswOpenStatus
            };
        }

        public async Task<ResponseBase> MerchantSave(MerchantAddRequest request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            var userId = EtmsHelper2.GetIdDecrypt(request.UserNo);
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            request.img_org_code = SystemConfig.ComConfig.DefaultImgUrl;
            request.img_tax_reg = SystemConfig.ComConfig.DefaultImgUrl;
            if (myTenant.LcswApplyStatus != EmLcswApplyStatus.NotApplied) //已申请过
            {
                return await MerchantEdit(request, tenantId, userId, myTenant);
            }
            else
            {
                return await MerchantAdd(request, tenantId, userId, myTenant);
            }
        }

        public async Task<ResponseBase> MerchantAdd(MerchantAddRequest request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            var userId = EtmsHelper2.GetIdDecrypt(request.UserNo);
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            if (myTenant.LcswApplyStatus != EmLcswApplyStatus.NotApplied)
            {
                return ResponseBase.CommonError("机构已申请过扫呗账户");
            }
            return await MerchantAdd(request, tenantId, userId, myTenant);
        }

        private async Task<ResponseBase> MerchantAdd(MerchantAddRequest request, int tenantId, long userId, SysTenant myTenant)
        {
            var addRes = _payLcswService.AddMerchant(new RequestAddMerchant()
            {
                account_name = request.account_name,
                account_no = request.account_no,
                bank_name = request.bank_name,
                bank_no = request.bank_no,
                business_code = request.business_code,
                business_name = request.business_name,
                license_type = request.license_type,
                merchant_address = request.merchant_address,
                merchant_alias = request.merchant_alias,
                merchant_business_type = request.merchant_business_type,
                merchant_city = request.merchant_city,
                merchant_city_code = request.merchant_city_code,
                merchant_company = request.merchant_company,
                merchant_county = request.merchant_county,
                merchant_county_code = request.merchant_county_code,
                merchant_email = request.merchant_email,
                merchant_name = request.merchant_name,
                merchant_person = request.merchant_person,
                merchant_phone = request.merchant_phone,
                merchant_province = request.merchant_province,
                merchant_province_code = request.merchant_province_code,
                settlement_type = request.settlement_type,
                settle_type = "1",
                account_type = request.account_type,
                company_account_name = request.company_account_name,
                company_account_no = request.company_account_no,
                company_bank_name = request.company_bank_name,
                company_bank_no = request.company_bank_no
            }
         , new RequestAddMerchantIsNull()
         {
             account_phone = request.account_phone,
             artif_nm = request.artif_nm,
             img_3rd_part = request.img_3rd_part,
             img_bankcard_a = request.img_bankcard_a,
             img_bankcard_b = request.img_bankcard_b,
             img_cashier = request.img_cashier,
             img_contract = request.img_contract,
             img_idcard_a = request.img_idcard_a,
             img_idcard_b = request.img_idcard_b,
             img_idcard_holding = request.img_idcard_holding,
             img_indoor = request.img_indoor,
             img_license = request.img_license,
             img_logo = request.img_logo,
             img_open_permits = request.img_open_permits,
             img_org_code = request.img_org_code,
             img_other = request.img_other,
             img_standard_protocol = request.img_standard_protocol,
             img_sub_account_promiss = request.img_sub_account_promiss,
             img_tax_reg = request.img_tax_reg,
             img_unincorporated = request.img_unincorporated,
             img_val_add_protocol = request.img_val_add_protocol,
             legalIdnum = request.legalIdnum,
             legalIdnumExpire = request.legalIdnumExpire,
             license_expire = request.license_expire,
             license_no = request.license_no,
             merchant_id_expire = request.merchant_id_expire,
             merchant_id_no = request.merchant_id_no,
             merchant_service_phone = request.merchant_service_phone,
             img_private_idcard_a = request.img_private_idcard_a,
             img_private_idcard_b = request.img_private_idcard_b,
             rate_code = "M0038",
             notify_url = SysWebApiAddressConfig.MerchantAuditCallbackUrl
         });
            if (!addRes.IsSuccess())
            {
                return ResponseBase.CommonError($"创建商户失败:{addRes.return_msg}");
            }
            var addTerminalRes = _payLcswService.AddTermina(new RequestAddTermina()
            {
                merchant_no = addRes.merchant_no
            });
            if (!addTerminalRes.IsSuccess())
            {
                LOG.Log.Error($"[创建终端失败]{myTenant.Name},{myTenant.TenantCode},{addTerminalRes.merchant_no}", this.GetType());
                return ResponseBase.CommonError($"创建终端失败:{addTerminalRes.return_msg}");
            }
            var queryRes = _payLcswService.QuerMerchant(addRes.merchant_no);
            if (!queryRes.IsSuccess())
            {
                return ResponseBase.CommonError("创建商户失败");
            }
            var lcswStatus = GetLcswStatus(queryRes.merchant_status);
            var now = DateTime.Now;

            request.merchant_business_typeDesc = LcswEm.GetMerchantBusinessType(request.merchant_business_type);
            request.account_typeDesc = LcswEm.GetAccountTypeDesc(request.account_type);
            request.settlement_typeDesc = LcswEm.GetSettlementType(request.settlement_type);

            var logLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(myTenant.Id);
            if (logLcsAccount == null)
            {
                var tenantLcsAccount = new SysTenantLcsAccount()
                {
                    AgentId = myTenant.AgentId,
                    ChangeTime = now,
                    CreationTime = now,
                    InstNo = Config._instNo,
                    IsDeleted = EmIsDeleted.Normal,
                    LcswApplyStatus = lcswStatus.LcswApplyStatus,
                    MerchantCompany = request.merchant_company,
                    MerchantName = request.merchant_name,
                    MerchantNo = addRes.merchant_no,
                    MerchantStatus = queryRes.merchant_status,
                    MerchantType = queryRes.merchant_type,
                    Remark = string.Empty,
                    ResultCode = addRes.result_code,
                    ReturnCode = addRes.return_code,
                    ReturnMsg = addRes.return_msg,
                    ReviewTime = null,
                    StoreCode = string.Empty,
                    TenantId = myTenant.Id,
                    TerminalId = addTerminalRes.terminal_id,
                    TerminalName = string.Empty,
                    AccessToken = addTerminalRes.access_token,
                    TraceNo = string.Empty,
                    MerchantInfoData = Newtonsoft.Json.JsonConvert.SerializeObject(queryRes),
                    MerchantRquestData = Newtonsoft.Json.JsonConvert.SerializeObject(request)
                };
                await _tenantLcsAccountDAL.AddTenantLcsAccount(tenantLcsAccount);
            }
            else
            {
                logLcsAccount.AgentId = myTenant.AgentId;
                logLcsAccount.ChangeTime = now;
                logLcsAccount.CreationTime = now;
                logLcsAccount.InstNo = Config._instNo;
                logLcsAccount.IsDeleted = EmIsDeleted.Normal;
                logLcsAccount.LcswApplyStatus = lcswStatus.LcswApplyStatus;
                logLcsAccount.MerchantCompany = request.merchant_company;
                logLcsAccount.MerchantName = request.merchant_name;
                logLcsAccount.MerchantNo = addRes.merchant_no;
                logLcsAccount.MerchantStatus = queryRes.merchant_status;
                logLcsAccount.MerchantType = queryRes.merchant_type;
                logLcsAccount.Remark = string.Empty;
                logLcsAccount.ResultCode = addRes.result_code;
                logLcsAccount.ReturnCode = addRes.return_code;
                logLcsAccount.ReturnMsg = addRes.return_msg;
                logLcsAccount.ReviewTime = null;
                logLcsAccount.StoreCode = string.Empty;
                logLcsAccount.TenantId = myTenant.Id;
                logLcsAccount.TerminalId = addTerminalRes.terminal_id;
                logLcsAccount.TerminalName = string.Empty;
                logLcsAccount.AccessToken = addTerminalRes.access_token;
                logLcsAccount.TraceNo = string.Empty;
                logLcsAccount.MerchantInfoData = Newtonsoft.Json.JsonConvert.SerializeObject(queryRes);
                logLcsAccount.MerchantRquestData = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                await _tenantLcsAccountDAL.EditTenantLcsAccount(logLcsAccount);
            }
            await _sysTenantDAL.UpdateTenantLcswInfo(myTenant.Id, lcswStatus.LcswApplyStatus, lcswStatus.LcswOpenStatus);

            _userOperationLogDAL.InitTenantId(myTenant.Id);
            await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
            {
                ClientType = EmUserOperationLogClientType.PC,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = "申请扫呗支付账户",
                Ot = now,
                Remark = string.Empty,
                TenantId = myTenant.Id,
                Type = (int)EmUserOperationType.LcsMgr,
                UserId = userId
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MerchantEdit(MerchantAddRequest request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            var userId = EtmsHelper2.GetIdDecrypt(request.UserNo);
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            if (myTenant.LcswApplyStatus == EmLcswApplyStatus.NotApplied)
            {
                return ResponseBase.CommonError("机构未申请过扫呗账户");
            }

            return await MerchantEdit(request, tenantId, userId, myTenant);
        }

        private async Task<ResponseBase> MerchantEdit(MerchantAddRequest request, int tenantId, long userId, SysTenant myTenant)
        {
            var tenantLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(myTenant.Id);
            if (tenantLcsAccount == null)
            {
                return ResponseBase.CommonError("扫呗账户不存在");
            }
            var editRes = _payLcswService.UpdateMerchant(tenantLcsAccount.MerchantNo, new RequestUpdateMerchant()
            {
                account_name = request.account_name,
                account_no = request.account_no,
                account_phone = request.account_phone,
                artif_nm = request.artif_nm,
                bank_name = request.bank_name,
                bank_no = request.bank_no,
                business_code = request.business_code,
                business_name = request.business_name,
                img_3rd_part = request.img_3rd_part,
                img_bankcard_a = request.img_bankcard_a,
                img_bankcard_b = request.img_bankcard_b,
                img_cashier = request.img_cashier,
                img_contract = request.img_contract,
                img_idcard_a = request.img_idcard_a,
                img_idcard_b = request.img_idcard_b,
                img_idcard_holding = request.img_idcard_holding,
                img_indoor = request.img_indoor,
                img_license = request.img_license,
                img_logo = request.img_logo,
                img_open_permits = request.img_open_permits,
                img_org_code = request.img_org_code,
                img_other = request.img_other,
                img_standard_protocol = request.img_standard_protocol,
                img_sub_account_promiss = request.img_sub_account_promiss,
                img_tax_reg = request.img_tax_reg,
                img_unincorporated = request.img_unincorporated,
                img_val_add_protocol = request.img_val_add_protocol,
                legalIdnum = request.legalIdnum,
                legalIdnumExpire = request.legalIdnumExpire,
                license_expire = request.license_expire,
                license_no = request.license_no,
                license_type = request.license_type,
                merchant_address = request.merchant_address,
                merchant_business_type = request.merchant_business_type,
                merchant_city = request.merchant_city,
                merchant_city_code = request.merchant_city_code,
                merchant_county = request.merchant_county,
                merchant_county_code = request.merchant_county_code,
                merchant_email = request.merchant_email,
                merchant_id_expire = request.merchant_id_expire,
                merchant_id_no = request.merchant_id_no,
                merchant_name = request.merchant_name,
                merchant_person = request.merchant_person,
                merchant_phone = request.merchant_phone,
                merchant_province = request.merchant_province,
                merchant_province_code = request.merchant_province_code,
                merchant_service_phone = request.merchant_service_phone,
                img_private_idcard_a = request.img_private_idcard_a,
                img_private_idcard_b = request.img_private_idcard_b,
                account_type = request.account_type,
                company_account_name = request.company_account_name,
                company_account_no = request.company_account_no,
                company_bank_name = request.company_bank_name,
                company_bank_no = request.company_bank_no,
                rate_code = "M0038",
                settlement_type = request.settlement_type,
                settle_type = "1",
                notify_url = SysWebApiAddressConfig.MerchantAuditCallbackUrl
            });
            if (!editRes.IsSuccess())
            {
                return ResponseBase.CommonError($"更新商户资料失败:{editRes.return_msg}");
            }
            var queryRes = _payLcswService.QuerMerchant(tenantLcsAccount.MerchantNo);
            if (!queryRes.IsSuccess())
            {
                return ResponseBase.CommonError($"更新商户资料失败:{queryRes.return_msg}");
            }
            var lcswStatus = GetLcswStatus(queryRes.merchant_status);
            var now = DateTime.Now;
            tenantLcsAccount.ChangeTime = now;
            tenantLcsAccount.LcswApplyStatus = lcswStatus.LcswApplyStatus;
            tenantLcsAccount.MerchantName = request.merchant_name;
            tenantLcsAccount.MerchantStatus = queryRes.merchant_status;
            tenantLcsAccount.MerchantType = queryRes.merchant_type;
            tenantLcsAccount.ResultCode = editRes.result_code;
            tenantLcsAccount.ReturnCode = editRes.return_code;
            tenantLcsAccount.ReturnMsg = editRes.return_msg;
            tenantLcsAccount.MerchantInfoData = Newtonsoft.Json.JsonConvert.SerializeObject(queryRes);

            request.merchant_business_typeDesc = LcswEm.GetMerchantBusinessType(request.merchant_business_type);
            request.account_typeDesc = LcswEm.GetAccountTypeDesc(request.account_type);
            request.settlement_typeDesc = LcswEm.GetSettlementType(request.settlement_type);
            tenantLcsAccount.MerchantRquestData = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            await _tenantLcsAccountDAL.EditTenantLcsAccount(tenantLcsAccount);
            await _sysTenantDAL.UpdateTenantLcswInfo(myTenant.Id, lcswStatus.LcswApplyStatus, lcswStatus.LcswOpenStatus);

            _userOperationLogDAL.InitTenantId(myTenant.Id);
            await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
            {
                ClientType = EmUserOperationLogClientType.PC,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = "修改扫呗支付账户",
                Ot = now,
                Remark = string.Empty,
                TenantId = myTenant.Id,
                Type = (int)EmUserOperationType.LcsMgr,
                UserId = userId
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> MerchantLcsAccountBind(MerchantLcsAccountBindRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var queryRes = _payLcswService.QuerMerchant(request.MerchantNo);
            if (!queryRes.IsSuccess())
            {
                return ResponseBase.CommonError("商户号未找到对应的商户");
            }
            var traceno = Guid.NewGuid().ToString("N");
            var addTerminalRes = _payLcswService.QueryTermina(traceno, request.TerminalId);
            if (!addTerminalRes.IsSuccess())
            {
                return ResponseBase.CommonError("终端号错误");
            }
            var now = DateTime.Now;
            var lcswStatus = GetLcswStatus(queryRes.merchant_status);
            var myMerchantSaveRequest = new MerchantSaveRequest();
            myMerchantSaveRequest = ReassignMerchantSaveRequestProperty(myMerchantSaveRequest, queryRes);
            var hisLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(request.LoginTenantId);
            if (hisLcsAccount == null)
            {
                await _tenantLcsAccountDAL.AddTenantLcsAccount(new SysTenantLcsAccount()
                {
                    AgentId = myTenant.AgentId,
                    ChangeTime = now,
                    CreationTime = now,
                    InstNo = Config._instNo,
                    IsDeleted = EmIsDeleted.Normal,
                    LcswApplyStatus = lcswStatus.LcswApplyStatus,
                    MerchantCompany = myMerchantSaveRequest.merchant_company,
                    MerchantName = myMerchantSaveRequest.merchant_name,
                    MerchantNo = request.MerchantNo,
                    MerchantStatus = queryRes.merchant_status,
                    MerchantType = queryRes.merchant_type,
                    Remark = string.Empty,
                    ResultCode = queryRes.result_code,
                    ReturnCode = queryRes.return_code,
                    ReturnMsg = queryRes.return_msg,
                    ReviewTime = null,
                    StoreCode = string.Empty,
                    TenantId = myTenant.Id,
                    TerminalId = addTerminalRes.terminal_id,
                    TerminalName = string.Empty,
                    AccessToken = addTerminalRes.access_token,
                    TraceNo = string.Empty,
                    MerchantInfoData = Newtonsoft.Json.JsonConvert.SerializeObject(queryRes),
                    MerchantRquestData = Newtonsoft.Json.JsonConvert.SerializeObject(myMerchantSaveRequest)
                });
            }
            else
            {
                hisLcsAccount.ChangeTime = now;
                hisLcsAccount.InstNo = Config._instNo;
                hisLcsAccount.LcswApplyStatus = lcswStatus.LcswApplyStatus;
                hisLcsAccount.MerchantCompany = myMerchantSaveRequest.merchant_company;
                hisLcsAccount.MerchantName = myMerchantSaveRequest.merchant_name;
                hisLcsAccount.MerchantNo = request.MerchantNo;
                hisLcsAccount.MerchantStatus = queryRes.merchant_status;
                hisLcsAccount.MerchantType = queryRes.merchant_type;
                hisLcsAccount.Remark = string.Empty;
                hisLcsAccount.ResultCode = queryRes.result_code;
                hisLcsAccount.ReturnCode = queryRes.return_code;
                hisLcsAccount.ReturnMsg = queryRes.return_msg;
                hisLcsAccount.ReviewTime = null;
                hisLcsAccount.StoreCode = string.Empty;
                hisLcsAccount.TenantId = myTenant.Id;
                hisLcsAccount.TerminalId = addTerminalRes.terminal_id;
                hisLcsAccount.TerminalName = string.Empty;
                hisLcsAccount.AccessToken = addTerminalRes.access_token;
                hisLcsAccount.TraceNo = string.Empty;
                hisLcsAccount.MerchantInfoData = Newtonsoft.Json.JsonConvert.SerializeObject(queryRes);
                hisLcsAccount.MerchantRquestData = Newtonsoft.Json.JsonConvert.SerializeObject(myMerchantSaveRequest);
                await _tenantLcsAccountDAL.EditTenantLcsAccount(hisLcsAccount);
            }
            await _sysTenantDAL.UpdateTenantLcswInfo(myTenant.Id, lcswStatus.LcswApplyStatus, lcswStatus.LcswOpenStatus);
            _userOperationLogDAL.InitTenantId(myTenant.Id);
            await _userOperationLogDAL.AddUserLog(new EtUserOperationLog()
            {
                ClientType = EmUserOperationLogClientType.PC,
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = $"绑定扫呗账户_{request.MerchantNo}",
                Ot = now,
                Remark = string.Empty,
                TenantId = myTenant.Id,
                Type = (int)EmUserOperationType.LcsMgr,
                UserId = request.LoginUserId
            });
            return ResponseBase.Success();
        }

        private MerchantSaveRequest ReassignMerchantSaveRequestProperty(MerchantSaveRequest request, ResponseQuerMerchant queryOut)
        {
            request.account_name = queryOut.account_name;
            request.account_no = queryOut.account_no;
            request.account_phone = queryOut.account_phone;
            request.account_type = queryOut.account_type;
            request.artif_nm = queryOut.artif_nm;
            request.bank_name = queryOut.bank_name;
            request.bank_no = queryOut.bank_no;
            request.business_name = queryOut.business_name;
            request.business_code = queryOut.business_code;
            request.daily_timely_status = queryOut.daily_timely_status;
            request.img_3rd_part = queryOut.img_3rd_part;
            request.img_bankcard_a = queryOut.img_bankcard_a;
            request.img_bankcard_b = queryOut.img_bankcard_b;
            request.img_cashier = queryOut.img_cashier;
            request.img_contract = queryOut.img_contract;
            request.img_idcard_a = queryOut.img_idcard_a;
            request.img_idcard_b = queryOut.img_idcard_b;
            request.img_idcard_holding = queryOut.img_idcard_holding;
            request.img_indoor = queryOut.img_indoor;
            request.img_license = queryOut.img_license;
            request.img_logo = queryOut.img_logo;
            request.img_open_permits = queryOut.img_open_permits;
            request.img_org_code = queryOut.img_org_code;
            request.img_other = queryOut.img_other;
            request.img_private_idcard_a = queryOut.img_private_idcard_a;
            request.img_private_idcard_b = queryOut.img_private_idcard_b;
            request.img_standard_protocol = queryOut.img_standard_protocol;
            request.img_sub_account_promiss = queryOut.img_sub_account_promiss;
            request.img_tax_reg = queryOut.img_tax_reg;
            request.img_unincorporated = queryOut.img_unincorporated;
            request.img_val_add_protocol = queryOut.img_val_add_protocol;
            request.legalIdnum = queryOut.legalIdnum;
            request.legalIdnumExpire = queryOut.legalIdnumExpire;
            request.license_expire = queryOut.license_expire;
            request.license_no = queryOut.license_no;
            request.license_type = queryOut.license_type;
            request.merchant_address = queryOut.merchant_address;
            request.merchant_alias = queryOut.merchant_alias;
            request.merchant_business_type = queryOut.merchant_business_type;
            request.merchant_city = queryOut.merchant_city;
            request.merchant_city_code = queryOut.merchant_city_code;
            request.merchant_company = queryOut.merchant_company;
            request.merchant_county = queryOut.merchant_county;
            request.merchant_county_code = queryOut.merchant_county_code;
            request.merchant_email = queryOut.merchant_email;
            request.merchant_id_expire = queryOut.merchant_id_expire;
            request.merchant_id_no = queryOut.merchant_id_no;
            request.merchant_name = queryOut.merchant_name;
            request.merchant_no = queryOut.merchant_no;
            request.merchant_person = queryOut.merchant_person;
            request.merchant_phone = queryOut.merchant_phone;
            request.merchant_province = queryOut.merchant_province;
            request.merchant_province_code = queryOut.merchant_province_code;
            request.merchant_service_phone = queryOut.merchant_service_phone;
            request.rate_code = queryOut.rate_code;
            request.settlement_type = queryOut.settlement_type;
            request.settle_type = queryOut.settle_type;

            request.merchant_business_typeDesc = LcswEm.GetMerchantBusinessType(request.merchant_business_type);
            request.account_typeDesc = LcswEm.GetAccountTypeDesc(request.account_type);
            request.settlement_typeDesc = LcswEm.GetSettlementType(request.settlement_type);
            return request;
        }

        private async Task<ResponseBase> MerchantQuery(int tenantId, long userId)
        {
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            if (myTenant == null)
            {
                return ResponseBase.CommonError("机构不存在");
            }
            var output = new MerchantQueryOutput()
            {
                LcswStatus = myTenant.LcswApplyStatus,
                UserNo = EtmsHelper2.GetIdEncrypt(userId),
                TenantNo = TenantLib.GetTenantEncrypt(tenantId)
            };

            if (myTenant.LcswApplyStatus == EmLcswApplyStatus.NotApplied)
            {
                return ResponseBase.Success(output);
            }
            var tenantLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(myTenant.Id);
            if (tenantLcsAccount == null)
            {
                return ResponseBase.CommonError("扫呗账户不存在");
            }
            //更新状态
            var queryRes = _payLcswService.QuerMerchant(tenantLcsAccount.MerchantNo);
            if (!queryRes.IsSuccess())
            {
                return ResponseBase.CommonError("扫呗账户信息异常");
            }
            var lcswStatus = GetLcswStatus(queryRes.merchant_status);
            var myQueryRes = Newtonsoft.Json.JsonConvert.DeserializeObject<MerchantSaveRequest>(tenantLcsAccount.MerchantRquestData);
            myQueryRes.merchant_no = tenantLcsAccount.MerchantNo;
            myQueryRes = ReassignMerchantSaveRequestProperty(myQueryRes, queryRes);
            var now = DateTime.Now;
            tenantLcsAccount.ChangeTime = now;
            tenantLcsAccount.LcswApplyStatus = lcswStatus.LcswApplyStatus;
            tenantLcsAccount.MerchantStatus = queryRes.merchant_status;
            tenantLcsAccount.MerchantType = queryRes.merchant_type;
            tenantLcsAccount.MerchantInfoData = Newtonsoft.Json.JsonConvert.SerializeObject(queryRes);
            await _tenantLcsAccountDAL.EditTenantLcsAccount(tenantLcsAccount);

            if (lcswStatus.LcswApplyStatus != myTenant.LcswApplyStatus)
            {
                await _sysTenantDAL.UpdateTenantLcswInfo(myTenant.Id, lcswStatus.LcswApplyStatus, lcswStatus.LcswOpenStatus);
                myTenant.LcswApplyStatus = lcswStatus.LcswApplyStatus;
            }

            if (EmLcswApplyStatus.IsSuccess(myTenant.LcswApplyStatus))
            {
                output.LcswStatus = 10; //使用10来标注 支付账户审核通过并已开通
            }
            output.MerchantInfo = myQueryRes;
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> MerchantQueryPC(MerchantQueryPCRequest request)
        {
            return await MerchantQuery(request.LoginTenantId, request.LoginUserId);
        }

        public async Task<ResponseBase> MerchantQueryH5(MerchantQueryH5Request request)
        {
            var tenantId = TenantLib.GetTenantDecrypt(request.TenantNo);
            var userId = EtmsHelper2.GetIdDecrypt(request.UserNo);
            return await MerchantQuery(tenantId, userId);
        }

        public async Task<MerchantAuditCallbackOutput> MerchantAuditCallback(MerchantAuditCallbackRequest request)
        {
            LOG.Log.Info("[MerchantAuditCallback]利楚扫呗商户申请回调", request, this.GetType());
            var tenantLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(request.merchant_no);
            var output = new MerchantAuditCallbackOutput()
            {
                return_code = "02",
                trace_no = request.trace_no,
                return_msg = "处理失败"
            };
            if (tenantLcsAccount == null)
            {
                LOG.Log.Error("[MerchantAuditCallback]利楚扫呗回调,未找到账户信息", request, this.GetType());
                return output;
            }
            var queryRes = _payLcswService.QuerMerchant(tenantLcsAccount.MerchantNo);
            if (!queryRes.IsSuccess())
            {
                LOG.Log.Error("[MerchantAuditCallback]利楚扫呗回调,未查询到商户信息", request, this.GetType());
                return output;
            }
            var lcswStatus = GetLcswStatus(queryRes.merchant_status);
            var now = DateTime.Now;
            tenantLcsAccount.ChangeTime = now;
            tenantLcsAccount.LcswApplyStatus = lcswStatus.LcswApplyStatus;
            tenantLcsAccount.MerchantName = queryRes.merchant_name;
            tenantLcsAccount.MerchantStatus = queryRes.merchant_status;
            tenantLcsAccount.MerchantType = queryRes.merchant_type;
            tenantLcsAccount.ResultCode = request.result_code;
            tenantLcsAccount.ReturnCode = request.return_code;
            tenantLcsAccount.ReturnMsg = request.return_msg;
            tenantLcsAccount.MerchantInfoData = Newtonsoft.Json.JsonConvert.SerializeObject(queryRes);
            await _tenantLcsAccountDAL.EditTenantLcsAccount(tenantLcsAccount);
            await _sysTenantDAL.UpdateTenantLcswInfo(tenantLcsAccount.TenantId, lcswStatus.LcswApplyStatus, lcswStatus.LcswOpenStatus);

            return new MerchantAuditCallbackOutput()
            {
                return_code = "01",
                return_msg = "处理成功",
                trace_no = request.trace_no
            };
        }
    }
}
