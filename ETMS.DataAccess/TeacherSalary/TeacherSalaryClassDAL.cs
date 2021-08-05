using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess.TeacherSalary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Utility;

namespace ETMS.DataAccess.TeacherSalary
{
    public class TeacherSalaryClassDAL : DataAccessBase, ITeacherSalaryClassDAL
    {
        public TeacherSalaryClassDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task<bool> DelTeacherSalaryClassTimes(long classRecordId)
        {
            await _dbWrapper.Execute($"DELETE EtTeacherSalaryClassTimes WHERE TenantId = {_tenantId} AND ClassRecordId = {classRecordId} ");
            return true;
        }

        public async Task<bool> SaveTeacherSalaryClassTimes(long classRecordId, List<EtTeacherSalaryClassTimes> entitys)
        {
            await DelTeacherSalaryClassTimes(classRecordId);
            if (entitys.Count == 1)
            {
                await _dbWrapper.Insert(entitys.First());
            }
            else
            {
                _dbWrapper.InsertRange(entitys);
            }
            return true;
        }

        public async Task<bool> DelTeacherSalaryClassDay(DateTime ot)
        {
            await _dbWrapper.Execute($"DELETE EtTeacherSalaryClassDay WHERE TenantId = {_tenantId} AND Ot = '{ot.EtmsToDateString()}' ");
            return true;
        }

        public async Task<bool> SaveTeacherSalaryClassDay(DateTime ot, List<EtTeacherSalaryClassDay> entitys)
        {
            await DelTeacherSalaryClassDay(ot);
            if (entitys.Count == 1)
            {
                await _dbWrapper.Insert(entitys.First());
            }
            else
            {
                _dbWrapper.InsertRange(entitys);
            }
            return true;
        }
    }
}
