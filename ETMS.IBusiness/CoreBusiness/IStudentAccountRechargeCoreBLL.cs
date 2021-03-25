using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentAccountRechargeCoreBLL: IBaseBLL
    {
        Task StudentAccountRechargeChange(StudentAccountRechargeChangeEvent request);

        Task<StudentAccountRechargeView> GetStudentAccountRechargeByPhone(string phone);

        Task<EtStudentAccountRecharge> GetStudentAccountRechargeByPhone2(string phone);

        Task<StudentAccountRechargeView> GetStudentAccountRechargeByStudentId(long studentId);

        Task<EtStudentAccountRecharge> GetStudentAccountRechargeByStudentId2(long studentId);
    }
}
