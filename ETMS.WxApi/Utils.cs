using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tencent;
using WxApi.MsgEntity;
using WxApi.ReceiveEntity;
using WxApi.SendEntity;

namespace WxApi
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <returns>响应信息</returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";//设置请求的方法
            request.Accept = "*/*";//设置Accept标头的值
            string responseStr = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())//获取响应
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();
                }
            }
            return responseStr;
        }
        public static string HttpPost(string url, Stream stream)
        {
            //当请求为https时，验证服务器证书
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            string responseStr = "";
            using (var reqstream = request.GetRequestStream())
            {
                stream.Position = 0L;
                stream.CopyTo(reqstream);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = reader.ReadToEnd();//获取响应
                }
            }
            return responseStr;
        }
        /// <summary>
        /// HTTP POST请求URL。
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="param">请求的参数</param>
        /// <param name="stream">如果响应的是文件，则此参数表示的是文件流</param>
        public static string HttpPost(string url, string param, out FileStreamInfo stream)
        {
            stream = null;
            //当请求为https时，验证服务器证书
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            string responseStr = "";
            using (StreamWriter requestStream = new StreamWriter(request.GetRequestStream()))
            {
                requestStream.Write(param);//将请求的数据写入到请求流中
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {

                if (response.ContentType == "application/octet-stream")//如果响应的是文件流，则不将响应流转换成字符串
                {
                    stream = new FileStreamInfo();
                    response.GetResponseStream().CopyTo(stream);
                    #region 获取响应的文件名
                    Regex reg = new Regex(@"(\w+)\.(\w+)");
                    var result = reg.Match(response.GetResponseHeader("Content-disposition")).Groups;
                    stream.FileName = result[0].Value;
                    #endregion
                    responseStr = "";
                }
                else
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();//获取响应
                    }
                }

            }
            return responseStr;
        }
        /// <summary>
        /// 带验证证书的POST请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="data">请求数据</param>
        /// <param name="certpath">证书路径</param>
        /// <param name="certpwd">证书密码</param>
        public static string HttpPost(string url, string data, string certpath = "", string certpwd = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //当请求为https时，验证服务器证书
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) =>
            {
                if (d == SslPolicyErrors.None)
                    return true;
                return false;
            });
            if (!string.IsNullOrEmpty(certpath) && !string.IsNullOrEmpty(certpwd))
            {
                X509Certificate2 cer = new X509Certificate2(certpath, certpwd,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                request.ClientCertificates.Add(cer);
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            string responseStr = "";
            using (StreamWriter requestStream = new StreamWriter(request.GetRequestStream()))
            {
                requestStream.Write(data);//将请求的数据写入到请求流中
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();//获取响应
                        Utils.WriteTxt("/debug.txt", responseStr);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return responseStr;
        }
        public static string HttpPostForm(string url, List<FormEntity> formEntities)
        {
            //分割字符串
            var boundary = "----" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            MemoryStream stream = new MemoryStream();
            #region 将非文件表单写入到内存流中
            foreach (var entity in formEntities.Where(f => f.IsFile == false))
            {
                var temp = new StringBuilder();
                temp.AppendFormat("\r\n--{0}", boundary);
                temp.AppendFormat("\r\nContent-Disposition: form-data; name=\"{0}\"", entity.Name);
                temp.Append("\r\n\r\n");
                temp.Append(entity.Value);
                byte[] b = Encoding.UTF8.GetBytes(temp.ToString());
                stream.Write(b, 0, b.Length);
            }
            #endregion
            #region 将文件表单写入到内存流
            foreach (var entity in formEntities.Where(f => f.IsFile == true))
            {
                byte[] filedata = null;
                var filename = Path.GetFileName(entity.Value);
                //表示是网络资源
                if (entity.Value.Contains("http"))
                {
                    //处理网络文件
                    using (var client = new WebClient())
                    {
                        filedata = client.DownloadData(entity.Value);
                    }
                }
                else
                {
                    //处理物理路径文件
                    using (FileStream file = new FileStream(entity.Value, FileMode.Open, FileAccess.Read))
                    {

                        filedata = new byte[file.Length];
                        file.Read(filedata, 0, (int)file.Length);
                    }
                }
                var temp = string.Format("\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\n\r\n",
                   boundary, entity.Name, filename);
                byte[] b = Encoding.UTF8.GetBytes(temp);
                stream.Write(b, 0, b.Length);
                stream.Write(filedata, 0, filedata.Length);
            }
            #endregion
            //结束标记
            byte[] foot_data = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            stream.Write(foot_data, 0, foot_data.Length);
            Stream reqStream = request.GetRequestStream();
            stream.Position = 0L;
            //将Form表单生成流写入到请求流中
            stream.CopyTo(reqStream);
            stream.Close();
            reqStream.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();//获取响应
                }
            }
        }

        public static string HttpPostForm2(string url, byte[] filedata, string name, string filename)
        {
            //分割字符串
            var boundary = "----" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            MemoryStream stream = new MemoryStream();

            var temp = string.Format("\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\n\r\n",
               boundary, name, filename);
            byte[] j = Encoding.UTF8.GetBytes(temp);
            stream.Write(j, 0, j.Length);
            stream.Write(filedata, 0, filedata.Length);
            //结束标记
            byte[] foot_data = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            stream.Write(foot_data, 0, foot_data.Length);
            Stream reqStream = request.GetRequestStream();
            stream.Position = 0L;
            //将Form表单生成流写入到请求流中
            stream.CopyTo(reqStream);
            stream.Close();
            reqStream.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();//获取响应
                }
            }
        }

        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public static void WriteTxt(string path, string txt)
        {
            return;
        }

        /// <summary>
        /// 将微信xml数据转换成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlstr"></param>
        /// <returns></returns>
        public static T ConvertObj<T>(string xmlstr)
        {
            try
            {
                XElement xdoc = XElement.Parse(xmlstr);
                //获取转换的数据类型
                var type = typeof(T);
                //创建实例
                var t = Activator.CreateInstance<T>();
                #region 基础属性赋值
                var ToUserName = type.GetProperty("ToUserName");
                ToUserName.SetValue(t, Convert.ChangeType(xdoc.Element("ToUserName").Value, ToUserName.PropertyType), null);
                xdoc.Element("ToUserName").Remove();

                var FromUserName = type.GetProperty("FromUserName");
                FromUserName.SetValue(t, Convert.ChangeType(xdoc.Element("FromUserName").Value, FromUserName.PropertyType), null);
                xdoc.Element("FromUserName").Remove();

                var CreateTime = type.GetProperty("CreateTime");
                CreateTime.SetValue(t, Convert.ChangeType(xdoc.Element("CreateTime").Value, CreateTime.PropertyType), null);
                xdoc.Element("CreateTime").Remove();

                var MsgType = type.GetProperty("MsgType");
                string msgtype = xdoc.Element("MsgType").Value.ToUpper();
                MsgType.SetValue(t, (MsgType)Enum.Parse(typeof(MsgType), msgtype), null);
                xdoc.Element("MsgType").Remove();

                //判断消息类型是否是事件
                if (msgtype == "EVENT")
                {
                    //获取事件类型
                    var EventType = type.GetProperty("Event");
                    string eventtype = xdoc.Element("Event").Value.ToUpper();
                    EventType.SetValue(t, (EventType)Enum.Parse(typeof(EventType), eventtype), null);
                    xdoc.Element("Event").Remove();
                }
                #endregion


                //遍历XML节点
                foreach (XElement element in xdoc.Elements())
                {
                    //根据xml节点的名称，获取实体的属性

                    if (msgtype == "EVENT")
                    {
                        if (element.Name == "ScanCodeInfo")
                        {
                            type.GetProperty("ScanType").SetValue(t, Convert.ChangeType(element.Element("ScanType").Value, TypeCode.String), null);
                            type.GetProperty("ScanResult").SetValue(t, Convert.ChangeType(element.Element("ScanResult").Value, TypeCode.String), null);
                            continue;
                        }
                        if (element.Name == "SendPicsInfo")
                        {
                            type.GetProperty("Count").SetValue(t, Convert.ChangeType(element.Element("Count").Value, TypeCode.Int32), null);
                            List<string> picMd5List = new List<string>();
                            foreach (XElement xElement in element.Element("PicList").Elements())
                            {
                                picMd5List.Add(xElement.Element("PicMd5Sum").Value);
                            }
                            type.GetProperty("PicMd5SumList").SetValue(t, picMd5List, null);
                            continue;
                        }
                    }
                    var pr = type.GetProperty(element.Name.ToString());
                    //给属性赋值
                    pr.SetValue(t, Convert.ChangeType(element.Value, pr.PropertyType), null);
                }
                return t;
            }
            catch (Exception)
            {
                return default(T);
            }
        }


        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        /// <summary>
        /// 发起post请求，并获取请求返回值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="obj">数据实体</param>
        /// <param name="url">接口地址</param>
        public static T PostResult<T>(List<FormEntity> formEntities, string url)
        {
            var retdata = HttpPostForm(url, formEntities);
            return JsonConvert.DeserializeObject<T>(retdata);
        }

        /// <summary>
        /// 发起post请求，并获取请求返回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="b"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T PostResult<T>(string url, byte[] b, string name, string filename)
        {
            var retdata = HttpPostForm2(url, b, name, filename);
            return JsonConvert.DeserializeObject<T>(retdata);
        }

        /// <summary>
        /// 发起post请求，并获取请求返回值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="obj">数据实体</param>
        /// <param name="url">接口地址</param>
        public static T PostResult<T>(object obj, string url)
        {
            //序列化设置
            var setting = new JsonSerializerSettings();


            //解决枚举类型序列化时，被转换成数字的问题
            setting.Converters.Add(new StringEnumConverter());
            var retdata = HttpPost(url, JsonConvert.SerializeObject(obj, setting));
            return JsonConvert.DeserializeObject<T>(retdata);
        }
        /// <summary>
        /// 以流的方式发起post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T PostResult<T>(Stream stream, string url)
        {
            var retdata = HttpPost(url, stream);
            return JsonConvert.DeserializeObject<T>(retdata);
        }
        /// <summary>
        /// 发起Get请求，并获取请求返回值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">接口地址</param>
        public static T GetResult<T>(string url)
        {
            var retdata = HttpGet(url);
            return JsonConvert.DeserializeObject<T>(retdata);
        }
    }
}
