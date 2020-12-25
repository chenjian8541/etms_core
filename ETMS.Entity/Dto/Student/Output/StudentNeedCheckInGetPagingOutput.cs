using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentNeedCheckInGetPagingOutput
    {
        public long NeedCheckLogId { get; set; }
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string ClassDesc { get; set; }

        public string StartTimeDesc { get; set; }

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
    }
}
