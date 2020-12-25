using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentNeedAttendClassGetPagingRequest : RequestPagingBase
    {
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND [Status] = {EmTempStudentNeedCheckClassStatus.NotAttendClass} AND Ot = '{DateTime.Now.EtmsToDateString()}' ");
            return condition.ToString();
        }
    }
}
