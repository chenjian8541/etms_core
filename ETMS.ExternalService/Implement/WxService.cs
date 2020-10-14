using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.ExternalService.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WxApi;
using WxApi.SendEntity;

namespace ETMS.ExternalService.Implement
{
    public class WxService : IWxService
    {
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
                    var dataKeys = new Dictionary<string, TemplateKey>();
                    dataKeys.Add("first", new TemplateKey() { value = $"{student.StudentName}同学，您明天有课程，请提前做好上课准备" });
                    dataKeys.Add("keyword1", new TemplateKey() { value = student.CourseName });
                    dataKeys.Add("keyword2", new TemplateKey() { value = request.ClassTimeDesc });
                    dataKeys.Add("keyword3", new TemplateKey() { value = request.ClassRoom });
                    dataKeys.Add("remark", new TemplateKey() { value = request.Remark });
                    TemplateNotice.Send(student.OpendId, request.TemplateId, request.Topcolor, dataKeys, request.AccessToken, request.Url);
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
                    var dataKeys = new Dictionary<string, TemplateKey>();
                    dataKeys.Add("first", new TemplateKey() { value = $"{student.StudentName}同学，您在今天{request.StartTimeDesc}有课程即将上课，可别迟到哦" });
                    dataKeys.Add("keyword1", new TemplateKey() { value = student.CourseName });
                    dataKeys.Add("keyword2", new TemplateKey() { value = request.ClassTimeDesc });
                    dataKeys.Add("keyword3", new TemplateKey() { value = request.ClassRoom });
                    dataKeys.Add("remark", new TemplateKey() { value = request.Remark });
                    TemplateNotice.Send(student.OpendId, request.TemplateId, request.Topcolor, dataKeys, request.AccessToken, request.Url);
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
                    var dataKeys = new Dictionary<string, TemplateKey>();
                    dataKeys.Add("first", new TemplateKey() { value = $"{student.Name}同学，您的课程{student.CourseName}已完成点名，本次课您已{student.StudentCheckStatusDesc}，消耗{student.DeClassTimesDesc}课时，剩余{student.SurplusClassTimesDesc}，请确认" });
                    dataKeys.Add("keyword1", new TemplateKey() { value = student.Name });
                    dataKeys.Add("keyword2", new TemplateKey() { value = request.ClassName });
                    dataKeys.Add("keyword3", new TemplateKey() { value = request.ClassTimeDesc });
                    switch (student.StudentCheckStatus)
                    {
                        case EmClassStudentCheckStatus.BeLate:
                            dataKeys.Add("keyword4", new TemplateKey() { value = student.StudentCheckStatusDesc, color = "#E6A23C" });
                            break;
                        case EmClassStudentCheckStatus.NotArrived:
                            dataKeys.Add("keyword4", new TemplateKey() { value = student.StudentCheckStatusDesc, color = "#F56C6C" });
                            break;
                        default:
                            dataKeys.Add("keyword4", new TemplateKey() { value = student.StudentCheckStatusDesc });
                            break;
                    }
                    dataKeys.Add("keyword5", new TemplateKey() { value = request.TeacherDesc });
                    dataKeys.Add("remark", new TemplateKey() { value = request.Remark });
                    var result = TemplateNotice.Send(student.OpendId, request.TemplateId, request.Topcolor, dataKeys, request.AccessToken, student.LinkUrl);
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
                    var dataKeys = new Dictionary<string, TemplateKey>();
                    dataKeys.Add("first", new TemplateKey() { value = $"{student.Name}同学，您好！您收到一条请假审核提醒" });
                    dataKeys.Add("keyword1", new TemplateKey() { value = student.Name });
                    dataKeys.Add("keyword2", new TemplateKey() { value = request.StartTimeDesc });
                    dataKeys.Add("keyword3", new TemplateKey() { value = request.EndTimeDesc });
                    if (student.HandleStatus == EmStudentLeaveApplyHandleStatus.NotPass)
                    {
                        dataKeys.Add("keyword4", new TemplateKey() { value = student.HandleStatusDesc, color = "#E6A23C" });
                    }
                    else
                    {
                        dataKeys.Add("keyword4", new TemplateKey() { value = student.HandleStatusDesc });
                    }
                    dataKeys.Add("keyword5", new TemplateKey() { value = student.HandleUser });
                    dataKeys.Add("remark", new TemplateKey() { value = request.Remark });
                    var result = TemplateNotice.Send(student.OpendId, request.TemplateId, request.Topcolor, dataKeys, request.AccessToken, request.Url);
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
                    var dataKeys = new Dictionary<string, TemplateKey>();
                    dataKeys.Add("first", new TemplateKey() { value = $"{student.Name}同学，您有新的订单已完成，共消费{request.AptSumDesc}元，已支付{request.PaySumDesc}元，请确认" });
                    dataKeys.Add("keyword1", new TemplateKey() { value = request.OrderNo });
                    dataKeys.Add("keyword2", new TemplateKey() { value = request.BuyDesc });
                    dataKeys.Add("keyword3", new TemplateKey() { value = request.TimeDedc });
                    dataKeys.Add("remark", new TemplateKey() { value = request.Remark });
                    var result = TemplateNotice.Send(student.OpendId, request.TemplateId, request.Topcolor, dataKeys, request.AccessToken, request.Url);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentLeaveApply]请假审核提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }
    }
}
