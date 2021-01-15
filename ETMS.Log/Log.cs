using System;
using log4net;
using log4net.Config;
using log4net.Repository;
using Newtonsoft.Json;
using System.IO;

namespace ETMS.LOG
{
    /// <summary>
    /// 日志操作
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 日志库
        /// </summary>
        private static readonly ILoggerRepository repository;

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Error(string message, Type type)
        {
            var log = LogManager.GetLogger(repository.Name, type);
            log.Error(message);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Error<T>(string msg, T request, Type type)
        {
            var log = LogManager.GetLogger(repository.Name, type);
            var message = string.Format("{0}:{1}", msg, JsonConvert.SerializeObject(request));
            log.Error(message);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="type"></param>
        public static void Error(string message, Exception exception, Type type)
        {
            var log = LogManager.GetLogger(repository.Name, type);
            log.Error(message, exception);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="type"></param>
        public static void Error<T>(T request, Exception exception, Type type) where T : class
        {
            var message = string.Format("请求参数:{0}", JsonConvert.SerializeObject(request));
            Error(message, exception, type);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="msg"></param>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="type"></param>
        public static void Error<T>(string msg, T request, Exception exception, Type type) where T : class
        {
            var message = string.Format("{0}:{1}", msg, JsonConvert.SerializeObject(request));
            Error(message, exception, type);
        }

        /// <summary>
        /// 记录一般信息日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Info(string message, Type type)
        {
            var log = LogManager.GetLogger(repository.Name, type);
            log.Info(message);
        }

        /// <summary>
        /// Debug信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Debug(string message, Type type)
        {
            var log = LogManager.GetLogger(repository.Name, type);
            log.Debug(message);
        }

        /// <summary>
        /// Debug信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        public static void Debug(object obj, Type type)
        {
            var message = string.Format("请求参数:{0}", JsonConvert.SerializeObject(obj));
            Debug(message, type);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Warn(string message, Type type)
        {
            var log = LogManager.GetLogger(repository.Name, type);
            log.Warn(message);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="type"></param>
        public static void Warn<T>(string msg, T request, Type type) where T : class
        {
            var message = string.Format("{0}请求参数:{1}", msg, JsonConvert.SerializeObject(request));
            Warn(message, type);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Fatal(string message, Type type)
        {
            var log = LogManager.GetLogger(repository.Name, type);
            log.Fatal(message);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="type"></param>
        public static void Fatal(string message, Exception exception, Type type)
        {
            var log = LogManager.GetLogger(repository.Name, type);
            log.Fatal(message, exception);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="exception"></param>
        /// <param name="type"></param>
        public static void Fatal<T>(T request, Exception exception, Type type) where T : class
        {
            var message = string.Format("请求参数:{0}", JsonConvert.SerializeObject(request));
            Fatal(message, exception, type);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="request"></param>
        /// <param name="type"></param>
        public static void Fatal<T>(string msg, T request, Type type) where T : class
        {
            var message = string.Format("{0}:{1}", msg, JsonConvert.SerializeObject(request));
            Fatal(message, type);
        }

        /// <summary>
        /// 静态构造函数
        /// 初始化log4net
        /// </summary>
        static Log()
        {
            repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo(Path.Combine(AppContext.BaseDirectory, "logger.config")));
        }
    }
}
