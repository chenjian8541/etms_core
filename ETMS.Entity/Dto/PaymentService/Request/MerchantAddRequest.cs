using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class MerchantAddRequest : MerchantSaveRequest, IValidate
    {

        public string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo) || string.IsNullOrEmpty(UserNo))
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(merchant_name))
            {
                return "商户名称不能为空";
            }
            if (string.IsNullOrEmpty(merchant_alias))
            {
                return "商户简称不能为空";
            }
            if (string.IsNullOrEmpty(merchant_company))
            {
                return "注册名称不能为空";
            }
            if (string.IsNullOrEmpty(merchant_province_code))
            {
                return "所在省不能为空";
            }
            if (string.IsNullOrEmpty(merchant_city_code))
            {
                return "所在市不能为空";
            }
            if (string.IsNullOrEmpty(merchant_county_code))
            {
                return "所在区县不能为空";
            }
            if (string.IsNullOrEmpty(merchant_address))
            {
                return "详细地址不能为空";
            }
            if (string.IsNullOrEmpty(merchant_person))
            {
                return "联系人姓名不能为空";
            }
            if (string.IsNullOrEmpty(merchant_phone))
            {
                return "联系人电话不能为空";
            }
            if (string.IsNullOrEmpty(merchant_email))
            {
                return "联系人邮箱不能为空";
            }
            if (string.IsNullOrEmpty(business_name))
            {
                return "行业类目名称不能为空";
            }
            if (string.IsNullOrEmpty(business_code))
            {
                return "行业类目编码不能为空";
            }
            if (settlement_type <= 0)
            {
                return "结算类型不能为空";
            }
            if (string.IsNullOrEmpty(account_name))
            {
                return "开户名称不能为空";
            }
            if (string.IsNullOrEmpty(account_no))
            {
                return "开户账号不能为空";
            }
            if (string.IsNullOrEmpty(bank_name))
            {
                return "开户银行不能为空";
            }
            if (string.IsNullOrEmpty(bank_no))
            {
                return "开户银行不能为空";
            }
            return string.Empty;
        }
    }
}
