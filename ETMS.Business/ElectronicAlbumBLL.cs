using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.Interaction.Output;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.ElectronicAlbum;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ElectronicAlbumBLL : IElectronicAlbumBLL
    {
        private readonly IElectronicAlbumTempDAL _electronicAlbumTempDAL;

        private readonly IElectronicAlbumDAL _electronicAlbumDAL;

        private readonly IElectronicAlbumDetailDAL _electronicAlbumDetailDAL;

        private readonly ISysElectronicAlbumDAL _sysElectronicAlbumDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IClassDAL _classDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IEventPublisher _eventPublisher;

        private readonly ILibMediaDAL _libMediaDAL;

        private readonly IUserDAL _userDAL;

        private readonly IElectronicAlbumStatisticsDAL _electronicAlbumStatisticsDAL;

        public ElectronicAlbumBLL(IElectronicAlbumTempDAL electronicAlbumTempDAL, IElectronicAlbumDAL electronicAlbumDAL,
           IElectronicAlbumDetailDAL electronicAlbumDetailDAL, ISysElectronicAlbumDAL sysElectronicAlbumDAL,
           IStudentDAL studentDAL, IClassDAL classDAL, IUserOperationLogDAL userOperationLogDAL, IEventPublisher eventPublisher,
           ILibMediaDAL libMediaDAL, IUserDAL userDAL, IElectronicAlbumStatisticsDAL electronicAlbumStatisticsDAL)
        {
            this._electronicAlbumTempDAL = electronicAlbumTempDAL;
            this._electronicAlbumDAL = electronicAlbumDAL;
            this._electronicAlbumDetailDAL = electronicAlbumDetailDAL;
            this._sysElectronicAlbumDAL = sysElectronicAlbumDAL;
            this._studentDAL = studentDAL;
            this._classDAL = classDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._eventPublisher = eventPublisher;
            this._libMediaDAL = libMediaDAL;
            this._userDAL = userDAL;
            this._electronicAlbumStatisticsDAL = electronicAlbumStatisticsDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _electronicAlbumTempDAL, _electronicAlbumDAL, _electronicAlbumDetailDAL,
                _studentDAL, _classDAL, _userOperationLogDAL, _libMediaDAL, _userDAL, _electronicAlbumStatisticsDAL);
        }

        public async Task<ResponseBase> SysElectronicAlbumGetPaging(SysElectronicAlbumGetPagingRequest request)
        {
            var pagingData = await _sysElectronicAlbumDAL.GetPaging(request);
            var output = new List<SysElectronicAlbumGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new SysElectronicAlbumGetPagingOutput()
                {
                    CId = p.Id,
                    CoverKey = p.CoverKey,
                    CoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.CoverKey),
                    Name = p.Name,
                    RenderUrl = AliyunOssUtil.GetAccessUrlHttps(p.RenderKey),
                    Type = p.Type
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<SysElectronicAlbumGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ElectronicAlbumGetPaging(ElectronicAlbumGetPagingRequest request)
        {
            var pagingData = await _electronicAlbumDAL.GetPaging(request);
            var output = new List<ElectronicAlbumGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                var tempBoxClass = new DataTempBox<EtClass>();
                var tempBoxUser = new DataTempBox<EtUser>();
                var vtNo = EtmsHelper3.OpenLinkGetVtNo(request.LoginTenantId, request.LoginUserId);
                foreach (var p in pagingData.Item1)
                {
                    var relatedDesc = string.Empty;
                    var typeDesc = string.Empty;
                    var userName = string.Empty;
                    if (p.Type == EmElectronicAlbumMyType.Student)
                    {
                        var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.RelatedId);
                        if (myStudent == null)
                        {
                            continue;
                        }
                        relatedDesc = myStudent.Name;
                        typeDesc = "学员";
                    }
                    else
                    {
                        var myClass = await ComBusiness.GetClass(tempBoxClass, _classDAL, p.RelatedId);
                        if (myClass == null)
                        {
                            continue;
                        }
                        relatedDesc = myClass.Name;
                        typeDesc = "班级";
                    }
                    var user = await ComBusiness.GetUser(tempBoxUser, _userDAL, p.UserId);
                    if (user != null)
                    {
                        userName = user.Name;
                    }
                    output.Add(new ElectronicAlbumGetPagingOutput()
                    {
                        CId = p.Id,
                        CIdNo = p.CIdNo,
                        CoverKey = p.CoverKey,
                        CoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.CoverKey),
                        CreateTime = p.CreateTime,
                        Name = p.Name,
                        ReadCount = p.ReadCount,
                        ShareCount = p.ShareCount,
                        RelatedDesc = relatedDesc,
                        TypeDesc = typeDesc,
                        Type = p.Type,
                        RelatedId = p.RelatedId,
                        RenderUrl = AliyunOssUtil.GetAccessUrlHttps(p.RenderKey),
                        Status = p.Status,
                        TempId = p.TempId,
                        TemplateId = p.TemplateId,
                        UpdateTime = p.UpdateTime,
                        UserId = p.UserId,
                        VtNo = vtNo,
                        UserName = userName
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ElectronicAlbumGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ElectronicAlbumCreateInit(ElectronicAlbumCreateInitRequest request)
        {
            var entity = new EtElectronicAlbumTemp()
            {
                IsDeleted = EmIsDeleted.Normal,
                CreateTime = DateTime.Now,
                Name = request.Name,
                RelatedId = request.RelatedId,
                TemplateId = request.TemplateId,
                TenantId = request.LoginTenantId,
                Type = request.Type.Value,
                UserId = request.LoginUserId
            };
            await _electronicAlbumTempDAL.AddElectronicAlbumTemp(entity);

            await _userOperationLogDAL.AddUserLog(request, $"创建电子相册-{request.Name}", EmUserOperationType.ElectronicAlbumMgr);
            return ResponseBase.Success(new ElectronicAlbumCreateInitOutput()
            {
                TempIdNo = EtmsHelper3.OpenLinkGetIdEncrypt(entity.Id),
                VtNo = EtmsHelper3.OpenLinkGetVtNo(request.LoginTenantId, request.LoginUserId)
            });
        }

        public async Task<ResponseBase> ElectronicAlbumPageInit(ElectronicAlbumPageInitRequest request)
        {
            ElectronicAlbumPageInitOutput output = null;
            if (!string.IsNullOrEmpty(request.CIdNo)) //编辑
            {
                var id = EtmsHelper3.OpenLinkGetIdDecrypt(request.CIdNo);
                var myElectronicAlbum = await _electronicAlbumDAL.GetElectronicAlbum(id);
                if (myElectronicAlbum == null)
                {
                    return ResponseBase.CommonError("相册不存在");
                }
                output = new ElectronicAlbumPageInitOutput()
                {
                    RenderKey = myElectronicAlbum.RenderKey,
                    RenderUrl = AliyunOssUtil.GetAccessUrlHttps(myElectronicAlbum.RenderKey),
                    NewRenderKey = myElectronicAlbum.RenderKey,
                    NewCoverKey = myElectronicAlbum.CoverKey,
                    Name = myElectronicAlbum.Name
                };
            }
            else
            {
                var tempId = EtmsHelper3.OpenLinkGetIdDecrypt(request.TempIdNo);
                var myTempElectronicAlbum = await _electronicAlbumTempDAL.GetElectronicAlbumTemp(tempId);
                if (myTempElectronicAlbum == null)
                {
                    return ResponseBase.CommonError("相册信息不存在");
                }
                var mySysElectronicAlbum = await _sysElectronicAlbumDAL.GetElectronicAlbum(myTempElectronicAlbum.TemplateId);
                if (mySysElectronicAlbum == null)
                {
                    return ResponseBase.CommonError("相册不存在");
                }
                var strDate = DateTime.Now.ToString("yyyyMMdd");
                var jsonKey = $"{strDate}/{tempId}.json";
                var imgKey = $"{strDate}/{tempId}.png";
                output = new ElectronicAlbumPageInitOutput()
                {
                    RenderKey = mySysElectronicAlbum.RenderKey,
                    RenderUrl = AliyunOssUtil.GetAccessUrlHttps(mySysElectronicAlbum.RenderKey),
                    NewRenderKey = AliyunOssUtil.GetFullKey(request.LoginTenantId, jsonKey, AliyunOssFileTypeEnum.AlbumLb),
                    NewCoverKey = AliyunOssUtil.GetFullKey(request.LoginTenantId, imgKey, AliyunOssFileTypeEnum.AlbumLb),
                    Name = myTempElectronicAlbum.Name
                };
            }
            var myImages = await _libMediaDAL.GetImages(EmLibType.ElectronicAlbum);
            output.ImgList = new List<AlbumLibImg>();
            if (myImages.Any())
            {
                foreach (var p in myImages)
                {
                    output.ImgList.Add(new AlbumLibImg()
                    {
                        ImgUrl = p.ImgUrl
                    });
                }
            }
            var myAudios = await _libMediaDAL.GetAudios(EmLibType.ElectronicAlbum);
            output.AudioList = new List<AlbumLibAudio>();
            if (myAudios.Any())
            {
                foreach (var p in myAudios)
                {
                    output.AudioList.Add(new AlbumLibAudio()
                    {
                        AudioUrl = p.AudioUrl,
                        Name = p.Name
                    });
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> ElectronicAlbumSave(ElectronicAlbumSaveRequest request)
        {
            return await ElectronicAlbumEditOrPublish(request, false);
        }

        public async Task<ResponseBase> ElectronicAlbumPublish(ElectronicAlbumPublishRequest request)
        {
            return await ElectronicAlbumEditOrPublish(request, true);
        }

        private async Task<ResponseBase> ElectronicAlbumEditOrPublish(ElectronicAlbumEditOrPublishRequest request, bool isPublish)
        {
            if (!string.IsNullOrEmpty(request.CIdNo)) //编辑
            {
                var id = EtmsHelper3.OpenLinkGetIdDecrypt(request.CIdNo);
                var myElectronicAlbum = await _electronicAlbumDAL.GetElectronicAlbum(id);
                if (myElectronicAlbum == null)
                {
                    return ResponseBase.CommonError("相册不存在");
                }
                return await ElectronicAlbumEdit(myElectronicAlbum, request, isPublish);
            }
            else
            {
                var tempId = EtmsHelper3.OpenLinkGetIdDecrypt(request.TempIdNo);
                var myTempElectronicAlbum = await _electronicAlbumTempDAL.GetElectronicAlbumTemp(tempId);
                if (myTempElectronicAlbum == null)
                {
                    return ResponseBase.CommonError("相册信息不存在");
                }
                var oldEntity = await _electronicAlbumDAL.GetElectronicAlbumByTempId(tempId);
                if (oldEntity != null)
                {
                    return await ElectronicAlbumEdit(oldEntity, request, isPublish);
                }
                var newEntity = new EtElectronicAlbum()
                {
                    CreateTime = myTempElectronicAlbum.CreateTime,
                    CoverKey = request.CoverKey,
                    CIdNo = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Name = myTempElectronicAlbum.Name,
                    ReadCount = 0,
                    RelatedId = myTempElectronicAlbum.RelatedId,
                    RenderKey = request.RenderKey,
                    ShareCount = 0,
                    Status = EmElectronicAlbumStatus.Save,
                    TempId = tempId,
                    TemplateId = myTempElectronicAlbum.TemplateId,
                    TenantId = request.LoginTenantId,
                    Type = myTempElectronicAlbum.Type,
                    UpdateTime = null,
                    UserId = request.LoginUserId
                };
                if (isPublish)
                {
                    newEntity.Status = EmElectronicAlbumStatus.Push;
                }
                await _electronicAlbumDAL.AddElectronicAlbum(newEntity);
                newEntity.CIdNo = EtmsHelper3.OpenLinkGetIdEncrypt(newEntity.Id);
                await _electronicAlbumDAL.EditElectronicAlbum(newEntity);

                _eventPublisher.Publish(new ElectronicAlbumInitEvent(request.LoginTenantId)
                {
                    MyElectronicAlbum = newEntity
                });
                return ResponseBase.Success();
            }
        }

        private async Task<ResponseBase> ElectronicAlbumEdit(EtElectronicAlbum myElectronicAlbum,
            ElectronicAlbumEditOrPublishRequest request, bool isPublish)
        {
            var oldStatus = myElectronicAlbum.Status;
            if (isPublish)
            {
                myElectronicAlbum.Status = EmElectronicAlbumStatus.Push;
            }
            myElectronicAlbum.RenderKey = request.RenderKey;
            myElectronicAlbum.CoverKey = request.CoverKey;
            myElectronicAlbum.UpdateTime = DateTime.Now;
            await _electronicAlbumDAL.EditElectronicAlbum(myElectronicAlbum);
            await _electronicAlbumDetailDAL.EditElectronicAlbumDetail(myElectronicAlbum);

            if (isPublish && oldStatus != EmElectronicAlbumStatus.Push) //重新发布
            {
                _eventPublisher.Publish(new NoticeStudentAlbumEvent(myElectronicAlbum.TenantId)
                {
                    AlbumId = myElectronicAlbum.Id,
                    Name = myElectronicAlbum.Name,
                    Type = myElectronicAlbum.Type,
                    RelatedId = myElectronicAlbum.RelatedId,
                    Time = DateTime.Now
                });
            }
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ElectronicAlbumDel(ElectronicAlbumDelRequest request)
        {
            var myElectronicAlbum = await _electronicAlbumDAL.GetElectronicAlbum(request.CId);
            if (myElectronicAlbum == null)
            {
                return ResponseBase.CommonError("相册不存在");
            }
            await _electronicAlbumDAL.DelElectronicAlbum(request.CId);
            await _electronicAlbumDetailDAL.DelElectronicAlbumDetail(request.CId);
            AliyunOssUtil.DeleteObject(myElectronicAlbum.RenderKey);
            AliyunOssUtil.DeleteObject(myElectronicAlbum.CoverKey);
            await _userOperationLogDAL.AddUserLog(request, $"删除电子相册-{myElectronicAlbum.Name}", EmUserOperationType.ElectronicAlbumMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ElectronicAlbumEditSimple(ElectronicAlbumEditSimpleRequest request)
        {
            var myElectronicAlbum = await _electronicAlbumDAL.GetElectronicAlbum(request.Id);
            if (myElectronicAlbum == null)
            {
                return ResponseBase.CommonError("相册不存在");
            }
            var oldStatus = myElectronicAlbum.Status;
            myElectronicAlbum.Name = request.Name;
            myElectronicAlbum.Status = request.NewStatus;
            await _electronicAlbumDAL.EditElectronicAlbum(myElectronicAlbum);
            await _electronicAlbumDetailDAL.EditElectronicAlbumDetail(myElectronicAlbum);

            if (request.NewStatus == EmElectronicAlbumStatus.Push && oldStatus != EmElectronicAlbumStatus.Push) //重新发布
            {
                _eventPublisher.Publish(new NoticeStudentAlbumEvent(myElectronicAlbum.TenantId)
                {
                    AlbumId = myElectronicAlbum.Id,
                    Name = myElectronicAlbum.Name,
                    Type = myElectronicAlbum.Type,
                    RelatedId = myElectronicAlbum.RelatedId,
                    Time = DateTime.Now
                });
            }

            await _userOperationLogDAL.AddUserLog(request, $"编辑电子相册-{request.Name}", EmUserOperationType.ElectronicAlbumMgr);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ElectronicAlbumGet(ElectronicAlbumGetRequest request)
        {
            var p = await _electronicAlbumDAL.GetElectronicAlbum(request.Id);
            if (p == null)
            {
                return ResponseBase.CommonError("相册不存在");
            }
            var relatedDesc = string.Empty;
            var typeDesc = string.Empty;
            if (p.Type == EmElectronicAlbumMyType.Student)
            {
                var myStudentBucket = await _studentDAL.GetStudent(p.RelatedId);
                if (myStudentBucket != null && myStudentBucket.Student != null)
                {
                    relatedDesc = myStudentBucket.Student.Name;
                }
                typeDesc = "学员";
            }
            else
            {
                var myClassBucket = await _classDAL.GetClassBucket(p.RelatedId);
                if (myClassBucket != null && myClassBucket.EtClass != null)
                {
                    relatedDesc = myClassBucket.EtClass.Name;
                }
                typeDesc = "班级";
            }
            return ResponseBase.Success(new ElectronicAlbumGetOutput()
            {
                CId = p.Id,
                CIdNo = p.CIdNo,
                CoverKey = p.CoverKey,
                CoverUrl = AliyunOssUtil.GetAccessUrlHttps(p.CoverKey),
                CreateTime = p.CreateTime,
                Name = p.Name,
                ReadCount = p.ReadCount,
                ShareCount = p.ShareCount,
                RelatedDesc = relatedDesc,
                TypeDesc = typeDesc,
                Type = p.Type,
                RelatedId = p.RelatedId,
                RenderUrl = AliyunOssUtil.GetAccessUrlHttps(p.RenderKey),
                Status = p.Status,
                TempId = p.TempId,
                TemplateId = p.TemplateId,
                UpdateTime = p.UpdateTime,
                UserId = p.UserId
            });
        }

        public async Task<ResponseBase> ElectronicAlbumDetailGetPaging(ElectronicAlbumDetailGetPagingRequest request)
        {
            var pagingData = await _electronicAlbumDetailDAL.GetPaging(request);
            var output = new List<ElectronicAlbumDetailGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                var tempBoxStudent = new DataTempBox<EtStudent>();
                foreach (var p in pagingData.Item1)
                {
                    var myStudent = await ComBusiness.GetStudent(tempBoxStudent, _studentDAL, p.StudentId);
                    if (myStudent == null)
                    {
                        continue;
                    }
                    output.Add(new ElectronicAlbumDetailGetPagingOutput()
                    {
                        StudentId = p.StudentId,
                        ReadCount = p.ReadCount,
                        ShareCount = p.ShareCount,
                        StudentName = myStudent.Name
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ElectronicAlbumDetailGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ElectronicAlbumStatisticsRead(ElectronicAlbumStatisticsReadRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsData = await _electronicAlbumStatisticsDAL.GetReadLog(currentDate, endDate);
            var echartsBar = new EchartsBar<int>();
            while (currentDate <= endDate)
            {
                var myStatisticsStudentCount = statisticsData.FirstOrDefault(p => p.Ot == currentDate);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myStatisticsStudentCount == null ? 0 : myStatisticsStudentCount.ReadCount);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(echartsBar);
        }

        public async Task<ResponseBase> ElectronicAlbumStatisticShare(ElectronicAlbumStatisticShareRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsData = await _electronicAlbumStatisticsDAL.GetShareLog(currentDate, endDate);
            var echartsBar = new EchartsBar<int>();
            while (currentDate <= endDate)
            {
                var myStatisticsStudentCount = statisticsData.FirstOrDefault(p => p.Ot == currentDate);
                echartsBar.XData.Add(currentDate.ToString("MM-dd"));
                echartsBar.MyData.Add(myStatisticsStudentCount == null ? 0 : myStatisticsStudentCount.ShareCount);
                currentDate = currentDate.AddDays(1);
            }
            return ResponseBase.Success(echartsBar);
        }
    }
}
