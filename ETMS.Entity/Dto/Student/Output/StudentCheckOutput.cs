using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentCheckOutput
    {
        public static StudentCheckOutput CheckFail(string errMsg, FaceInfo faceWhite)
        {
            return new StudentCheckOutput()
            {
                CheckResult = null,
                CheckState = StudentCheckOutputCheckState.Fail,
                ErrMsg = errMsg,
                FaceBlack = null,
                FaceWhite = faceWhite,
                NeedDeClassTimes = null
            };
        }

        /// <summary>
        /// 考勤状态  <see cref="StudentCheckOutputCheckState"/>
        /// </summary>
        public byte CheckState { get; set; }

        public string ErrMsg { get; set; }

        /// <summary>
        /// 考勤结果
        /// </summary>
        public CheckResult CheckResult { get; set; }

        /// <summary>
        /// 需要记上课的课次
        /// </summary>
        public List<StudentNeedDeClassTimes> NeedDeClassTimes { get; set; }

        /// <summary>
        /// 白名单
        /// </summary>
        public FaceInfo FaceWhite { get; set; }

        /// <summary>
        /// 黑名单
        /// </summary>
        public FaceInfo FaceBlack { get; set; }
    }

    public class StudentNeedDeClassTimes
    {
        public long ClassTimesId { get; set; }

        public string ClassName { get; set; }

        public string CourseName { get; set; }

        public string ClassOtDesc { get; set; }

        public string TeacherDesc { get; set; }
    }

    public class FaceInfo
    {
        public long StudentId { get; set; }

        public string FaceUrl { get; set; }
    }

    public class CheckResult
    {
        public long StudentCheckOnLogId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentAvatar { get; set; }

        public string CheckTypeDesc { get; set; }

        public byte CheckType { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string CheckOt { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string CheckTime { get; set; }

        /// <summary>
        /// 扣减课时
        /// </summary>
        public string DeClassTimesDesc { get; set; }
    }

    public struct StudentCheckOutputCheckState
    {
        public const byte Fail = 0;

        public const byte Success = 1;
    }
}
