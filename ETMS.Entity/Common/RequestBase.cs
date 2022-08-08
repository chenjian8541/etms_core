using ETMS.Entity.Enum;
using ETMS.Entity.View.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    /// <summary>
    /// 请求基类
    /// </summary>
    public class RequestBase : IValidate
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long LoginUserId { set; get; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int LoginTenantId { get; set; }

        /// <summary>
        /// 登录时间戳
        /// </summary>
        public string LoginTimestamp { get; set; }

        /// <summary>
        /// 客户端类型  <see cref="EmUserOperationLogClientType"/>
        /// </summary>
        public int LoginClientType { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 是否启用数据限制
        /// </summary>
        public bool IsDataLimit { private get; set; }

        public bool GetIsDataLimit()
        {
            return this.IsDataLimit;
        }

        public bool GetIsDataLimit(int t)
        {
            if (!IsDataLimit)
            {
                return false;
            }
            if (AuthorityValueDataBag != null)
            {
                switch (t)
                {
                    case 1:
                        return AuthorityValueDataBag.Student;
                    case 2:
                        return AuthorityValueDataBag.StudentTrackLog;
                    case 3:
                        return AuthorityValueDataBag.ClassRecord;
                    case 4:
                        return AuthorityValueDataBag.Order;
                    case 5:
                        return AuthorityValueDataBag.Class;
                    case 6:
                        return AuthorityValueDataBag.ClassTimes;
                    case 7:
                        return AuthorityValueDataBag.Finance;
                    case 8:
                        return AuthorityValueDataBag.BascData;
                    case 9:
                        return AuthorityValueDataBag.Interaction;
                }
            }
            return true;
        }

        /// <summary>
        /// 隐私类型 <see cref="ETMS.Entity.Enum.EmRoleSecrecyType"/>
        /// </summary>
        public int SecrecyType { get; set; }

        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        public AuthorityValueDataDetailView AuthorityValueDataBag { get; set; }

        public SecrecyDataView SecrecyDataBag { get; set; }

        protected string DataFilterWhere
        {
            get
            {
                return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal}";
            }
        }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public virtual string Validate()
        {
            return string.Empty;
        }
    }
}
