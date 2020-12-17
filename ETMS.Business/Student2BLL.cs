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

namespace ETMS.Business
{
    public class Student2BLL : IStudent2BLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly IAiface _aiface;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public Student2BLL(IStudentDAL studentDAL, IAiface aiface, IUserOperationLogDAL userOperationLogDAL)
        {
            this._studentDAL = studentDAL;
            this._aiface = aiface;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._aiface.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _userOperationLogDAL);
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
    }
}
