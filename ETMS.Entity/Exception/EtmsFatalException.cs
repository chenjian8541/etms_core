using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity
{
    public class EtmsFatalException : Exception
    {
        public EtmsFatalException(string message) : base(message)
        { }
    }
}
