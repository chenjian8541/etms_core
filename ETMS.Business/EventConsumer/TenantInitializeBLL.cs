using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess.ShareTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    /// <summary>
    /// 初始化机构数据，可以重复执行
    /// </summary>
    public class TenantInitializeBLL : ITenantInitializeBLL
    {
        private readonly IShareTemplateIdDAL _shareTemplateIdDAL;

        private int _tenantId;

        public TenantInitializeBLL(IShareTemplateIdDAL shareTemplateIdDAL)
        {
            this._shareTemplateIdDAL = shareTemplateIdDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._tenantId = tenantId;
            this.InitDataAccess(tenantId, _shareTemplateIdDAL);
        }

        public async Task TenantInitializeConsumerEvent(TenantInitializeEvent request)
        {
            await this.InitShareTemplateDefault();
        }

        /// <summary>
        /// 初始化分享展示模板
        /// </summary>
        /// <returns></returns>
        private async Task InitShareTemplateDefault()
        {
            var isInit = await _shareTemplateIdDAL.IsInitializeSystemData();
            if (isInit)
            {
                return;
            }

            var now = DateTime.Now;
            var entitys = new List<EtShareTemplate>();
            entitys.Add(new EtShareTemplate()
            {
                Type = EmShareTemplateType.Link,
                UseType = EmShareTemplateUseType.Growth,
                Name = "默认成长档案分享链接模板",
                Title = "{{学员姓名}}同学的成长档案",
                Summary = "{{档案内容}}",
                ImgKey = "system/material/share/link/4.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = _tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            entitys.Add(new EtShareTemplate()
            {
                Type = EmShareTemplateType.Link,
                UseType = EmShareTemplateUseType.ClassEvaluate,
                Name = "默认课后点评分享链接模板",
                Title = "{{点评老师}}老师对{{学员姓名}}在{{课程名称}}的上课点评",
                Summary = "{{点评内容}}",
                ImgKey = "system/material/share/link/3.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = _tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            entitys.Add(new EtShareTemplate()
            {
                Type = EmShareTemplateType.Link,
                UseType = EmShareTemplateUseType.StudentPhoto,
                Name = "默认电子相册分享链接模板",
                Title = "{{学员姓名}}的相册",
                Summary = "{{相册标题}}",
                ImgKey = "system/material/share/link/1.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = _tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            entitys.Add(new EtShareTemplate()
            {
                Type = EmShareTemplateType.Link,
                UseType = EmShareTemplateUseType.MicWebsite,
                Name = "默认微官网分享链接模板",
                Title = "{{机构名称}}",
                Summary = "{{机构名称}}的官网 点击进入！！",
                ImgKey = "system/material/share/link/5.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = _tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            entitys.Add(new EtShareTemplate()
            {
                Type = EmShareTemplateType.Link,
                UseType = EmShareTemplateUseType.OnlineMall,
                Name = "默认在线商城分享链接模板",
                Title = "在线商城-{{机构名称}}",
                Summary = "在线报名、购课，在线选择班级",
                ImgKey = "system/material/share/link/2.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = _tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            entitys.Add(new EtShareTemplate()
            {
                Type = EmShareTemplateType.ShowTemplate,
                UseType = EmShareTemplateUseType.Growth,
                Name = "默认成长档案展示模板",
                Title = "{{学员姓名}}同学的成长档案",
                Summary = "{{档案类型}}",
                ImgKey = "system/material/share/poster/9.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = _tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            entitys.Add(new EtShareTemplate()
            {
                Type = EmShareTemplateType.ShowTemplate,
                UseType = EmShareTemplateUseType.ClassEvaluate,
                Name = "默认课后点评展示模板",
                Title = "{{学员姓名}}的课程『{{课程名称}}』",
                Summary = "来自{{点评老师}}老师的点评",
                ImgKey = "system/material/share/poster/6.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = _tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            _shareTemplateIdDAL.AddShareTemplate(entitys);
        }
    }
}
