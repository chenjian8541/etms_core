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

        public List<string> AnswerMediasUrl { get; set; }

        public List<CommentOutput> CommentOutputs { get; set; }
    }
}
