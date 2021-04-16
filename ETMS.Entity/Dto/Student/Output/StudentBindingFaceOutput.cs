using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Output
{
    public class StudentBindingFaceOutput
    {
        public StudentBindingFaceOutput() { }

        public StudentBindingFaceOutput(byte state, string msg="")
        {
            this.BindingState = state;
            this.ErrMsg = msg;
        }

        /// <summary>
        /// <see cref="StudentBindingFaceOutputState"/>
        /// </summary>
        public byte BindingState { get; set; }

        public string ErrMsg { get; set; }

        public bool IsSameStudent { get; set; }

        public string SameStudentName { get; set; }

        public string SameStudentPhone { get; set; }
    }

    public struct StudentBindingFaceOutputState
    {
        public const byte Fail = 0;

        public const byte Success = 1;

    }
}
