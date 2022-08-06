using ETMS.Entity.CacheBucket.ShareTemplate;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Lib
{
    public class ShareTemplateLib
    {
        private static List<EtShareTemplate> _sysShareTemplates;

        public static List<EtShareTemplate> GetNewSysInitShareTemplates(int tenantId)
        {
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
                TenantId = tenantId,
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
                TenantId = tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            entitys.Add(new EtShareTemplate()
            {
                Type = EmShareTemplateType.Link,
                UseType = EmShareTemplateUseType.Achievement,
                Name = "默认成绩单分享链接模板",
                Title = "{{学员姓名}}同学的成绩单",
                Summary = "{{考试名称}}",
                ImgKey = "system/material/share/link/3.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = tenantId,
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
                TenantId = tenantId,
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
                TenantId = tenantId,
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
                TenantId = tenantId,
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
                TenantId = tenantId,
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
                TenantId = tenantId,
                CreateTime = now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            });
            return entitys;
        }

        internal static ShareTemplateUseTypeBucket GetShareTemplate(int useType)
        {
            var myShareTemplateLink = _sysShareTemplates.FirstOrDefault(p => p.UseType == useType && p.Type == EmShareTemplateType.Link);
            var myShareTemplatePoster = _sysShareTemplates.FirstOrDefault(p => p.UseType == useType && p.Type == EmShareTemplateType.ShowTemplate);
            return new ShareTemplateUseTypeBucket()
            {
                MyShareTemplateLink = myShareTemplateLink,
                MyShareTemplateShow = myShareTemplatePoster
            };
        }

        static ShareTemplateLib()
        {
            _sysShareTemplates = GetNewSysInitShareTemplates(0);
        }
    }
}
