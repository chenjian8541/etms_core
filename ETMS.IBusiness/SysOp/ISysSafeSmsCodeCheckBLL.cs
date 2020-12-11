using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.SysOp
{
    public interface ISysSafeSmsCodeCheckBLL
    {
        Task<ResponseBase> SysSafeSmsCodeSend(int tenantId,string phone);

        ResponseBase SysSafeSmsCodeCheck(int tenantId, string smsCode);
      }
}
