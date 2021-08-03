using ETMS.Business.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Etms.Tools.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateParentToken("13410271814");
            //CreateTencentCloudAccount();
            //Encrypt3DESSqlConnection();
            //Decrypt3DESSqlConnection();
            //Console.WriteLine(TenantLib.GetTenantEncrypt(207));
            //Console.WriteLine(TenantLib.GetTenantDecrypt("ODEwNDIwNw"));

            //var myDate = EtmsHelper.GetWeekStartEndDate(DateTime.Now.AddDays(5));
            //CreateParentToken();
            //Console.WriteLine(DateTime.MinValue.IsEffectiveDate());
            //Console.WriteLine(DateTime.MaxValue.IsEffectiveDate());
            //var date = Convert.ToDateTime("2021-10-1");
            //Console.WriteLine(date.IsEffectiveDate());
            //int? a = null;

            //var now = DateTime.Now;
            //var startTime = 1230;
            //var newNow = new DateTime(now.Year, now.Month, now.Day, startTime / 100, startTime % 100,0);

            //Console.WriteLine(newNow);

            //var now = DateTime.Now.Date.AddDays(-2);
            //Console.WriteLine(EtmsHelper2.GetThisWeek(now));
            //Console.WriteLine(EtmsHelper2.GetThisMonth(now));
            //Console.WriteLine(EtmsHelper2.GetLastWeek(now));
            //Console.WriteLine(EtmsHelper2.GetLastMonth(now));
            //AliyunOssSTS2();
            //var account = new Test()
            //{
            //    Money = 300
            //};
            //Console.WriteLine(JsonConvert.SerializeObject(account));
            //var now = DateTime.Now.Date;
            //var startTime = new DateTime(now.Year, now.Month, 1);
            //var endTime = startTime.AddMonths(1).AddDays(-1);
            //Console.WriteLine(startTime);
            //Console.WriteLine(endTime);
            //var process = new EtmsProcess();
            //var a = 1;
            //while (a < 100)
            //{
            // process.ProcessRole();
            // }

            //var mystudents = new List<MyStudent>();
            //mystudents.Add(new MyStudent() { Id = 1, Name = "1" });
            //mystudents.Add(new MyStudent() { Id = 2, Name = "2" });
            //mystudents.Add(new MyStudent() { Id = 3, Name = "3" });
            //mystudents.Add(new MyStudent() { Id = 4, Name = "4" });

            //var this3 = mystudents.FirstOrDefault(p => p.Id == 3);
            //this3.Name = "李小白";

            //Console.WriteLine(mystudents.First(p => p.Id == 3).Name);

            //var time = DateTime.Now;
            //var firstDate = new DateTime(time.Year, time.Month, 1);
            //var startTimeDesc = firstDate.EtmsToDateString();
            //var endTimeDesc = firstDate.AddMonths(1).EtmsToDateString();
            //Console.WriteLine(startTimeDesc);
            //Console.WriteLine(endTimeDesc);

            //var year = 2021;
            //var startTime = new DateTime(year, 1, 1);
            //var endTime = startTime.AddYears(1).AddDays(-1);
            //var time = DateTime.Now;
            //var startTimeDesc = time.Date.EtmsToDateString();
            //var endTimeDesc = time.AddDays(1).Date.EtmsToDateString();
            //Console.WriteLine(startTimeDesc);
            //Console.WriteLine(endTimeDesc);

            new EtmsProcess().ProcessT();

            //var s = EtmsHelper2.GetTenantEncryptOpenApi99(5402);


            Console.WriteLine("已完成");
            Console.Read();
        }

        public static void Go()
        {
            throw new Exception("发送错误了");
        }

        public static string GetTenantEncryptOpenApi99(int t)
        {
            return EtmsHelper2.GetTenantEncryptOpenApi99(t);
        }

        public static int GetTenantDecryptOpenApi99(string strNo)
        {
            return EtmsHelper2.GetTenantDecryptOpenApi99(strNo);
        }

        public static string GetPhoneEncrypt(string phone)
        {
            var strEncrypt = $"8104{phone}";
            var bytes = Encoding.UTF8.GetBytes(strEncrypt);
            return Convert.ToBase64String(bytes);
        }

        public static string GetPhoneDecrypt(string strEncrypt)
        {
            var bytes = Convert.FromBase64String(strEncrypt);
            var strCode = Encoding.UTF8.GetString(bytes);
            strCode = strCode.Substring(4);
            return strCode;
        }

        private static void GetPwd()
        {
            while (true)
            {
                var s = Console.ReadLine();
                Console.WriteLine(DbDecrypt3DES(s));
            }
        }

        private static string SerializeObjectDynamicData(dynamic postData)
        {
            return JsonConvert.SerializeObject(postData);
        }

        public static AlibabaCloud.SDK.Sts20150401.Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                AccessKeyId = "LTAI5tGMXgiia8tRJm8F6hd7",
                AccessKeySecret = "1a5DVo7YO7MEMWGulJcZSKAaYJkoeP",
            };
            config.Endpoint = "sts.cn-qingdao.aliyuncs.com";
            return new AlibabaCloud.SDK.Sts20150401.Client(config);
        }

        public static void AliyunOssSTS2()
        {
            AlibabaCloud.SDK.Sts20150401.Client client = CreateClient("accessKeyId", "accessKeySecret");
            AlibabaCloud.SDK.Sts20150401.Models.AssumeRoleRequest assumeRoleRequest = new AlibabaCloud.SDK.Sts20150401.Models.AssumeRoleRequest
            {
                DurationSeconds = 900,
                Policy = "{ \"Version\": \"1\", \"Statement\": [  {    \"Effect\": \"Allow\",        \"Action\": [          \"oss:PutObject\"        ],    \"Resource\": [      \"acs:oss:*:*:*\"    ]  } ] }",
                RoleArn = "acs:ram::1956403767422035:role/xiaohebang",
                RoleSessionName = "etms_xiaohebang",
            };
            var res = client.AssumeRole(assumeRoleRequest);
            Console.WriteLine(res);
        }

        public static string DbDecrypt3DES(string connectionString)
        {
            return CryptogramHelper.Decrypt3DES(connectionString, SystemConfig.CryptogramConfig.Key);
        }

        public static string DbEncrypt3DES(string connectionString)
        {
            return CryptogramHelper.Encrypt3DES(connectionString, SystemConfig.CryptogramConfig.Key);
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

        public static void CreateParentToken(string phone)
        {
            var exTime = DateTime.Now.Date.AddDays(7).EtmsGetTimestamp().ToString();
            var parentTokenConfig = new ParentTokenConfig()
            {
                ExTimestamp = exTime,
                Phone = phone,
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

    public class Test
    {
        public decimal Money { get; set; }
    }

    public class MyStudent
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
