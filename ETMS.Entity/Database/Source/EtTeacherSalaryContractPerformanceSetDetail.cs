using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtTeacherSalaryContractPerformanceSetDetail")]
    public class EtTeacherSalaryContractPerformanceSetDetail : Entity<long>
    {
        public long TeacherId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeType"/>
        /// </summary>
        public byte ComputeType { get; set; }

        /// <summary>
        /// 关联ID (班级、课程)
        /// 0：代表全局设置
        /// </summary>
        public long RelationId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeMode"/>
        /// </summary>
        public byte ComputeMode { get; set; }

        public decimal? MinLimit { get; set; }

        public decimal? MaxLimit { get; set; }

        public decimal ComputeValue { get; set; }

    }
}
