using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveHomeworkStudentGetUnansweredOutput
    {
        public long HomeworkDetailId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentAvatar { get; set; }

        public string ReadStatusDesc { get; set; }

        public byte ReadStatus { get; set; }
    }
}
