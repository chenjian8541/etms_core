using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassRecordStudentChangeRequest : RequestBase
    {
        public long ClassRecordStudentId { get; set; }

        /// <summary>
        /// 到课状态 <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte NewStudentCheckStatus { get; set; }

        /// <summary>
        /// 扣课时
        /// </summary>
        public decimal NewDeClassTimes { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string NewRemark { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int NewRewardPoints { get; set; } 

        public override string Validate()
        {
            if (ClassRecordStudentId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}
