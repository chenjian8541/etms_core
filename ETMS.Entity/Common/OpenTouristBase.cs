using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Common
{
    public abstract class OpenTouristBase : IValidate
    {
        /// <summary>
        /// 客户端类型  <see cref="ETMS.Entity.Enum.EmUserOperationLogClientType"/>
        /// </summary>
        public int ClientType { get; set; }

        public virtual string Validate()
        {
            return String.Empty;
        }
    }
}
