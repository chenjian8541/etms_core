using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.External.Request
{
    public class ImportCourseTimesRequest : RequestBase
    {
        public List<ImportCourseTimesItem> ImportCourseTimess { get; set; }

        public override string Validate()
        {
            if (ImportCourseTimess == null || ImportCourseTimess.Count == 0)
            {
                return "导入的学员个数必须大于0";
            }
            return base.Validate();
        }
    }

    public class ImportCourseTimesItem {

        public string StudentName { get; set; }

        public string Phone { get; set; }

        public string CourseName { get; set; }

        public int BuyQuantity { get; set; }

        public int GiveQuantity { get; set; }

        public decimal SurplusQuantity { get; set; }

        public DateTime? EndTime { get; set; }

        public decimal AptSum { get; set; }

        public decimal PaySum { get; set; }

        public string PayTypeName { get; set; }

        public DateTime OrderOt { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmCourseType"/>
        /// </summary>
        public byte CourseType { get; set; }

        public string PhoneRelationshipDesc { get; set; }

        public string GenderDesc { get; set; }

        public DateTime? Birthday { get; set; }

        public string SchoolName { get; set; }

        public string GradeDesc { get; set; }

        public string SourceDesc { get; set; }

        public string PhoneBak { get; set; }

        public string HomeAddress { get; set; }

        public string Remark { get; set; }

        public string CardNo { get; set; }

        public List<ImportStudentExtendInfo> ExtendInfoList { get; set; }
    }
}
