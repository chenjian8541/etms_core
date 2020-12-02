using ETMS.Entity.Enum;
using ETMS.Entity.ExternalService.Dto.Request;
using ETMS.Entity.ExternalService.Dto.Request.User;
using ETMS.ExternalService.Contract;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using Newtonsoft.Json;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
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

        public const string LinkColor = "#1890ff";

        public const string NotFollowErrorCode = "43004";

        public const string NotFollowErrorMsg = "require subscribe";

        private readonly IStudentWechatDAL _studentWechatDAL;

        private readonly IUserWechatDAL _userWechatDAL;

        private readonly ISysWechartAuthTemplateMsgDAL _sysWechartAuthTemplateMsgDAL;

        private readonly IStudentDAL _studentDAL;

        public WxService(IStudentWechatDAL studentWechatDAL, ISysWechartAuthTemplateMsgDAL sysWechartAuthTemplateMsgDAL,
            IUserWechatDAL userWechatDAL, IStudentDAL studentDAL)
        {
            this._studentWechatDAL = studentWechatDAL;
            this._sysWechartAuthTemplateMsgDAL = sysWechartAuthTemplateMsgDAL;
            this._userWechatDAL = userWechatDAL;
            this._studentDAL = studentDAL;
        }

        private string GetFirstDesc(NoticeRequestBase requestBase, string first)
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

        private string ResetTemplateId(NoticeRequestBase requestBase)
        {
            LOG.Log.Info($"[ResetTemplateId]添加模板消息ID：requestBase:{JsonConvert.SerializeObject(requestBase)}", this.GetType());
            var result = TemplateApi.Addtemplate(requestBase.AccessToken, requestBase.TemplateIdShort);
            LOG.Log.Info($"[ResetTemplateId]添加模板消息ID：result:{JsonConvert.SerializeObject(result)}", this.GetType());
            if (result.errcode == ReturnCode.请求成功)
            {
                _sysWechartAuthTemplateMsgDAL.SaveSysWechartAuthTemplateMsg(requestBase.AuthorizerAppid, requestBase.TemplateIdShort, result.template_id).Wait();
                return result.template_id;
            }
            else
            {
                throw new Exception($"[ResetTemplateId]添加模板消息ID失败：requestBase:{JsonConvert.SerializeObject(requestBase)}");
            }
        }

        private string GetTemplateId(NoticeRequestBase requestBase)
        {
            var myWechartAuthTemplateMsg = _sysWechartAuthTemplateMsgDAL.GetSysWechartAuthTemplateMsg(requestBase.AuthorizerAppid, requestBase.TemplateIdShort).Result;
            if (myWechartAuthTemplateMsg == null)
            {
                return ResetTemplateId(requestBase);
            }
            return myWechartAuthTemplateMsg.TemplateId;
        }

        private void ProcessStudentEequireSubscribe(int loginTenantId, long studentId, string phone, string opendId, string errorMsg)
        {
            if (errorMsg.IndexOf(NotFollowErrorCode) != -1 && errorMsg.IndexOf(NotFollowErrorMsg) != -1)
            {
                _studentWechatDAL.InitTenantId(loginTenantId);
                _studentWechatDAL.DelOpendId(phone, opendId).Wait();
                _studentDAL.InitTenantId(loginTenantId);
                _studentDAL.UpdateStudentIsNotBindingWechat(new List<long>() { studentId });
                LOG.Log.Info($"[ProcessStudentEequireSubscribe]移除已取消关注的学员公众号信息,loginTenantId:{loginTenantId},studentId:{studentId},phone:{phone},opendId:{opendId}", this.GetType());
            }
        }

        private void ProcessUserEequireSubscribe(int loginTenantId, long userId, string opendId, string errorMsg)
        {
            if (errorMsg.IndexOf(NotFollowErrorCode) != -1 && errorMsg.IndexOf(NotFollowErrorMsg) != -1)
            {
                _userWechatDAL.InitTenantId(loginTenantId);
                _userWechatDAL.DelOpendId(userId, opendId).Wait();
                LOG.Log.Info($"[ProcessUserEequireSubscribe]移除已取消关注的员工公众号信息,loginTenantId:{loginTenantId},userId:{userId},opendId:{opendId}", this.GetType());
            }
        }

        private void ProcessInvalidTemplateId(NoticeRequestBase requestBase, string errorMsg)
        {
            if (errorMsg.IndexOf("invalid template_id") != -1 && errorMsg.IndexOf("40037") != -1)
            {
                requestBase.TemplateId = ResetTemplateId(requestBase);
            }
        }

        public void NoticeStudentsOfClassBeforeDay(NoticeStudentsOfClassBeforeDayRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassBeforeDay]发送上课通知出错2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassBeforeDay]发送上课通知出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeStudentsOfClassToday(NoticeStudentsOfClassTodayRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassToday]发送上课通知出错2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassToday]发送上课通知出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeClassCheckSign(NoticeClassCheckSignRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassBeforeDay]签到确认提醒2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentsOfClassBeforeDay]签到确认提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeStudentLeaveApply(NoticeStudentLeaveApplyRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，您收到一条请假审核提醒")),
                        keyword1 = new TemplateDataItem(student.Name, DefaultColor),
                        keyword2 = new TemplateDataItem(request.StartTimeDesc, DefaultColor),
                        keyword3 = new TemplateDataItem(request.EndTimeDesc, DefaultColor),
                        keyword4 = new TemplateDataItem(keyword4Desc, keyword4Color),
                        keyword5 = new TemplateDataItem(student.HandleUser, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, request.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[NoticeStudentLeaveApply]请假审核提醒2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentLeaveApply]请假审核提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeStudentContracts(NoticeStudentContractsRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[NoticeStudentLeaveApply]请假审核提醒2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentLeaveApply]请假审核提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void HomeworkAdd(HomeworkAddRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，您有新的作业待完成。")),
                        keyword1 = new TemplateDataItem(request.ExDateDesc, DefaultColor),
                        keyword2 = new TemplateDataItem(request.HomeworkTitle, DefaultColor),
                        keyword3 = new TemplateDataItem("点击查看详情", LinkColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, student.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[HomeworkAdd]作业布置通知2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[HomeworkAdd]作业布置通知:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void HomeworkExpireRemind(HomeworkExpireRemindRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，您有作业还未提交，请及时完成。")),
                        keyword1 = new TemplateDataItem(student.ClassName, DefaultColor),
                        keyword2 = new TemplateDataItem(student.HomeworkTitle, DefaultColor),
                        keyword3 = new TemplateDataItem("点击查看详情", LinkColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, student.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[HomeworkExpireRemind]作业截止时间提醒2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[HomeworkExpireRemind]作业截止时间提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void HomeworkComment(HomeworkCommentRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，{request.UserName}老师点评了您的作业，点击查看详情")),
                        keyword1 = new TemplateDataItem(request.HomeworkTitle, LinkColor),
                        keyword2 = new TemplateDataItem(request.OtDesc, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, student.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[HomeworkComment]作业点评2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[HomeworkComment]作业点评2:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void GrowthRecordAdd(GrowthRecordAddRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, "您收到一份学员成长档案，点击查看详情")),
                        keyword1 = new TemplateDataItem(student.ClassName, DefaultColor),
                        keyword2 = new TemplateDataItem(student.Name, LinkColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, student.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[GrowthRecordAdd]档案新增提醒2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[GrowthRecordAdd]档案新增提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void WxMessage(WxMessageRequest request)
        {
            request.TemplateId = GetTemplateId(request);
            foreach (var student in request.Students)
            {
                try
                {
                    if (string.IsNullOrEmpty(student.OpendId))
                    {
                        continue;
                    }
                    var tenantNameDesc = string.Empty;
                    if (string.IsNullOrEmpty(request.TenantSmsSignature))
                    {
                        tenantNameDesc = request.TenantName;
                    }
                    else
                    {
                        tenantNameDesc = request.TenantSmsSignature;
                    }
                    var data = new
                    {
                        first = new TemplateDataItem($"您收到一份来自[{tenantNameDesc}]的通知，赶紧点击看一看吧！"),
                        keyword1 = new TemplateDataItem(student.Name, DefaultColor),
                        keyword2 = new TemplateDataItem(request.OtDesc, DefaultColor),
                        keyword3 = new TemplateDataItem("点击查看详情", LinkColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, student.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[WxMessage]微信通知2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[WxMessage]微信通知:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void StudentEvaluate(StudentEvaluateRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，老师已对您的上课表现进行了点评，点击查看详情")),
                        keyword1 = new TemplateDataItem(student.Name, DefaultColor),
                        keyword2 = new TemplateDataItem(request.CourseName, LinkColor),
                        keyword3 = new TemplateDataItem(request.TeacherName, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, student.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[StudentEvaluate]课后点评2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[StudentEvaluate]课后点评:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void StudentCourseSurplus(StudentCourseSurplusRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，您的课程{student.CourseName}剩余{student.SurplusQuantityDesc}，点击查看详情")),
                        keyword1 = new TemplateDataItem(student.Name, DefaultColor),
                        keyword2 = new TemplateDataItem(student.Phone, DefaultColor),
                        keyword3 = new TemplateDataItem(student.SurplusQuantityDesc, LinkColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, student.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[StudentCourseSurplus]剩余课程2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[StudentCourseSurplus]剩余课程:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void StudentMakeup(StudentMakeupRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.Name}同学，您的课程{request.CourseName}已重新排课，请提前做好上课准备")),
                        keyword1 = new TemplateDataItem(request.ClassOt, DefaultColor),
                        keyword2 = new TemplateDataItem(request.ClassTime, DefaultColor),
                        keyword3 = new TemplateDataItem(request.TeacherDesc, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, request.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[StudentMakeup]插班补课2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[StudentMakeup]插班补课:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeStudentCourseNotEnough(NoticeStudentCourseNotEnoughRequest request)
        {
            request.TemplateId = GetTemplateId(request);
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
                        first = new TemplateDataItem(GetFirstDesc(request, $"{student.StudentName}同学，您的课程{request.CourseName}剩余不足{request.NotEnoughDesc}，为了不影响您正常上课，请及时续费！")),
                        keyword1 = new TemplateDataItem(student.StudentName, DefaultColor),
                        keyword2 = new TemplateDataItem(request.CourseName, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, student.OpendId, request.TemplateId, request.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[NoticeStudentCourseNotEnough]课时不足续费提醒2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessStudentEequireSubscribe(request.LoginTenantId, student.StudentId, student.Phone, student.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeStudentCourseNotEnough]课时不足续费提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeUserOfClassToday(NoticeUserOfClassTodayRequest request)
        {
            request.TemplateId = GetTemplateId(request);
            foreach (var user in request.Users)
            {
                try
                {
                    if (string.IsNullOrEmpty(user.OpendId))
                    {
                        continue;
                    }
                    var data = new
                    {
                        first = new TemplateDataItem(GetFirstDesc(request, $"{user.UserName}老师，您在今天{request.StartTimeDesc}有课程即将上课，可别迟到哦")),
                        keyword1 = new TemplateDataItem(user.CourseName, DefaultColor),
                        keyword2 = new TemplateDataItem(request.ClassTimeDesc, DefaultColor),
                        keyword3 = new TemplateDataItem(request.ClassRoom, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, user.OpendId, request.TemplateId, request.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[NoticeUserOfClassToday]发送上课通知出错2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessUserEequireSubscribe(request.LoginTenantId, user.UserId, user.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeUserOfClassToday]发送上课通知出错:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }

        public void NoticeTeacherOfHomeworkFinish(NoticeTeacherOfHomeworkFinishRequest request)
        {
            request.TemplateId = GetTemplateId(request);
            foreach (var user in request.Users)
            {
                try
                {
                    if (string.IsNullOrEmpty(user.OpendId))
                    {
                        continue;
                    }
                    var data = new
                    {
                        first = new TemplateDataItem(GetFirstDesc(request, $"{user.UserName}老师，您的学生作业提交成功，请及时批阅")),
                        keyword1 = new TemplateDataItem(request.StudentName, DefaultColor),
                        keyword2 = new TemplateDataItem(request.HomeworkTitle, DefaultColor),
                        keyword3 = new TemplateDataItem(request.FinishTime, DefaultColor),
                        remark = new TemplateDataItem(request.Remark, DefaultColor)
                    };
                    TemplateApi.SendTemplateMessage(request.AccessToken, user.OpendId, request.TemplateId, request.Url, data);
                }
                catch (ErrorJsonResultException exJsonResultException)
                {
                    LOG.Log.Error($"[NoticeTeacherOfHomeworkFinish]学员提交作业提醒2:{JsonConvert.SerializeObject(request)}", exJsonResultException, this.GetType());
                    ProcessUserEequireSubscribe(request.LoginTenantId, user.UserId, user.OpendId, exJsonResultException.Message);
                    ProcessInvalidTemplateId(request, exJsonResultException.Message);
                }
                catch (Exception ex)
                {
                    LOG.Log.Error($"[NoticeTeacherOfHomeworkFinish]学员提交作业提醒:{JsonConvert.SerializeObject(request)}", ex, this.GetType());
                }
            }
        }
    }
}
