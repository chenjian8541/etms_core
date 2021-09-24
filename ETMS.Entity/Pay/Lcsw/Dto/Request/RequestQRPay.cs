using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Pay.Lcsw.Dto.Request
{
    /// <summary>
    /// 聚合支付请求类
    /// </summary>
   public class RequestQRPay
    {
        /******************************必填参数*********************************/
        /// <summary>
        /// 版本号，当前版本110,130版本控制链接有效期为2小时
        /// </summary>
        public string pay_ver { get; set; }
        /// <summary>
        /// 请求类型，000
        /// </summary>
        public string pay_type { get; set; }
        /// <summary>
        /// 接口类型，当前类型016
        /// </summary>
        public string service_id { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string merchant_no { get; set; }
        /// <summary>
        /// 终端号
        /// </summary>
        public string terminal_id { get; set; }
        /// <summary>
        /// 终端流水号，填写商户系统的订单号
        /// </summary>
        public string terminal_trace { get; set; }
        /// <summary>
        /// 终端交易时间，yyyyMMddHHmmss，全局统一时间格式
        /// </summary>
        public string terminal_time { get; set; }
        /// <summary>
        /// 金额，单位分
        /// </summary>
        public string total_fee { get; set; }
        /// <summary>
        /// 签名字符串,拼装所有传递参数（字典序）+令牌，UTF-8编码，32位md5加密转换
        /// </summary>
        public string key_sign { get; set; }

        /******************************选填参数*********************************/
        /// <summary>
        /// 订单描述
        /// </summary>
        public string order_body { get; set; }
        /// <summary>
        /// 外部系统通知地址
        /// </summary>
        public string notify_url { get; set; }
        /// <summary>
        /// 附加数据，原样返回
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 是否允许订单重复,1:不允许terminal_trace重复2.不允许terminal_trace+terminal_time重复,0或不传:允许重复
        /// </summary>
        public string repeated_trace { get; set; }

    }
}
