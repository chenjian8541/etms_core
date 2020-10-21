using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveHomeworkStudentGetAnsweredOutput
    {
        public long HomeworkDetailId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentAvatar { get; set; }

        public string AnswerOtDesc { get; set; }

        public string AnswerContent { get; set; }

        /// <summary>
        /// <see cref="AnsweredOutputAnswerStatus"/>
        /// </summary>
        public byte AnswerStatus { get; set; }

        public string AnswerStatusDesc { get; set; }

        public List<string> AnswerMediasUrl { get; set; }

        public List<CommentOutput> CommentOutputs { get; set; }
    }

    public struct AnsweredOutputAnswerStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 逾期
        /// </summary>
        public const byte Late = 1;

        public static Tuple<byte, string> GetAnswerStatus(DateTime? exDate, DateTime submitOt)
        {
            if (exDate == null || exDate.Value > submitOt)
            {
                return Tuple.Create(Normal, "正常");
            }
            return Tuple.Create(Late, "逾期提交");
        }
    }
}
