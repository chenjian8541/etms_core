using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCheckByHkRequest : RequestBase
    {
        public long StudentId { get; set; }

        /// <summary> 
        /// 考勤形式  <see cref="ETMS.Entity.Enum.EmStudentCheckOnLogCheckForm"/>
        /// </summary>
        public byte CheckForm { get; set; }

        /// <summary>
        /// 人脸图片
        /// </summary>
        public string FaceImageBase64 { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public decimal CurrTemperature { get; set; }

        /// <summary>
        /// 温度是否异常
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsAbnomalTemperature { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            if (CheckForm == EmStudentCheckOnLogCheckForm.Face && string.IsNullOrEmpty(FaceImageBase64))
            {
                return "请上传考勤图片";
            }
            return string.Empty;
        }
    }
}
