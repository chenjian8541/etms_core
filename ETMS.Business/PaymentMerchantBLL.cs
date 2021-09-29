using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
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

        public PaymentMerchantBLL(ISysTenantDAL sysTenantDAL, ITenantLcsAccountDAL tenantLcsAccountDAL, IPayLcswService payLcswService,
            IUserOperationLogDAL userOperationLogDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
            this._payLcswService = payLcswService;
            this._userOperationLogDAL = userOperationLogDAL;
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
            if (EmLcswApplyStatus.IsSuccess(myTenant.LcswApplyStatus))
            {
                output.LcswStatus = 10; //使用10来标注 支付账户审核通过并已开通
            }
            var tenantLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(myTenant.Id);
            if (tenantLcsAccount == null)
            {
                return ResponseBase.CommonError("扫呗账户不存在");
            }
            if (!string.IsNullOrEmpty(tenantLcsAccount.MerchantRquestData))
            {
                var queryRes = Newtonsoft.Json.JsonConvert.DeserializeObject<MerchantSaveRequest>(tenantLcsAccount.MerchantRquestData);
                queryRes.merchant_no = tenantLcsAccount.MerchantNo;
                output.MerchantInfo = queryRes;
            }

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
            LOG.Log.Info("[MerchantAuditCallback]利楚扫呗回调", request, this.GetType());
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

            return new MerchantAuditCallbackOutput();
        }
    }
}
