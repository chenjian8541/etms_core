using ETMS.Entity.View;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business.Common
{
    public class TenantLib
    {
        public static string GetTenantLoginUrl(int tenantId, string mainLoginParms)
        {
            var enStr = GetTenantEncrypt(tenantId);
            return string.Format(mainLoginParms, enStr);
        }

        /// <summary>
        /// 加密tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static string GetTenantEncrypt(int tenantId)
        {
            return EtmsHelper2.GetTenantEncrypt(tenantId);
        }

        /// <summary>
        /// 解密TenantNo
        /// 从第四位开始才有效
        /// </summary>
        /// <param name="strEncrypt"></param>
        /// <returns></returns>
        public static int GetTenantDecrypt(string strEncrypt)
        {
            return EtmsHelper2.GetTenantDecrypt(strEncrypt);
        }

        public static string GetPhoneEncrypt(string phone)
        {
            return EtmsHelper2.GetPhoneEncrypt(phone);
        }

        public static string GetPhoneDecrypt(string strEncrypt)
        {
            return EtmsHelper2.GetPhoneDecrypt(strEncrypt);
        }

        public static string GetTenantEntranceEncrypt(int tenantId, long userId, long nowTimestamp)
        {
            var str = $"8104_{tenantId}_{userId}_{nowTimestamp}";
            return EtmsHelper2.GetUrlEncrypt(str);
        }

        public static TenantEntrancePCView GetTenantEntranceDecrypt(string strEncrypt)
        {
            var str = EtmsHelper2.GetUrlDecrypt(strEncrypt);
            var values = str.Split('_');
            return new TenantEntrancePCView()
            {
                TenantId = values[1].ToInt(),
                UserId = values[2].ToLong(),
                NowTimestamp = values[3].ToLong()
            };
        }
    }
}
