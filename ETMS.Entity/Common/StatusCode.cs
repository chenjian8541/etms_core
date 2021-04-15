using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    /// <summary>
    /// 状态码
    /// </summary>
    public class StatusCode
    {
        /// <summary>
        /// 成功标识
        /// </summary>
        public const string Succeed = "10000";

        /// <summary>
        /// 错误请求,数据校验错误
        /// </summary>
        public const string BadRequest = "10001";

        /// <summary>
        /// 通用错误
        /// </summary>
        public const string CommonError = "10002";

        /// <summary>
        /// 无权限访问
        /// </summary>
        public const string Forbidden = "10003";

        /// <summary>
        /// 未登录
        /// </summary>
        public const string NotLogin = "10004";

        /// <summary>
        /// 未查询到符合的数据
        /// </summary>
        public const string DataNotQueried = "10005";

        /// <summary>
        /// 数据无权访问 
        /// </summary>
        public const string DataForbidden = "10006";

        /// <summary>
        /// 访问过于频繁
        /// </summary>
        public const string TooFrequent = "10007";

        /// <summary>
        /// 业务处理上的错误
        /// </summary>
        public const string BusError = "10008";

        /// <summary>
        /// 家长未关联学员
        /// </summary>
        public const string ParentUnBindStudent = "10009";

        /// <summary>
        /// 人脸错误
        /// </summary>
        public const string FaceError = "10010";

        /// <summary>
        /// 登录次数超过限制
        /// </summary>
        public const string Login20003 = "20003";

    }
}
