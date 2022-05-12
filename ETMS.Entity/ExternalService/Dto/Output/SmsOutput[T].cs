using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.ExternalService.Dto.Output
{
    public class SmsOutput<T>
    {
        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

        public T ResultData { get; set; }

        public static SmsOutput<T> Success(T data)
        {
            return new SmsOutput<T>()
            {
                IsSuccess = true,
                Msg = string.Empty,
                ResultData = data
            };
        }

        public static SmsOutput<T> Fail(string msg = "")
        {
            return new SmsOutput<T>()
            {
                IsSuccess = false,
                Msg = msg
            };
        }
    }
}
