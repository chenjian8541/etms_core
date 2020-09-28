using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Utility
{
    /// <summary>
    /// 随机数
    /// </summary>
    public class RandomHelper
    {
        /// <summary>
        /// 获取一个短信验证码
        /// </summary>
        /// <returns></returns>
        public static string GetSmsCode()
        {
            var rNum = new Random();
            return $"{rNum.Next(0, 9)}{rNum.Next(0, 9)}{rNum.Next(0, 9)}{rNum.Next(0, 9)}";
        }
    }
}
