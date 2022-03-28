using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.Head.Output
{
    public class HeadAllGetOut
    {
        public long CId { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime ExDate { get; set; }

        /// <summary>
        /// 短信数量
        /// </summary>
        public int SmsCount { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

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

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime Ot { get; set; }

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

        public long Value { get; set; }

        public string Label { get; set; }
    }
}
