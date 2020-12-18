using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Dto.Student.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.IncrementLib;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;

namespace ETMS.Business
{
    public class Student2BLL : IStudent2BLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly IAiface _aiface;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IStudentCheckOnLogDAL _studentCheckOnLogDAL;

        private readonly IClassDAL _classDAL;

        private readonly ICourseDAL _courseDAL;

        public Student2BLL(IStudentDAL studentDAL, IAiface aiface, IUserOperationLogDAL userOperationLogDAL,
            IStudentCheckOnLogDAL studentCheckOnLogDAL, IClassDAL classDAL, ICourseDAL courseDAL)
        {
            this._studentDAL = studentDAL;
            this._aiface = aiface;
            this._userOperationLogDAL = userOperationLogDAL;
            this._studentCheckOnLogDAL = studentCheckOnLogDAL;
            this._classDAL = classDAL;
            this._courseDAL = courseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._aiface.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _userOperationLogDAL, _studentCheckOnLogDAL, _classDAL, _courseDAL);
        }

        public async Task<ResponseBase> StudentFaceListGet(StudentFaceListGetRequest request)
        {
            var studentFaces = await _studentDAL.GetStudentFace();
            var output = new List<StudentFaceListGetOutput>();
            if (studentFaces.Any())
            {
                foreach (var p in studentFaces)
                {
                    output.Add(new StudentFaceListGetOutput()
                    {
                        StudentId = p.Id,
                        FaceUrl = AliyunOssUtil.GetAccessUrlHttps(p.FaceGreyKey)
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentRelieveFace(StudentRelieveFaceKeyRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            if (string.IsNullOrEmpty(student.FaceKey) && string.IsNullOrEmpty(student.FaceGreyKey))
            {
                return ResponseBase.CommonError("人脸信息已清除");
            }
            await _aiface.StudentClearFace(student.Id);
            await _studentDAL.StudentRelieveFaceKey(student.Id);
            AliyunOssUtil.DeleteObject(student.FaceKey, student.FaceGreyKey);

            await _userOperationLogDAL.AddUserLog(request, $"清除人脸信息：姓名:{student.Name},手机号码:{student.Phone}", Entity.Enum.EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentBindingFace(StudentBindingFaceKeyRequest request)
        {
            var studentBucket = await _studentDAL.GetStudent(request.CId);
            if (studentBucket == null || studentBucket.Student == null)
            {
                return ResponseBase.CommonError("学员不存在");
            }
            var student = studentBucket.Student;
            var imgOssKey = ImageLib.SaveStudentFace(request.LoginTenantId, request.FaceImageBase64);
            var initFaceResult = await _aiface.StudentInitFace(request.CId, AliyunOssUtil.GetAccessUrlHttps(imgOssKey));
            if (!initFaceResult)
            {
                return ResponseBase.CommonError("保存人脸信息失败");
            }
            await _studentDAL.StudentBindingFaceKey(request.CId, imgOssKey, imgOssKey);

            if (!string.IsNullOrEmpty(student.FaceKey) || !string.IsNullOrEmpty(student.FaceGreyKey))
            {
                AliyunOssUtil.DeleteObject(student.FaceKey, student.FaceGreyKey);
            }

            await _userOperationLogDAL.AddUserLog(request, $"采集学员人脸信息：姓名:{student.Name},手机号码:{student.Phone}", Entity.Enum.EmUserOperationType.StudentManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCheckOnLogGetPaging(StudentCheckOnLogGetPagingRequest request)
        {
            var pagingData = await _studentCheckOnLogDAL.GetPaging(request);
            var output = new List<StudentCheckOnLogGetPagingOutput>();
            var tempBoxStudent = new DataTempBox<EtStudent>();
            var tempBoxClass = new DataTempBox<EtClass>();
            var tempBoxCourse = new DataTempBox<EtCourse>();
            foreach (var p in pagingData.Item1)
            {
                var student = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                if (student == null)
                {
                    continue;
                }
                var explain = string.Empty;
                if (p.CheckType == EmStudentCheckOnLogCheckType.CheckIn)
                {
                    switch (p.Status)
                    {
                        case EmStudentCheckOnLogStatus.NormalNotClass:
                            explain = "签到成功";
                            break;
                        case EmStudentCheckOnLogStatus.NormalAttendClass:
                        case EmStudentCheckOnLogStatus.BeRollcall:
                            var ckClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.ClassId.Value);
                            var ckCourse = await ComBusiness.GetCourseName(tempBoxCourse, _courseDAL, p.CourseId.Value);
                            explain = $"签到成功 - 记上课：班级:{ckClass?.Name},上课时间:{p.ClassOtDesc},消耗课程:{ckCourse},扣课时:{ComBusiness2.GetDeClassTimesDesc(p.DeType, p.DeClassTimes, p.ExceedClassTimes)}";
                            break;
                        case EmStudentCheckOnLogStatus.Revoke:
                            explain = "签到成功 - 已撤销记上课";
                            break;
                    }
                }
                else
                {
                    explain = "签退成功";
                }
                output.Add(new StudentCheckOnLogGetPagingOutput()
                {
                    CheckForm = p.CheckForm,
                    CheckFormDesc = EmStudentCheckOnLogCheckForm.GetStudentCheckOnLogCheckFormDesc(p.CheckForm),
                    CheckOt = p.CheckOt,
                    CheckMedium = GetCheckMedium(p.CheckForm, p.CheckMedium),
                    CheckType = p.CheckType,
                    CheckTypeDesc = EmStudentCheckOnLogCheckType.GetStudentCheckOnLogCheckTypeDesc(p.CheckType),
                    Status = p.Status,
                    StudentCheckOnLogId = p.Id,
                    StudentId = p.StudentId,
                    StudentName = student.Name,
                    StudentPhone = student.Phone,
                    Explain = explain
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentCheckOnLogGetPagingOutput>(pagingData.Item2, output));
        }

        private string GetCheckMedium(byte checkForm, string checkMedium)
        {
            switch (checkForm)
            {
                case EmStudentCheckOnLogCheckForm.Card:
                    return AliyunOssUtil.GetAccessUrlHttps(checkMedium);
                case EmStudentCheckOnLogCheckForm.Face:
                    return checkMedium;
                case EmStudentCheckOnLogCheckForm.TeacherManual:
                    return checkMedium;
            }
            return string.Empty;
        }

        public async Task<ResponseBase> StudentCheckByCard(StudentCheckByCardRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCheckByTeacher(StudentCheckByTeacherRequest request)
        {
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentCheckByFace(StudentCheckByFaceRequest request) { return ResponseBase.Success(); }

        public async Task<ResponseBase> StudentCheckByFace2(StudentCheckByFace2Request request) { return ResponseBase.Success(); }

        public async Task<ResponseBase> StudentCheckOnLogRevoke(StudentCheckOnLogRevokeRequest request) { return ResponseBase.Success(); }
    }
}
