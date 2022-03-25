using ETMS.Entity.Config;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ETMS.Business.Common
{
    /// <summary>
    /// Jwt帮助类
    /// </summary>
    internal class JwtHelper
    {
        /// <summary>
        /// 创建一个访问Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expiresTime"></param>
        /// 
        /// <returns></returns>
        internal static string GenerateToken(int tenantId, long userId, string nowTimestamp, out DateTime expiresTime)
        {
            var tokenValue = $"{tenantId},{userId},{nowTimestamp}";
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

        /// <summary>
        /// alien创建一个访问Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expiresTime"></param>
        /// 
        /// <returns></returns>
        internal static string AlienGenerateToken(int headId, long userId, string nowTimestamp, out DateTime expiresTime)
        {
            var tokenValue = $"etmsAlien,{headId},{userId},{nowTimestamp}";
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
