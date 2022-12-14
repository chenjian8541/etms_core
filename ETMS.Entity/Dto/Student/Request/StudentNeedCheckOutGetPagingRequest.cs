using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentNeedCheckOutGetPagingRequest : RequestPagingBase
    {
        /// <summary>
        /// 学员ID
        /// </summary>
        public long? StudentId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND IsCheckIn = {EmBool.True} AND IsCheckOut = {EmBool.False} AND Ot = '{DateTime.Now.EtmsToDateString()}' ");
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId.Value}");
            }
            return condition.ToString();
        }
    }
}
