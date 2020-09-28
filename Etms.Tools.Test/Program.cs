using ETMS.Entity.Config;
using ETMS.Utility;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Etms.Tools.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var s = Console.ReadLine();
                var isPhone = IsMobilePhone(s);
                Console.WriteLine(isPhone);
            }
            Console.Read();
        }

        /// <summary>
        /// 判断输入的字符串是否是一个合法的手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string input)
        {
            var regex = new Regex("^1[34578]\\d{9}$");
            return regex.IsMatch(input);

        }

        private static void Sms()
        {
            var name = "XIONGMANhy";
            var pwd = "YDAJGWIR";
            var key = EtmsGetTimestamp(DateTime.UtcNow);
            var password = md5($"{md5(pwd)}{key}");
        }

        private static void Encrypt3DES()
        {
            var conStr = Console.ReadLine();
            Console.WriteLine(conStr);
            using (var con = new SqlConnection(conStr))
            {
                con.Open();
                Console.WriteLine("数据库打开成功");
            }
            var res = CryptogramHelper.Encrypt3DES(conStr, SystemConfig.CryptogramConfig.Key);
            Console.WriteLine(res);
        }

        public static long EtmsGetTimestamp(DateTime myTime)
        {
            var ts = myTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalSeconds;
        }

        public static string md5(string str)
        {
            //创建MD5对象
            MD5 md5 = MD5.Create();
            //开始加密
            //将字符串转换为字节数组
            byte[] buffer = Encoding.Default.GetBytes(str);
            //返回一个完成加密的字节数组
            byte[] MD5buffer = md5.ComputeHash(buffer);
            //将字节数组每个元素Tostring()
            string newstr = "";
            for (int i = 0; i < MD5buffer.Length; i++)
            {
                //同时10进制转换为16进制
                newstr += MD5buffer[i].ToString("x2");
            }
            return newstr;

        }
    }
}
