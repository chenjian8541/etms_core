using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class TenantOtherConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmValidPhoneType"/>
        /// </summary>
        public byte ValidPhoneType { get; set; }
    }
}
