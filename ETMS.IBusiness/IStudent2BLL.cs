﻿using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudent2BLL : IBaseBLL
    {
        Task<ResponseBase> StudentFaceListGet(StudentFaceListGetRequest request);

        Task<ResponseBase> StudentRelieveFace(StudentRelieveFaceKeyRequest request);

        Task<ResponseBase> StudentBindingFace(StudentBindingFaceKeyRequest request);

        Task<ResponseBase> StudentCheckOnLogGetPaging(StudentCheckOnLogGetPagingRequest request);

        Task<ResponseBase> StudentCheckByCard(StudentCheckByCardRequest request);

        Task<ResponseBase> StudentCheckByTeacher(StudentCheckByTeacherRequest request);

        Task<ResponseBase> StudentCheckByFace(StudentCheckByFaceRequest request);

        Task<ResponseBase> StudentCheckByFace2(StudentCheckByFace2Request request);

        Task<ResponseBase> StudentCheckChoiceClass(StudentCheckChoiceClassRequest request);

        Task<ResponseBase> StudentCheckOnLogRevoke(StudentCheckOnLogRevokeRequest request);

        Task<ResponseBase> StudentNeedCheckStatistics(StudentNeedCheckStatisticsRequest request);

        Task<ResponseBase> StudentNeedCheckInGetPaging(StudentNeedCheckInGetPagingRequest request);

        Task<ResponseBase> StudentNeedCheckOutGetPaging(StudentNeedCheckOutGetPagingRequest request);

        Task<ResponseBase> StudentNeedAttendClassGetPaging(StudentNeedAttendClassGetPagingRequest request);

        Task<ResponseBase> StudentNeedLogCheckIn(StudentNeedLogCheckInRequest request);

        Task<ResponseBase> StudentNeedLogCheckOut(StudentNeedLogCheckOutRequest request);

        Task<ResponseBase> StudentNeedLogAttendClass(StudentNeedLogAttendClassRequest request);
    }
}