using ETMS.Entity.Config;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ETMS.Business.EtmsManage.Common
{
    public class AgentJwtHelper
    {
        /// <summary>
        /// 创建一个访问Token
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name=""></param>
        /// <param name="expiresTime"></param>
        /// <returns></returns>
        internal static string GenerateToken(int agentId, out DateTime expiresTime)
        {
            var tokenValue = agentId.ToString();
            var claims = new[]
               {
                   new Claim(SystemConfig.AuthenticationConfig.ClaimType,tokenValue)
               };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SystemConfig.AuthenticationConfig.IssuerSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            expiresTime = DateTime.Now.AddDays(SystemConfig.AuthenticationConfig.ExpiresDay);
            var token = new JwtSecurityToken(
                issuer: SystemConfig.AuthenticationConfig.ValidIssuer,
                audience: SystemConfig.AuthenticationConfig.ValidAudience,
                claims: claims,
                expires: expiresTime,
                signingCredentials: creds);
            return $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}";
        }
    }
}
