using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Temp;
using ETMS.Entity.View.OnlyOneFiled;

namespace ETMS.DataAccess
{
    public class ClassDAL : DataAccessBase<ClassBucket>, IClassDAL
    {
        public ClassDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ClassBucket> GetDb(params object[] keys)
        {
            var etClass = await _dbWrapper.Find<EtClass>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (etClass == null)
            {
                return null;
            }
            var classStudents = await _dbWrapper.FindList<EtClassStudent>(p => p.TenantId == _tenantId && p.ClassId == etClass.Id && p.IsDeleted == EmIsDeleted.Normal);
            return new ClassBucket()
            {
                EtClass = etClass,
                EtClassStudents = classStudents
            };
        }

        public async Task<ClassBucket> GetClassBucket(long classId)
        {
            return await base.GetCache(_tenantId, classId);
        }

        public async Task<long> AddClass(EtClass etClass)
        {
            await this._dbWrapper.Insert(etClass, async () => await base.UpdateCache(_tenantId, etClass.Id));
            return etClass.Id;
        }

        public async Task<bool> EditClass(EtClass etClass)
        {
            await this._dbWrapper.Update(etClass, async () => await base.UpdateCache(_tenantId, etClass.Id));
            return true;
        }

        public async Task<bool> DelClass(long classId, bool isIgnoreCheck = false)
        {
            var classRecord = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM EtClassRecord WHERE ClassId = {classId}");
            if (!isIgnoreCheck && classRecord != null)
            {
                return false;
            }
            var strSql = new StringBuilder();
            strSql.Append($"UPDATE EtClass SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {classId}; ");
            strSql.Append($"DELETE EtClassStudent WHERE ClassId = {classId}; ");
            strSql.Append($"DELETE EtClassTimesRule WHERE ClassId = {classId};");
            strSql.Append($"DELETE EtClassTimes WHERE ClassId  = {classId};");
            strSql.Append($"DELETE EtClassTimesStudent WHERE ClassId = {classId};");
            strSql.Append($"UPDATE EtClassTimesReservationLog SET [Status] = {EmClassTimesReservationLogStatus.Invalidation} WHERE TenantId = {_tenantId} AND ClassId = {classId} AND [Status] = {EmClassTimesReservationLogStatus.Normal} ;");
            await _dbWrapper.Execute(strSql.ToString());
            RemoveCache(_tenantId, classId);
            return true;
        }

        public async Task<IEnumerable<OnlyOneFiledDateTime>> GetClassRecordAllDate(long classId)
        {
            var sql = $"SELECT TOP 2000 ClassOt AS Ot FROM EtClassRecord WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ClassId = {classId} GROUP BY ClassOt";
            return await _dbWrapper.ExecuteObject<OnlyOneFiledDateTime>(sql);
        }

        public async Task<bool> DelClassDepth(long classId)
        {
            await DelClass(classId, true);
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtClassRecord SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordEvaluateTeacher SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordEvaluateStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordPointsApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordOperationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordAbsenceLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTryCalssLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTempStudentNeedCheckClass SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveHomework SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveHomeworkDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCheckOnLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTeacherSalaryClassDay SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTeacherSalaryClassTimes SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
            var tempSql = sql.ToString();
            LOG.Log.Info($"[DelClassDepth]执行深度删除:{tempSql}", this.GetType());
            await _dbWrapper.Execute(tempSql);
            return true;
        }

        public async Task<bool> AddClassStudent(EtClassStudent etClassStudent)
        {
            await _dbWrapper.Insert(etClassStudent);
            await base.UpdateCache(_tenantId, etClassStudent.ClassId);
            return true;
        }

        public async Task<bool> AddClassStudent(List<EtClassStudent> etClassStudents)
        {
            _dbWrapper.InsertRange(etClassStudents);
            await base.UpdateCache(_tenantId, etClassStudents[0].ClassId);
            return true;
        }

        public async Task<bool> DelClassStudent(long classId, long id)
        {
            await _dbWrapper.Execute($"DELETE EtClassStudent WHERE Id = {id}");
            await base.UpdateCache(_tenantId, classId);
            return true;
        }

        public async Task<bool> DelClassStudentByStudentId(long classId, long studentId)
        {
            await _dbWrapper.Execute($"DELETE EtClassStudent WHERE TenantId = {_tenantId} AND ClassId = {classId} AND StudentId = {studentId} ");
            await base.UpdateCache(_tenantId, classId);
            return true;
        }

        public async Task<long> AddClassTimesRule(long classId, EtClassTimesRule rule)
        {
            await _dbWrapper.Insert(rule);
            return rule.Id;
        }

        public bool AddClassTimesRule(long classId, IEnumerable<EtClassTimesRule> rules)
        {
            _dbWrapper.InsertRange(rules);
            return true;
        }
        public async Task<Tuple<IEnumerable<EtClass>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtClass>("EtClass", "*", request.PageSize, request.PageCurrent, "CompleteStatus,Id DESC", request.ToString());
        }

        public async Task<bool> IsClassCanNotBeDelete(long classId)
        {
            var classRecord = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM EtClassRecord WHERE ClassId = {classId}");
            return classRecord != null;
        }

        public async Task<bool> SetClassOverOneToMany(List<long> classIds, DateTime overTime)
        {
            var strSql = new StringBuilder();
            foreach (var classId in classIds)
            {
                strSql.Append($"UPDATE EtClass SET CompleteStatus = {EmClassCompleteStatus.Completed},CompleteTime = '{overTime.EtmsToString()}',StudentIds = '',StudentNums = 0 WHERE Id = {classId};");
                strSql.Append($"DELETE EtClassStudent WHERE ClassId = {classId}; ");
                strSql.Append($"DELETE EtClassTimesRule WHERE ClassId = {classId};");
                strSql.Append($"DELETE EtClassTimes WHERE ClassId  = {classId} AND Status = {EmClassTimesStatus.UnRollcall};");
                strSql.Append($"DELETE EtClassTimesStudent WHERE ClassId = {classId} AND Status = {EmClassTimesStatus.UnRollcall};");
            }
            await _dbWrapper.Execute(strSql.ToString());
            foreach (var classId in classIds)
            {
                await UpdateCache(_tenantId, classId);
            }
            return true;
        }

        public async Task<bool> SetClassOverOneToOne(long classId, DateTime overTime)
        {
            var strSql = new StringBuilder();
            strSql.Append($"UPDATE EtClass SET CompleteStatus = {EmClassCompleteStatus.Completed},CompleteTime = '{overTime.EtmsToString()}',StudentIds = '',StudentNums = 0 WHERE Id = {classId};");
            strSql.Append($"DELETE EtClassStudent WHERE ClassId = {classId}; ");
            strSql.Append($"DELETE EtClassTimesRule WHERE ClassId = {classId};");
            strSql.Append($"DELETE EtClassTimes WHERE ClassId  = {classId} AND Status = {EmClassTimesStatus.UnRollcall};");
            strSql.Append($"DELETE EtClassTimesStudent WHERE ClassId = {classId} AND Status = {EmClassTimesStatus.UnRollcall};");
            await _dbWrapper.Execute(strSql.ToString());
            await UpdateCache(_tenantId, classId);
            return true;
        }

        public async Task<bool> IsStudentBuyCourse(long studentId, long courseId)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM EtStudentCourseDetail WHERE StudentId = {studentId} AND  CourseId = {courseId}");
            return obj != null;
        }

        public async Task<bool> IsStudentInClass(long classId, long studentId)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM EtClassStudent WHERE ClassId = {classId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal}");
            return obj != null;
        }

        public async Task<int> GetClassTimesRuleCount(long classId)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT COUNT(0) FROM EtClassTimesRule WHERE ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal}");
            return obj == null ? 0 : obj.ToInt();
        }

        public async Task<IEnumerable<EtClassTimesRule>> GetClassTimesRule(long classId, int startTime, int endTime, List<byte> weekDay)
        {
            var weekWhere = string.Empty;
            if (weekDay.Count > 1)
            {
                weekWhere = $" [Week] IN ({string.Join(',', weekDay)})";
            }
            else
            {
                weekWhere = $" [Week] = {weekDay[0]}";
            }
            return await _dbWrapper.ExecuteObject<EtClassTimesRule>(
                $"SELECT TOP 100 * FROM EtClassTimesRule WHERE ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal} AND {weekWhere} AND ((StartTime<='{startTime}' AND EndTime > '{startTime}') OR (StartTime<'{endTime}' AND EndTime >= '{endTime}') OR (StartTime>='{startTime}' AND EndTime <= '{endTime}'))");
        }

        public async Task<IEnumerable<EtClassTimes>> GetClassTimesRuleTeacher(long teacherId, int startTime,
            int endTime, List<byte> weekDay, DateTime startDate, DateTime? endDate, int topCount, long excRuleId = 0)
        {
            var weekWhere = new StringBuilder();
            if (weekDay.Count > 1)
            {
                weekWhere.Append($" [Week] IN ({string.Join(',', weekDay)})");
            }
            else
            {
                weekWhere.Append($" [Week] = {weekDay[0]}");
            }
            if (endDate != null)
            {
                weekWhere.Append($" AND ClassOt <= '{endDate.EtmsToDateString()}'");
            }
            if (excRuleId > 0)
            {
                weekWhere.Append($" AND RuleId <> {excRuleId}");
            }
            return await _dbWrapper.ExecuteObject<EtClassTimes>(
                $"SELECT TOP {topCount} * FROM EtClassTimes WHERE [Status] = {EmClassTimesStatus.UnRollcall} AND IsDeleted = {EmIsDeleted.Normal} AND Teachers LIKE '%,{teacherId},%' AND ClassOt >= '{startDate.EtmsToDateString()}' AND {weekWhere} AND ((StartTime<='{startTime}' AND EndTime > '{startTime}') OR (StartTime<'{endTime}' AND EndTime >= '{endTime}') OR (StartTime>='{startTime}' AND EndTime <= '{endTime}'))");
        }

        public async Task<List<EtClassTimesRule>> GetClassTimesRule(long classId)
        {
            return await _dbWrapper.FindList<EtClassTimesRule>(p => p.TenantId == _tenantId && p.ClassId == classId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<EtClassTimesRule> GetClassTimesRuleBuyId(long id)
        {
            return await _dbWrapper.Find<EtClassTimesRule>(p => p.Id == id);
        }

        public async Task EditClassTimesRule(EtClassTimesRule rule)
        {
            await _dbWrapper.Update(rule);
        }

        public async Task<bool> UpdateClassPlanTimes(long classId, byte newScheduleStatus)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT COUNT(0) FROM  EtClassTimes WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal}");
            var planCount = obj == null ? 0 : obj.ToInt();
            await _dbWrapper.Execute($"UPDATE EtClass SET PlanCount = {planCount},ScheduleStatus = {newScheduleStatus} WHERE id = {classId}");
            await base.UpdateCache(_tenantId, classId);
            return true;
        }

        public async Task<bool> UpdateClassPlanTimes(long classId)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT COUNT(0) FROM  EtClassTimes WHERE TenantId = {_tenantId} AND ClassId = {classId} AND IsDeleted = {EmIsDeleted.Normal}");
            var planCount = obj == null ? 0 : obj.ToInt();
            await _dbWrapper.Execute($"UPDATE EtClass SET PlanCount = {planCount} WHERE id = {classId}");
            await base.UpdateCache(_tenantId, classId);
            return true;
        }
        public async Task<bool> DelClassTimesRule(long classId, long ruleId)
        {
            var strSql = $"DELETE EtClassTimesRule WHERE Id = {ruleId} ;";
            await _dbWrapper.Execute(strSql);
            strSql = $"DELETE EtClassTimes WHERE RuleId = {ruleId} AND [Status] = {EmClassTimesStatus.UnRollcall};";
            await _dbWrapper.Execute(strSql);
            strSql = $"DELETE EtClassTimesStudent WHERE RuleId = {ruleId} AND [Status] = {EmClassTimesStatus.UnRollcall};";
            await _dbWrapper.Execute(strSql);
            strSql = $"UPDATE EtClassTimesReservationLog SET [Status] = {EmClassTimesReservationLogStatus.Invalidation} WHERE TenantId = {_tenantId} AND RuleId = {ruleId} AND [Status] = {EmClassTimesReservationLogStatus.Normal} ;";
            await _dbWrapper.Execute(strSql);
            await UpdateClassPlanTimes(classId);
            return true;
        }

        public async Task<bool> ClassEditStudentInfo(long classId, string studentIds, int studentNums)
        {
            await _dbWrapper.Execute($"UPDATE EtClass SET StudentIds = '{studentIds}',StudentNums = {studentNums} WHERE Id = {classId}");
            await base.UpdateCache(_tenantId, classId);
            return true;
        }

        public List<string> GetSyncClassInfoSql(long classId, string studentIdsClass,
            string courseList, string classRoomIds, string teachers, int teacherNum, int? limitStudentNums, int limitStudentNumsType,
            int studentClassCount)
        {
            var sql = new List<string>();
            sql.Add($"UPDATE EtClassTimes SET StudentIdsClass = '{studentIdsClass}',StudentCount = StudentTempCount + {studentClassCount} WHERE ClassId = {classId} AND [Status] = {EmClassTimesStatus.UnRollcall} AND IsDeleted = {EmIsDeleted.Normal} ");
            sql.Add($"UPDATE EtClassTimes SET CourseList = '{courseList}' WHERE ClassId = {classId} AND CourseListIsAlone = {EmBool.False} AND [Status] = {EmClassTimesStatus.UnRollcall} AND IsDeleted = {EmIsDeleted.Normal} ");
            sql.Add($"UPDATE EtClassTimes SET ClassRoomIds = '{classRoomIds}' WHERE ClassId = {classId} AND ClassRoomIdsIsAlone = {EmBool.False} AND [Status] = {EmClassTimesStatus.UnRollcall} AND IsDeleted = {EmIsDeleted.Normal} ");
            sql.Add($"UPDATE EtClassTimes SET Teachers = '{teachers}',TeacherNum = {teacherNum} WHERE ClassId = {classId} AND TeachersIsAlone = {EmBool.False} AND [Status] = {EmClassTimesStatus.UnRollcall} AND IsDeleted = {EmIsDeleted.Normal} ");
            sql.Add($"UPDATE EtClassTimes SET LimitStudentNums = {limitStudentNums.EtmsToSqlString()},LimitStudentNumsType = {limitStudentNumsType} WHERE ClassId = {classId} AND LimitStudentNumsIsAlone = {EmBool.False} AND [Status] = {EmClassTimesStatus.UnRollcall} AND IsDeleted = {EmIsDeleted.Normal} ");
            return sql;
        }

        public async Task<IEnumerable<EtClass>> GetStudentClass(long studentId)
        {
            var sql = $"SELECT * FROM EtClass WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND DataType = {EmClassDataType.Normal} AND CompleteStatus = {EmClassCompleteStatus.UnComplete} AND StudentIds LIKE '%,{studentId},%'";
            return await _dbWrapper.ExecuteObject<EtClass>(sql);
        }

        public async Task<bool> UpdateClassTeachers(List<long> ids, string teachers, int teacherNum)
        {
            var strSql = new StringBuilder();
            foreach (var classId in ids)
            {
                strSql.Append($"UPDATE EtClass SET Teachers = '{teachers}',TeacherNum = {teacherNum} WHERE id = {classId}");
            }
            await _dbWrapper.Execute(strSql.ToString());
            foreach (var classId in ids)
            {
                await UpdateCache(_tenantId, classId);
            }
            return true;
        }

        public async Task RemoveStudent(long studentId)
        {
            var sql = $"SELECT ClassId FROM EtClassStudent WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal}";
            var ids = await _dbWrapper.ExecuteObject<OnlyClassId>(sql);
            sql = $"DELETE EtClassStudent WHERE TenantId = {_tenantId} AND StudentId = {studentId}";
            await _dbWrapper.Execute(sql);
            if (ids != null && ids.Any())
            {
                foreach (var p in ids)
                {
                    UpdateCache(_tenantId, p.ClassId).Wait();
                }
            }
        }

        public async Task<List<EtClass>> GetEtClassByOrderId(long orderId)
        {
            return await _dbWrapper.FindList<EtClass>(p => p.TenantId == _tenantId && p.OrderId == orderId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<IEnumerable<EtClass>> GetClassOfTeacher(long teacherId)
        {
            var sql = $"SELECT TOP 200 * FROM EtClass WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND DataType = {EmClassDataType.Normal} AND CompleteStatus = {EmClassCompleteStatus.UnComplete} AND Teachers LIKE '%,{teacherId},%'";
            return await _dbWrapper.ExecuteObject<EtClass>(sql);
        }

        public async Task<IEnumerable<EtClass>> GetClassOfCourseIdOneToMore(long courseId, string queryClassName = "")
        {
            var sql = $"SELECT TOP 200 * FROM EtClass WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Type] = {EmClassType.OneToMany} AND DataType = {EmClassDataType.Normal} AND CompleteStatus = {EmClassCompleteStatus.UnComplete} AND CourseList LIKE '%,{courseId},%'";
            if (!string.IsNullOrEmpty(queryClassName))
            {
                sql = $"{sql} AND [Name] LIKE '%{queryClassName}%' ";
            }
            return await _dbWrapper.ExecuteObject<EtClass>(sql);
        }

        public async Task<IEnumerable<EtClass>> GetStudentOneToOneClassNormal(long studentId, long courseId)
        {
            var sql = $"SELECT * FROM EtClass WHERE TenantId = {_tenantId} AND [Type] = {EmClassType.OneToOne} AND CompleteStatus = {EmClassCompleteStatus.UnComplete} AND IsDeleted = {EmIsDeleted.Normal} AND DataType = {EmClassDataType.Normal} AND StudentIds LIKE '%,{studentId},%' AND CourseList LIKE '%,{courseId},%'";
            return await _dbWrapper.ExecuteObject<EtClass>(sql);
        }

        public async Task<IEnumerable<OnlyClassId>> GetStudentCourseInClass(long studentId, long courseId)
        {
            var sql = $"SELECT ClassId FROM EtClassStudent WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND IsDeleted = {EmIsDeleted.Normal} GROUP BY ClassId ";
            return await _dbWrapper.ExecuteObject<OnlyClassId>(sql);
        }

        public async Task<IEnumerable<EtClassStudent>> GetStudentCourseInClass(long studentId)
        {
            var sql = $"SELECT TOP 200 * FROM EtClassStudent WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal} ";
            return await _dbWrapper.ExecuteObject<EtClassStudent>(sql);
        }

        public async Task<bool> UpdateClassFinishInfo(long classId, int finishCount, decimal finishClassTimes)
        {
            await _dbWrapper.Execute($"UPDATE EtClass SET FinishCount = {finishCount},FinishClassTimes = {finishClassTimes} WHERE TenantId = {_tenantId} AND Id = {classId} ");
            return true;
        }
    }
}
