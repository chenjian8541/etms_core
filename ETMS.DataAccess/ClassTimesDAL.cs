using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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
            await ClassTimesReservationLogEditStatus(id, EmClassTimesReservationLogStatus.Invalidation);
            return count > 0;
        }

        public async Task<List<EtClassTimesStudent>> GetClassTimesStudent(long classTimesId)
        {
            return await _dbWrapper.FindList<EtClassTimesStudent>(p => p.TenantId == _tenantId && p.ClassTimesId == classTimesId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<List<EtClassTimesStudent>> GetClassTimesStudentAboutReservation(long classTimesId)
        {
            return await _dbWrapper.FindList<EtClassTimesStudent>(p => p.TenantId == _tenantId && p.ClassTimesId == classTimesId
            && p.IsReservation == EmBool.True && p.IsDeleted == EmIsDeleted.Normal);
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

        public async Task<Tuple<IEnumerable<EtClassTimes>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtClassTimes>("EtClassTimes", "*", request.PageSize, request.PageCurrent, "ClassOt ASC,StartTime ASC", request.ToString());
        }

        public async Task<IEnumerable<EtClassTimes>> GetList(IValidate request)
        {
            var sql = $"SELECT * FROM EtClassTimes WHERE {request}";
            return await _dbWrapper.ExecuteObject<EtClassTimes>(sql);
        }

        public async Task<bool> UpdateClassTimesIsClassCheckSign(long classTimesId, long classRecordId, byte newStatus, EtClassRecord record)
        {
            var myEtClassTimes = await _dbWrapper.Find<EtClassTimes>(p => p.Id == classTimesId);
            if (myEtClassTimes != null)
            {
                myEtClassTimes.Status = newStatus;
                myEtClassTimes.ClassRecordId = classRecordId;
                myEtClassTimes.ClassOt = record.ClassOt;
                myEtClassTimes.Week = record.Week;
                myEtClassTimes.StartTime = record.StartTime;
                myEtClassTimes.EndTime = record.EndTime;
                myEtClassTimes.ClassContent = record.ClassContent;
                myEtClassTimes.CourseList = record.CourseList;
                myEtClassTimes.Teachers = record.Teachers;
                myEtClassTimes.TeacherNum = record.TeacherNum;
                myEtClassTimes.ClassRoomIds = record.ClassRoomIds;
                myEtClassTimes.StudentIdsTemp = "";
                myEtClassTimes.StudentIdsClass = record.StudentIds;
                await _dbWrapper.Update(myEtClassTimes);
            }
            var classOt = record.ClassOt.EtmsToString();
            await _dbWrapper.Execute($"UPDATE EtClassTimesStudent SET [Status] = {newStatus},ClassOt='{classOt}' WHERE ClassTimesId = {classTimesId} ;");
            return true;
        }

        public async Task<bool> UpdateClassTimesClassCheckSignRevoke(long classTimesId, byte newStatus)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassTimes SET [Status] = {newStatus} ,ClassRecordId = null WHERE Id = {classTimesId} ;");
            sql.Append($"UPDATE EtClassTimesStudent SET [Status] = {newStatus} WHERE ClassTimesId = {classTimesId} ;");
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

        public async Task SyncClassTimesReservationLog(EtClassTimes classTimes)
        {
            await _dbWrapper.Execute($"UPDATE EtClassTimesReservationLog SET [Week] = {classTimes.Week},[StartTime] = {classTimes.StartTime},EndTime = {classTimes.EndTime},[ClassOt] = '{classTimes.ClassOt.EtmsToString()}' WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimes.Id} AND [Status] = {EmClassTimesReservationLogStatus.Normal} AND IsDeleted = {EmIsDeleted.Normal} ");
        }

        public async Task ClassTimesReservationLogAdd(EtClassTimesReservationLog entity)
        {
            await _dbWrapper.Insert(entity);
        }

        public async Task ClassTimesReservationLogSetCancel(long classTimesId, long studentId)
        {
            await _dbWrapper.Execute($"UPDATE EtClassTimesReservationLog SET [Status] = {EmClassTimesReservationLogStatus.Cancel} WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal} ");
        }

        public async Task<int> ClassTimesReservationLogGetCount(long courseId, long studentId, DateTime time)
        {
            var ot = time.EtmsToDateString();
            var myTime = Convert.ToInt32(time.ToString("HHmm"));
            var obj = await _dbWrapper.ExecuteScalar($"SELECT COUNT(0) FROM EtClassTimesReservationLog WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND [Status] = {EmClassTimesReservationLogStatus.Normal} AND (ClassOt> '{ot}' OR (ClassOt = '{ot}' AND EndTime > {myTime})) ");
            return obj.ToInt();
        }

        public async Task<Tuple<IEnumerable<EtClassTimesReservationLog>, int>> ReservationLogGetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtClassTimesReservationLog>("EtClassTimesReservationLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task ClassTimesReservationLogEditStatus(long classTimesId, byte newStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtClassTimesReservationLog SET [Status] = {newStatus} WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} AND [Status] = {EmClassTimesReservationLogStatus.Normal} ");
        }

        public async Task ClassTimesReservationLogEditStatusBuyClassCheck(long classTimesId, List<long> inStudentId)
        {
            //先将状态标记为失效
            await _dbWrapper.Execute($"UPDATE EtClassTimesReservationLog SET [Status] = {EmClassTimesReservationLogStatus.Invalidation} WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} AND [Status] = {EmClassTimesReservationLogStatus.Normal} ");

            //再将到课学员标记为 已上课
            if (inStudentId != null && inStudentId.Count > 0)
            {
                await _dbWrapper.Execute($"UPDATE EtClassTimesReservationLog SET [Status] = {EmClassTimesReservationLogStatus.BeClassArrived} WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} AND StudentId IN ({string.Join(',', inStudentId)}) ");
            }
        }

        public async Task<IEnumerable<ClassTimesClassOtGroupCountView>> ClassTimesClassOtGroupCount(IValidate request)
        {
            var sql = $"SELECT ClassOt,COUNT(ClassOt) AS TotalCount FROM EtClassTimes WHERE {request} GROUP BY ClassOt";
            return await _dbWrapper.ExecuteObject<ClassTimesClassOtGroupCountView>(sql);
        }

        public async Task<IEnumerable<EtClassTimes>> GetClassTimes(IValidate request)
        {
            var sql = $"SELECT TOP 100 * FROM EtClassTimes WHERE {request}";
            return await _dbWrapper.ExecuteObject<EtClassTimes>(sql);
        }

        public async Task<IEnumerable<OnlyId>> GetMyTempOrReservationClassTimes(long studentId)
        {
            var sql = $"SELECT TOP 500 Id FROM EtClassTimes WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassTimesStatus.UnRollcall} AND (StudentIdsTemp LIKE '%,{studentId},%' OR StudentIdsReservation LIKE '%,{studentId},%')";
            return await _dbWrapper.ExecuteObject<OnlyId>(sql);
        }

        public async Task<IEnumerable<EtClassTimes>> GetStudentClassTimes(long studentId, DateTime startDate, DateTime endDate, int topLimit = 50)
        {
            var sql = $"SELECT TOP {topLimit} * FROM EtClassTimes WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassTimesStatus.UnRollcall} AND ClassOt >= '{startDate.EtmsToDateString()}' AND ClassOt <= '{endDate.EtmsToDateString()}' AND (StudentIdsTemp LIKE '%,{studentId},%' OR StudentIdsReservation LIKE '%,{studentId},%' OR StudentIdsClass LIKE '%,{studentId},%') ORDER BY ClassOt,StartTime";
            return await _dbWrapper.ExecuteObject<EtClassTimes>(sql);
        }

        public async Task<IEnumerable<GetClassTimesStudentView>> GetClassTimesStudent(IEnumerable<long> classTimesIds)
        {
            if (classTimesIds == null || !classTimesIds.Any())
            {
                return null;
            }
            if (classTimesIds.Count() == 1)
            {
                return await this._dbWrapper.ExecuteObject<GetClassTimesStudentView>(
                    $"SELECT ClassTimesId,StudentId,StudentType FROM EtClassTimesStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ClassTimesId = {classTimesIds.First()}");
            }
            return await this._dbWrapper.ExecuteObject<GetClassTimesStudentView>(
                         $"SELECT ClassTimesId,StudentId,StudentType FROM EtClassTimesStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ClassTimesId IN ({string.Join(',', classTimesIds)})");
        }

        public async Task<IEnumerable<EtClassTimes>> GetStudentOneToOneClassTimes(long classId, DateTime classOt)
        {
            return await _dbWrapper.FindList<EtClassTimes>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.ClassId == classId && p.ClassOt == classOt);
        }

        public async Task<IEnumerable<EtClassTimes>> GetClassTimes(long teacherId, long studentId, DateTime classOt)
        {
            return await _dbWrapper.ExecuteObject<EtClassTimes>(
                $"SELECT TOP 50 * FROM EtClassTimes WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmClassTimesStatus.UnRollcall} AND ClassOt = '{classOt.EtmsToDateString()}' AND (Teachers LIKE '%,{teacherId},%' OR StudentIdsClass LIKE '%,{studentId},%' OR StudentIdsTemp LIKE '%,{studentId},%' OR StudentIdsReservation LIKE '%,{studentId},%')");
        }
    }
}
