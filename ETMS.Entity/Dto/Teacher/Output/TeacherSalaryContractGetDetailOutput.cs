using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryContractGetDetailOutput
    {
        public long TeacherId { get; set; }

        public string TeacherName { get; set; }

        public string TeacherPhone { get; set; }

        public bool IsOpenClassPerformance { get; set; }

        public List<TeacherSalaryContractFixedItem> FixedItems { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeType"/>
        /// </summary>
        public byte ComputeType { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryGradientCalculateType"/>
        /// </summary>
        public byte GradientCalculateType { get; set; }

        public List<TeacherSalaryContractPerformanceSet> PerformanceSetItems { get; set; }
    }

    public class TeacherSalaryContractFixedItem
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }
    }

    public class TeacherSalaryContractPerformanceSet
    {
        /// <summary>
        /// 关联ID (班级、课程)
        /// 0：代表全局设置
        /// </summary>
        public long RelationId { get; set; }

        /// <summary>
        /// 关联名称
        /// </summary>
        public string RelationName { get; set; }

        /// <summary>
        /// 延伸信息
        /// </summary>
        public List<string> RelationExtend { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeMode"/>
        /// </summary>
        public byte ComputeMode { get; set; }

        public string ComputeModeUnitDesc { get; set; }

        public string ComputeModeDesc { get; set; }

        public List<TeacherSalaryContractPerformanceSetDetail> SetDetails { get; set; }
    }

    public class TeacherSalaryContractPerformanceSetDetail
    {
        public decimal? MinLimit { get; set; }

        public decimal? MaxLimit { get; set; }

        public decimal ComputeValue { get; set; }
    }
}
