using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent5.Output
{
    public class StudentReservation1v1LessonsGetOutput
    {
        public StudentReservation1v1LessonsGetOutput() { }

        public StudentReservation1v1LessonsGetOutput(string errMsg)
        {
            this.IsSuccess = false;
            this.ErrMsg = errMsg;
        }

        public bool IsSuccess { get; set; }

        public string ErrMsg { get; set; }

        public IEnumerable<StudentReservation1v1LessonsItem> Items { get; set; }
    }

    public class StudentReservation1v1LessonsItem
    {

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public string Desc { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmStudentReservation1v1LessonsStatus"/>
        /// </summary>
        public int Status { get; set; }

        public bool IsSelect { get; set; }
    }
}
