using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class TeacherClassRecordEvaluateStudentDetailOutput
    {
        public string EvaluateUserName { get; set; }

        public string EvaluateUserAvatar { get; set; }

        public DateTime EvaluateOt { get; set; }

        public string EvaluateContent { get; set; }

        public List<string> EvaluateMedias { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool EvaluateIsRead { get; set; }

        public long Id { get; set; }
    }
}
