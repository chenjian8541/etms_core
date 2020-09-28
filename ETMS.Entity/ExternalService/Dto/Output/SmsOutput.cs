using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.ExternalService.Dto.Output
{
    public class SmsOutput
    {
        public bool IsSuccess { get; set; }

        public string Msg { get; set; }

        public static SmsOutput Success(string msg = "")
        {
            return new SmsOutput()
            {
                IsSuccess = true,
                Msg = msg
            };
        }

        public static SmsOutput Fail(string msg = "")
        {
            return new SmsOutput()
            {
                IsSuccess = false,
                Msg = msg
            };
        }
    }
}
