using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class LibMediaFliesBLL : ILibMediaFliesBLL
    {
        private readonly ILibMediaDAL _libMediaDAL;

        public LibMediaFliesBLL(ILibMediaDAL libMediaDAL)
        {
            this._libMediaDAL = libMediaDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _libMediaDAL);
        }

        public async Task<ResponseBase> ImageGetPaging(ImageGetPagingRequest request)
        {
            var pagingData = await _libMediaDAL.GetPagingImg(request);
            var output = new List<ImageGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new ImageGetPagingOutput()
                {
                    CId = p.Id,
                    ImgKey = p.ImgKey,
                    ImgUrl = p.ImgUrl,
                    Type = p.Type
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ImageGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> ImageAdd(ImageAddRequest request)
        {
            await _libMediaDAL.AddImage(new EtLibImages()
            {
                CreateTime = DateTime.Now,
                ImgKey = request.ImgKey,
                ImgUrl = AliyunOssUtil.GetAccessUrlHttps(request.ImgKey),
                IsDeleted = EmIsDeleted.Normal,
                TenantId = request.LoginTenantId,
                Type = request.Type
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ImageDel(ImageDelRequest request)
        {
            AliyunOssUtil.DeleteObject(request.Key);
            await _libMediaDAL.DelImage(request.CId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ImageListGet(ImageListGetRequest request)
        {
            var output = new List<ImageListGetOutput>();
            var logs = await _libMediaDAL.GetImages(request.Type.Value);
            foreach (var p in logs)
            {
                output.Add(new ImageListGetOutput()
                {
                    CId = p.Id,
                    ImgUrl = p.ImgUrl
                });
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> AudioGetPaging(AudioGetPagingRequest request)
        {
            var pagingData = await _libMediaDAL.GetPagingAudio(request);
            var output = new List<AudioGetPagingOutput>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new AudioGetPagingOutput()
                {
                    CId = p.Id,
                    Type = p.Type,
                    AudioKey = p.AudioKey,
                    AudioUrl = p.AudioUrl,
                    Name = p.Name
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<AudioGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> AudioAdd(AudioAddRequest request)
        {
            await _libMediaDAL.AddAudio(new EtLibAudios()
            {
                AudioKey = request.AudioKey,
                AudioUrl = AliyunOssUtil.GetAccessUrlHttps(request.AudioKey),
                CreateTime = DateTime.Now,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = request.LoginTenantId,
                Type = request.Type,
                Name = request.Name
            });
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AudioDel(AudioDelRequest request)
        {
            AliyunOssUtil.DeleteObject(request.Key);
            await _libMediaDAL.DelAudio(request.CId);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> AudioListGet(AudioListGetRequest request)
        {
            var output = new List<AudioListGetOutput>();
            var logs = await _libMediaDAL.GetAudios(request.Type.Value);
            foreach (var p in logs)
            {
                output.Add(new AudioListGetOutput()
                {
                    CId = p.Id,
                    AudioUrl = p.AudioUrl
                });
            }
            return ResponseBase.Success(output);
        }
    }
}
