using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryContractGetPagingRequest : RequestPagingBase
    {
        public long? TeacherId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (TeacherId != null)
            {
                condition.Append($" AND Id = {TeacherId.Value}");
            }
            return condition.ToString();
        }
    }
}