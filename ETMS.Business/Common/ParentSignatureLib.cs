using ETMS.Entity.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ETMS.Utility;

namespace ETMS.Business.Common
{
    public class ParentSignatureLib
    {
        /// <summary>
        /// 学员端登录生成签名信息
        /// </summary>
        /// <param name="parentLoginInfo"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetSignature(ParentTokenConfig parentLoginInfo)
        {
            var strLoginInfo = $"{parentLoginInfo.TenantId}_{parentLoginInfo.Phone}_{parentLoginInfo.ExTimestamp}";
            var strSignature = GetSignature(strLoginInfo);
            return Tuple.Create(strLoginInfo, strSignature);
        }

        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <param name="strLoginInfo"></param>
        /// <returns></returns>
        public static ParentTokenConfig GetParentLoginInfo(string strLoginInfo)
        {
            var infos = strLoginInfo.Split('_');
            return new ParentTokenConfig()
            {
                TenantId = infos[0].ToInt(),
                Phone = infos[1],
                ExTimestamp = infos[2]
            };
        }

        /// <summary>
        /// 获取open2的登录签名
        /// </summary>
        /// <param name="miniPgmUserId"></param>
        /// <returns></returns>
        public static string GetOpenParent2Signature(long miniPgmUserId)
        {
            return GetSignature(miniPgmUserId.ToString());
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="miniPgmUserId"></param>
        /// <param name="strSignature"></param>
        /// <returns></returns>
        public static bool CheckOpenParent2Signature(long miniPgmUserId, string strSignature)
        {
            return GetSignature(miniPgmUserId.ToString()).Equals(strSignature);
        }

        /// <summary>
        /// 生成签名信息
        /// </summary>
        /// <param name="strLoginInfo"></param>
        /// <returns></returns>
        private static string GetSignature(string strLoginInfo)
        {
            var sortStr = string.Concat($"{SystemConfig.ParentAccessConfig.SignatureSecret}{strLoginInfo}".OrderBy(c => c));
            using (var hash = System.Security.Cryptography.MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(sortStr);
                var md5Val = hash.ComputeHash(bytes);
                return Convert.ToBase64String(md5Val);
            }
        }

        /// <summary>
        /// 验证学员端签名
        /// </summary>
        /// <param name="strLoginInfo"></param>
        /// <param name="strSignature"></param>
        /// <returns></returns>
        public static bool CheckSignature(string strLoginInfo, string strSignature)
        {
            return GetSignature(strLoginInfo).Equals(strSignature);
        }
    }
}
