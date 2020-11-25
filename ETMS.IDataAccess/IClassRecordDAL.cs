using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassRecordDAL : IBaseDAL
    {
        Task<long> AddEtClassRecord(EtClassRecord etClassRecord, List<EtClassRecordStudent> classRecordStudents);

        void AddClassRecordAbsenceLog(List<EtClassRecordAbsenceLog> classRecordAbsenceLogs);

        void AddClassRecordPointsApplyLog(List<EtClassRecordPointsApplyLog> classRecordPointsApplyLog);

        Task<Tuple<IEnumerable<EtClassRecord>, int>> GetPaging(RequestPagingBase request);

        Task<EtClassRecord> GetClassRecord(long classRecordId);

        Task<List<EtClassRecordStudent>> GetClassRecordStudents(long classRecordId);

        Task<Tuple<IEnumerable<ClassRecordAbsenceLogView>, int>> GetClassRecordAbsenceLogPaging(RequestPagingBase request);

        Task<EtClassRecordAbsenceLog> GetClassRecordAbsenceLog(long id);

        Task<List<EtClassRecordAbsenceLog>> GetClassRecordAbsenceLogByClassRecordId(long classRecordId);

        Task<bool> UpdateClassRecordAbsenceLog(EtClassRecordAbsenceLog log);

        Task<EtClassRecordPointsApplyLog> GetClassRecordPointsApplyLog(long id);

        Task<bool> EditClassRecordPointsApplyLog(EtClassRecordPointsApplyLog log);

        Task<Tuple<IEnumerable<EtClassRecordStudent>, int>> GetClassRecordStudentPaging(IPagingRequest request);

        Task<EtClassRecordStudent> GetEtClassRecordStudentById(long id);

        Task<bool> EditClassRecord(EtClassRecord classRecord);

        Task<bool> EditClassRecordStudent(EtClassRecordStudent etClassRecordStudent);

        Task<Tuple<IEnumerable<ClassRecordPointsApplyLogView>, int>> GetClassRecordPointsApplyLog(RequestPagingBase request);

        Task<List<EtClassRecordPointsApplyLog>> GetClassRecordPointsApplyLogByClassRecordId(long classRecordId);

        Task<bool> SetClassRecordRevoke(long classRecordId);

        Task<bool> ClassRecordAddEvaluateStudentCount(long classRecordId, int addCount);

        Task<bool> AddClassRecordOperationLog(EtClassRecordOperationLog log);

        Task<List<EtClassRecordOperationLog>> GetClassRecordOperationLog(long classRecordId);
    }
}
