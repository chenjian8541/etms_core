using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员跟进记录内容类型
    /// </summary>
    public struct EmStudentTrackContentType
    {
        /// <summary>
        /// 自定义 (手动创建跟进记录)
        /// </summary>
        public const int Customize = 0;

        /// <summary>
        /// 预约试听
        /// </summary>
        public const int ApplyTryClass = 1;

        /// <summary>
        /// 已体验(试听)
        /// </summary>
        public const int IsTryClassFinish = 2;

        /// <summary>
        /// 已失效(试听)
        /// </summary>
        public const int IsTryClassInvalid = 3;

        /// <summary>
        /// 取消试听
        /// </summary>
        public const int CancelApplyTryClass = 4;

        /// <summary>
        /// 插板补课
        /// </summary>
        public const int AddMakeup = 5;

        /// <summary>
        /// 取消插班补课
        /// </summary>
        public const int CancelMakeup = 6;

        public static string GetContentTypeDesc(byte type)
        {
            switch (type)
            {
                case EmStudentTrackContentType.Customize:
                    return "自定义";
                case EmStudentTrackContentType.ApplyTryClass:
                    return "预约试听";
                case EmStudentTrackContentType.IsTryClassFinish:
                    return "已体验(试听)";
                case EmStudentTrackContentType.CancelApplyTryClass:
                    return "取消试听";
                case EmStudentTrackContentType.AddMakeup:
                    return "插班补课";
                case EmStudentTrackContentType.CancelMakeup:
                    return "取消插班补课";
                default:
                    return "已失效(试听)";
            }
        }
    }
}
