using ETMS.Entity.Config;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class TeacherSalaryDefaultFundsItemsView
    {
        public long Id { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryFundsItemsType"/>
        /// </summary>
        public byte Type { get; set; }

        public string Name { get; set; }

        public int OrderIndex { get; set; }

        /// <summary>
        /// <see cref="EmBool"/>
        /// 启用状态
        /// </summary>
        public byte Status { get; set; }

        public static List<TeacherSalaryDefaultFundsItemsView> GetDefaultFundsItems()
        {
            return new List<TeacherSalaryDefaultFundsItemsView>() {
             new TeacherSalaryDefaultFundsItemsView(){
                 Id = -30,
                 Name="基本工资",
                 Status = EmBool.True,
                 OrderIndex = 0,
                 Type = EmTeacherSalaryFundsItemsType.Add
             },
             new TeacherSalaryDefaultFundsItemsView(){
                 Id = -20,
                 Name="奖金",
                 Status = EmBool.True,
                 OrderIndex = 0,
                 Type = EmTeacherSalaryFundsItemsType.Add
             },
             new TeacherSalaryDefaultFundsItemsView(){
                 Id = SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId,
                 Name="上课绩效工资",
                 Status = EmBool.True,
                 OrderIndex = 0,
                 Type =  EmTeacherSalaryFundsItemsType.Add
             },
            };
        }
    }
}
