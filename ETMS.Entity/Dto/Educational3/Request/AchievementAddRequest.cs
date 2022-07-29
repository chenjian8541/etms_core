using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational3.Request
{
    public class AchievementAddRequest: RequestBase
    {
        public string Name { get; set; }

        public long? SubjectId { get; set; }

        public decimal ScoreTotal { get; set; }

        public DateTime? ExamOt { get; set; }

        public int SourceType { get; set; }

        public byte ShowParent { get; set; }

        public byte ShowRankParent { get; set; }

        public bool IsPublish { get; set; }

        public List<AchievementAddStudent> Students { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入考试名称";
            }
            if (SubjectId == null)
            {
                return "请选择考试科目";
            }
            if (ScoreTotal <= 0)
            {
                return "请输入满分";
            }
            if (ExamOt == null)
            {
                return "请选择考试时间";
            }
            if (Students == null || Students.Count == 0)
            {
                return "请选择考试学员";
            }
            return base.Validate();
        }
    }
}
