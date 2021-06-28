using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenApi99.Output
{
    public class TenantInfoGetOutput
    {
        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime ExDate { get; set; }

        /// <summary>
        /// 短信数量
        /// </summary>
        public int SmsCount { get; set; }

        /// <summary>
        /// 限制用户数
        /// (0:不限制)
        /// </summary>
        public int MaxUserCount { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机构代码
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// 机构电话
        /// </summary>
        public string Phone { get; set; }

        public string FaceApiAppid { get; set; }

        public string FaceApiApiKey { get; set; }

        public string FaceApiSecretKey { get; set; }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 短信签名
        /// </summary>
        public string SmsSignature { get; set; }
    }
}
