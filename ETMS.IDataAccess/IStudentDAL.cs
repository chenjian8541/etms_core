using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
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

        void AddStudentExtend(List<EtStudentExtendInfo> studentExtendInfos);

        Task AddStudentNotUpCache(EtStudent student);

        Task<long> AddStudent(EtStudent student, List<EtStudentExtendInfo> studentExtendInfos);

        Task<bool> EditStudent(EtStudent student, List<EtStudentExtendInfo> studentExtendInfos);

        Task<bool> EditStudent(EtStudent student);

        Task<bool> CheckStudentHasOrder(long studentId);

        Task<bool> DelStudent(long studentId);

        Task<bool> DelStudentDepth(long studentId);

        Task<Tuple<IEnumerable<EtStudent>, int>> GetStudentPaging(IPagingRequest request);

        Task<bool> ExistStudent(string name, string phone, long id = 0);

        Task<bool> ExistStudentPhone(string phone, long id = 0);

        Task<EtStudent> GetStudent(string name, string phone);

        Task<bool> DeductionPoint(long studentId, int dePoint);

        Task<bool> AddPoint(long studentId, int addPoint);

        Task<bool> StudentEnrolmentEventChangeInfo(long studentId, int addPoint, byte? newStudentType);

        Task<bool> EditStudentTrackUser(List<long> studentIds, long newTrackUserId);

        Task<bool> EditStudentLearningManager(List<long> studentIds, long newLearningManager);

        Task<bool> EditStudentType(long studentId, byte newStudentType, DateTime? endClassOt);

        Task UpdateStudentIsBindingWechat(List<long> studentIds);

        Task UpdateStudentIsNotBindingWechat(List<long> studentIds);

        Task<Tuple<IEnumerable<GetAllStudentPagingOutput>, int>> GetAllStudentPaging(GetAllStudentPagingRequest request);

        Task<EtStudent> GetStudent(string cardNo);

        Task<EtStudent> GetStudentByDb(string cardNo);

        Task<bool> StudentRelieveCardNo(long id, string cardNo);

        Task<bool> StudentBindingCardNo(long id, string cardNo, string oldCardNo, byte newHkCardStatus);

        Task<bool> StudentBindingFaceKey(long id, string faceKey, string faceGreyKey, byte newHkFaceStatus);

        Task<bool> StudentRelieveFaceKey(long id);

        Task StudentFaceClear();

        Task<bool> UpdateStudentFaceUseLastTime(long id, DateTime faceUseLastTime);

        Task<IEnumerable<StudentFaceView>> GetStudentFace();

        Task<List<EtStudent>> GetStudentsByPhone(string phone);

        Task<EtStudent> GetStudentsByPhoneOrNameOne(string phone);

        Task<List<EtStudent>> GetStudentsByPhoneMini(string phone);

        Task<bool> ChangePwd(long studentId, string newPwd);

        Task<EtStudent> GetStudentByPwd(string phone, string pwd);

        Task UpdateStudentClassInfo(long studentId);

        Task<StudentClassInfoView> GetStudentClassInfo(long studentId);

        Task EditStudent2(EtStudent entity);

        Task UpdateStudentCourseStatus(long studentId, byte newCourseStatus);

        Task UpdateStudentClassIds(long studentId, string classIds);

        Task UpdateStudentCourseIds(long studentId, string courseIds);

        Task<bool> SetStudentTypeIsRead(long studentId);

        Task UpdateStudentCourseRestoreTime(List<long> studentIds);

        Task<IEnumerable<EtStudent>> GetTrackMustToday(long userId, DateTime date);

        Task UpdateStudentLastGoClassTime(long studentId, DateTime? time);
    }
}
