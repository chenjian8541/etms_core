using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent.Output
{
    public class ParentLoginBySmsOpenOutput
    {
        /// <summary>
        /// <see cref="ParentLoginBySmsOutputLoginStatus"/>
        /// </summary>
        public int LoginStatus { get; set; }

        public string S { get; set; }

        public string L { get; set; }

        public string ExpiresIn { get; set; }
    }

    public struct ParentLoginBySmsOutputLoginStatus
    {
        public const byte Success = 0;

        public const byte Unregistered = 1;
    }
}
