using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysStudentWechartDAL : ISysStudentWechartDAL, IEtmsManage
    {
        public async Task<SysStudentWechart> GetSysStudentWechart(string openid)
        {
            return await this.Find<SysStudentWechart>(p => p.WechatOpenid == openid && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<SysStudentWechart> GetSysStudentWechart(long id)
        {
            return await this.Find<SysStudentWechart>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task AddSysStudentWechart(SysStudentWechart entity)
        {
            await this.Insert(entity);
        }

        public async Task EditSysStudentWechart(SysStudentWechart entity)
        {
            await this.Update(entity);
        }

        public async Task DelSysStudentWechart(string openid)
        {
            await this.Execute($"DELETE SysStudentWechart WHERE WechatOpenid = '{openid}'");
        }
    }
}
