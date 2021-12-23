using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.ConfigMgr.Output;
using ETMS.Entity.Dto.ConfigMgr.Request;
using ETMS.Entity.Enum;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.ShareTemplate;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ShareTemplateBLL : IShareTemplateBLL
    {
        private readonly IShareTemplateIdDAL _shareTemplateIdDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public ShareTemplateBLL(IShareTemplateIdDAL shareTemplateIdDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._shareTemplateIdDAL = shareTemplateIdDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _shareTemplateIdDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> ShareTemplateGet(ShareTemplateGetRequest request)
        {
            var log = await this._shareTemplateIdDAL.GetShareTemplate(request.Id);
            if (log == null)
            {
                return ResponseBase.CommonError("模板不存在");
            }
            return ResponseBase.Success(new ShareTemplateGetOutput()
            {
                Id = log.Id,
                ImgKey = log.ImgKey,
                ImgKeyUrl = AliyunOssUtil.GetAccessUrlHttps(log.ImgKey),
                IsSystem = log.IsSystem,
                Name = log.Name,
                Status = log.Status,
                Summary = log.Summary,
                Title = log.Title,
                Type = log.Type,
                TypeDesc = EmShareTemplateType.GetShareTemplateTypeDesc(log.Type),
                UseType = log.UseType,
                UseTypeDesc = EmShareTemplateUseType.GetShareTemplateUseTypeDesc(log.UseType)
            });
        }

        public async Task<ResponseBase> ShareTemplateAdd(ShareTemplateAddRequest request)
        {
            var now = DateTime.Now;
            await _shareTemplateIdDAL.AddShareTemplate(new EtShareTemplate()
            {
                CreateTime = now,
                ImgKey = request.ImgKey,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.False,
                Name = request.Name,
                Status = EmShareTemplateStatus.Disabled,
                Summary = request.Summary,
                TenantId = request.LoginTenantId,
                Title = request.Title,
                Type = request.Type,
                UpdateTime = null,
                UserId = request.LoginUserId,
                UseType = request.UseType
            });
            await _userOperationLogDAL.AddUserLog(request, $"添加分享链接模板-{request.Name}",
                EmUserOperationType.SystemConfigModify, now);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ShareTemplateEdit(ShareTemplateEditRequest request)
        {
            var log = await this._shareTemplateIdDAL.GetShareTemplate(request.Id);
            if (log == null)
            {
                return ResponseBase.CommonError("模板不存在");
            }
            var now = DateTime.Now;
            log.Name = request.Name;
            log.ImgKey = request.ImgKey;
            log.Summary = request.Summary;
            log.Title = request.Title;
            log.UpdateTime = now;
            await _shareTemplateIdDAL.EditShareTemplate(log);

            await _userOperationLogDAL.AddUserLog(request, $"编辑分享链接模板-{request.Name}",
                EmUserOperationType.SystemConfigModify, now);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ShareTemplateDel(ShareTemplateDelRequest request)
        {
            var log = await this._shareTemplateIdDAL.GetShareTemplate(request.Id);
            if (log == null)
            {
                return ResponseBase.CommonError("模板不存在");
            }
            if (log.IsSystem == EmBool.True)
            {
                return ResponseBase.CommonError("无法删除系统模板");
            }
            await _shareTemplateIdDAL.DelShareTemplate(log.Id, log.UseType);

            await _userOperationLogDAL.AddUserLog(request, $"删除分享链接模板-{log.Name}",
                EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ShareTemplateChangeStatus(ShareTemplateChangeStatusRequest request)
        {
            var log = await this._shareTemplateIdDAL.GetShareTemplate(request.Id);
            if (log == null)
            {
                return ResponseBase.CommonError("模板不存在");
            }
            if (log.Status == request.NewStatus)
            {
                return ResponseBase.Success();
            }
            await _shareTemplateIdDAL.ChangeShareTemplateStatus(log, request.NewStatus);

            var tag = request.NewStatus == EmShareTemplateStatus.Enabled ? "启用" : "禁用";
            await _userOperationLogDAL.AddUserLog(request, $"{tag}分享链接模板-{log.Name}", EmUserOperationType.SystemConfigModify);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> ShareTemplateGetPaging(ShareTemplateGetPagingRequest request)
        {
            var pagingData = await _shareTemplateIdDAL.GetPaging(request);
            var output = new List<ShareTemplateGetPagingOutput>();
            if (pagingData.Item1.Any())
            {
                foreach (var p in pagingData.Item1)
                {
                    output.Add(new ShareTemplateGetPagingOutput()
                    {
                        Id = p.Id,
                        ImgKeyUrl = AliyunOssUtil.GetAccessUrlHttps(p.ImgKey),
                        IsSystem = p.IsSystem,
                        Name = p.Name,
                        Status = p.Status,
                        Type = p.Type,
                        TypeDesc = EmShareTemplateType.GetShareTemplateTypeDesc(p.Type),
                        UseType = p.UseType,
                        UseTypeDesc = EmShareTemplateUseType.GetShareTemplateUseTypeDesc(p.UseType),
                        UpdateTime = p.UpdateTime
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<ShareTemplateGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
