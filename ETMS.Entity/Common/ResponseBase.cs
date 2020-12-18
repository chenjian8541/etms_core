using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    /// <summary>
    /// 响应基类
    /// </summary>
    public class ResponseBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResponseBase()
        {
            this.code = StatusCode.Succeed;
            this.message = string.Empty;
        }

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="returnMsg"></param>
        public ResponseBase(string returnCode, string returnMsg)
        {
            this.code = returnCode;
            this.message = returnMsg;
        }

        /// <summary>
        /// 返回状态码
        /// </summary>
        /// <see cref="TTY.GMP.Entity.Common.StatusCode"/>
        public string code { get; set; }

        /// <summary>
        /// 返回信息，如非空，为错误原因
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 返回的数据
        /// </summary>
        public object resultData { get; set; }

        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="entity">成功返回的数据信息</param>
        /// <returns></returns>
        public static ResponseBase Success(object entity = null)
        {
            var response = new ResponseBase();
            response.code = StatusCode.Succeed;
            response.resultData = entity;
            return response;
        }

        /// <summary>
        /// 代码异常错误
        /// </summary>
        /// <returns></returns>
        public static ResponseBase UnKnownError()
        {
            var response = new ResponseBase();
            response.code = StatusCode.CommonError;
            response.message = "系统开小差请稍后再试";
            return response;
        }

        public static ResponseBase CommonError(string msg)
        {
            var response = new ResponseBase();
            response.code = StatusCode.CommonError;
            response.message = msg;
            return response;
        }

        public static ResponseBase BusyError()
        {
            var response = new ResponseBase();
            response.code = StatusCode.CommonError;
            response.message = "正在处理，请稍后再试";
            return response;
        }

        public static ResponseBase ParentTokenExTime()
        {
            var response = new ResponseBase();
            response.code = StatusCode.Forbidden;
            response.message = "凭证已过期";
            return response;
        }
    }
}
