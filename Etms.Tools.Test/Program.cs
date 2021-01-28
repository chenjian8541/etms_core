﻿using ETMS.Business.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.Utility;
using System;
using System.Collections.Generic;
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
            //CreateParentToken();
            //CreateTencentCloudAccount();
            //Encrypt3DESSqlConnection();
            //Decrypt3DESSqlConnection();
            Console.WriteLine(TenantLib.GetTenantEncrypt(207));
            Console.WriteLine(TenantLib.GetTenantDecrypt("ODEwNDIwNw"));
            Console.WriteLine();
            Console.Read();
        }

        public static void CreateTencentCloudAccount()
        {
            var data = new List<TencentCloudAccountView>() {
              new TencentCloudAccountView(){
                   TencentCloudId= 0,
                   SecretId = "AKIDuVreBCHGSQ9eT8ucplStNdkurcGToXEk",
                   SecretKey = "Xw4ze3H9zax3GDuPedMmytECvc3E9jiM",
                   Endpoint = "iai.tencentcloudapi.com",
                   Region = "ap-beijing"
              }
            };
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Console.WriteLine(s);
        }

        public static string GetTimeDuration(int startTime, int endTime)
        {
            var startDate = new DateTime(2020, 01, 01, startTime / 100, startTime % 100, 0);
            var endDate = new DateTime(2020, 01, 01, endTime / 100, endTime % 100, 0);
            var times = endDate - startDate;
            var s = times.Hours + times.Minutes / 60.0;
            return s.ToString("F2");

            var de = endTime - startTime;
            var hour = de / 60;
            var minute = (de % 60) / 60.0;
            if (minute == 0)
            {
                return hour.ToString();
            }
            else
            {
                return (hour + minute).ToString("F2");
            }
        }

        public static void CreateParentToken()
        {
            var exTime = DateTime.Now.Date.AddDays(7).EtmsGetTimestamp().ToString();
            var parentTokenConfig = new ParentTokenConfig()
            {
                ExTimestamp = exTime,
                Phone = "13410271814",
                TenantId = 1
            };
            var signatureInfo = ParentSignatureLib.GetSignature(parentTokenConfig);
            Console.WriteLine(signatureInfo.Item1);
            Console.WriteLine(signatureInfo.Item2);
            while (true)
            {
                var s = Console.ReadLine();
                var isPhone = IsMobilePhone(s);
                Console.WriteLine(isPhone);
            }
        }

        public static void CreateSysTenantWechartAuth()
        {
            var entity = new SysTenantWechartAuth()
            {
                Id = 0,
                AuthorizerAppid = "wxdb620f6794fdcf83",
                AuthorizeState = EmSysTenantWechartAuthAuthorizeState.Authorized,
                NickName = "小禾帮",
                HeadImg = "http://wx.qlogo.cn/mmopen/BggnTAMTGIxa5EQnRkIlvr51AcIGICQ1gic5znVonymACBt7DU4WQnxTuxmpmeQLIF8hPYTKUlvF4qmsNbW12NRH9EKmUMbqB/0",
                ServiceTypeInfo = EmWechartAuthServiceTypeInfo.ServiceType2,
                VerifyTypeInfo = EmWechartAuthVerifyTypeInfo.VerifyType0,
                UserName = "gh_52de00cc566d",
                PrincipalName = string.Empty,
                Alias = string.Empty,
                BusinessInfo = string.Empty,
                QrcodeUrl = "http://mmbiz.qpic.cn/mmbiz_jpg/iahOPb43Solz0fKUrvDNXEzucK0vkBtJbhJaiaCvbPL1wO4g7ibFkO6r6XiafkrUicK8FEvSuK5J3sqib1U8GtbFUg5w/0",
                PermissionsKey = "1,15,4,7,2,11,6,8,13,9,10,24,54",
                PermissionsValue = "消息管理权限,自定义菜单权限,网页服务权限,群发与通知权限,用户管理权限,素材管理权限,微信多客服权限,微信卡券权限,微信门店权限,微信扫一扫权限,微信连WIFI权限,开放平台帐号管理权限,54",
                CreateOt = DateTime.Now,
                IsDeleted = 0,
                ModifyOt = DateTime.Now,
                Remark = "默认数据",
                TenantId = 0
            };
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
            Console.WriteLine(s);
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

        private static void Decrypt3DESSqlConnection()
        {
            var conStr = Console.ReadLine();
            var res = CryptogramHelper.Decrypt3DES(conStr, SystemConfig.CryptogramConfig.Key);
            Console.WriteLine(res);
        }

        private static void Encrypt3DESSqlConnection()
        {
            var conStr = Console.ReadLine();
            Console.WriteLine(conStr);
            //try
            //{
            //    using (var con = new SqlConnection(conStr))
            //    {
            //        con.Open();
            //        Console.WriteLine("数据库打开成功");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
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
