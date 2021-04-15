using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity
{
    public class EtmsErrorException : Exception
    {
        public EtmsErrorException(string message) : base(message)
        { }
    }
}
