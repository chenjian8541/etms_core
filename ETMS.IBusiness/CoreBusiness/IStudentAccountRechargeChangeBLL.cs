using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentAccountRechargeChangeBLL: IBaseBLL
    {
        Task<EtStudentAccountRecharge> GetStudentAccountRecharge(string phone, string phoneBak);

        Task StudentAccountRechargeChange(StudentAccountRechargeChangeEvent request);
    }
}
