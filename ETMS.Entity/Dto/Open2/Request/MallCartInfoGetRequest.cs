using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class MallCartInfoGetRequest : Open2Base
    {
        public string CId { get; set; }

        public long Id { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(CId))
            {
                return "请求数据格式错误";
            }
            Id = EtmsHelper2.GetIdDecrypt2(CId);
            if (Id <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}