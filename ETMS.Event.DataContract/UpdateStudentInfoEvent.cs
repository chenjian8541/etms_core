using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class UpdateStudentInfoEvent : Event
    {
        public UpdateStudentInfoEvent(int tenantId) : base(tenantId)
        { }

        public EtStudent MyStudent { get; set; }

        public bool IsAnalyzeStudentClass { get; set; }

        /// <summary>
        /// 课程自动升级
        /// </summary>
        public bool IsOpentGradeAutoUpgrade { get; set; }

        public List<EtGrade> AllGrade { get; set; }
    }
}

