using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class ClassTimesDAL : DataAccessBase, IClassTimesDAL
    {
        public ClassTimesDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task<long> AddClassTimes(EtClassTimes classTime)
        {
            await _dbWrapper.Insert(classTime);
            return classTime.Id;
        }

        public bool AddClassTimes(IEnumerable<EtClassTimes> classTimes)
        {
            return _dbWrapper.InsertRange(classTimes);
        }

        public async Task<EtClassTimes> GetClassTimes(long id)
        {
            return await _dbWrapper.Find<EtClassTimes>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> EditClassTimes(EtClassTimes classTime)
        {
            return await _dbWrapper.Update(classTime);
        }

        public async Task<bool> DelClassTimes(long id)
        {
            var count = await _dbWrapper.Execute($"DELETE EtClassTimes WHERE id = {id} AND [Status] = {EmClassTimesStatus.UnRollcall}");
            if (count > 0)
            {
                await _dbWrapper.Execute($"DELETE EtClassTimesStudent WHERE ClassTimesId = {id}");
            }
            return count > 0;
        }

        public async Task<List<EtClassTimesStudent>> GetClassTimesStudent(long classTimesId)
        {
            return await _dbWrapper.FindList<EtClassTimesStudent>(p => p.TenantId == _tenantId && p.ClassTimesId == classTimesId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<EtClassTimesStudent> GetClassTimesStudentById(long id)
        {
            return await _dbWrapper.Find<EtClassTimesStudent>(id);
        }

        /// <summary>
        /// 判断某个班级某个时间点是否有课
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="classId"></param>
        /// <param name="exClassTimesId"></param>
        /// <returns></returns>
        public async Task<bool> ExistClassTimes(DateTime dateTime, int startTime, int endTime, long classId, long exClassTimesId)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM EtClassTimes WHERE TenantId = {_tenantId} AND ClassId = {classId} AND ClassOt = '{dateTime.EtmsToDateString()}' AND IsDeleted = {EmIsDeleted.Normal} AND Id <> {exClassTimesId}  AND ((StartTime<='{startTime}' AND EndTime > '{startTime}') OR (StartTime<'{endTime}' AND EndTime >= '{endTime}') OR (StartTime>='{startTime}' AND EndTime <= '{endTime}'))");
            return obj != null;
        }

        public async Task<bool> UpdateClassTimesStudent(long classTimesId, DateTime newClassOt)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtClassTimesStudent SET ClassOt = '{newClassOt.EtmsToDateString()}' WHERE ClassTimesId = {classTimesId}");
            return count > 0;
        }

        public bool AddClassTimesStudent(List<EtClassTimesStudent> etClassTimesStudents)
        {
            return _dbWrapper.InsertRange(etClassTimesStudents);
        }

        public async Task<long> AddClassTimesStudent(EtClassTimesStudent etClassTimesStudent)
        {
            await _dbWrapper.Insert(etClassTimesStudent);
            return etClassTimesStudent.Id;
        }

        public async Task<bool> DelClassTimesStudent(long id)
        {
            var count = await _dbWrapper.Execute($"DELETE EtClassTimesStudent WHERE id = {id}");
            return count > 0;
        }

        public async Task<Tuple<IEnumerable<EtClassTimes>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtClassTimes>("EtClassTimes", "*", request.PageSize, request.PageCurrent, "ClassOt ASC,StartTime ASC", request.ToString());
        }

        public async Task<IEnumerable<EtClassTimes>> GetList(IValidate request)
        {
            var sql = $"SELECT * FROM EtClassTimes WHERE {request.ToString()}";
            return await _dbWrapper.ExecuteObject<EtClassTimes>(sql);
        }

        public async Task<bool> UpdateClassTimesIsClassCheckSign(long classTimesId, long classRecordId, byte newStatus, EtClassRecord record)
        {
            var sql = new StringBuilder();
            var classOt = record.ClassOt.EtmsToString();
            sql.Append($"UPDATE EtClassTimes SET [Status] = {newStatus} ,ClassRecordId = {classRecordId},ClassOt='{classOt}',Week={record.Week},StartTime={record.StartTime},EndTime={record.EndTime},ClassContent='{record.ClassContent}',CourseList='{record.CourseList}',Teachers='{record.Teachers}',TeacherNum='{record.TeacherNum}',ClassRoomIds='{record.ClassRoomIds}',StudentIdsTemp='',StudentIdsClass='{record.StudentIds}' WHERE Id = {classTimesId} ;");
            sql.Append($"UPDATE EtClassTimesStudent SET [Status] = {newStatus},ClassOt='{classOt}' WHERE ClassTimesId = {classTimesId} ;");
            await _dbWrapper.Execute(sql.ToString());
            return true;
        }

        public async Task<EtClassTimesStudent> GetClassTimesStudent(long classTimesId, long studentTryCalssLogId)
        {
            return await _dbWrapper.Find<EtClassTimesStudent>(p => p.ClassTimesId == classTimesId && p.StudentTryCalssLogId == studentTryCalssLogId &&
            p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<EtClassTimesStudent> GetClassTimesTryStudent(long studentId, long courseId, DateTime classOt)
        {
            var log = await this._dbWrapper.Find<EtClassTimesStudent>(p => p.TenantId == _tenantId && p.StudentId == studentId &&
            p.CourseId == courseId && p.ClassOt == classOt.Date && p.RuleId == 0 && p.Status == EmClassTimesStatus.UnRollcall
            && p.IsDeleted == EmIsDeleted.Normal && p.StudentType == EmClassStudentType.TryCalssStudent);
            return log;
        }

        public async Task<IEnumerable<EtClassTimes>> GetStudentCheckOnAttendClass(DateTime checkOt, long studentId, int relationClassTimesLimitMinuteCard)
        {
            var minMime = checkOt.AddMinutes(-relationClassTimesLimitMinuteCard).ToString("HHmm").ToInt();
            var maxMime = checkOt.AddMinutes(relationClassTimesLimitMinuteCard).ToString("HHmm").ToInt();
            return await _dbWrapper.ExecuteObject<EtClassTimes>(
                $"SELECT * FROM EtClassTimes WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassTimesStatus.UnRollcall} AND ClassOt = '{checkOt.EtmsToDateString()}' AND StartTime >= {minMime} AND StartTime <= {maxMime} AND (StudentIdsTemp LIKE '%,{studentId},%' OR StudentIdsClass LIKE '%,{studentId},%')");
        }

        public async Task SyncClassTimesOfClassTimesRule(EtClassTimesRule rule)
        {
            await _dbWrapper.Execute($"UPDATE EtClassTimes SET ReservationType = {rule.ReservationType},StartTime={rule.StartTime},EndTime={rule.EndTime},ClassContent='{rule.ClassContent}',Teachers='{rule.Teachers}',ClassRoomIds='{rule.ClassRoomIds}',CourseList='{rule.CourseList}',CourseListIsAlone={EmBool.True},ClassRoomIdsIsAlone={EmBool.True},TeachersIsAlone={EmBool.True} WHERE RuleId = {rule.Id} AND TenantId = {_tenantId} AND [Status] = {EmClassTimesStatus.UnRollcall} AND IsDeleted = {EmIsDeleted.Normal} ");
        }

        public async Task SyncClassTimesReservationType(List<long> classTimesIds, byte newReservationType)
        {
            await _dbWrapper.Execute($"UPDATE [EtClassTimes] SET ReservationType = {newReservationType} WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND Id In ({string.Join(',', classTimesIds)})");
        }
    }
}
