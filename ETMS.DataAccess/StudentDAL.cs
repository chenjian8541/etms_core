using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
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
using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Temp;
using ETMS.Entity.View;

namespace ETMS.DataAccess
{
    public class StudentDAL : DataAccessBase<StudentBucket>, IStudentDAL
    {
        private readonly IStudent2DAL _student2DAL;

        public StudentDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider, IStudent2DAL student2DAL) : base(dbWrapper, cacheProvider)
        {
            this._student2DAL = student2DAL;
        }

        public override void InitTenantId(int tenantId)
        {
            base.InitTenantId(tenantId);
            _student2DAL.InitTenantId(tenantId);
        }

        public override void ResetTenantId(int tenantId)
        {
            base.ResetTenantId(tenantId);
            _student2DAL.ResetTenantId(tenantId);
        }

        protected override async Task<StudentBucket> GetDb(params object[] keys)
        {
            var studentId = keys[1].ToLong();
            var student = await _dbWrapper.Find<EtStudent>(p => p.TenantId == _tenantId && p.Id == studentId && p.IsDeleted == EmIsDeleted.Normal);
            var studentExtendInfos = await _dbWrapper.FindList<EtStudentExtendInfo>(p => p.TenantId == _tenantId && p.StudentId == studentId && p.IsDeleted == EmIsDeleted.Normal);
            if (student == null)
            {
                return null;
            }
            return new StudentBucket()
            {
                Student = student,
                StudentExtendInfos = studentExtendInfos
            };
        }

        public async Task<StudentBucket> GetStudent(long studentId)
        {
            return await this.GetCache(_tenantId, studentId);
        }

        public void AddStudent(List<EtStudent> students)
        {
            _dbWrapper.InsertRange(students);
        }

        public void AddStudentExtend(List<EtStudentExtendInfo> studentExtendInfos)
        {
            _dbWrapper.InsertRange(studentExtendInfos);
        }

        public async Task AddStudentNotUpCache(EtStudent student)
        {
            await _dbWrapper.Insert(student);
        }

        public async Task<long> AddStudent(EtStudent student, List<EtStudentExtendInfo> studentExtendInfos)
        {
            await _dbWrapper.Insert(student);
            if (studentExtendInfos != null && studentExtendInfos.Any())
            {
                foreach (var s in studentExtendInfos)
                {
                    s.StudentId = student.Id;
                }
                _dbWrapper.InsertRange(studentExtendInfos);
            }
            await base.UpdateCache(_tenantId, student.Id);
            return student.Id;
        }

        public async Task<bool> EditStudent(EtStudent student, List<EtStudentExtendInfo> studentExtendInfos)
        {
            await _dbWrapper.Update(student);
            await _dbWrapper.Execute($"DELETE EtStudentExtendInfo WHERE StudentId = {student.Id}");
            if (studentExtendInfos != null && studentExtendInfos.Any())
            {
                _dbWrapper.InsertRange(studentExtendInfos);
            }
            await base.UpdateCache(_tenantId, student.Id);
            return true;
        }

        public async Task<bool> EditStudent(EtStudent student)
        {
            await _dbWrapper.Update(student, async () => await UpdateCache(_tenantId, student.Id));
            return true;
        }

        public async Task<bool> CheckStudentHasOrder(long studentId)
        {
            var obj = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM [EtOrder] WHERE StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal}");
            return obj != null;
        }

        public async Task<bool> DelStudent(long studentId)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {studentId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentExtendInfo SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCheckOnLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND StudentId = {studentId} ;");
            sql.Append($"UPDATE EtStudentWechat SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassTimesRuleStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTryCalssApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTryCalssLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentPointsLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveHomeworkDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveGrowthRecordDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveWxMessageDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCheckOnLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtGiftExchangeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtGiftExchangeLogDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentPointsLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassTimesStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassTimesReservationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            var tempSql = sql.ToString();
            LOG.Log.Info($"[DelStudent]执行删除:{tempSql}", this.GetType());
            await _dbWrapper.Execute(tempSql);
            base.RemoveCache(_tenantId, studentId);
            return true;
        }

        public async Task<bool> DelStudentDepth(long studentId)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {studentId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentExtendInfo SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCheckOnLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND StudentId = {studentId} ;");
            sql.Append($"UPDATE EtStudentTrackLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND StudentId = {studentId} ;");
            sql.Append($"UPDATE EtOrder SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND StudentId = {studentId} ;");
            sql.Append($"UPDATE EtOrderDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE OrderId in (select Id from EtOrder where StudentId = {studentId} and TenantId = {_tenantId}) and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCourse SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCourseDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentWechat SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassTimesRuleStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassTimesStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassTimesReservationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordEvaluateTeacher SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordEvaluateStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordPointsApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtClassRecordAbsenceLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCourseConsumeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTryCalssApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTryCalssLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentOperationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtGiftExchangeLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtGiftExchangeLogDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentPointsLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentLeaveApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentNotice SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentNotice SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtCouponsStudentUse SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTempStudentNeedCheck SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTempStudentNeedCheckClass SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveHomeworkDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveGrowthRecordDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtActiveWxMessageDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentCheckOnLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtStudentAccountRechargeBinder SET IsDeleted = {EmIsDeleted.Deleted} WHERE StudentId = {studentId} and TenantId = {_tenantId} ;");
            var tempSql = sql.ToString();
            LOG.Log.Info($"[DelStudentDepth]执行深度删除:{tempSql}", this.GetType());
            await _dbWrapper.Execute(tempSql);
            base.RemoveCache(_tenantId, studentId);
            //删除此学员一对一班级
            var myClassOneToOne = await _dbWrapper.ExecuteObject<EtClass>($"SELECT * FROM EtClass WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Type] = {EmClassType.OneToOne} AND StudentIds = ',{studentId},'");
            if (myClassOneToOne != null && myClassOneToOne.Any())
            {
                foreach (var p in myClassOneToOne)
                {
                    var classId = p.Id;
                    var sql2 = new StringBuilder();
                    sql2.Append($"UPDATE EtClass SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {classId}; ");
                    sql2.Append($"DELETE EtClassStudent WHERE ClassId = {classId} and TenantId = {_tenantId} ; ");
                    sql2.Append($"DELETE EtClassTimesRuleStudent WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"DELETE EtClassTimesRule WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"DELETE EtClassTimes WHERE ClassId  = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"DELETE EtClassTimesStudent WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtClassRecord SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtClassRecordStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtClassRecordEvaluateTeacher SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtClassRecordEvaluateStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtClassRecordPointsApplyLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtClassRecordOperationLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtClassRecordAbsenceLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtTryCalssLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtTempStudentNeedCheckClass SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtActiveHomework SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtActiveHomeworkDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    sql2.Append($"UPDATE EtStudentCheckOnLog SET IsDeleted = {EmIsDeleted.Deleted} WHERE ClassId = {classId} and TenantId = {_tenantId} ;");
                    var tempsql2 = sql2.ToString();
                    await _dbWrapper.Execute(tempsql2);
                }
            }
            return true;
        }

        public async Task<Tuple<IEnumerable<EtStudent>, int>> GetStudentPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtStudent>("EtStudent", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> ExistStudent(string name, string phone, long id = 0)
        {
            var user = await _dbWrapper.Find<EtStudent>(p => p.TenantId == _tenantId && p.Phone == phone && p.Name == name && p.Id != id && p.IsDeleted == EmIsDeleted.Normal);
            return user != null;
        }

        public async Task<bool> ExistStudentPhone(string phone, long id = 0)
        {
            var user = await _dbWrapper.Find<EtStudent>(p => p.TenantId == _tenantId && p.Phone == phone && p.Id != id && p.IsDeleted == EmIsDeleted.Normal);
            return user != null;
        }

        public async Task<EtStudent> GetStudent(string name, string phone)
        {
            return await _dbWrapper.Find<EtStudent>(p => p.TenantId == _tenantId && p.Phone == phone && p.Name == name && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> DeductionPoint(long studentId, int dePoint)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET Points = Points - {dePoint} WHERE Id = {studentId}");
            await base.UpdateCache(_tenantId, studentId);
            return true;
        }

        public async Task<bool> AddPoint(long studentId, int addPoint)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET Points = Points + {addPoint} WHERE Id = {studentId}");
            await base.UpdateCache(_tenantId, studentId);
            return true;
        }

        public async Task<bool> StudentEnrolmentEventChangeInfo(long studentId, int addPoint, byte? newStudentType)
        {
            if (addPoint == 0 && newStudentType == null)
            {
                return false;
            }
            var setSql = new List<string>();
            if (addPoint > 0)
            {
                setSql.Add($"Points = Points+{addPoint}");
            }
            if (newStudentType != null)
            {
                setSql.Add($"StudentType = {newStudentType}");
            }
            await _dbWrapper.Execute($"UPDATE EtStudent SET {string.Join(',', setSql)} WHERE id = {studentId}");
            await UpdateCache(_tenantId, studentId);
            return true;
        }

        public async Task<bool> EditStudentTrackUser(List<long> studentIds, long newTrackUserId)
        {
            var sql = $"UPDATE EtStudent SET TrackUser = {newTrackUserId} WHERE id IN ({string.Join(',', studentIds)})";
            await this._dbWrapper.Execute(sql);
            foreach (var studentId in studentIds)
            {
                await base.UpdateCache(_tenantId, studentId);
            }
            return true;
        }

        public async Task<bool> EditStudentLearningManager(List<long> studentIds, long newLearningManager)
        {
            var sql = $"UPDATE EtStudent SET LearningManager = {newLearningManager} WHERE id IN ({string.Join(',', studentIds)})";
            await this._dbWrapper.Execute(sql);
            foreach (var studentId in studentIds)
            {
                await base.UpdateCache(_tenantId, studentId);
            }
            return true;
        }

        public async Task<bool> EditStudentType(long studentId, byte newStudentType, DateTime? endClassOt)
        {
            string sql = string.Empty;
            switch (newStudentType)
            {
                case EmStudentType.HistoryStudent: //毕业
                    sql = $"UPDATE EtStudent SET StudentType = {newStudentType},EndClassOt = '{endClassOt.Value.EtmsToDateString()}' WHERE id = {studentId}";
                    break;
                case EmStudentType.HiddenStudent: //潜在
                    sql = $"UPDATE EtStudent SET StudentType = {newStudentType},TrackStatus = {EmStudentTrackStatus.NotTrack} WHERE id = {studentId}";
                    break;
                case EmStudentType.ReadingStudent:
                    sql = $"UPDATE EtStudent SET StudentType = {newStudentType} WHERE id = {studentId}";
                    break;
            }
            await this._dbWrapper.Execute(sql);
            await base.UpdateCache(_tenantId, studentId);
            return true;
        }

        public async Task UpdateStudentIsBindingWechat(List<long> studentIds)
        {
            var sql = string.Empty;
            if (studentIds.Count == 1)
            {
                sql = $"UPDATE EtStudent SET IsBindingWechat = {EmIsBindingWechat.Yes} WHERE Id = {studentIds[0]}";
            }
            else
            {
                sql = $"UPDATE EtStudent SET IsBindingWechat = {EmIsBindingWechat.Yes} WHERE Id IN ({string.Join(',', studentIds)})";
            }
            await _dbWrapper.Execute(sql);
            foreach (var studentId in studentIds)
            {
                await base.UpdateCache(_tenantId, studentId);
            }
        }

        public async Task UpdateStudentIsNotBindingWechat(List<long> studentIds)
        {
            var sql = string.Empty;
            if (studentIds.Count == 1)
            {
                sql = $"UPDATE EtStudent SET IsBindingWechat = {EmIsBindingWechat.No} WHERE Id = {studentIds[0]}";
            }
            else
            {
                sql = $"UPDATE EtStudent SET IsBindingWechat = {EmIsBindingWechat.No} WHERE Id IN ({string.Join(',', studentIds)})";
            }
            await _dbWrapper.Execute(sql);
            foreach (var studentId in studentIds)
            {
                await base.UpdateCache(_tenantId, studentId);
            }
        }

        public async Task<Tuple<IEnumerable<GetAllStudentPagingOutput>, int>> GetAllStudentPaging(GetAllStudentPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<GetAllStudentPagingOutput>("EtStudent", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtStudent> GetStudent(string cardNo)
        {
            return await _student2DAL.GetStudent(cardNo);
        }

        public async Task<EtStudent> GetStudentByDb(string cardNo)
        {
            return await _student2DAL.GetStudentByDb(cardNo);
        }

        public async Task<bool> StudentRelieveCardNo(long id, string cardNo)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET CardNo = '',HkCardStatus = {EmBool.False} WHERE TenantId = {_tenantId} AND Id = {id}");
            await base.UpdateCache(_tenantId, id);
            _student2DAL.RemoveCache(cardNo);
            return true;
        }

        public async Task<bool> StudentBindingCardNo(long id, string cardNo, string oldCardNo, byte newHkCardStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET CardNo = '{cardNo}',HkCardStatus = {newHkCardStatus} WHERE TenantId = {_tenantId} AND Id = {id}");
            await base.UpdateCache(_tenantId, id);
            await _student2DAL.UpdateCache(cardNo);
            if (!string.IsNullOrEmpty(oldCardNo))
            {
                _student2DAL.RemoveCache(oldCardNo);
            }
            return true;
        }

        public async Task<bool> StudentBindingFaceKey(long id, string faceKey, string faceGreyKey, byte newHkFaceStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET FaceKey = '{faceKey}' ,FaceGreyKey = '{faceGreyKey}',HkFaceStatus = {newHkFaceStatus} WHERE TenantId = {_tenantId} AND Id = {id}");
            await base.UpdateCache(_tenantId, id);
            return true;
        }

        public async Task<bool> StudentRelieveFaceKey(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET FaceKey = '' ,FaceGreyKey = '',HkFaceStatus = {EmBool.False} WHERE TenantId = {_tenantId} AND Id = {id}");
            await base.UpdateCache(_tenantId, id);
            return true;
        }

        public async Task StudentFaceClear()
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET FaceKey = '' ,FaceGreyKey = '',HkFaceStatus = {EmBool.False} WHERE TenantId = {_tenantId} ");
        }

        public async Task<bool> UpdateStudentFaceUseLastTime(long id, DateTime faceUseLastTime)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET FaceUseLastTime = '{faceUseLastTime.EtmsToString()}' WHERE TenantId = {_tenantId} AND Id = {id} ");
            await base.UpdateCache(_tenantId, id);
            return true;
        }

        public async Task<IEnumerable<StudentFaceView>> GetStudentFace()
        {
            return await _dbWrapper.ExecuteObject<StudentFaceView>(
                $"SELECT TOP 100 Id,FaceGreyKey FROM EtStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND FaceGreyKey <> '' ORDER BY FaceUseLastTime DESC");
        }

        public async Task<List<EtStudent>> GetStudentsByPhone(string phone)
        {
            return await this._dbWrapper.FindList<EtStudent>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Phone == phone);
        }

        public async Task<EtStudent> GetStudentsByPhoneOrNameOne(string key)
        {
            var obj = await _dbWrapper.ExecuteObject<EtStudent>(
                $"SELECT TOP 1 * FROM EtStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND (Phone = '{key}' OR Name = '{key}') ORDER BY Id DESC");
            return obj.FirstOrDefault();
        }

        public async Task<List<EtStudent>> GetStudentsByPhoneMini(string phone)
        {
            return await this._dbWrapper.FindListMini<EtStudent>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Phone == phone);
        }

        public async Task<bool> ChangePwd(long studentId, string newPwd)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET [Password] = '{newPwd}' WHERE Id = {studentId}");
            await base.UpdateCache(_tenantId, studentId);
            return true;
        }

        public async Task<EtStudent> GetStudentByPwd(string phone, string pwd)
        {
            var sql = $"SELECT TOP 1 * from EtStudent WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} AND (Phone = '{phone}' or PhoneBak = '{phone}') AND Password = '{pwd}' ";
            var obj = await _dbWrapper.ExecuteObject<EtStudent>(sql);
            return obj.FirstOrDefault();
        }

        public async Task UpdateStudentClassInfo(long studentId)
        {
            var studentClassInfo = await GetStudentClassInfo(studentId);
            await _dbWrapper.Execute($"UPDATE EtStudent SET IsClassSchedule = {studentClassInfo.IsClassSchedule} , IsJoinClass = {studentClassInfo.IsJoinClass} WHERE Id = {studentId}");
            RemoveCache(_tenantId, studentId);
        }

        public async Task<StudentClassInfoView> GetStudentClassInfo(long studentId)
        {
            var objClassSchedule = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0 FROM EtClassStudent INNER JOIN EtClassTimesRule ON EtClassStudent.ClassId = EtClassTimesRule.ClassId AND EtClassStudent.StudentId = {studentId} WHERE EtClassStudent.IsDeleted = {EmIsDeleted.Normal} AND EtClassTimesRule.IsDeleted = {EmIsDeleted.Normal} AND EtClassStudent.StudentId = {studentId}");
            var objJoinClass = await _dbWrapper.ExecuteScalar($"SELECT TOP 1 0  FROM EtClassStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND StudentId = {studentId}");
            var isClassSchedule = EmBool.False;
            var isJoinClass = EmBool.False;
            if (objClassSchedule != null)
            {
                isClassSchedule = EmBool.True;
            }
            if (objJoinClass != null)
            {
                isJoinClass = EmBool.True;
            }
            return new StudentClassInfoView()
            {
                IsClassSchedule = isClassSchedule,
                IsJoinClass = isJoinClass
            };
        }

        public async Task EditStudent2(EtStudent entity)
        {
            await _dbWrapper.Update(entity);
            RemoveCache(_tenantId, entity.Id);
        }

        public async Task UpdateStudentCourseStatus(long studentId, byte newCourseStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET CourseStatus = {newCourseStatus} WHERE Id = {studentId} AND TenantId = {_tenantId}");
            RemoveCache(_tenantId, studentId);
        }

        public async Task UpdateStudentClassIds(long studentId, string classIds)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET ClassIds = '{classIds}' WHERE Id = {studentId}");
            RemoveCache(_tenantId, studentId);
        }

        public async Task UpdateStudentCourseIds(long studentId, string courseIds)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET CourseIds = '{courseIds}' WHERE Id = {studentId}");
            RemoveCache(_tenantId, studentId);
        }

        public async Task<bool> SetStudentTypeIsRead(long studentId)
        {
            var exCount = await _dbWrapper.Execute(
                $"UPDATE EtStudent SET StudentType = {EmStudentType.ReadingStudent} WHERE Id = {studentId} AND TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} ");
            return exCount > 0;
        }

        public async Task UpdateStudentCourseRestoreTime(List<long> studentIds)
        {
            if (studentIds.Count == 1)
            {
                await _dbWrapper.Execute($"UPDATE EtStudent SET CourseStatus = {EmStudentCourseStatus.Normal} WHERE Id = {studentIds[0]} AND TenantId = {_tenantId} AND CourseStatus = {EmStudentCourseStatus.StopOfClass}");
                RemoveCache(_tenantId, studentIds[0]);
            }
            else
            {
                await _dbWrapper.Execute($"UPDATE EtStudent SET CourseStatus = {EmStudentCourseStatus.Normal} WHERE Id IN ({string.Join(',', studentIds)}) AND TenantId = {_tenantId} AND CourseStatus = {EmStudentCourseStatus.StopOfClass}");
                foreach (var id in studentIds)
                {
                    RemoveCache(_tenantId, id);
                }
            }
        }

        public async Task<IEnumerable<EtStudent>> GetTrackMustToday(long userId, DateTime date)
        {
            return await _dbWrapper.ExecuteObject<EtStudent>(
                $"SELECT TOP 50 * FROM EtStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND TrackUser = {userId} AND NextTrackTime = '{date.EtmsToDateString()}'");
        }

        public async Task UpdateStudentLastGoClassTime(long studentId, DateTime? time)
        {
            if (time == null)
            {
                await _dbWrapper.Execute($"UPDATE [EtStudent] SET LastGoClassTime = null WHERE Id = {studentId} AND TenantId = {_tenantId}");
            }
            else
            {
                await _dbWrapper.Execute($"UPDATE [EtStudent] SET LastGoClassTime = '{time.EtmsToString()}' WHERE Id = {studentId} AND TenantId = {_tenantId}");
            }
            RemoveCache(_tenantId, studentId);
        }
    }
}
