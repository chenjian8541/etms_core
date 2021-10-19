using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Common
{
    public class Img
    {
        public Img() { }

        public Img(string k, string l)
        {
            this.Key = k;
            this.Url = l;
        }

        public string Key { get; set; }

        public string Url { get; set; }
    }
}
