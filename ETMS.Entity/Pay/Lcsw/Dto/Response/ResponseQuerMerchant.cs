using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request.Response
{
    /// <summary>
    /// 查询商户返回信息返回信息
    /// </summary>
    [Serializable]
    public class ResponseQuerMerchant : BaseResult
    {
        /// <summary>
        /// 商户状态，01审核通过，但未签署电子协议，02审核驳回，03审核中,05审核通过且已签署电子协议
        /// </summary>
        public string merchant_status { get; set; }
        /// <summary>
        /// 创建商户类型，1普通商户，2二级商户
        /// </summary>
        public int merchant_type { get; set; }
        /// <summary>
        /// 请求流水号
        /// </summary>
        public string trace_no { get; set; }
        /// <summary>
        /// 机构号
        /// </summary>
        public string inst_no { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string merchant_name { get; set; }
        /// <summary>
        /// 商户简称
        /// </summary>
        public string merchant_alias { get; set; }
        /// <summary>
        /// 商户注册名称
        /// </summary>
        public string merchant_company { get; set; }
        /// <summary>
        /// 所在省
        /// </summary>
        public string merchant_province { get; set; }
        /// <summary>
        /// 省编码
        /// </summary>
        public string merchant_province_code { get; set; }
        /// <summary>
        /// 所在市
        /// </summary>
        public string merchant_city { get; set; }
        /// <summary>
        /// 市编码
        /// </summary>
        public string merchant_city_code { get; set; }
        /// <summary>
        /// 所在区县
        /// </summary>
        public string merchant_county { get; set; }
        /// <summary>
        /// 所在区县编码
        /// </summary>
        public string merchant_county_code { get; set; }
        /// <summary>
        /// 商户详细地址
        /// </summary>
        public string merchant_address { get; set; }
        /// <summary>
        /// 商户联系人姓名
        /// </summary>
        public string merchant_person { get; set; }
        /// <summary>
        /// 商户联系人电话（唯一）
        /// </summary>
        public string merchant_phone { get; set; }
        /// <summary>
        /// 商户联系人邮箱（唯一）
        /// </summary>
        public string merchant_email { get; set; }
        /// <summary>
        /// 行业类目名称
        /// </summary>
        public string business_name { get; set; }
        /// <summary>
        /// 行业类目编码
        /// </summary>
        public string business_code { get; set; }
        /// <summary>
        /// 商户类型:1企业，2个体工商户，3小微商户
        /// </summary>
        public int? merchant_business_type { get; set; }
        /// <summary>
        /// 账户类型，1对公，2对私
        /// </summary>
        public int? account_type { get; set; }
        /// <summary>
        /// 结算类型:1.法人结算 2.非法人结算
        /// </summary>
        public int? settlement_type { get; set; }
        /// <summary>
        /// 营业证件类型：0营业执照，1三证合一，2手持身份证
        /// </summary>
        public int? license_type { get; set; }
        /// <summary>
        /// 入账银行卡开户名（结算人姓名/公司名）
        /// </summary>
        public string account_name { get; set; }
        /// <summary>
        /// 入账银行卡卡号
        /// </summary>
        public string account_no { get; set; }
        /// <summary>
        /// 入账银行卡开户支行
        /// </summary>
        public string bank_name { get; set; }
        /// <summary>
        /// 开户支行联行号
        /// </summary>
        public string bank_no { get; set; }
        /// <summary>
        /// 清算类型：1自动结算；2手动结算，
        /// </summary>
        public string settle_type { get; set; }
        /// <summary>
        /// 自动清算金额（单位分）
        /// </summary>
        public string settle_amount { get; set; }
        /// <summary>
        /// 客服电话
        /// </summary>
        public string merchant_service_phone { get; set; }
        /// <summary>
        /// D1状态,0不开通，1开通
        /// </summary>
        public int? daily_timely_status { get; set; }
        /// <summary>
        /// D1手续费代码
        /// </summary>
        public string daily_timely_code { get; set; }
        /// <summary>
        /// 营业证件号码
        /// </summary>
        public string license_no { get; set; }
        /// <summary>
        /// 营业证件到期日（格式YYYY-MM-DD）
        /// </summary>
        public string license_expire { get; set; }
        /// <summary>
        /// 法人名称
        /// </summary>
        public string artif_nm { get; set; }
        /// <summary>
        /// 法人身份证号
        /// </summary>
        public string legalIdnum { get; set; }
        /// <summary>
        /// 法人身份证有效期（格式YYYY-MM-DD）
        /// </summary>
        public string legalIdnumExpire { get; set; }
        /// <summary>
        /// 结算人身份证号码
        /// </summary>
        public string merchant_id_no { get; set; }
        /// <summary>
        /// 结算人身份证有效期，格式YYYYMMDD，长期填写29991231
        /// </summary>
        public string merchant_id_expire { get; set; }
        /// <summary>
        /// 入账银行预留手机号
        /// </summary>
        public string account_phone { get; set; }
        /// <summary>
        /// 限制信用卡使用,0不限制，1限制
        /// </summary>
        public int? no_credit { get; set; }
        /// <summary>
        /// 支付费率代码
        /// </summary>
        public string rate_code { get; set; }
        /// <summary>
        /// 是否绿洲商户：0非绿洲，1开通绿洲
        /// </summary>
        public int? greenstatus { get; set; }
        /// <summary>
        /// 营业执照照片
        /// </summary>
        public string img_license { get; set; }
        /// <summary>
        /// 法人身份证正面照片
        /// </summary>
        public string img_idcard_a { get; set; }
        /// <summary>
        /// 法人身份证反面照片
        /// </summary>
        public string img_idcard_b { get; set; }
        /// <summary>
        /// 入账银行卡正面照片
        /// </summary>
        public string img_bankcard_a { get; set; }
        /// <summary>
        /// 入账银行卡反面照片
        /// </summary>
        public string img_bankcard_b { get; set; }
        /// <summary>
        /// 商户门头照片
        /// </summary>
        public string img_logo { get; set; }
        /// <summary>
        /// 三联单合同照片
        /// </summary>
        public string img_contract { get; set; }
        /// <summary>
        /// 其他证明材料
        /// </summary>
        public string img_other { get; set; }
        /// <summary>
        /// 内部前台照片
        /// </summary>
        public string img_indoor { get; set; }
        /// <summary>
        /// 本人手持身份证照片
        /// </summary>
        public string img_idcard_holding { get; set; }
        /// <summary>
        /// 开户许可证照片
        /// </summary>
        public string img_open_permits { get; set; }
        /// <summary>
        /// 组织机构代码证照片
        /// </summary>
        public string img_org_code { get; set; }
        /// <summary>
        /// 税务登记证照片
        /// </summary>
        public string img_tax_reg { get; set; }
        /// <summary>
        /// 入账非法人证明照片
        /// </summary>
        public string img_unincorporated { get; set; }
        /// <summary>
        /// 对私账户身份证正面照片
        /// </summary>
        public string img_private_idcard_a { get; set; }
        /// <summary>
        /// 对私账户身份证反面照片
        /// </summary>
        public string img_private_idcard_b { get; set; }
        /// <summary>
        /// 商户标准协议照片
        /// </summary>
        public string img_standard_protocol { get; set; }
        /// <summary>
        /// 商户增值协议照片
        /// </summary>
        public string img_val_add_protocol { get; set; }
        /// <summary>
        /// 分账承诺函
        /// </summary>
        public string img_sub_account_promiss { get; set; }
        /// <summary>
        /// 收银台照片
        /// </summary>
        public string img_cashier { get; set; }
        /// <summary>
        /// 第三方平台截图
        /// </summary>
        public string img_3rd_part { get; set; }
        /// <summary>
        /// 审核状态通知地址
        /// </summary>
        public string notify_url { get; set; }
        /// <summary>
        /// 微信公众号appid，普通商户公众号支付使用
        /// </summary>
        public string wx_appid { get; set; }
        /// <summary>
        /// 微信公众号appsecret，普通商户公众号支付使用，
        /// </summary>
        public string wx_appsecret { get; set; }
    }
}
