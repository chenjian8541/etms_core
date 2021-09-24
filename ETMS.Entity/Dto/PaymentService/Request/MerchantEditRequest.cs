﻿using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class MerchantEditRequest : IValidate
    {
        /**
         * 扫呗商户号
         */
        public string merchant_no { get; set; }
        /**
         * 商户名称，扫呗系统全局唯一不可重复，最多50个汉字且不能包含特殊符号参考验重接口
         */
        public string merchant_name { get; set; }
        /**
         * 	所在省
         */
        public string merchant_province { get; set; }
        /**
         * 省编码
         */
        public string merchant_province_code { get; set; }
        /**
         * 所在市
         */
        public string merchant_city { get; set; }
        /**
         * 市编码
         */
        public string merchant_city_code { get; set; }
        /**
         * 所在区县
         */
        public string merchant_county { get; set; }
        /**
         * 所在区县编码
         */
        public string merchant_county_code { get; set; }
        /**
         * 	商户详细地址
         */
        public string merchant_address { get; set; }
        /**
         * 商户联系人姓名
         */
        public string merchant_person { get; set; }
        /**
         * 	商户联系人电话（唯一）
         */
        public string merchant_phone { get; set; }
        /**
         * 商户联系人邮箱（唯一）
         */
        public string merchant_email { get; set; }
        /**
         * 	客服电话
         */
        public string merchant_service_phone { get; set; }
        /**
         * 	行业类目名称
         */
        public string business_name { get; set; }
        /**
         * 	行业类目编码，由扫呗技术支持提供表格
         */
        public string business_code { get; set; }
        /**
         * 商户类型:1企业，2个体工商户，3小微商户
         */
        public int merchant_business_type { get; set; }
        /**
         * 账户类型，1对公，2对私
         */
        public string account_type { get; set; }
        /**
         * 营业证件类型：0营业执照，1三证合一，2手持身份证
         */
        public int license_type { get; set; }
        /**
         * 营业证件号码
         */
        public string license_no { get; set; }
        /**
         * 营业证件到期日（格式YYYY-MM-DD）
         */
        public string license_expire { get; set; }
        /**
         * 	法人名称
         */
        public string artif_nm { get; set; }
        /**
         * 	法人身份证号
         */
        public string legalIdnum { get; set; }
        /**
         * 	法人身份证有效期（格式YYYY-MM-DD）
         */
        public string legalIdnumExpire { get; set; }
        /**
         * 结算人身份证号码
         */
        public string merchant_id_no { get; set; }
        /**
         * 结算人身份证有效期，格式YYYYMMDD，长期填写29991231
         */
        public string merchant_id_expire { get; set; }
        /**
         * 入账银行卡开户名（结算人姓名/公司名）
         */
        public string account_name { get; set; }
        /**
         * 入账银行卡卡号
         */
        public string account_no { get; set; }
        /**
         * 入账银行预留手机号
         */
        public string account_phone { get; set; }
        /**
         * 	入账银行卡开户支行
         */
        public string bank_name { get; set; }
        /**
         * 开户支行联行号，由扫呗技术支持提供表格
         */
        public string bank_no { get; set; }
        /**
         * 对公户结算账户开户名
         */
        public string company_account_name { get; set; }
        /**
         * 对公户结算账户开户号
         */
        public string company_account_no { get; set; }
        /**
         * 	对公户结算账户开户支行
         */
        public string company_bank_name { get; set; }
        /**
         * 对公户结算账户开户支行联行号，由扫呗技术支持提供表格
         */
        public string company_bank_no { get; set; }
        /**
         * 	营业执照照片
         */
        public string img_license { get; set; }
        /**
         * 负责人身份证正面照片
         */
        public string img_idcard_a { get; set; }
        /**
         * 负责人身份证反面照片
         */
        public string img_idcard_b { get; set; }
        /**
         * 	银行卡正面照片
         */
        public string img_bankcard_a { get; set; }
        /**
         * 银行卡反面照片
         */
        public string img_bankcard_b { get; set; }
        /**
         * 商户门头照片
         */
        public string img_logo { get; set; }
        /**
         * 	内部前台照片
         */
        public string img_indoor { get; set; }
        /**
         * 店内环境照片
         */
        public string img_contract { get; set; }
        /**
         * 	其他证明材料
         */
        public string img_other { get; set; }
        /**
         * 	本人手持身份证照片
         */
        public string img_idcard_holding { get; set; }
        /**
         * 	商户联系人身份证照片(正面)
         */
        public string img_merchant_person_idcard { get; set; }
        /**
         * 	开户许可证照片
         */
        public string img_open_permits { get; set; }
        /**
         * 组织机构代码证照片
         */
        public string img_org_code { get; set; }
        /**
         * 税务登记证照片
         */
        public string img_tax_reg { get; set; }
        /**
         * 	入账非法人证明照片
         */
        public string img_unincorporated { get; set; }
        /**
         * 对私账户身份证正面照片
         */
        public string img_public_idcard_a { get; set; }
        /**
         * 	对私账户身份证反面照片
         */
        public string img_public_idcard_b { get; set; }
        /**
         * 商户总分店关系证明
         */
        public string img_standard_protocol { get; set; }
        /**
         * 	商户增值协议照片
         */
        public string img_val_add_protocol { get; set; }
        /**
         * 分账承诺函
         */
        public string img_sub_account_promiss { get; set; }
        /**
         * 微信支付物料照片
         */
        public string img_cashier { get; set; }
        /**
         * 第三方平台截图
         */
        public string img_3rd_part { get; set; }
        /**
         * 支付宝支付物料照片
         */
        public string img_alicashier { get; set; }
        /**
         * 业务员门头合照
         */
        public string img_salesman_logo { get; set; }
        /**
         * 云闪付营销物料布放照片
         */
        public string img_union_materiel { get; set; }


        /// <summary>
        /// 机构ID
        /// </summary>
        public string TenantNo { get; set; }

        public string UserNo { get; set; }

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
            if (string.IsNullOrEmpty(account_type))
            {
                return "账户类型不能为空";
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
