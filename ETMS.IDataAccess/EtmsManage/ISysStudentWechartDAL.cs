using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysStudentWechartDAL
    {
        Task<SysStudentWechart> GetSysStudentWechart(string openid);

        Task<SysStudentWechart> GetSysStudentWechart(long id);

        Task AddSysStudentWechart(SysStudentWechart entity);

        Task DelSysStudentWechart(string openid);
    }
}
