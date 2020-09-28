using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 学员跟进状态
    /// </summary>
    public struct EmStudentTrackStatus
    {
        /// <summary>
        /// 待跟进
        /// </summary>
        public const int NotTrack = 0;

        /// <summary>
        /// 跟进中
        /// </summary>
        public const int IsTracking = 1;

        /// <summary>
        /// 已约课
        /// </summary>
        public const int IsReserveCourse = 2;

        /// <summary>
        /// 已体验
        /// </summary>
        public const int IsExperienced = 3;

        /// <summary>
        /// 已失效
        /// </summary>
        public const int IsInvalid = 4;

        public static string GetTrackStatusDesc(byte trackStatus)
        {
            switch (trackStatus)
            {
                case EmStudentTrackStatus.NotTrack:
                    return "待跟进";
                case EmStudentTrackStatus.IsTracking:
                    return "跟进中";
                case EmStudentTrackStatus.IsReserveCourse:
                    return "已约课";
                case EmStudentTrackStatus.IsExperienced:
                    return "已体验";
                default:
                    return "已失效";
            }
        }
    }
}
