using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    public class ParentRequestBase : IValidate
    {
        /// <summary>
        /// 登陆手机号
        /// </summary>
        public string LoginPhone { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int LoginTenantId { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExTime { get; set; }

        /// <summary>
        /// 家长学员信息
        /// </summary>
        public List<long> ParentStudentIds { get; set; }

        /// <summary>
        /// 聚合支付状态类型 <see cref="ETMS.Entity.Enum.EtmsManage.EmAgtPayType"/>
        /// </summary>
        public int AgtPayType { get; set; }

        protected virtual string DataFilterWhere2()
        {
            return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal} ";
        }

        protected virtual string DataFilterWhere(string studentFieldName = "StudentId")
        {
            if (ParentStudentIds.Count == 0)
            {
                return "1=2";
            }
            if (ParentStudentIds.Count == 1)
            {
                return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal} AND {studentFieldName} = {ParentStudentIds[0]}";
            }
            else
            {
                return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal} AND {studentFieldName} IN ({string.Join(',', ParentStudentIds)})";
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
