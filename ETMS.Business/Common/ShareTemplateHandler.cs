using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Common.Output;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    public class ShareTemplateHandler
    {
        #region 成长档案

        private static string ReplaceContentGrowth(string template, string studentName, string growthContent,
            string growthContentTagDesc, string growthOtDesc)
        {
            return template
                .Replace("{{学员姓名}}", studentName)
                .Replace("{{档案内容}}", growthContent)
                .Replace("{{档案类型}}", growthContentTagDesc)
                .Replace("{{档案日期}}", growthOtDesc);
        }

        public static ShareContent TemplateLinkGrowth(EtShareTemplate myTemplate, string studentName, string growthContent,
            string growthContentTagDesc, string growthOtDesc)
        {
            if (myTemplate == null)
            {
                return null;
            }
            return new ShareContent()
            {
                Title = ReplaceContentGrowth(myTemplate.Title, studentName, growthContent, growthContentTagDesc, growthOtDesc),
                Summary = ReplaceContentGrowth(myTemplate.Summary, studentName, growthContent, growthContentTagDesc, growthOtDesc),
                ImgUrl = AliyunOssUtil.GetAccessUrlHttps(myTemplate.ImgKey)
            };
        }

        public static ShowContent TemplateShowGrowth(EtShareTemplate myTemplate, string studentName, string growthContent,
            string growthContentTagDesc, string growthOtDesc)
        {
            if (myTemplate == null)
            {
                return null;
            }
            return new ShowContent()
            {
                Title = ReplaceContentGrowth(myTemplate.Title, studentName, growthContent, growthContentTagDesc, growthOtDesc),
                Summary = ReplaceContentGrowth(myTemplate.Summary, studentName, growthContent, growthContentTagDesc, growthOtDesc),
                ImgUrl = AliyunOssUtil.GetAccessUrlHttps(myTemplate.ImgKey)
            };
        }

        #endregion

        #region 课后点评

        private static string ReplaceContentClassEvaluate(string template, string studentName, string className, string courseName,
            string classOt, string evaluateContent, string teacherName)
        {
            return template
                .Replace("{{学员姓名}}", studentName)
                .Replace("{{班级名称}}", className)
                .Replace("{{课程名称}}", courseName)
                .Replace("{{上课日期}}", classOt)
                .Replace("{{点评内容}}", evaluateContent)
                .Replace("{{点评老师}}", teacherName);
        }

        public static ShareContent TemplateLinkClassEvaluate(EtShareTemplate myTemplate, string studentName, string className, string courseName,
            string classOt, string evaluateContent, string teacherName)
        {
            if (myTemplate == null)
            {
                return null;
            }
            return new ShareContent()
            {
                Title = ReplaceContentClassEvaluate(myTemplate.Title, studentName, className, courseName, classOt, evaluateContent, teacherName),
                Summary = ReplaceContentClassEvaluate(myTemplate.Summary, studentName, className, courseName, classOt, evaluateContent, teacherName),
                ImgUrl = AliyunOssUtil.GetAccessUrlHttps(myTemplate.ImgKey)
            };
        }

        public static ShowContent TemplateShowClassEvaluate(EtShareTemplate myTemplate, string studentName, string className, string courseName,
            string classOt, string evaluateContent, string teacherName)
        {
            if (myTemplate == null)
            {
                return null;
            }
            return new ShowContent()
            {
                Title = ReplaceContentClassEvaluate(myTemplate.Title, studentName, className, courseName, classOt, evaluateContent, teacherName),
                Summary = ReplaceContentClassEvaluate(myTemplate.Summary, studentName, className, courseName, classOt, evaluateContent, teacherName),
                ImgUrl = AliyunOssUtil.GetAccessUrlHttps(myTemplate.ImgKey)
            };
        }

        #endregion

        #region 电子相册

        private static string ReplaceContentStudentPhoto(string template, string studentName, string photoTitle)
        {
            return template
                .Replace("{{学员姓名}}", studentName)
                .Replace("{{相册标题}}", photoTitle);
        }

        public static ShareContent TemplateLinkStudentPhoto(EtShareTemplate myTemplate, string studentName, string photoTitle)
        {
            if (myTemplate == null)
            {
                return null;
            }
            return new ShareContent()
            {
                Title = ReplaceContentStudentPhoto(myTemplate.Title, studentName, photoTitle),
                Summary = ReplaceContentStudentPhoto(myTemplate.Summary, studentName, photoTitle),
                ImgUrl = AliyunOssUtil.GetAccessUrlHttps(myTemplate.ImgKey)
            };
        }

        #endregion

        #region 微官网

        private static string ReplaceContentMicWebsite(string template, string tenantName)
        {
            return template.Replace("{{机构名称}}", tenantName);
        }

        public static ShareContent TemplateLinkMicWebsite(EtShareTemplate myTemplate, string tenantName)
        {
            if (myTemplate == null)
            {
                return null;
            }
            return new ShareContent()
            {
                Title = ReplaceContentMicWebsite(myTemplate.Title, tenantName),
                Summary = ReplaceContentMicWebsite(myTemplate.Summary, tenantName),
                ImgUrl = AliyunOssUtil.GetAccessUrlHttps(myTemplate.ImgKey)
            };
        }

        #endregion

        #region 在线商城

        private static string ReplaceContentOnlineMall(string template, string tenantName)
        {
            return template.Replace("{{机构名称}}", tenantName);
        }

        public static ShareContent TemplateLinkOnlineMall(EtShareTemplate myTemplate, string tenantName)
        {
            if (myTemplate == null)
            {
                return null;
            }
            return new ShareContent()
            {
                Title = ReplaceContentOnlineMall(myTemplate.Title, tenantName),
                Summary = ReplaceContentOnlineMall(myTemplate.Summary, tenantName),
                ImgUrl = AliyunOssUtil.GetAccessUrlHttps(myTemplate.ImgKey)
            };
        }

        #endregion
    }
}
