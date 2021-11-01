using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryContractGetPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 是否老师
        /// </summary>
        public bool IsTeacher { get; set; }

        /// <summary>
        /// 是否设置绩效工资
        /// </summary>
        public bool IsSetContractPerformance { get; set; }

        public string IsTeacherDesc { get; set; }

        /// <summary>
        /// 工资设置状态
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte SalaryContractStatus { get; set; }

        public string SalaryContract0 { get; set; }
        public string SalaryContract1 { get; set; }
        public string SalaryContract2 { get; set; }
        public string SalaryContract3 { get; set; }
        public string SalaryContract4 { get; set; }
        public string SalaryContract5 { get; set; }
        public string SalaryContract6 { get; set; }
        public string SalaryContract7 { get; set; }
        public string SalaryContract8 { get; set; }
        public string SalaryContract9 { get; set; }
        public string SalaryContract10 { get; set; }
        public string SalaryContract11 { get; set; }
        public string SalaryContract12 { get; set; }
        public string SalaryContract13 { get; set; }
        public string SalaryContract14 { get; set; }
        public string SalaryContract15 { get; set; }
        public string SalaryContract16 { get; set; }
        public string SalaryContract17 { get; set; }
        public string SalaryContract18 { get; set; }
        public string SalaryContract19 { get; set; }
        public string SalaryContract20 { get; set; }
        public string SalaryContract21 { get; set; }
        public string SalaryContract22 { get; set; }
        public string SalaryContract23 { get; set; }
        public string SalaryContract24 { get; set; }
        public string SalaryContract25 { get; set; }
        public string SalaryContract26 { get; set; }
        public string SalaryContract27 { get; set; }
        public string SalaryContract28 { get; set; }
        public string SalaryContract29 { get; set; }
        public string SalaryContract30 { get; set; }

        public void SetSalaryContract(int index, string value)
        {
            //考虑到性能  不使用发射
            switch (index)
            {
                case 0:
                    this.SalaryContract0 = value;
                    break;
                case 1:
                    this.SalaryContract1 = value;
                    break;
                case 2:
                    this.SalaryContract2 = value;
                    break;
                case 3:
                    this.SalaryContract3 = value;
                    break;
                case 4:
                    this.SalaryContract4 = value;
                    break;
                case 5:
                    this.SalaryContract5 = value;
                    break;
                case 6:
                    this.SalaryContract6 = value;
                    break;
                case 7:
                    this.SalaryContract7 = value;
                    break;
                case 8:
                    this.SalaryContract8 = value;
                    break;
                case 9:
                    this.SalaryContract9 = value;
                    break;
                case 10:
                    this.SalaryContract10 = value;
                    break;
                case 11:
                    this.SalaryContract11 = value;
                    break;
                case 12:
                    this.SalaryContract12 = value;
                    break;
                case 13:
                    this.SalaryContract13 = value;
                    break;
                case 14:
                    this.SalaryContract14 = value;
                    break;
                case 15:
                    this.SalaryContract15 = value;
                    break;
                case 16:
                    this.SalaryContract16 = value;
                    break;
                case 17:
                    this.SalaryContract17 = value;
                    break;
                case 18:
                    this.SalaryContract18 = value;
                    break;
                case 19:
                    this.SalaryContract19 = value;
                    break;
                case 20:
                    this.SalaryContract20 = value;
                    break;
                case 21:
                    this.SalaryContract21 = value;
                    break;
                case 22:
                    this.SalaryContract22 = value;
                    break;
                case 23:
                    this.SalaryContract23 = value;
                    break;
                case 24:
                    this.SalaryContract24 = value;
                    break;
                case 25:
                    this.SalaryContract25 = value;
                    break;
                case 26:
                    this.SalaryContract26 = value;
                    break;
                case 27:
                    this.SalaryContract27 = value;
                    break;
                case 28:
                    this.SalaryContract28 = value;
                    break;
                case 29:
                    this.SalaryContract29 = value;
                    break;
                case 30:
                    this.SalaryContract30 = value;
                    break;
            }
        }
    }
}
