using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    public class OpenApi99Base : IValidate
    {
        public string TenantNo { get; set; }

        private int _tenantId;

        public int LoginTenantId
        {
            get
            {
                if (_tenantId > 0)
                {
                    return _tenantId;
                }
                _tenantId = EtmsHelper2.GetTenantDecryptOpenApi99(TenantNo);
                return _tenantId;
            }
        }

        protected string DataFilterWhere
        {
            get
            {
                return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal}";
            }
        }

        public virtual string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo))
            {
                return "请求数据格式错误";
            }
            if (LoginTenantId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
