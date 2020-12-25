using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentNeedAttendClassGetPagingOutput
    {
        public long NeedCheckClassLogId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 是否绑定磁卡
        /// </summary>
        public bool IsBindingCard { get; set; }

        /// <summary>
        /// 是否采集了人脸
        /// </summary>
        public bool IsBindingFaceKey { get; set; }

        /// <summary>
        /// 人脸图片
        /// </summary>
        public string FaceKeyUrl { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        public string ClassOt { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        public string WeekDesc { get; set; }

        public string TimeDesc { get; set; }
    }
}
