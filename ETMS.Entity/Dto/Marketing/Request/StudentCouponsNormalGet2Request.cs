using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class StudentCouponsNormalGet2Request : RequestPagingBase
    {
        public long StudentId { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            return base.Validate();
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND StudentId = {StudentId} AND [Status] = {EmCouponsStudentStatus.Unused} AND (ExpiredTime IS NULL OR ExpiredTime >= '{DateTime.Now.Date}')");
            return condition.ToString();
        }
    }
}

