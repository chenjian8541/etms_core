using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryUserPerformanceDetailGetPagingRequest : RequestPagingBase
    {
        public long? UserPerformanceId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (UserPerformanceId != null)
            {
                condition.Append($" AND UserPerformanceId = {UserPerformanceId.Value}");
            }
            return condition.ToString();
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            //if (UserPerformanceId == null)
            //{
            //    return "请求数据格式错误";
            //}
            return base.Validate();
        }
    }
}
