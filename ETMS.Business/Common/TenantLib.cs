using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business.Common
{
    public class TenantLib
    {
        /// <summary>
        /// 加密tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static string GetTenantEncrypt(int tenantId)
        {
            var strEncrypt = $"8104{tenantId}";
            var bytes = Encoding.UTF8.GetBytes(strEncrypt);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 解密TenantNo
        /// 从第四位开始才有效
        /// </summary>
        /// <param name="strEncrypt"></param>
        /// <returns></returns>
        public static int GetTenantDecrypt(string strEncrypt)
        {
            if (strEncrypt.Equals("000"))
            {
                return 0;
            }
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            strCode = strCode.Substring(4);
            return strCode.ToInt();
        }
    }
}
