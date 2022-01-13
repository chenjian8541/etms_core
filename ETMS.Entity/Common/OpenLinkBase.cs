using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Common
{
    /// <summary>
    /// 开放链接 访问
    /// </summary>
    public class OpenLinkBase : IValidate
    {
        public string VtNo { get; set; }

        private int _tenantId;

        public int LoginTenantId
        {
            get
            {
                return _tenantId;
            }
        }

        private long _userId;

        public long LoginUserId
        {
            get
            {
                return _userId;
            }
        }

        private DateTime _expiredTime;

        public DateTime LoginExpiredTime
        {
            get
            {
                return _expiredTime;
            }
        }

        public virtual string Validate()
        {
            if (string.IsNullOrEmpty(VtNo))
            {
                return "请求数据格式错误";
            }
            var analyzeView = EtmsHelper3.OpenLinkAnalyzeVtNo(this.VtNo);
            if (analyzeView.ExTime < DateTime.Now)
            {
                return "此访问地址已过期，请重新进入";
            }
            this._tenantId = analyzeView.TenantId;
            this._userId = analyzeView.UserId;
            this._expiredTime = analyzeView.ExTime;
            return string.Empty;
        }
    }
}
