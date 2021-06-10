using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class GetAllStudentPagingRequest : RequestPagingBase
    {
        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public int? StudentType { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StudentType != null)
            {
                condition.Append($" AND StudentType = {StudentType.Value}");
            }
            return condition.ToString();
        }
    }
}
