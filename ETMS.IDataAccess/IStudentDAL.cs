using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentDAL : IBaseDAL
    {
        Task<StudentBucket> GetStudent(long studentId);

        void AddStudent(List<EtStudent> students);

        Task<long> AddStudent(EtStudent student, List<EtStudentExtendInfo> studentExtendInfos);

        Task<bool> EditStudent(EtStudent student, List<EtStudentExtendInfo> studentExtendInfos);

        Task<bool> EditStudent(EtStudent student);

        Task<bool> CheckStudentHasOrder(long studentId);

        Task<bool> DelStudent(long studentId);

        Task<Tuple<IEnumerable<EtStudent>, int>> GetStudentPaging(RequestPagingBase request);

        Task<bool> ExistStudent(string name, string phone, long id = 0);

        Task<bool> DeductionPoint(long studentId, int dePoint);

        Task<bool> AddPoint(long studentId, int addPoint);

        Task<bool> StudentEnrolmentEventChangeInfo(long studentId, int addPoint, byte newStudentType);

        Task<bool> EditStudentTrackUser(List<long> studentIds, long newTrackUserId);

        Task<bool> EditStudentLearningManager(List<long> studentIds, long newLearningManager);

        Task<bool> EditStudentType(long studentId, byte newStudentType, DateTime? endClassOt );

        Task UpdateStudentIsBindingWechat(List<long> studentIds);
    }
}
