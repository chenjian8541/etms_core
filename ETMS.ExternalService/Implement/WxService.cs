using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.ExternalService.Contract;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System;
using System.Collections.Generic;
using System.Text;
using WxApi;
using WxApi.SendEntity;

namespace ETMS.ExternalService.Implement
{
    public class WxService : IWxService
    {
        public const string DefaultColor = "";

        public void NoticeStudentsOfClassBeforeDay(NoticeStudentsOfClassBeforeDayRequest request)
        {
            foreach (var student in request.Students)
            {
                try
                {
                    if (string.IsNullOrEmpty(student.OpendId))
                    {
                        continue;
                    }
                    var data = new
                    {
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.StudentName}同学，您明天有课程，请提前做好上课准备")),
                        keyword1 = new TemplateDataItem(student.CourseName, DefaultColor),
                        keyword2 = new TemplateDataItem(request.ClassTimeDesc, DefaultColor),
                        keyword3 = new TemplateDataItem(request.ClassRoom, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, request.Url, data);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassBeforeDay]发送上课通知出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeStudentsOfClassToday(NoticeStudentsOfClassTodayRequest request)
        {
            foreach (var student in request.Students)
            {
                try
                {
                    if (string.IsNullOrEmpty(student.OpendId))
                    {
                        continue;
                    }
                    var data = new
                    {
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.StudentName}同学，您在今天{request.StartTimeDesc}有课程即将上课，可别迟到哦")),
                        keyword1 = new TemplateDataItem(student.CourseName, DefaultColor),
                        keyword2 = new TemplateDataItem(request.ClassTimeDesc, DefaultColor),
                        keyword3 = new TemplateDataItem(request.ClassRoom, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, request.Url, data);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassToday]发送上课通知出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeClassCheckSign(NoticeClassCheckSignRequest request)
        {
            foreach (var student in request.Students)
            {
                try
                {
                    if (string.IsNullOrEmpty(student.OpendId))
                    {
                        continue;
                    }
                    var keyword4Desc = string.Empty;
                    var keyword4Color = string.Empty;
                    switch (student.StudentCheckStatus)
                    {
                        case EmClassStudentCheckStatus.BeLate:
                            keyword4Desc = student.StudentCheckStatusDesc;
                            keyword4Color = "#E6A23C";
                            break;
                        case EmClassStudentCheckStatus.NotArrived:
                            keyword4Desc = student.StudentCheckStatusDesc;
                            keyword4Color = "#F56C6C";
                            break;
                        default:
                            keyword4Desc = student.StudentCheckStatusDesc;
                            break;
                    }
                    var data = new
                    {
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，您的课程{student.CourseName}已完成点名，本次课您已{student.StudentCheckStatusDesc}，消耗{student.DeClassTimesDesc}课时，剩余{student.SurplusClassTimesDesc}，请确认")),
                        keyword1 = new TemplateDataItem(student.Name, DefaultColor),
                        keyword2 = new TemplateDataItem(request.ClassName, DefaultColor),
                        keyword3 = new TemplateDataItem(request.ClassTimeDesc, DefaultColor),
                        keyword4 = new TemplateDataItem(keyword4Desc, keyword4Color),
                        keyword5 = new TemplateDataItem(request.TeacherDesc, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor),
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, student.LinkUrl, data);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassBeforeDay]签到确认提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeStudentLeaveApply(NoticeStudentLeaveApplyRequest request)
        {
            foreach (var student in request.Students)
            {
                try
                {
                    if (string.IsNullOrEmpty(student.OpendId))
                    {
                        continue;
                    }
                    var keyword4Desc = string.Empty;
                    var keyword4Color = string.Empty;
                    if (student.HandleStatus == EmStudentLeaveApplyHandleStatus.NotPass)
                    {
                        keyword4Desc = student.HandleStatusDesc;
                        keyword4Color = "#E6A23C";
                    }
                    else
                    {
                        keyword4Desc = student.HandleStatusDesc;
                    }
                    var data = new
                    {
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，您好！您收到一条请假审核提醒")),
                        keyword1 = new TemplateDataItem(student.Name, DefaultColor),
                        keyword2 = new TemplateDataItem(request.StartTimeDesc, DefaultColor),
                        keyword3 = new TemplateDataItem(request.EndTimeDesc, DefaultColor),
                        keyword4 = new TemplateDataItem(keyword4Desc, keyword4Color),
                        keyword5 = new TemplateDataItem(student.HandleUser, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, request.Url, data);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentLeaveApply]请假审核提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeStudentContracts(NoticeStudentContractsRequest request)
        {
            foreach (var student in request.Students)
            {
                try
                {
                    if (string.IsNullOrEmpty(student.OpendId))
                    {
                        continue;
                    }
                    var data = new
                    {
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，您有新的订单已完成，共消费{request.AptSumDesc}元，已支付{request.PaySumDesc}元，请确认")),
                        keyword1 = new TemplateDataItem(request.OrderNo, DefaultColor),
                        keyword2 = new TemplateDataItem(request.BuyDesc, DefaultColor),
                        keyword3 = new TemplateDataItem(request.TimeDedc, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, request.Url, data);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentLeaveApply]请假审核提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public string GetFirstDesc(NoticeRequestBase requestBase, string first)
        {
            if (requestBase.WechartAuthorizerId == 0)
            {
                if (string.IsNullOrEmpty(requestBase.TenantSmsSignature))
                {
                    return $"『 {requestBase.TenantName}』\r\n{first}";
                }
                else
                {
                    return $"『 {requestBase.TenantSmsSignature} 』\r\n{first}";
                }
            }
            else
            {
                return first;
            }
        }
    }
}
