using ETMS.IBusiness;
using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Open2.Request;
using ETMS.IBusiness.Wechart;
using ETMS.LOG;
using ETMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using ETMS.Entity.Dto.OpenParent.Request;
using ETMS.Business.Common;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Parent.Output;
using ETMS.Entity.Dto.OpenParent.Output;
using System.Collections.Generic;
using ETMS.Entity.Dto.Student.Output;
using System.Linq;

namespace ETMS.Business
{
    public class OpenParentBLL : IOpenParentBLL
    {
        private readonly IParentBLL _parentBLL;

        private readonly IStudentExtendFieldDAL _studentExtendFieldDAL;

        private readonly IStudentDAL _studentDAL;

        public OpenParentBLL(IParentBLL parentBLL, IStudentExtendFieldDAL studentExtendFieldDAL, IStudentDAL studentDAL)
        {
            this._parentBLL = parentBLL;
            this._studentExtendFieldDAL = studentExtendFieldDAL;
            this._studentDAL = studentDAL;
        }

        private void InitTenantId(int tenantId)
        {
            this._studentExtendFieldDAL.InitTenantId(tenantId);
            this._studentDAL.InitTenantId(tenantId);
        }

        public async Task<ResponseBase> ParentLoginSendSms(ParentOpenLoginSendSmsRequest request)
        {
            return await _parentBLL.ParentLoginSendSms(new ParentLoginSendSmsRequest()
            {
                Code = string.Empty,
                Phone = request.Phone,
                TenantNo = request.TenantNo
            });
        }

        public async Task<ResponseBase> ParentLoginBySms(ParentOpenLoginBySmsRequest request)
        {
            var res = await _parentBLL.ParentLoginBySms(new ParentLoginBySmsRequest()
            {
                Code = string.Empty,
                IpAddress = string.Empty,
                Phone = request.Phone,
                SmsCode = request.SmsCode,
                StudentWechartId = string.Empty,
                TenantNo = request.TenantNo
            });
            if (res.IsResponseSuccess())
            {
                var result = res.resultData as ParentLoginBySmsOutput;
                return ResponseBase.Success(new ParentLoginBySmsOpenOutput()
                {
                    LoginStatus = ParentLoginBySmsOutputLoginStatus.Success,
                    ExpiresIn = result.ExpiresIn,
                    L = result.L,
                    S = result.S
                });
            }
            if (res.ExtCode == StatusCode.ParentUnBindStudent)
            {
                return ResponseBase.Success(new ParentLoginBySmsOpenOutput()
                {
                    LoginStatus = ParentLoginBySmsOutputLoginStatus.Unregistered
                });
            }
            return res;
        }

        public async Task<ResponseBase> ParentRegisterSendSms(ParentRegisterSendSmsRequest request)
        {
            return await _parentBLL.ParentLoginSendSms2(new ParentLoginSendSmsRequest()
            {
                Code = string.Empty,
                Phone = request.Phone,
                TenantNo = request.TenantNo
            });
        }

        public async Task<ResponseBase> ParentRegister(ParentRegisterOpenRequest request)
        {
            return await _parentBLL.ParentRegister(new ParentRegisterRequest()
            {
                Address = request.Address,
                Phone = request.Phone,
                Remark = request.Remark,
                StudentName = request.StudentName,
                SmsCode = request.SmsCode,
                TenantNo = request.TenantNo,
                Code = string.Empty
            });
        }

        private Tuple<int, long?> GetRegisterInfo(string tno)
        {
            var strSp = tno.Split("_");
            long? trackUser = null;
            var tenantId = TenantLib.GetIdDecrypt2(strSp[0]);
            if (strSp.Length > 1)
            {
                trackUser = TenantLib.GetIdDecrypt(strSp[1]);
            }
            return Tuple.Create(tenantId, trackUser);
        }

        public async Task<ResponseBase> StudentOpenRegisterInit(StudentOpenRegisterInitRequest request)
        {
            var registerInfo = GetRegisterInfo(request.Tno);
            this.InitTenantId(registerInfo.Item1);
            var output = new StudentOpenRegisterInitOutput()
            {
                StudentExtendItems = new List<StudentExtendItemOutput>()
            };
            var studentExtendFileds = await _studentExtendFieldDAL.GetAllStudentExtendField();
            foreach (var file in studentExtendFileds)
            {
                output.StudentExtendItems.Add(new StudentExtendItemOutput()
                {
                    FieldId = file.Id,
                    FieldDisplayName = file.DisplayName,
                    Value = string.Empty
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentOpenRegisterSendSms(StudentOpenRegisterSendSmsRequest request)
        {
            var registerInfo = GetRegisterInfo(request.Tno);
            return await _parentBLL.ParentLoginSendSms3(new ParentLoginSendSms3Request()
            {
                Code = string.Empty,
                Phone = request.Phone,
                TenantId = registerInfo.Item1
            });
        }

        public async Task<ResponseBase> StudentPhoneCheck(StudentPhoneCheckRequest request)
        {
            var registerInfo = GetRegisterInfo(request.Tno);
            var output = new StudentPhoneCheckOutput()
            {
                IsHas = false
            };
            this.InitTenantId(registerInfo.Item1);
            var myStudent = await _studentDAL.GetStudentsByPhoneOrNameOne(request.Phone);
            if (myStudent != null)
            {
                output.IsHas = true;
                output.StudentInfo = new StudentPhoneInfo()
                {
                    StudentName = myStudent.Name,
                };
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> StudentOpenRegisterSubmit(StudentOpenRegisterSubmitRequest request)
        {
            var registerInfo = GetRegisterInfo(request.Tno);
            return await _parentBLL.ParentRegister2(new ParentRegister2Request()
            {
                TenantId = registerInfo.Item1,
                TrackUser = registerInfo.Item2,
                Name = request.Name,
                Phone = request.Phone,
                SmsCode = request.SmsCode,
                StudentExtendItems = request.StudentExtendItems,
                Address = request.Address,
                Remark = request.Remark,
                Gender = request.Gender,
                RecommenderPhone = request.RecommenderPhone
            });
        }
    }
}
