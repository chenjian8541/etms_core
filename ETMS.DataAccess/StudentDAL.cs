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
            await _dbWrapper.Execute($"UPDATE EtStudent SET IsDeleted = {EmIsDeleted.Deleted} WHERE id = {studentId};DELETE EtStudentExtendInfo WHERE StudentId = {studentId}");
            base.RemoveCache(_tenantId, studentId);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtStudent>, int>> GetStudentPaging(RequestPagingBase request)
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

        public async Task<bool> StudentEnrolmentEventChangeInfo(long studentId, int addPoint, byte newStudentType)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET Points = Points+{addPoint} ,StudentType = {newStudentType} WHERE id = {studentId}");
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

        public async Task<bool> StudentRelieveCardNo(long id, string cardNo)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET CardNo = '' WHERE TenantId = {_tenantId} AND Id = {id}");
            await base.UpdateCache(_tenantId, id);
            _student2DAL.RemoveCache(cardNo);
            return true;
        }

        public async Task<bool> StudentBindingCardNo(long id, string cardNo, string oldCardNo)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET CardNo = '{cardNo}' WHERE TenantId = {_tenantId} AND Id = {id}");
            await base.UpdateCache(_tenantId, id);
            await _student2DAL.UpdateCache(cardNo);
            if (!string.IsNullOrEmpty(oldCardNo))
            {
                _student2DAL.RemoveCache(oldCardNo);
            }
            return true;
        }

        public async Task<bool> StudentBindingFaceKey(long id, string faceKey, string faceGreyKey)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET FaceKey = '{faceKey}' ,FaceGreyKey = '{faceGreyKey}' WHERE TenantId = {_tenantId} AND Id = {id}");
            await base.UpdateCache(_tenantId, id);
            return true;
        }

        public async Task<bool> StudentRelieveFaceKey(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtStudent SET FaceKey = '' ,FaceGreyKey = '' WHERE TenantId = {_tenantId} AND Id = {id}");
            await base.UpdateCache(_tenantId, id);
            return true;
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
    }
}
