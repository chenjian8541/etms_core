using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business.Common
{
    public class ParentLib
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
        /// 从第四位开始才有效
        /// </summary>
        /// <param name="strEncrypt"></param>
        /// <returns></returns>
        public static int GetTenantDecrypt(string strEncrypt)
        {
            strEncrypt = strEncrypt.Substring(4);
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            return strCode.ToInt();
        }
    }
}
